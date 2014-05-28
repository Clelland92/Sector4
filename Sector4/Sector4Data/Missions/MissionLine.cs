

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Sector4Data
{
    /// <summary>
    /// A line of missions, presented to the player in order.
    /// </summary>
    /// <remarks>
    /// In other words, only one mission is presented at a time and 
    /// must be competed before the line can continue.
    /// </remarks>
    public class MissionLine : ContentObject
#if WINDOWS
, ICloneable
#endif
    {
        /// <summary>
        /// The name of the mission line.
        /// </summary>
        private string name;

        /// <summary>
        /// The name of the mission line.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        /// <summary>
        /// An ordered list of content names of missions that will be presented in order.
        /// </summary>
        private List<string> missionContentNames = new List<string>();

        /// <summary>
        /// An ordered list of content names of missions that will be presented in order.
        /// </summary>
        public List<string> MissionContentNames
        {
            get { return missionContentNames; }
            set { missionContentNames = value; }
        }


        /// <summary>
        /// An ordered list of missions that will be presented in order.
        /// </summary>
        private List<Mission> missions = new List<Mission>();

        /// <summary>
        /// An ordered list of missions that will be presented in order.
        /// </summary>
        [ContentSerializerIgnore]
        public List<Mission> Missions
        {
            get { return missions; }
        }
    


        #region Content Type Reader


        /// <summary>
        /// Reads a MissionLine object from the content pipeline.
        /// </summary>
        public class MissionLineReader : ContentTypeReader<MissionLine>
        {
            /// <summary>
            /// Reads a MissionLine object from the content pipeline.
            /// </summary>
            protected override MissionLine Read(ContentReader input, 
                MissionLine existingInstance)
            {
                MissionLine missionLine = existingInstance;
                if (missionLine == null)
                {
                    missionLine = new MissionLine();
                }

                missionLine.AssetName = input.AssetName;

                missionLine.Name = input.ReadString();

                missionLine.MissionContentNames.AddRange(input.ReadObject<List<string>>());
                foreach (string contentName in missionLine.MissionContentNames)
                {
                    
missionLine.missions.Add(input.ContentManager.Load<Mission>(
                        Path.Combine("Missions", contentName)));

                }

                return missionLine;
            }
        }


        #endregion


        #region ICloneable Members


        public object Clone()
        {
            MissionLine missionLine = new MissionLine();

            missionLine.AssetName = AssetName;
            missionLine.name = name;
            missionLine.missionContentNames.AddRange(missionContentNames);
            foreach (Mission mission in missions)
            {
                missionLine.missions.Add(mission.Clone() as Mission);
            }

            return missionLine;
        }


        #endregion
    }
}
