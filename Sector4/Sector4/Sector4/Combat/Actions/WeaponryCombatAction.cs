

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Sector4Data;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Sector4
{
    /// <summary>
    /// A rangedweapon-firing combat action
    /// </summary>
    class WeaponryCombatAction : CombatAction
    {
        #region State


        /// <summary>
        /// Returns true if the action is offensive
        /// </summary>
        public override bool IsOffensive
        {
            get { return RangedWeapon.IsOffensive; }
        }

        
        /// <summary>
        /// Returns true if the player can use this action.
        /// </summary>
        public override bool IsCharacterValidUser
        {
            get
            {
                return (RangedWeapon.AmmoPointCost <= Combatant.Statistics.AmmoPoints);
            }
        }


        /// <summary>
        /// Returns true if this action requires a target.
        /// </summary>
        public override bool IsTargetNeeded
        {
            get { return true; }
        }


        #endregion


        #region RangedWeapon


        /// <summary>
        /// The rangedweapon used in this action.
        /// </summary>
        private RangedWeapon rangedweapon;

        /// <summary>
        /// The rangedweapon used in this action.
        /// </summary>
        public RangedWeapon RangedWeapon
        {
            get { return rangedweapon; }
        }


        /// <summary>
        /// The current position of the rangedweapon sprite.
        /// </summary>
        private Vector2 rangedweaponSpritePosition;


        /// <summary>
        /// Apply the action's rangedweapon to the given target.
        /// </summary>
        /// <returns>True if there was any effect on the target.</returns>
        private bool ApplyRangedWeapon(Combatant rangedweaponTarget)
        {
            StatisticsValue effectStatistics = CalculateRangedWeaponDamage(combatant, rangedweapon);
            if (rangedweapon.IsOffensive)
            {
                // calculate the defense
                Int32Range defenseRange = rangedweaponTarget.Character.AmmoDefenseRange + 
                    rangedweaponTarget.Statistics.AmmoalDefense;
                Int32 defense = defenseRange.GenerateValue(Session.Random);
                // subtract the defense
                effectStatistics -= new StatisticsValue(defense,
                    defense, defense, defense, defense, defense);
                // make sure that this only contains damage
                effectStatistics.ApplyMinimum(new StatisticsValue());
                // damage the target
                rangedweaponTarget.Damage(effectStatistics, rangedweapon.TargetDuration);
            }
            else
            {
                // make sure that this only contains healing
                effectStatistics.ApplyMinimum(new StatisticsValue());
                // heal the target
                rangedweaponTarget.Heal(effectStatistics, rangedweapon.TargetDuration);
            }

            return !effectStatistics.IsZero;
        }


        #endregion


        #region RangedWeapon Projectile Data


        /// <summary>
        /// The speed at which the projectile moves, in units per second.
        /// </summary>
        private const float projectileSpeed = 600f;


        /// <summary>
        /// The direction of the projectile.
        /// </summary>
        private Vector2 projectileDirection;


        /// <summary>
        /// The distance covered so far by the projectile.
        /// </summary>
        private float projectileDistanceCovered = 0f;


        /// <summary>
        /// The total distance between the original combatant position and the target.
        /// </summary>
        private float totalProjectileDistance;


        /// <summary>
        /// The sprite effect on the projectile, if any.
        /// </summary>
        private SpriteEffects projectileSpriteEffect = SpriteEffects.None;


        /// <summary>
        /// The sound effect cue for the traveling projectile.
        /// </summary>
        private Cue projectileCue;


        #endregion


        #region Combat Stage


        /// <summary>
        /// Starts a new combat stage.  Called right after the stage changes.
        /// </summary>
        /// <remarks>The stage never changes into NotStarted.</remarks>
        protected override void StartStage()
        {
            switch (stage)
            {
                case CombatActionStage.Preparing: // called from Start()
                    {
                        // play the animations
                        combatant.CombatSprite.PlayAnimation("RangedWeaponShoot");
                        rangedweaponSpritePosition = Combatant.Position;
                        rangedweapon.RangedWeaponSprite.PlayAnimation("Creation");
                        // remove the ammo points
                        Combatant.PayCostForRangedWeapon(rangedweapon);
                    }
                    break;

                case CombatActionStage.Advancing:
                    {
                        // play the animations
                        rangedweapon.RangedWeaponSprite.PlayAnimation("Traveling");
                        // calculate the projectile destination
                        projectileDirection = Target.Position -
                            Combatant.OriginalPosition;
                        totalProjectileDistance = projectileDirection.Length();
                        projectileDirection.Normalize();
                        projectileDistanceCovered = 0f;
                        // determine if the projectile is flipped
                        if (Target.Position.X > Combatant.Position.X)
                        {
                            projectileSpriteEffect = SpriteEffects.FlipHorizontally;
                        }
                        else
                        {
                            projectileSpriteEffect = SpriteEffects.None;
                        }
                        // get the projectile's cue and play it
                        projectileCue = AudioManager.GetCue(rangedweapon.TravelingCueName);
                        if (projectileCue != null)
                        {
                            projectileCue.Play();
                        }
                    }
                    break;

                case CombatActionStage.Executing:
                    {
                        // play the animation
                        rangedweapon.RangedWeaponSprite.PlayAnimation("Impact");
                        // stop the projectile sound effect
                        if (projectileCue != null)
                        {
                            projectileCue.Stop(AudioStopOptions.Immediate);
                        }
                        // apply the rangedweapon effect to the primary target
                        bool damagedAnyone = ApplyRangedWeapon(Target);
                        // apply the rangedweapon to the secondary targets
                        foreach (Combatant targetCombatant in
                            CombatEngine.SecondaryTargetedCombatants)
                        {
                            // skip dead or dying targets
                            if (targetCombatant.IsDeadOrDying)
                            {
                                continue;
                            }
                            // apply the rangedweapon
                            damagedAnyone |= ApplyRangedWeapon(targetCombatant);
                        }
                        // play the impact sound effect
                        if (damagedAnyone)
                        {
                            AudioManager.PlayCue(rangedweapon.ImpactCueName);
                            if (rangedweapon.Overlay != null)
                            {
                                rangedweapon.Overlay.PlayAnimation(0);
                                rangedweapon.Overlay.ResetAnimation();
                            }
                        }
                    }
                    break;

                case CombatActionStage.Returning:
                    // play the animation
                    combatant.CombatSprite.PlayAnimation("Idle");
                    break;

                case CombatActionStage.Finishing:
                    // play the animation
                    combatant.CombatSprite.PlayAnimation("Idle");
                    break;

                case CombatActionStage.Complete:
                    // play the animation
                    combatant.CombatSprite.PlayAnimation("Idle");
                    // make sure that the overlay has stopped
                    rangedweapon.Overlay.StopAnimation();
                    break;
            }
        }


        
        protected override void UpdateCurrentStage(GameTime gameTime)
        {
            switch (stage)
            {
                case CombatActionStage.Advancing:
                    if (projectileDistanceCovered < totalProjectileDistance)
                    {
                        projectileDistanceCovered += projectileSpeed *
                            (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    rangedweaponSpritePosition = combatant.OriginalPosition +
                        projectileDirection * projectileDistanceCovered;
                    break;
            }
        }


        /// <summary>
        /// Returns true if the combat action is ready to proceed to the next fight.
        /// </summary>
        protected override bool IsReadyForNextStage
        {
            get
            {
                switch (stage)
                {
                    case CombatActionStage.Preparing: 
                        return (combatant.CombatSprite.IsPlaybackComplete &&
                            rangedweapon.RangedWeaponSprite.IsPlaybackComplete);

                    case CombatActionStage.Advancing: 
                        if (rangedweapon.RangedWeaponSprite.IsPlaybackComplete ||
                            (projectileDistanceCovered >= totalProjectileDistance))
                        {
                            projectileDistanceCovered = totalProjectileDistance;
                            return true;
                        }
                        return false;

                    case CombatActionStage.Executing: 
                        return rangedweapon.RangedWeaponSprite.IsPlaybackComplete;
                }

                
                return base.IsReadyForNextStage;
            }
        }


        #endregion


        #region Heuristic


        /// <summary>
        /// The heuristic used to compare actions of this type to similar ones.
        /// </summary>
        public override int Heuristic
        {
            get
            {
                return (Combatant.Statistics.AmmoalOffense + 
                    RangedWeapon.TargetEffectRange.HealthPointsRange.Average);
            }
        }


        #endregion


        #region Initialization


        
        public WeaponryCombatAction(Combatant combatant, RangedWeapon rangedweapon)
            : base(combatant)
        {
            // check the parameter
            if (rangedweapon == null)
            {
                throw new ArgumentNullException("rangedweapon");
            }

            // assign the parameter
            this.rangedweapon = rangedweapon;
            this.adjacentTargets = this.rangedweapon.AdjacentTargets;
        }


        /// <summary>
        /// Start executing the combat action.
        /// </summary>
        public override void Start()
        {
            // play the creation sound effect
            AudioManager.PlayCue(rangedweapon.CreatingCueName);

            base.Start();
        }


        #endregion


        #region Updating


        /// <summary>
        /// Updates the action over time.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // update the animations
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            rangedweapon.RangedWeaponSprite.UpdateAnimation(elapsedSeconds);
            if (rangedweapon.Overlay != null)
            {
                rangedweapon.Overlay.UpdateAnimation(elapsedSeconds);
                if (!rangedweapon.Overlay.IsPlaybackComplete &&
                    Target.CombatSprite.IsPlaybackComplete)
                {
                    rangedweapon.Overlay.StopAnimation();
                }
            }

            base.Update(gameTime);
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draw any elements of the action that are independent of the player.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw the rangedweapon projectile
            if (!rangedweapon.RangedWeaponSprite.IsPlaybackComplete)
            {
                if (stage == CombatActionStage.Advancing)
                {
                    rangedweapon.RangedWeaponSprite.Draw(spriteBatch, rangedweaponSpritePosition, 0f,
                        projectileSpriteEffect);
                }
                else
                {
                    rangedweapon.RangedWeaponSprite.Draw(spriteBatch, rangedweaponSpritePosition, 0f);
                }
            }

            // draw the rangedweapon overlay
            if (!rangedweapon.Overlay.IsPlaybackComplete)
            {
                rangedweapon.Overlay.Draw(spriteBatch, Target.Position, 0f);
            }

            base.Draw(gameTime, spriteBatch);
        }


        #endregion


        #region Static Calculation Methods


        /// <summary>
        /// Calculate the rangedweapon damage done by the given combatant and rangedweapon.
        /// </summary>
        public static StatisticsValue CalculateRangedWeaponDamage(Combatant combatant, 
            RangedWeapon rangedweapon)
        {
            // check the parameters
            if (combatant == null)
            {
                throw new ArgumentNullException("combatant");
            }
            if (rangedweapon == null)
            {
                throw new ArgumentNullException("rangedweapon");
            }

            // get the ranged offense from the character's class, gear, and bonuses
            // -- note that this includes stat buffs
            int ammoalOffense = combatant.Statistics.AmmoalOffense;

            // add the ranged offense to the rangedweapon
            StatisticsValue damage = 
                rangedweapon.TargetEffectRange.GenerateValue(Session.Random);
            damage.HealthPoints += (damage.HealthPoints != 0) ? ammoalOffense : 0;
            damage.AmmoPoints += (damage.AmmoPoints != 0) ? ammoalOffense : 0;
            damage.PhysicalOffense += (damage.PhysicalOffense != 0) ? ammoalOffense : 0;
            damage.PhysicalDefense += (damage.PhysicalDefense != 0) ? ammoalOffense : 0;
            damage.AmmoalOffense += (damage.AmmoalOffense != 0) ? ammoalOffense : 0;
            damage.AmmoalDefense += (damage.AmmoalDefense != 0) ? ammoalOffense : 0;

            // add in the rangedweapon damage
            return damage;
        }


        #endregion
    }
}
