

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Sector4Data;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Sector4
{
    /// <summary>
    /// A melee-attack combat action
    /// </summary>
    class MeleeCombatAction : CombatAction
    {
        #region State


        /// <summary>
        /// Returns true if the action is offensive
        /// </summary>
        public override bool IsOffensive
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


        #region Advancing/Returning Data


        /// <summary>
        /// The speed at which the advancing character moves
        /// </summary>
        private const float advanceSpeed = 300f;


        /// <summary>
        /// The offset from the advance destination to the target position
        /// </summary>
        private static readonly Vector2 advanceOffset = new Vector2(85f, 0f);


        /// <summary>
        /// The direction of the advancement.
        /// </summary>
        private Vector2 advanceDirection;


        /// <summary>
        /// The distance covered so far by the advance/return action
        /// </summary>
        private float advanceDistanceCovered = 0f;


        /// <summary>
        /// The total distance between the original combatant  and the target.
        /// </summary>
        private float totalAdvanceDistance;


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
                        
                        combatant.CombatSprite.PlayAnimation("Idle");
                    }
                    break;

                case CombatActionStage.Advancing:
                    {
                        // play the animation
                        combatant.CombatSprite.PlayAnimation("Walk");
                        // calculate the advancing destination
                        if (Target.Position.X > Combatant.Position.X)
                        {
                            advanceDirection = Target.Position -
                                Combatant.OriginalPosition - advanceOffset;
                        }
                        else
                        {
                            advanceDirection = Target.Position -
                                Combatant.OriginalPosition + advanceOffset;
                        }
                        totalAdvanceDistance = advanceDirection.Length();
                        advanceDirection.Normalize();
                        advanceDistanceCovered = 0f;
                    }
                    break;

                case CombatActionStage.Executing:
                    {
                        // play the animation
                        combatant.CombatSprite.PlayAnimation("Attack");
                        // play the audio
                        Weapon weapon = combatant.Character.GetEquippedWeapon();
                        if (weapon != null)
                        {
                            AudioManager.PlayCue(weapon.SwingCueName);
                        }
                        else
                        {
                            AudioManager.PlayCue("WeaponSwing");
                        }
                    }
                    break;

                case CombatActionStage.Returning:
                    {
                        // play the animation
                        combatant.CombatSprite.PlayAnimation("Walk");
                        // calculate the damage
                        Int32Range damageRange = combatant.Character.TargetDamageRange +
                            combatant.Statistics.PhysicalOffense;
                        Int32Range defenseRange = Target.Character.HealthDefenseRange + 
                            Target.Statistics.PhysicalDefense;
                        int damage = Math.Max(0, 
                            damageRange.GenerateValue(Session.Random) -
                            defenseRange.GenerateValue(Session.Random));
                        // apply the damage
                        if (damage > 0)
                        {
                            // play the audio
                            Weapon weapon = combatant.Character.GetEquippedWeapon();
                            if (weapon != null)
                            {
                                AudioManager.PlayCue(weapon.HitCueName);
                            }
                            else
                            {
                                AudioManager.PlayCue("WeaponHit");
                            }
                            // damage the target
                            Target.DamageHealth(damage, 0);
                            if ((weapon != null) && (weapon.Overlay != null))
                            {
                                weapon.Overlay.PlayAnimation(0);
                                weapon.Overlay.ResetAnimation();
                            }
                        }
                    }
                    break;

                case CombatActionStage.Finishing:
                    {
                        // play the animation
                        combatant.CombatSprite.PlayAnimation("Idle");
                    }
                    break;

                case CombatActionStage.Complete:
                    {
                        // play the animation
                        combatant.CombatSprite.PlayAnimation("Idle");
                    }
                    break;
            }
        }


        
        protected override void UpdateCurrentStage(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (stage)
            {
                case CombatActionStage.Advancing:
                    {
                        // move to the destination
                        if (advanceDistanceCovered < totalAdvanceDistance)
                        {
                            advanceDistanceCovered = Math.Min(advanceDistanceCovered +
                                advanceSpeed * elapsedSeconds, totalAdvanceDistance);
                        }
                        // update the combatant's position
                        combatant.Position = combatant.OriginalPosition +
                            advanceDirection * advanceDistanceCovered;
                    }
                    break;

                case CombatActionStage.Returning:
                    {
                        // move to the destination
                        if (advanceDistanceCovered > 0f)
                        {
                            advanceDistanceCovered -= advanceSpeed * elapsedSeconds;
                        }
                        combatant.Position = combatant.OriginalPosition +
                            advanceDirection * advanceDistanceCovered;
                    }
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
                        return true;

                    case CombatActionStage.Advancing: 
                        if (advanceDistanceCovered >= totalAdvanceDistance)
                        {
                            advanceDistanceCovered = totalAdvanceDistance;
                            combatant.Position = combatant.OriginalPosition +
                                advanceDirection * totalAdvanceDistance;
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case CombatActionStage.Executing: 
                        return combatant.CombatSprite.IsPlaybackComplete;

                    case CombatActionStage.Returning: 
                        if (advanceDistanceCovered <= 0f)
                        {
                            advanceDistanceCovered = 0f;
                            combatant.Position = combatant.OriginalPosition;
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case CombatActionStage.Finishing: 
                        return true;
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
                return combatant.Character.TargetDamageRange.Average;
            }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructs a new MeleeCombatAction object.
        /// </summary>
        
        public MeleeCombatAction(Combatant combatant)
            : base(combatant) { }

        
        #endregion


        #region Updating


        /// <summary>
        /// Updates the action over time.
        /// </summary>
        public override void Update(GameTime gameTime) 
        {
            // update the weapon animation
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Weapon weapon = Combatant.Character.GetEquippedWeapon();
            if ((weapon != null) && (weapon.Overlay != null))
            {
                weapon.Overlay.UpdateAnimation(elapsedSeconds);
            }

            // update the action
            base.Update(gameTime);
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draw any elements of the action that are independent of the player.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw the weapon overlay 
            Weapon weapon = Combatant.Character.GetEquippedWeapon();
            if ((weapon != null) && (weapon.Overlay != null) && 
                !weapon.Overlay.IsPlaybackComplete)
            {
                weapon.Overlay.Draw(spriteBatch, Target.Position, 0f);
            }

            base.Draw(gameTime, spriteBatch);
        }


        #endregion
    }
}
