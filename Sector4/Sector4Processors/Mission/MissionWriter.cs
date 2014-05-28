

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Sector4Data;
#endregion

namespace Sector4Processors
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class MissionWriter : Sector4Writer<Mission>
    {
        protected override void Write(ContentWriter output, Mission value)
        {
            output.Write(value.Name);
            output.Write(value.Description);
            output.Write(value.ObjectiveMessage);
            output.Write(value.CompletionMessage);
            output.WriteObject(value.GearRequirements);
            output.WriteObject(value.MonsterRequirements);
            output.WriteObject(value.FixedCombatEntries);
            output.WriteObject(value.ChestEntries);
            output.Write(value.DestinationMapContentName);
            output.Write(value.DestinationNpcContentName);
            output.Write(value.DestinationObjectiveMessage);
            output.Write(value.ExperienceReward);
            output.Write(value.MoneyReward);
            output.WriteObject(value.GearRewardContentNames);
        }
    }
}
