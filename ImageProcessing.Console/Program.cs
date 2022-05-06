using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Time;
using Common.Core.Shapes;
using Common.Core.Colors;
using Common.GraphTheory.GridGraphs;

using ImageProcessing.Images;

using CONSOLE = System.Console;

namespace ImageProcessing.Console
{
    class Program
    {

        private static readonly string FOLDER = "C:/Users/Justin/OneDrive/Desktop/";

        static void Main(string[] args)
        {
            var circle = new Circle2f(32, 32, 16);
            var box = new Box2f(-8, -8, 8, 8);

            var greyscale = new GreyScaleImage2D(64, 64);

            greyscale.DrawBox(box, ColorRGBA.White, true, WRAP_MODE.WRAP, BLEND_MODE.ALPHA);

            greyscale.SaveAsRaw(FOLDER + "greyscale");
            
        }

        private static void WriteLine(object obj)
        {
            CONSOLE.WriteLine(obj);
        }

    }
}
