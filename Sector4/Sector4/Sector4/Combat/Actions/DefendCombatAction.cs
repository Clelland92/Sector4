

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
    class DefendCombatAction : CombatAction
    {
        #region State


        /// <summary>
        /// Returns true if the action is offensive
        /// </summary>
        public override bool IsOffensive
        {
            get { return false; }
        }


        /// <summary>
        /// Returns true if this action requires a target.
        /// </summary>
        public override bool  IsTargetNeeded
        {
            get { return false; }
        }


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
                    Combatant.CombatSprite.PlayAnimation("Defend");
                    break;

                case CombatActionStage.Executing:
                    Combatant.CombatEffects.AddStatistics(new StatisticsValue(
                        0, 0, 0, Combatant.Character.CharacterStatistics.PhysicalDefense,
                        0, Combatant.Character.CharacterStatistics.AmmoalDefense), 1);
                    break;
            }
        }


        #endregion


        #region Heuristic


        /// <summary>
        /// The heuristic used to compare actions 
        /// </summary>
        public override int Heuristic
        {
            get 
            {
                return 0;
            }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructs a new DefendCombatAction object.
        /// </summary>
        /// <param name="character">The character performing the action.</param>
        public DefendCombatAction(Combatant combatant)
            : base(combatant) { }

        
        #endregion
    }
}
