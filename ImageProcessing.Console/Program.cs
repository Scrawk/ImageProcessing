﻿using System;
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

            var bmp1 = new Bitmap(FOLDER + "test.jpg");
            var bmp2 = new Bitmap(FOLDER + "Grass2.bmp");

            var image1 = ToImage(bmp1);
            var image2 = ToImage(bmp2);

            var greyscale = image1.ToGreyScaleImage();

            var dft = greyscale.DFT();

            var idft = dft.iDFT();

            //greyscale = dft.ToGreyScaleImage();
            //greyscale = GreyScaleImage2D.HalfOffset(greyscale);
            //greyscale.Abs();

            idft.SaveAsRaw(FOLDER + "idft");

            /*
            var histo1 = new ColorHistogram(image1, 256);
            var histo2 = new ColorHistogram(image2, 256);

            histo1.Normalize();

            var colors = new ColorRGBA[]
            {
                ColorRGBA.Red,
                ColorRGBA.Green,
                ColorRGBA.Blue,
                ColorRGBA.White
            };

            var lineGraph = histo1.CreateHistogramLineGraph(colors, ColorRGBA.Black, 256);

            lineGraph.SaveAsRaw(FOLDER + "LineGraph");

            //image1.SaveAsRaw(FOLDER + "image1");
            //image2.SaveAsRaw(FOLDER + "image2");
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
