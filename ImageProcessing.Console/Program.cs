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
        static void Main(string[] args)
        {


            int bufferSize = 32;

            var bitmap1 = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/Grass1.png"));
            var bitmap2 = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/Grass2.png"));

            int width1 = bitmap1.Width;
            int height1 = bitmap1.Height;

            int width2 = bitmap2.Width;
            int height2 = bitmap2.Height;

            int width = bitmap1.Width + bitmap2.Width;
            int height = bitmap1.Height;

            var image = new ColorImage2D(width - bufferSize, height);

            image.Fill((x, y) => 
            {

                ColorRGB pixel = new ColorRGB();

                if (x < width1 - bufferSize)
                {
                    pixel = ToColorRGB(bitmap1.GetPixel(x, y));
                }
                else if (x >= width1 - bufferSize && x < width1)
                {
                    var pixel1 = bitmap1.GetPixel(x, y);
                    var pixel2 = bitmap2.GetPixel(x - width1 + bufferSize, y);

                    var col1 = ToColorRGB(pixel1);
                    var col2 = ToColorRGB(pixel2);

                    var sd = ColorRGB.SqrDistance(col1, col2);

                    pixel = new ColorRGB(sd);

                }
                else if(x >= width1)
                {
                    pixel = ToColorRGB(bitmap2.GetPixel(x - width1, y));
                }

                return pixel; 
            });


            var graph = new GridGraph(bufferSize, height);

            int offset = width1 - bufferSize;

            for(int x = 0; x < graph.Width; x++)
            {
                for (int y = 0; y < graph.Height; y++)
                {
                    float w1 = image.GetChannel(offset + x, y, 0);

                    for(int i = 0; i < 8; i++)
                    {
                        int xi = x + D8.OFFSETS[i, 0];
                        int yi = y + D8.OFFSETS[i, 1];

                        if (xi < 0 || xi > graph.Width - 1) continue;
                        if (yi < 0 || yi > graph.Height - 1) continue;

                        float w2 = image.GetChannel(offset + xi, yi, 0);

                        int w = (int)((w1 + w2) * 128);

                        graph.AddDirectedWeightedEdge(x, y, i, w);
    
                    }
                    
                }
            }

            var source = new Point2i(0, graph.Height / 2);
            var target = new Point2i(graph.Width-1, graph.Height / 2);

            //var source = new Point2i(graph.Width / 2, 0);
            //var target = new Point2i(graph.Width / 2, graph.Height-1);

            image.SetPixel(offset + source.x, source.y, ColorRGB.Red);
            image.SetPixel(offset + target.x, target.y, ColorRGB.Red);

            /*
            GridSearch search = new GridSearch(graph.Width, graph.Height);
            //graph.PrimsMinimumSpanningTree(search, source.x, source.y);
            //graph.BreadthFirst(search, source.x, source.y);

            var path = new List<Point2i>();
            search.GetPath(target, path);

            foreach(var p in path)
            {
                image.SetPixel(offset + p.x, p.y, ColorRGB.Red);
            }
            */

            var edges = graph.MinCut2(source, target);
            CONSOLE.WriteLine("Edges " + edges.Count);

            foreach (var edge in edges)
            {
                var e = edge.Data as GridEdge;

                image.SetPixel(offset + e.From.x, e.From.y, ColorRGB.Red);
            }

            var filename = "C:/Users/Justin/OneDrive/Desktop/Test.raw";
            var bytes = image.ToBytes(8);
            File.WriteAllBytes(filename, bytes);

            CONSOLE.WriteLine(filename);

        }

        private static ColorRGB ToColorRGB(Color pixel)
        {
            float r = pixel.R / 255.0f;
            float g = pixel.G / 255.0f;
            float b = pixel.B / 255.0f;

            return new ColorRGB(r, g, b);
        }
    }
}
