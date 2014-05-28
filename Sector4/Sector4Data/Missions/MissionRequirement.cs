

#region Using Statements
using System;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Sector4Data
{
    /// <summary>
    /// A requirement for a particular number of a piece of content.
    /// </summary>
    /// <remarks>Used to track gear acquired and monsters killed.</remarks>
    public class MissionRequirement<T> : ContentEntry<T> where T : ContentObject
    {
        /// <summary>
        /// The quantity of the content entry that has been acquired.
        /// </summary>
        private int completedCount;

        /// <summary>
        /// The quantity of the content entry that has been acquired.
        /// </summary>
        [ContentSerializerIgnore]
        public int CompletedCount
        {
            get { return completedCount; }
            set { completedCount = value; }
        }


        #region Content Type Reader


        /// <summary>
        /// Reads a MissionRequirement object from the content pipeline.
        /// </summary>
        public class MissionRequirementReader : ContentTypeReader<MissionRequirement<T>>
        {
            /// <summary>
            /// Reads a MissionRequirement object from the content pipeline.
            /// </summary>
            protected override MissionRequirement<T> Read(ContentReader input,
                MissionRequirement<T> existingInstance)
            {
                MissionRequirement<T> requirement = existingInstance;
                if (requirement == null)
                {
                    requirement = new MissionRequirement<T>();
                }

                input.ReadRawObject<ContentEntry<T>>(requirement as ContentEntry<T>);
                if (typeof(T) == typeof(Gear))
                {
                    requirement.Content = input.ContentManager.Load<T>(
                        System.IO.Path.Combine("Gear", requirement.ContentName));
                }
                else if (typeof(T) == typeof(Monster))
                {
                    requirement.Content = input.ContentManager.Load<T>(
                        System.IO.Path.Combine(@"Characters\Monsters", 
                        requirement.ContentName));
                }

                return requirement;
            }
        }


        #endregion
    }
}
