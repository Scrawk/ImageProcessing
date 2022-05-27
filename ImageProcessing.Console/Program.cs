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
using ImageProcessing.Thresholding;

using CONSOLE = System.Console;


namespace ImageProcessing.Console
{
    class Program
    {

        private static readonly string FOLDER = "C:/Users/Justin/OneDrive/Desktop/";

        static void Main(string[] args)
        {

            var bmp = new Bitmap(FOLDER + "lenna.png");
            var image = ToImage(bmp);
            var grey = image.ToGreyScaleImage();

            var histo = new Histogram(grey, 256);

            var colors = new ColorRGBA[]
            {
                ColorRGBA.Red,
                ColorRGBA.Green,
                ColorRGBA.Blue,
                ColorRGBA.White
            };

            var graph = histo.CreateHistogramBarGraph(ColorRGBA.White, ColorRGBA.Black, 256);

            graph.SaveAsRaw(FOLDER + "image.raw");

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
