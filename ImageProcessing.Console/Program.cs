using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Time;
using Common.Core.Shapes;
using Common.Core.Colors;
using Common.Core.Directions;
using Common.GraphTheory.AdjacencyGraphs;
using Common.GraphTheory.GridGraphs;

using ImageProcessing.Images;

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

        private static float GetWeight(Bitmap bm, ColorImage2D ci, int x, int y)
        {
            var col1 = ToColorRGB(bm.GetPixel(x, y));
            var col2 = ci.GetPixel(x, y, WRAP_MODE.CLAMP);

            var sd = ColorRGB.SqrDistance(col1, col2);

            return sd;
        }

        static void Main(string[] args)
        {

            const int cutSize = 32;
            const int bufferSize = 2;

            var bitmap = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/Test_Gravel_512.png"));

            int width = bitmap.Width;
            int height = bitmap.Height;

            var image = new ColorImage2D(width, height);

            WriteLine("Image " + image);

            image.Fill((x, y) =>
            {

                ColorRGB pixel = new ColorRGB();

                if (x < width - cutSize)
                {
                    pixel = ToColorRGB(bitmap.GetPixel(x, y));
                }
                else if (x >= width - cutSize)
                {
                    pixel = ToColorRGB(bitmap.GetPixel(width - 1 - x, y));
                }

                return pixel;
            });

            var graph = new GridGraph(cutSize - bufferSize, height);
            int offset = width - cutSize;

            graph.Iterate((x, y, i) =>
             {
                 int xi = x + D8.OFFSETS[i, 0];
                 int yi = y + D8.OFFSETS[i, 1];

                 if (graph.InBounds(xi, yi))
                 {
                     float w1 = GetWeight(bitmap, image, offset + x, y);
                     float w2 = GetWeight(bitmap, image, offset + xi, yi);
                     float w = Math.Max(1, (int)((w1 + w2) * 255));

                     graph.AddDirectedWeightedEdge(x, y, i, w);
                 }

             });

            //graph.Print();
            
            var source = new Point2i(graph.Width/2, 0);
            var target = new Point2i(graph.Width / 2, graph.Height-1);

            var search = graph.PrimsMinimumSpanningTree(source.x, source.y);

            //search.Print();

            var path = search.GetPath(target);

            foreach(var p in path)
            {
                for (int i = p.x; i >= 0; i--)
                {
                    var pixel = ToColorRGB(bitmap.GetPixel(offset + i, p.y));
                    image.SetPixel(offset + i, p.y, pixel);
                }

                for (int i = p.x; i < graph.Width; i++)
                {
                    //var pixel = ToColorRGB(bitmap.GetPixel(i, p.y));
                    //image.SetPixel(offset + i, p.y, pixel);
                }
            }

            //WriteLine("Path " + path.Count);
            //DrawPath(graph, path, image, offset);


            var filename = "C:/Users/Justin/OneDrive/Desktop/Test.raw";
            var bytes = image.ToBytes(8);
            File.WriteAllBytes(filename, bytes);

            CONSOLE.WriteLine(filename);
            
        }

        private static void DrawPath(GridGraph graph, List<Point2i> path, ColorImage2D image, int offset)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var p = path[i];
                image.SetPixel(offset + p.x, p.y, ColorRGB.Red);
            }
        }

    }
}
