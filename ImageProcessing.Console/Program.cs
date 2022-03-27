using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Time;
using Common.Core.Shapes;
using Common.Core.Colors;
using Common.Core.Directions;
using Common.GraphTheory.AdjacencyGraphs;
using Common.GraphTheory.GridGraphs;

using ImageProcessing.Images;
using ImageProcessing.Synthesis;

using CONSOLE = System.Console;

namespace ImageProcessing.Console
{
    class Program
    {
        static void WriteLine(object obj)
        {
            if (obj == null)
                System.Console.WriteLine("Null");
            else
                System.Console.WriteLine(obj.ToString());
        }

        private static ColorRGB ToColorRGB(Color pixel)
        {
            float r = pixel.R / 255.0f;
            float g = pixel.G / 255.0f;
            float b = pixel.B / 255.0f;

            return new ColorRGB(r, g, b);
        }

        static void Main(string[] args)
        {

            var bitmap = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/Test_Gravel_512.png"));

            var image = new ColorImage2D(bitmap.Width, bitmap.Height);
            image.Fill((x, y) =>
            {
                return ToColorRGB(bitmap.GetPixel(x, y));
            });

            image = ImageSynthesis.CreateSeamlessImage(image, 128, 8, 6);

            var filename = "C:/Users/Justin/OneDrive/Desktop/Test.raw";
            var bytes = image.ToBytes(8);
            File.WriteAllBytes(filename, bytes);

            CONSOLE.WriteLine(filename);

        }

        private static void DrawPath(GridGraph graph, List<Point2i> path, ColorImage2D image, int offset)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var p = path[i];
                image.SetPixel(offset + p.x, p.y, ColorRGB.Red);
            }
        }

    }
}
