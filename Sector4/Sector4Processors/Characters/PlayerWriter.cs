

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
    public class PlayerWriter : Sector4Writer<Player>
    {
        FightingCharacterWriter fightingCharacterWriter = null;

        protected override void Initialize(ContentCompiler compiler)
        {
            fightingCharacterWriter = compiler.GetTypeWriter(typeof(FightingCharacter))
                as FightingCharacterWriter;

            base.Initialize(compiler);
        }

        protected override void Write(ContentWriter output, Player value)
        {
            output.WriteRawObject<FightingCharacter>(value as FightingCharacter, 
                fightingCharacterWriter);
            output.Write(value.Money);
            output.Write(value.IntroductionDialogue);
            output.Write(value.JoinAcceptedDialogue);
            output.Write(value.JoinRejectedDialogue);
            output.Write(value.ActivePortraitTextureName);
            output.Write(value.InactivePortraitTextureName);
            output.Write(value.UnselectablePortraitTextureName);
        }
    }
}
