

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
    public class CharacterLevelingStatisticsWriter :
        Sector4Writer<CharacterLevelingStatistics>
    {
        protected override void Write(ContentWriter output, 
            CharacterLevelingStatistics value)
        {
            output.Write(value.HealthPointsIncrease);
            output.Write(value.AmmoPointsIncrease);
            output.Write(value.PhysicalOffenseIncrease);
            output.Write(value.PhysicalDefenseIncrease);
            output.Write(value.AmmoalOffenseIncrease);
            output.Write(value.AmmoalDefenseIncrease);

            output.Write(value.LevelsPerHealthPointsIncrease);
            output.Write(value.LevelsPerAmmoPointsIncrease);
            output.Write(value.LevelsPerPhysicalOffenseIncrease);
            output.Write(value.LevelsPerPhysicalDefenseIncrease);
            output.Write(value.LevelsPerAmmoalOffenseIncrease);
            output.Write(value.LevelsPerAmmoalDefenseIncrease);
        }
    }
}
