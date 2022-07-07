using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Common.Core.Numerics;
using Common.Core.RandomNum;
using Common.Core.Shapes;
using Common.Core.Colors;
using Common.Core.Extensions;

using ImageProcessing.Images;
using ImageProcessing.Statistics;
using ImageProcessing.Thresholding;
using ImageProcessing.IO;

using CONSOLE = System.Console;


namespace ImageProcessing.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
        }


        /*
        private static readonly string TEST_FOLDER = "F:/Projects/Visual Studio Projects/ImageProcessing/ImageProcessing.Test/TestImages/";
        private static readonly string DESKTOP = "C:/Users/Justin/OneDrive/Desktop/";

        public struct Partical
        {
            public Point2f position;
            public ColorRGBA color;
            public float time;
        }

        public static void Main(string[] args)
        {
            int width = 1024;
            int height = width;
            float scale = 10;

            var vector = new VectorImage2D(width, height);

            vector.Fill((x, y) =>
            {
                double u = x / (vector.Width - 1.0);
                double v = y / (vector.Height - 1.0);

                u *= scale;
                v *= scale;

                return Function1(u, v);
            });

            var max = vector.MaxMagnitude();
            vector.Normalize(max);

            var map = new ColorImage2D(width, height);
            map.Fill(ColorRGBA.LightGrey);
            var particals = new List<Partical>(vector.Size.Product);

            var rng = new Random(0);

            vector.Iterate((x, y) =>
            {
                var p = new Partical();
                p.position = new Point2f(x, y);
                p.color = rng.NextColorRGBA();
                particals.Add(p);
            });

            particals.Shuffle(rng);
            int numPaths = particals.Count / 1000;

            var paths = new List<List<Partical>>();

            for (int i = 0; i < numPaths; i++)
                paths.Add(new List<Partical>());

            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < numPaths; j++)
                {
                    var p = particals[j];
                    int x = (int)p.position.x;
                    int y = (int)p.position.y;

                    float u = x / (map.Width - 1.0f);
                    float v = y / (map.Height - 1.0f);

                    var vec = vector.GetValue(u, v);
                    //var vec = Function1(x, y);   

                    p.position += vec;
                    particals[j] = p;

                    paths[j].Add(p);
                }
            }

            foreach (var path in paths)
            {
                for (int i = 0; i < path.Count-1; i++)
                {
                    var p1 = path[i];
                    var p2 = path[i+1];

                    map.DrawLine(p1.position, p2.position, p2.color);
                }
            }

            var param = TGAParams.Default;
            map.WriteTGA(DESKTOP + "VectorMap", param);

            WriteLine("Done");
        }

        private static Vector2f Function1(double x, double y)
        {
            double i = Math.Sin(x) + Math.Sin(y);
            double j = Math.Sin(x) - Math.Sin(y);

            return new Vector2f(i, j);
        }

        private static Vector2f Function2(int x, int y)
        {
            double fx = x * 1;
            double fy = y * 1;
            double i = Math.Sin(fx + fy);
            double j = Math.Cos(fx - fy);

            return new Vector2f(i, j);
        }

        private static Vector2f Function3(int x, int y)
        {
            double fx = x * 0.5;
            double fy = y * 0.5;

            return new Vector2f(fy, -fx);
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

        private static void WriteLine(object obj)
        {
            CONSOLE.WriteLine(obj);
        }
        */

    }
}
