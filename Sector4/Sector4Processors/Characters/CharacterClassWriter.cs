

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
    public class CharacterClassWriter : Sector4Writer<CharacterClass>
    {
        protected override void Write(ContentWriter output, CharacterClass value)
        {
            output.Write(value.Name);
            output.WriteObject(value.InitialStatistics);
            output.WriteObject(value.LevelingStatistics);
            output.WriteObject(value.LevelEntries);
            output.Write(value.BaseExperienceValue);
            output.Write(value.BaseMoneyValue);
        }
    }
}
