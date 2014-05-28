

#region Using Statements
using System;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Sector4Data
{
    /// <summary>
    /// An NPC that does not fight and does not join the party.
    /// </summary>
    public class MissionNpc : Character
    {
        #region Dialogue Data


        /// <summary>
        /// The dialogue that the Npc says when it is greeted in the world.
        /// </summary>
        private string introductionDialogue;

        /// <summary>
        /// The dialogue that the Npc says when it is greeted in the world.
        /// </summary>
        public string IntroductionDialogue
        {
            get { return introductionDialogue; }
            set { introductionDialogue = value; }
        }


        #endregion


        #region Content Type Reader


        /// <summary>
        /// Read a MissionNpc object from the content pipeline.
        /// </summary>
        public class MissionNpcReader : ContentTypeReader<MissionNpc>
        {
            protected override MissionNpc Read(ContentReader input, 
                MissionNpc existingInstance)
            {
                MissionNpc missionNpc = existingInstance;
                if (missionNpc == null)
                {
                    missionNpc = new MissionNpc();
                }

                input.ReadRawObject<Character>(missionNpc as Character);

                missionNpc.IntroductionDialogue = input.ReadString();

                return missionNpc;
            }
        }


        #endregion
    }
}
