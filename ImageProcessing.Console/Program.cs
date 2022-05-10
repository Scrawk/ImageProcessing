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
using ImageProcessing.Statistics;

using CONSOLE = System.Console;

namespace ImageProcessing.Console
{
    class Program
    {

        private static readonly string FOLDER = "C:/Users/Justin/OneDrive/Desktop/";

        static void Main(string[] args)
        {
            var color = new ColorImage2D(256, 256);
            color.LoadFromRaw(FOLDER + "Tile0_Image0.raw", false, BIT_DEPTH.B8);


            var histogram = new ColorHistogram(256);
            histogram.Load(color);


            var image = histogram.HistogramToImage(0, ColorRGBA.White, ColorRGBA.Black, 256);

            image.SaveAsRaw(FOLDER + "himage_" + image.Width + "_" + image.Height);

            WriteLine(histogram);

            /*
            var image = color.ToGreyScaleImage();

            var histogram = new Histogram(256);
            histogram.Load(image);
            WriteLine(histogram);

            var himage = histogram.HistogramToImage(ColorRGBA.White, ColorRGBA.Black, 256);
            var chimage = histogram.CumulativeHistogramToImage(ColorRGBA.White, ColorRGBA.Black, 256);

            WriteLine(himage);
            WriteLine(chimage);

            himage.SaveAsRaw(FOLDER + "himage_" + himage.Width + "_" + himage.Height);
            chimage.SaveAsRaw(FOLDER + "chimage_" + chimage.Width + "_" + chimage.Height);
            */

            WriteLine("Done");
            
        }

        private static void WriteLine(object obj)
        {
            CONSOLE.WriteLine(obj);
        }

    }
}
