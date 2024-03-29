

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
    public class GameStartDescriptionWriter : Sector4Writer<GameStartDescription>
    {
        protected override void Write(ContentWriter output, GameStartDescription value)
        {
            if (value.PlayerContentNames.Count <= 0)
            {
                throw new ArgumentException(
                    "The starting party must have at least one player in it.");
            }

            output.Write(value.MapContentName);
            output.WriteObject(value.PlayerContentNames);
            output.Write(value.MissionLineContentName);
        }
    }
}
