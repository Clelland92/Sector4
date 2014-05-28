

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Sector4Data;
#endregion

namespace Sector4
{
    
    class CombatantPlayer : Combatant
    {
        /// <summary>
        /// The Player object encapsulated by this object.
        /// </summary>
        private Player player;

        /// <summary>
        /// The Player object encapsulated by this object.
        /// </summary>
        public Player Player
        {
            get { return player; }
        }
        
        /// <summary>
        /// The character encapsulated by this combatant.
        /// </summary>
        public override FightingCharacter Character
        {
            get { return player as FightingCharacter; }
        }


        #region State Data


        /// <summary>
        /// The current state of this combatant.
        /// </summary>
        public override Character.CharacterState State
        {
            get { return player.State; }
            set
            {
                if (value == player.State)
                {
                    return;
                }
                player.State = value;
                switch (player.State)
                {
                    case Sector4Data.Character.CharacterState.Idle:
                        CombatSprite.PlayAnimation("Idle");
                        break;

                    case Sector4Data.Character.CharacterState.Hit:
                        CombatSprite.PlayAnimation("Hit");
                        break;

                    case Sector4Data.Character.CharacterState.Dying:
                        player.StatisticsModifiers.HealthPoints = 
                            -1 * player.CharacterStatistics.HealthPoints;
                        CombatSprite.PlayAnimation("Die");
                        break;
                }
            }
        }


        #endregion


        #region Graphics Data


        /// <summary>
        /// Accessor for the combat sprite for this combatant.
        /// </summary>
        public override AnimatingSprite CombatSprite
        {
            get { return player.CombatSprite; }
        }


        #endregion


        #region Current Statistics


        /// <summary>
        /// The current statistics of this combatant.
        /// </summary>
        public override StatisticsValue Statistics
        {
            get { return player.CurrentStatistics + CombatEffects.TotalStatistics; }
        }


        /// <summary>
        /// Heals the combatant by the given amount.
        /// </summary>
        public override void Heal(StatisticsValue healingStatistics, int duration)
        {
            if (duration > 0)
            {
                CombatEffects.AddStatistics(healingStatistics, duration);
            }
            else
            {
                player.StatisticsModifiers += healingStatistics;
                player.StatisticsModifiers.ApplyMaximum(new StatisticsValue());
            }
            base.Heal(healingStatistics, duration);
        }


        /// <summary>
        /// Damages the combatant by the given amount.
        /// </summary>
        public override void Damage(StatisticsValue damageStatistics, int duration)
        {
            if (duration > 0)
            {
                CombatEffects.AddStatistics(new StatisticsValue() - damageStatistics,
                    duration);
            }
            else
            {
                player.StatisticsModifiers -= damageStatistics;
                player.StatisticsModifiers.ApplyMaximum(new StatisticsValue());
            }
            base.Damage(damageStatistics, duration);
        }


        /// <summary>
        /// Pay the ammo cost for the given rangedweapon.
        /// </summary>
        /// <returns>True if the cost could be paid (and therefore was paid).</returns>
        public override bool PayCostForRangedWeapon(RangedWeapon rangedweapon)
        {
            // check the parameter.
            if (rangedweapon == null)
            {
                throw new ArgumentNullException("rangedweapon");
            }

            // check the requirements
            if (Statistics.AmmoPoints < rangedweapon.AmmoPointCost)
            {
                return false;
            }

            // reduce the player's ammo points by the rangedweapon's cost
            player.StatisticsModifiers.AmmoPoints -= rangedweapon.AmmoPointCost;

            return true;
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Construct a new CombatantPlayer object containing the given player.
        /// </summary>
        public CombatantPlayer(Player player) : base()
        {
            // check the parameter
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            // assign the parameters
            this.player = player;

            // if the player starts dead, make sure the sprite is already "dead"
            if (IsDeadOrDying)
            {
                if (Statistics.HealthPoints > 0)
                {
                    State = Sector4Data.Character.CharacterState.Idle;
                }
                else
                {
                    CombatSprite.PlayAnimation("Die");
                    CombatSprite.AdvanceToEnd();
                }
            }
            else
            {
                State = Sector4Data.Character.CharacterState.Idle;
                CombatSprite.PlayAnimation("Idle");
            }
        }


        #endregion


        #region Updating


        /// <summary>
        /// Update the player for this frame.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        #endregion
    }
}
