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
            var bmp1 = new Bitmap(FOLDER + "Grass1.bmp");
            var bmp2 = new Bitmap(FOLDER + "Grass2_dark.bmp");

            var image1 = ToImage(bmp1);
            var image2 = ToImage(bmp2);

            var chisto1 = new ColorHistogram(image1, 256);
            var chisto2 = new ColorHistogram(image2, 256);

            chisto2.Match(chisto1);

            var image = chisto2.ToImage();
            image.SaveAsRaw(FOLDER + "matched");

            //var greyscale1 = image1.ToGreyScaleImage();
            //var greyscale2 = image2.ToGreyScaleImage();

            //var histo1 = new Histogram(greyscale1, 256);
            //var histo2 = new Histogram(greyscale2, 256);

            //var lineGraph1 = histo1.CreateHistogramLineGraph(ColorRGBA.White, ColorRGBA.Black, 256);
            //var lineGraph2 = histo2.CreateHistogramLineGraph(ColorRGBA.White, ColorRGBA.Black, 256);

            //lineGraph1.SaveAsRaw(FOLDER + "lineGraph1");
            //lineGraph2.SaveAsRaw(FOLDER + "lineGraph2_before");

            //histo2.Match(histo1);

            //greyscale2 = histo2.ToImage();

            //greyscale2.SaveAsRaw(FOLDER + "matched");

            //lineGraph2 = histo2.CreateHistogramLineGraph(ColorRGBA.White, ColorRGBA.Black, 256);
            //lineGraph2.SaveAsRaw(FOLDER + "lineGraph2_after");

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
