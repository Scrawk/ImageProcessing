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

        private static ColorImage2D ToImage(Bitmap bitmap)
        {
            var image = new ColorImage2D(bitmap.Width, bitmap.Height);
            image.Fill((x, y) =>
            {
                return ToColorRGB(bitmap.GetPixel(x, y));
            });

            return image;
        }

        static void Main(string[] args)
        {

            var bitmap = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/TexturesCom_Gravel0101_2_S.jpg"));
            var source = ToImage(bitmap);

            var timer = new Timer();
            timer.Start();

            var set = new WangTileSet(3, 2, 128);
            set.CreateTiles(source, 0, true);

            var orthogonalTiling = set.CreateTilesImage();
            WriteLine(orthogonalTiling);
            orthogonalTiling.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/OrthogonalTiling.raw");

            var sequentialTiling = set.CreateTileMappingImage(256, 256, 0);
            WriteLine(sequentialTiling);
            sequentialTiling.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/SequentialTiling.raw");

            timer.Stop();
            WriteLine("Done in " + timer.ElapsedSeconds);

        }

    }
}
