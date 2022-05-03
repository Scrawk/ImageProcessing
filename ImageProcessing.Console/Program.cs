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

            var image = new ColorImage2D(60, 57);

        }

        private static void WriteLine(object obj)
        {
            CONSOLE.WriteLine(obj);
        }

    }
}
