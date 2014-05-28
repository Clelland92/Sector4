

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Sector4Data
{
    /// <summary>
    /// The requirements and rewards for each level for a character class.
    /// </summary>
    public class CharacterLevelDescription
    {
        #region Experience Requirements


        /// <summary>
        /// The amount of additional experience necessary to achieve this level.
        /// </summary>
        private int experiencePoints;

        /// <summary>
        /// The amount of additional experience necessary to achieve this level.
        /// </summary>
        public int ExperiencePoints
        {
            get { return experiencePoints; }
            set { experiencePoints = value; }
        }


        #endregion


        #region RangedWeapon Rewards


        /// <summary>
        /// The content names of the rangedweapons given to the character 
        /// when it reaches this level.
        /// </summary>
        private List<string> rangedweaponContentNames = new List<string>();

        /// <summary>
        /// The content names of the rangedweapons given to the character 
        /// when it reaches this level.
        /// </summary>
        public List<string> RangedWeaponContentNames
        {
            get { return rangedweaponContentNames; }
            set { rangedweaponContentNames = value; }
        }


        /// <summary>
        /// RangedWeapons given to the character when it reaches this level.
        /// </summary>
        private List<RangedWeapon> rangedweapons = new List<RangedWeapon>();

        /// <summary>
        /// RangedWeapons given to the character when it reaches this level.
        /// </summary>
        [ContentSerializerIgnore]
        public List<RangedWeapon> RangedWeapons
        {
            get { return rangedweapons; }
            set { rangedweapons = value; }
        }
        

        #endregion


        #region Content Type Reader


        /// <summary>
        /// Read a CharacterLevelDescription object from the content pipeline.
        /// </summary>
        public class CharacterLevelDescriptionReader : 
            ContentTypeReader<CharacterLevelDescription>
        {
            /// <summary>
            /// Read a CharacterLevelDescription object from the content pipeline.
            /// </summary>
            protected override CharacterLevelDescription Read(ContentReader input,
                CharacterLevelDescription existingInstance)
            {
                CharacterLevelDescription desc = existingInstance;
                if (desc == null)
                {
                    desc = new CharacterLevelDescription();
                }

                desc.ExperiencePoints = input.ReadInt32();
                desc.RangedWeaponContentNames.AddRange(input.ReadObject<List<string>>());

                // load all of the rangedweapons immediately
                foreach (string rangedweaponContentName in desc.RangedWeaponContentNames)
                {
                    desc.rangedweapons.Add(input.ContentManager.Load<RangedWeapon>(
                        System.IO.Path.Combine("RangedWeapons", rangedweaponContentName)));
                }

                return desc;
            }
        }


        #endregion
    }
}