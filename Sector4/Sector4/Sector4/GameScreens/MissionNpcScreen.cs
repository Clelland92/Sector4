

#region Using Statements
using System;
using Sector4Data;
#endregion

namespace Sector4
{
    class MissionNpcScreen : NpcScreen<MissionNpc>
    {
       
        public MissionNpcScreen(MapEntry<MissionNpc> mapEntry)
            : base(mapEntry)
        {
            // assign and check the parameter
            MissionNpc missionNpc = character as MissionNpc;
            if (missionNpc == null)
            {
                throw new ArgumentException(
                    "MissionNpcScreen requires a MapEntry with a MissionNpc");
            }

            // check to see if this is NPC is the current mission destination
            if ((Session.Mission != null) && 
                (Session.Mission.Stage == Mission.MissionStage.RequirementsMet) &&
                TileEngine.Map.AssetName.EndsWith(
                    Session.Mission.DestinationMapContentName) &&
                (Session.Mission.DestinationNpcContentName == mapEntry.ContentName))
            {
                // use the mission completion dialog
                this.DialogueText = Session.Mission.CompletionMessage;
                // mark the mission for completion
                // -- the session will not update until the pop-up screens are cleared
                Session.Mission.Stage = Mission.MissionStage.Completed;
            }
            else
            {
                // this NPC is not the destination, so use the npc's welcome text
                this.DialogueText = missionNpc.IntroductionDialogue;
            }
        }
    }
}