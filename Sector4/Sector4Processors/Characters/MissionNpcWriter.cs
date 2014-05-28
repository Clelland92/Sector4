

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
    public class MissionNpcWriter : Sector4Writer<MissionNpc>
    {
        CharacterWriter characterWriter = null;

        protected override void Initialize(ContentCompiler compiler)
        {
            characterWriter = compiler.GetTypeWriter(typeof(Character)) 
                as CharacterWriter;

            base.Initialize(compiler);
        }

        protected override void Write(ContentWriter output, MissionNpc value)
        {
            output.WriteRawObject<Character>(value as Character, characterWriter);
            output.Write(value.IntroductionDialogue);
        }
    }
}
