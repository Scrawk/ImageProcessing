using System;
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

        private static readonly string FOLDER = "C:/Users/Justin/OneDrive/Desktop/";

        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchMarkTest>();
        }

        public class BenchMarkTest
        {

            public ColorImage2D image;

            [GlobalSetup]
            public void Setup()
            {
                image = new ColorImage2D(16, 16);

                var rnd = new Random(0);
                image.Fill((x, y) =>
                {
                    var r = (float)rnd.NextDouble();
                    var g = (float)rnd.NextDouble();
                    var b = (float)rnd.NextDouble();

                    return new ColorRGBA(r, g, b, 1);
                });
            }

            [Benchmark]
            public void GetPixel1()
            {
                int width = image.Width;
                int height = image.Height;  

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var p = image.GetPixel(x, y);
                    }
                }
            }

            [Benchmark]
            public void GetPixel2()
            {
                int width = image.Width;
                int height = image.Height;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float u = x / (width - 1.0f);
                        float v = y / (height - 1.0f);

                        var p = image.GetPixel(u, v);
                    }
                }
            }

            [Benchmark]
            public void GetPixel3()
            {
                int width = image.Width;
                int height = image.Height;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var p = image[x, y];
                    }
                }
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
