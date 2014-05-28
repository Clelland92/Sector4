

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
    /// A item-firing combat action
    /// </summary>
    class ItemCombatAction : CombatAction
    {
        #region State


        /// <summary>
        /// Returns true if the action is offensive
        /// </summary>
        public override bool IsOffensive
        {
            get { return Item.IsOffensive; }
        }


        /// <summary>
        /// Returns true if the player can use this action.
        /// </summary>
        public override bool IsCharacterValidUser
        {
            get { return true; }
        }


        /// <summary>
        /// Returns true if this action requires a target.
        /// </summary>
        public override bool IsTargetNeeded
        {
            get { return true; }
        }


        #endregion


        #region Item


        /// <summary>
        /// The item used in this action.
        /// </summary>
        private Item item;

        /// <summary>
        /// The item used in this action.
        /// </summary>
        public Item Item
        {
            get { return item; }
        }


        /// <summary>
        /// The current position of the item sprite.
        /// </summary>
        private Vector2 itemSpritePosition;


        /// <summary>
        /// Apply the action's item to the given target.
        /// </summary>
        /// <returns>True if there was any effect on the target.</returns>
        private bool ApplyItem(Combatant itemTarget)
        {
            StatisticsValue effectStatistics = CalculateItemDamage(combatant, item);
            if (item.IsOffensive)
            {
                // calculate the defense
                Int32Range defenseRange = itemTarget.Character.AmmoDefenseRange +
                    itemTarget.Statistics.AmmoalDefense;
                Int32 defense = defenseRange.GenerateValue(Session.Random);
                // subtract the defense
                effectStatistics -= new StatisticsValue(defense,
                    defense, defense, defense, defense, defense);
                // make sure that this only contains damage
                effectStatistics.ApplyMinimum(new StatisticsValue());
                // damage the target
                itemTarget.Damage(effectStatistics, item.TargetDuration);
            }
            else
            {
                // make sure taht this only contains healing
                effectStatistics.ApplyMinimum(new StatisticsValue());
                // heal the target
                itemTarget.Heal(effectStatistics, item.TargetDuration);
            }
            return !effectStatistics.IsZero;
        }
        

        #endregion


        #region Item Projectile Data


        /// <summary>
        /// The speed at which the projectile moves, in units per second.
        /// </summary>
        private const float projectileSpeed = 300f;


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
        /// Starts a new combat stage.  
        /// </summary>
        
        protected override void StartStage()
        {
            switch (stage)
            {
                case CombatActionStage.Preparing: 
                    {
                        // play the animations
                        combatant.CombatSprite.PlayAnimation("ItemShoot");
                        itemSpritePosition = Combatant.Position;
                        item.RangedWeaponSprite.PlayAnimation("Creation");
                        Session.Party.RemoveFromInventory(item, 1);
                    }
                    break;

                case CombatActionStage.Advancing:
                    {
                        // play the animations
                        item.RangedWeaponSprite.PlayAnimation("Traveling");
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
                        projectileCue = AudioManager.GetCue(item.TravelingCueName);
                        if (projectileCue != null)
                        {
                            projectileCue.Play();
                        }
                    }
                    break;

                case CombatActionStage.Executing:
                    // play the animation
                    item.RangedWeaponSprite.PlayAnimation("Impact");
                    // stop the projectile sound effect
                    if (projectileCue != null)
                    {
                        projectileCue.Stop(AudioStopOptions.Immediate);
                    }
                    // apply the item effect to the  target
                    bool damagedAnyone = ApplyItem(Target);
                    // apply the item effect to the  targets
                    foreach (Combatant targetCombatant in
                        CombatEngine.SecondaryTargetedCombatants)
                    {
                        // skip any dead or dying combatants
                        if (targetCombatant.IsDeadOrDying)
                        {
                            continue;
                        }
                        // apply the effect
                        damagedAnyone |= ApplyItem(targetCombatant);
                    }
                    // play the impact sound effect
                    if (damagedAnyone)
                    {
                        AudioManager.PlayCue(item.ImpactCueName);
                        if (item.Overlay != null)
                        {
                            item.Overlay.PlayAnimation(0);
                            item.Overlay.ResetAnimation();
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
                    break;
            }
        }


        /// <summary>
        /// Update the action for the current stage.
        /// </summary>
        
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
                    itemSpritePosition = combatant.OriginalPosition +
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
                            item.RangedWeaponSprite.IsPlaybackComplete);

                    case CombatActionStage.Advancing: 
                        if (item.RangedWeaponSprite.IsPlaybackComplete ||
                            (projectileDistanceCovered >= totalProjectileDistance))
                        {
                            projectileDistanceCovered = totalProjectileDistance;
                            return true;
                        }
                        return false;

                    case CombatActionStage.Executing: 
                        return item.RangedWeaponSprite.IsPlaybackComplete;
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
                return Item.TargetEffectRange.HealthPointsRange.Average;
            }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructs a new ItemCombatAction object.
        /// </summary>
        
        public ItemCombatAction(Combatant combatant, Item item)
            : base(combatant)
        {
            
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if ((item.Usage & Item.ItemUsage.Combat) == 0)
            {
                throw new ArgumentException("Combat items must have Combat usage.");
            }

            
            this.item = item;
            this.adjacentTargets = this.item.AdjacentTargets;
        }


        /// <summary>
        /// Start executing the combat action.
        /// </summary>
        public override void Start()
        {
            // play the creation sound effect
            AudioManager.PlayCue(item.UsingCueName);

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
            item.RangedWeaponSprite.UpdateAnimation(elapsedSeconds);
            if (item.Overlay != null)
            {
                item.Overlay.UpdateAnimation(elapsedSeconds);
                if (!item.Overlay.IsPlaybackComplete &&
                    Target.CombatSprite.IsPlaybackComplete)
                {
                    item.Overlay.StopAnimation();
                }
            }

            base.Update(gameTime);
        }


        #endregion


        #region Drawing


       
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            if (!item.RangedWeaponSprite.IsPlaybackComplete)
            {
                if (stage == CombatActionStage.Advancing)
                {
                    item.RangedWeaponSprite.Draw(spriteBatch, itemSpritePosition, 0f,
                        projectileSpriteEffect);
                }
                else
                {
                    item.RangedWeaponSprite.Draw(spriteBatch, itemSpritePosition, 0f);
                }
            }

            
            if (!item.Overlay.IsPlaybackComplete)
            {
                item.Overlay.Draw(spriteBatch, Target.Position, 0f);
            }

            base.Draw(gameTime, spriteBatch);
        }


        #endregion


        #region Static Calculation Methods


        /// <summary>
        /// Calculate the item damage done by the given combatant and item.
        /// </summary>
        public static StatisticsValue CalculateItemDamage(Combatant combatant,
            Item item)
        {
            // check the parameters
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            
            return item.TargetEffectRange.GenerateValue(Session.Random);
        }


        #endregion
    }
}
