

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Sector4Data
{
    /// <summary>
    /// The definition of a type of character.
    /// </summary>
    public class CharacterClass : ContentObject
    {
        #region Description


        /// <summary>
        /// The name of the character class.
        /// </summary>
        private string name;

        /// <summary>
        /// The name of the character class.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        #endregion


        #region Statistics


        /// <summary>
        /// The initial statistics of characters that use this class.
        /// </summary>
        private StatisticsValue initialStatistics = new StatisticsValue();

        /// <summary>
        /// The initial statistics of characters that use this class.
        /// </summary>
        public StatisticsValue InitialStatistics
        {
            get { return initialStatistics; }
            set { initialStatistics = value; }
        }

        
        #endregion


        #region Leveling


        /// <summary>
        /// Statistics changes for leveling up characters that use this class.
        /// </summary>
        private CharacterLevelingStatistics levelingStatistics;

        /// <summary>
        /// Statistics changes for leveling up characters that use this class.
        /// </summary>
        public CharacterLevelingStatistics LevelingStatistics
        {
            get { return levelingStatistics; }
            set { levelingStatistics = value; }
        }


        /// <summary>
        /// Entries of the requirements and rewards for each level of this class.
        /// </summary>
        private List<CharacterLevelDescription> levelEntries = 
            new List<CharacterLevelDescription>();

        /// <summary>
        /// Entries of the requirements and rewards for each level of this class.
        /// </summary>
        public List<CharacterLevelDescription> LevelEntries
        {
            get { return levelEntries; }
            set { levelEntries = value; }
        }


        /// <summary>
        /// Calculate the statistics of a character of this class and the given level.
        /// </summary>
        public StatisticsValue GetStatisticsForLevel(int characterLevel)
        {
            // check the parameter
            if (characterLevel <= 0)
            {
                throw new ArgumentOutOfRangeException("characterLevel");
            }

            // start with the initial statistics
            StatisticsValue output = initialStatistics;

            // add each level of leveling statistics
            for (int i = 1; i < characterLevel; i++)
            {
                if ((levelingStatistics.LevelsPerHealthPointsIncrease > 0) &&
                    ((i % levelingStatistics.LevelsPerHealthPointsIncrease) == 0))
                {
                    output.HealthPoints += levelingStatistics.HealthPointsIncrease;
                }
                if ((levelingStatistics.LevelsPerAmmoPointsIncrease > 0) &&
                    ((i % levelingStatistics.LevelsPerAmmoPointsIncrease) == 0))
                {
                    output.AmmoPoints += levelingStatistics.AmmoPointsIncrease;
                }
                if ((levelingStatistics.LevelsPerPhysicalOffenseIncrease > 0) &&
                    ((i % levelingStatistics.LevelsPerPhysicalOffenseIncrease) == 0))
                {
                    output.PhysicalOffense += levelingStatistics.PhysicalOffenseIncrease;
                }
                if ((levelingStatistics.LevelsPerPhysicalDefenseIncrease > 0) &&
                    ((i % levelingStatistics.LevelsPerPhysicalDefenseIncrease) == 0))
                {
                    output.PhysicalDefense += levelingStatistics.PhysicalDefenseIncrease;
                }
                if ((levelingStatistics.LevelsPerAmmoalOffenseIncrease > 0) &&
                    ((i % levelingStatistics.LevelsPerAmmoalOffenseIncrease) == 0))
                {
                    output.AmmoalOffense += levelingStatistics.AmmoalOffenseIncrease;
                }
                if ((levelingStatistics.LevelsPerAmmoalDefenseIncrease > 0) &&
                    ((i % levelingStatistics.LevelsPerAmmoalDefenseIncrease) == 0))
                {
                    output.AmmoalDefense += levelingStatistics.AmmoalDefenseIncrease;
                }
            }

            return output;
        }


        /// <summary>
        /// Build a list of all rangedweapons available to a character 
        /// of this class and the given level.
        /// </summary>
        public List<RangedWeapon> GetAllRangedWeaponsForLevel(int characterLevel)
        {
            // check the parameter
            if (characterLevel <= 0)
            {
                throw new ArgumentOutOfRangeException("characterLevel");
            }

            // go through each level and add the rangedweapons to the output list
            List<RangedWeapon> rangedweapons = new List<RangedWeapon>();

            for (int i = 0; i < characterLevel; i++)
            {
                if (i >= levelEntries.Count)
                {
                    break;
                }

                // add new rangedweapons, and level up existing ones
                foreach (RangedWeapon rangedweapon in levelEntries[i].RangedWeapons)
                {
                    RangedWeapon existingRangedWeapon = rangedweapons.Find(
                        delegate(RangedWeapon testRangedWeapon)
                        {
                            return rangedweapon.AssetName == testRangedWeapon.AssetName; 
                        });
                    if (existingRangedWeapon == null)
                    {
                        rangedweapons.Add(rangedweapon.Clone() as RangedWeapon);
                    }
                    else
                    {
                        existingRangedWeapon.Level++;
                    }
                }
            }

            return rangedweapons;
        }


        #endregion


        #region Value Data


        /// <summary>
        /// The base experience value of Npcs of this character class.
        /// </summary>
        /// <remarks>Used for calculating combat rewards.</remarks>
        private int baseExperienceValue;

        /// <summary>
        /// The base experience value of Npcs of this character class.
        /// </summary>
        /// <remarks>Used for calculating combat rewards.</remarks>
        public int BaseExperienceValue
        {
            get { return baseExperienceValue; }
            set { baseExperienceValue = value; }
        }


        /// <summary>
        /// The base money value of Npcs of this character class.
        /// </summary>
        /// <remarks>Used for calculating combat rewards.</remarks>
        private int baseMoneyValue;

        /// <summary>
        /// The base money value of Npcs of this character class.
        /// </summary>
        /// <remarks>Used for calculating combat rewards.</remarks>
        public int BaseMoneyValue
        {
            get { return baseMoneyValue; }
            set { baseMoneyValue = value; }
        }


        #endregion


        #region Content Type Reader


        /// <summary>
        /// Reads a CharacterClass object from the content pipeline.
        /// </summary>
        public class CharacterClassReader : ContentTypeReader<CharacterClass>
        {
            /// <summary>
            /// Reads a CharacterClass object from the content pipeline.
            /// </summary>
            protected override CharacterClass Read(ContentReader input, 
                CharacterClass existingInstance)
            {
                CharacterClass characterClass = existingInstance;
                if (characterClass == null)
                {
                    characterClass = new CharacterClass();
                }

                characterClass.AssetName = input.AssetName;

                characterClass.Name = input.ReadString();
                characterClass.InitialStatistics = 
                    input.ReadObject<StatisticsValue>();
                characterClass.LevelingStatistics = 
                    input.ReadObject<CharacterLevelingStatistics>();
                characterClass.LevelEntries.AddRange(
                    input.ReadObject<List<CharacterLevelDescription>>());
                characterClass.BaseExperienceValue = input.ReadInt32();
                characterClass.BaseMoneyValue = input.ReadInt32();

                return characterClass;
            }
        }


        #endregion
    }
}
