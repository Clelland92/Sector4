

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
#endregion

namespace Sector4Processors
{
    public abstract class Sector4Writer<T> : ContentTypeWriter<T>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            Type type = typeof(T);

            string readerText = type.FullName;
            string shortTypeName = type.Name;
            if (shortTypeName.EndsWith("`1"))
            {
                // build the name of a templated type
                shortTypeName = shortTypeName.Substring(0, shortTypeName.Length - 2);
                readerText = readerText.Insert(readerText.IndexOf("`1") + 2, "+" + 
                    shortTypeName + "Reader"); 
            }
            else
            {
                // build the name of a non-templated type
                readerText += "+" + shortTypeName + "Reader";
            }
            readerText += ", Sector4Data";

            // replace the suffix name on the Xbox 360
            // -- since the processor runs on Windows, it needs to reference 
            //    Sector4DataWindows.  However, this means that type.FullName
            //    will specify Sector4Windows in the interior type of templates
            if (targetPlatform == TargetPlatform.Xbox360)
            {
                readerText = readerText.Replace("Windows", "Xbox");
            }

            System.Diagnostics.Debug.WriteLine("Reader:  " + readerText);

            return readerText;
        }


        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            Type type = typeof(T);

            string typeText = type.FullName + ", Sector4Data";

            // replace the suffix name on the Xbox 360
            // -- since the processor runs on Windows, it needs to reference 
            //    Sector4DataWindows.  However, this means that type.FullName
            //    will specify Sector4Windows in the interior type of templates
            if (targetPlatform == TargetPlatform.Xbox360)
            {
                typeText = typeText.Replace("Windows", "Xbox");
            }

            System.Diagnostics.Debug.WriteLine("Type:  " + typeText);

            return typeText;
        }
    }
}
