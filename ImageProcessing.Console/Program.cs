using System;
using System.IO;

using Common.Core.Numerics;
using Common.Core.Time;
using Common.Core.Shapes;

using ImageProcessing.Images;

using CONSOLE = System.Console;

namespace ImageProcessing.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            string fileIn = "D:/Terrain/Esperance/Cut2/Esperance_WaterMask_Co_4m.raw";
            string fileOut = "D:/Terrain/Esperance/Cut2/Esperance_WaterDepth_Co_4m.raw";

            var bytes = File.ReadAllBytes(fileIn);

            var binary = new BinaryImage2D(256, 256);
            binary.FromBytes(bytes, 8);

            int border = 256;
            var bounds = binary.Bounds;

            var expand = Box2i.Expand(bounds, border);

            binary = BinaryImage2D.Crop(binary, expand);

            var distance = BinaryImage2D.ApproxEuclideanDistance(binary);

            distance = GreyScaleImage2D.Crop(distance, bounds + (border, border));

            distance.Modify(x => MathUtil.Log(1.0f + x * 0.1f));

            distance.Normalize();

            bytes = distance.ToBytes(16);
            File.WriteAllBytes(fileOut, bytes);

        }
    }
}
