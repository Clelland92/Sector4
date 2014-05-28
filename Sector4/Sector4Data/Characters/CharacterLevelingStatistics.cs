

#region Using Statements
using System;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Sector4Data
{
    /// <summary>
    /// Information about how to increment statistics with additional character levels.
    /// </summary>
#if !XBOX
    [Serializable]
#endif
    public struct CharacterLevelingStatistics
    {
        /// <summary>
        /// The amount that the character's health points will increase.
        /// </summary>
        public Int32 HealthPointsIncrease;

        /// <summary>
        /// The number of levels between each health point increase.
        /// </summary>
        public Int32 LevelsPerHealthPointsIncrease;


        /// <summary>
        /// The amount that the character's ammo points will increase.
        /// </summary>
        public Int32 AmmoPointsIncrease;

        /// <summary>
        /// The number of levels between each ammo point increase.
        /// </summary>
        public Int32 LevelsPerAmmoPointsIncrease;


        /// <summary>
        /// The amount that the character's physical offense will increase.
        /// </summary>
        public Int32 PhysicalOffenseIncrease;

        /// <summary>
        /// The number of levels between each physical offense increase.
        /// </summary>
        public Int32 LevelsPerPhysicalOffenseIncrease;


        /// <summary>
        /// The amount that the character's physical defense will increase.
        /// </summary>
        public Int32 PhysicalDefenseIncrease;

        /// <summary>
        /// The number of levels between each physical defense increase.
        /// </summary>
        public Int32 LevelsPerPhysicalDefenseIncrease;


        /// <summary>
        /// The amount that the character's ammoal offense will increase.
        /// </summary>
        public Int32 AmmoalOffenseIncrease;

        /// <summary>
        /// The number of levels between each ammoal offense increase.
        /// </summary>
        public Int32 LevelsPerAmmoalOffenseIncrease;


        /// <summary>
        /// The amount that the character's ammoal defense will increase.
        /// </summary>
        public Int32 AmmoalDefenseIncrease;

        /// <summary>
        /// The number of levels between each ammoal defense increase.
        /// </summary>
        public Int32 LevelsPerAmmoalDefenseIncrease;


        #region Content Type Reader


        /// <summary>
        /// Reads a CharacterLevelingStatistics object from the content pipeline.
        /// </summary>
        public class CharacterLevelingStatisticsReader : 
            ContentTypeReader<CharacterLevelingStatistics>
        {
            /// <summary>
            /// Reads a CharacterLevelingStatistics object from the content pipeline.
            /// </summary>
            protected override CharacterLevelingStatistics Read(ContentReader input,
                CharacterLevelingStatistics existingInstance)
            {
                CharacterLevelingStatistics stats = existingInstance;

                stats.HealthPointsIncrease = input.ReadInt32();
                stats.AmmoPointsIncrease = input.ReadInt32();
                stats.PhysicalOffenseIncrease = input.ReadInt32();
                stats.PhysicalDefenseIncrease = input.ReadInt32();
                stats.AmmoalOffenseIncrease = input.ReadInt32();
                stats.AmmoalDefenseIncrease = input.ReadInt32();

                stats.LevelsPerHealthPointsIncrease = input.ReadInt32();
                stats.LevelsPerAmmoPointsIncrease = input.ReadInt32();
                stats.LevelsPerPhysicalOffenseIncrease = input.ReadInt32();
                stats.LevelsPerPhysicalDefenseIncrease = input.ReadInt32();
                stats.LevelsPerAmmoalOffenseIncrease = input.ReadInt32();
                stats.LevelsPerAmmoalDefenseIncrease = input.ReadInt32();

                return stats;
            }
        }


        #endregion
    }
}
