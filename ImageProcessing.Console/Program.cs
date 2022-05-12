using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.RandomNum;
using Common.Core.Shapes;
using Common.Core.Colors;
using Common.GraphTheory.GridGraphs;

using ImageProcessing.Images;
using ImageProcessing.Statistics;

using CONSOLE = System.Console;


namespace ImageProcessing.Console
{
    class Program
    {

        private static readonly string FOLDER = "C:/Users/Justin/OneDrive/Desktop/";

        static void Main(string[] args)
        {

            int width = 128;
            int height = 128;


            var random = new GreyScaleImage2D(width, height);
            var gaussion = new GreyScaleImage2D(width, height);
            var possion = new GreyScaleImage2D(width, height);

            var rnd = new SystemRandom(0);

            random.Iterate((x, y) =>
            {
                var u = rnd.NextFloat();
                var g = (float)Math.Abs(rnd.NextGaussian(0, 1));
                var p = (float)rnd.NextPoisson(1000);

                random[x, y] = u;
                gaussion[x, y] = g;
                possion[x, y] = p;

            });

            possion.MinMax(out float min, out float max);

            WriteLine(min + " "  + max);

            possion.Normalize();

            var random_dft = GreyScaleImage2D.HalfOffset(random.DFT().ToGreyScaleImage());
            var gaussion_dft = GreyScaleImage2D.HalfOffset(gaussion.DFT().ToGreyScaleImage());
            var possion_dft = GreyScaleImage2D.HalfOffset(possion.DFT().ToGreyScaleImage());

            //possion_dft.Normalize();
            //gaussion.Normalize();

            random_dft.SaveAsRaw(FOLDER + "random_dft");
            gaussion_dft.SaveAsRaw(FOLDER + "gaussion_dft");
            possion_dft.SaveAsRaw(FOLDER + "possion_dft");

            /*
            var bmp1 = new Bitmap(FOLDER + "test.jpg");
            var bmp2 = new Bitmap(FOLDER + "Grass2.bmp");

            var image1 = ToImage(bmp1);
            var image2 = ToImage(bmp2);

            var greyscale = image1.ToGreyScaleImage();

            var dft = greyscale.DFT();

            var idft = dft.iDFT();

            dft.ToGreyScaleImage().SaveAsRaw(FOLDER + "dft");
            idft.SaveAsRaw(FOLDER + "idft");
            */

            WriteLine("Done");
            
        }

        private static ColorImage2D ToImage(Bitmap bm)
        {
            int width = bm.Width;
            int height = bm.Height; 

            var image = new ColorImage2D(width, height);

            image.Fill((x, y) =>
            {
                var color = bm.GetPixel(x, y);
                return new ColorRGBA(color.R, color.G, color.B, color.A) / 255.0f;
            });

            return image;
        }

        private static void WriteLine(object obj)
        {
            CONSOLE.WriteLine(obj);
        }

    }
}
