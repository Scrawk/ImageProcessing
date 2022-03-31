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

            var set = new WangTileSet(2, 128);
            WriteLine(set);

            set.Test();

            WriteLine("Done");

            /*
            var timer = new Timer();    
            timer.Start();

            var bitmap = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/TexturesCom_Gravel0160_2_S.jpg"));

            var image = new ColorImage2D(bitmap.Width, bitmap.Height);
            image.Fill((x, y) =>
            {
                return ToColorRGB(bitmap.GetPixel(x, y));
            });

            int overlap = 8;
            int exemplarSize = 128 + overlap;
            int imageSize = 512;

            var exemplars = new ExemplarSet(exemplarSize);
            exemplars.CreateExemplarFromRandom(image, 0, 32);

            var systhesis = new ImageSynthesis(exemplars, imageSize, overlap);

            systhesis.CreateSeamlessImage();

            systhesis.Image.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/Image1.raw");

            timer.Stop();

            WriteLine("Done in " + timer.ElapsedMilliseconds);
            */
        }

    }
}
