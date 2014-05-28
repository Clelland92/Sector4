

#region Using Statements
using System;
#endregion

namespace Sector4
{
    enum CombatEndingState
    {
        /// <summary>
        /// All of the monsters died in combat.
        /// </summary>
        Victory,

        /// <summary>
        /// The player successfully fled from combat.
        /// </summary>
        Fled,

        /// <summary>
        /// All of the players died in combat.
        /// </summary>
        Loss,
    }
}
