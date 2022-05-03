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

        static void Main(string[] args)
        {

            int w = 32;
            int h = 32;

            var image = ColorImage2D.Random<ColorImage2D>(w, h, 0);

            

            WriteLine(image);


        }

        private static void WriteLine(object obj)
        {
            CONSOLE.WriteLine(obj);
        }

    }
}
