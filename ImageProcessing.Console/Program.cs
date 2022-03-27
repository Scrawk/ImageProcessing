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

        static void Main(string[] args)
        {

            const int imageSize = 256;
            const int graphSize = 32;
       
            var bitmap = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/Test_Gravel_512.png"));
            int width = bitmap.Width;
            int height = bitmap.Height;

            var image = new ColorImage2D(width, height);
            image.Fill((x, y) =>
            {
                return ToColorRGB(bitmap.GetPixel(x, y));
            });

            var image1 = ColorImage2D.Crop(image, new Box2i(0, 0, width / 2, height / 2));
            var image2 = ColorImage2D.Crop(image, new Box2i(width / 2, 0, width, height / 2));
            var image3 = ColorImage2D.Crop(image, new Box2i(0, height / 2, width / 2, height));
            var image4 = ColorImage2D.Crop(image, new Box2i(width / 2, height / 2, width, height));

            image = new ColorImage2D(imageSize * 2 - graphSize, imageSize);
            int offset = imageSize - graphSize;

            WriteLine("Image " + image);

            image1.Iterate((x, y) =>
            {
                image[x, y] = image1[x, y];
            });

            image4.Iterate((x, y) =>
            {
                image[offset + x, y] = image4[x, y];
            });

            var graph = new GridGraph(graphSize, imageSize);

            graph.Iterate((x, y, i) =>
             {
                 int xi = x + D8.OFFSETS[i, 0];
                 int yi = y + D8.OFFSETS[i, 1];

                 var col1 = image1[offset + x, y];
                 var col4 = image4[x, y];

                 var w1 = ColorRGB.SqrDistance(col1, col4);

                 if (graph.InBounds(xi, yi))
                 {
                     var col1i = image1[offset + xi, yi];
                     var col4i = image4[xi, yi];

                     var w2 = ColorRGB.SqrDistance(col1i, col4i);
                     var w = Math.Max(1, (w1+w2) * 255);

                     graph.AddDirectedWeightedEdge(x, y, i, w);
                 }
             });


            var search = new GridSearch(graph.Width, graph.Height);
            var path = new List<Point2i>();
            float cost = float.PositiveInfinity;
            int samples = 4;

            for (int x = 0; x < samples; x++)
            {
                for (int y = 0; y < samples; y++)
                {
                    int incrementS = (graph.Width / samples) * x;
                    int incrementT = (graph.Width / samples) * y;

                    var source = new Point2i(incrementS, 0);
                    var target = new Point2i(incrementT, graph.Height - 1);

                    search.Clear();
                    graph.PrimsMinimumSpanningTree(search, source.x, source.y);

                    var p = search.GetPath(target);

                    if (p.Count == 0)
                    {
                        //WriteLine("Empty path");
                        continue;
                    }

                    if (p[0] != target)
                    {
                        //WriteLine("Path did not reach target");
                        continue;
                    }

                    float c = search.GetPathCost(p, graph);

                    if (c < cost)
                    {
                        cost = c;
                        path = p;
                    }
                }
            }

            foreach (var p in path)
            {
                for (int i = 0; i < p.x; i++)
                {
                    var pixel = image1[offset + i, p.y];
                    image.SetPixel(offset + i, p.y, pixel);
                }
            }

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
