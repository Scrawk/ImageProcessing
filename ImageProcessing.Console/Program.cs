﻿using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Common.Core.Numerics;
using Common.Core.RandomNum;
using Common.Core.Shapes;
using Common.Core.Colors;
using Common.GraphTheory.GridGraphs;

using ImageProcessing.Images;
using ImageProcessing.Statistics;
using ImageProcessing.Thresholding;
using ImageProcessing.IO;

using CONSOLE = System.Console;


namespace ImageProcessing.Console
{
    public class Program
    {

        private static readonly string TEST_FOLDER = "F:/Projects/Visual Studio Projects/ImageProcessing/ImageProcessing.Test/TestImages/";
        private static readonly string DESKTOP = "C:/Users/Justin/OneDrive/Desktop/";

        public static void Main(string[] args)
        {
            //BenchmarkRunner.Run<BenchMarkTest>();

            //var bmp = new Bitmap(TEST_FOLDER + "PNG/lenna_poster.png");
            //var image = ToImage(bmp);

            /*
            var image = new ColorImage2D();
            image.ReadTGA(TEST_FOLDER + "TGA/lenna_24_ps.tga");

            var param3 = TGAParams.Default;
            param3.Format = PIXEL_FORMAT_IO.BGR;
            param3.FlipY = true;
            param3.RLE = true;

            image.WriteTGA(DESKTOP + "lenna_test.tga", param3);
            */
        }

        public class BenchMarkTest
        {

            [GlobalSetup]
            public void Setup()
            {

            }

            [Benchmark]
            public void Test()
            {

            }
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
