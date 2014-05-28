

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
    public class RangedWeaponWriter : Sector4Writer<RangedWeapon>
    {
        protected override void Write(ContentWriter output, RangedWeapon value)
        {
            output.Write(value.Name);
            output.Write(value.Description);
            output.Write(value.AmmoPointCost);
            output.Write(value.IconTextureName);
            output.Write(value.IsOffensive);
            output.Write(value.TargetDuration);
            output.WriteObject(value.InitialTargetEffectRange);
            output.Write(value.AdjacentTargets);
            output.WriteObject(value.LevelingProgression);
            output.Write(value.CreatingCueName);
            output.Write(value.TravelingCueName);
            output.Write(value.ImpactCueName);
            output.Write(value.BlockCueName);
            output.WriteObject(value.RangedWeaponSprite);
            output.WriteObject(value.Overlay);
        }
    }
}
