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

            var timer = new Timer();    
            timer.Start();

            var bitmap = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/TexturesCom_Gravel0174_1_S.jpg"));

            var image = new ColorImage2D(bitmap.Width, bitmap.Height);
            image.Fill((x, y) =>
            {
                return ToColorRGB(bitmap.GetPixel(x, y));
            });

            var exemplars = new ExemplarSet(128);
            exemplars.CreateExemplarFromRandom(image, 0, 32);

            var pair = ImageSynthesis.CreateSeamlessImageTest(exemplars, 512, 8);

            pair.Item1.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/Image1.raw");
            pair.Item2.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/Image2.raw");
            pair.Item3.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/Mask.raw");

            timer.Stop();

            WriteLine("Done in " + timer.ElapsedMilliseconds);
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
