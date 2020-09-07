using System;

using Common.Core.Numerics;
using Common.Core.Time;
using Common.Geometry.Shapes;

using ImageProcessing.Images;

using CONSOLE = System.Console;

namespace ImageProcessing.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            var k = FilterKernel2D.GaussianKernel(0.75f);

            CONSOLE.WriteLine("Sum = " + k.Sum());

        }
    }
}
