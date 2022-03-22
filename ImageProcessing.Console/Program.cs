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
            if(obj == null)
                System.Console.WriteLine("Null");
            else
                System.Console.WriteLine(obj.ToString());
        }

        static void Main(string[] args)
        {

            int bufferSize = 8;

            var bitmap1 = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/Grass1_32.png"));
            var bitmap2 = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/Grass2_32.png"));

            int width1 = bitmap1.Width;
            int height1 = bitmap1.Height;

            int width2 = bitmap2.Width;
            int height2 = bitmap2.Height;

            int width = bitmap1.Width + bitmap2.Width;
            int height = bitmap1.Height;

            var image = new ColorImage2D(width - bufferSize, height);

            WriteLine(image);

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


            var grid_graph = new GridGraph(bufferSize, height);

            int offset = width1 - bufferSize;

            grid_graph.Iterate((x, y) =>
            {
                float w1 = image.GetChannel(offset + x, y, 0);

                for (int i = 0; i < 8; i++)
                {
                    int xi = x + D8.OFFSETS[i, 0];
                    int yi = y + D8.OFFSETS[i, 1];

                    if (xi < 0 || xi > grid_graph.Width - 1) continue;
                    if (yi < 0 || yi > grid_graph.Height - 1) continue;

                    float w2 = image.GetChannel(offset + xi, yi, 0);

                    int w = Math.Min(1, (int)((w1 + w2) * 255));

                    grid_graph.AddDirectedWeightedEdge(x, y, i, w);
                }
            });

            int scale = 5;
            image = image.Rescale(scale, RESCALE.POINT);

            /*
            var adj_graph = grid_graph.ToDirectedGraph();

            //adj_graph.Print();

            var source = adj_graph.AddVertex();
            var target = adj_graph.AddVertex();

            //WriteLine("Source " + source);
            //WriteLine("Target " + target);

            for (int y = 0; y < grid_graph.Height; y++)
            {
                int len = (grid_graph.Width - 1);
                int i = y * len;
                int j = len + y * len;

                int w1 = 255;
                int w2 = 255;  

                adj_graph.AddDirectedEdge(source.Index, i, w1);
                adj_graph.AddDirectedEdge(j, target.Index, w2);
            }

            var edges = adj_graph.MinCut(source.Index, target.Index);
            //WriteLine("Edges " + edges.Count);

            Point2i S = new Point2i(offset - 10, image.Height / 2);
            Point2i T = new Point2i(offset + 10, image.Height / 2);

            foreach (var e in edges)
            {

                var from = adj_graph.GetVertex(e.From);
                var to = adj_graph.GetVertex(e.To);

                if (from == source)
                {
                    var to_edges = adj_graph.GetEdges(to.Index);
                    if (to_edges == null || to_edges.Count == 0)
                        continue;

                    var gridData = to_edges[0].Data as GridEdge;
                    if (gridData == null) continue;

                    var v = gridData.To;
                    v.x += offset;

                    image.DrawLine(S, v, ColorRGB.Red);


                }
                else if(from == target)
                {

                }
                else
                {
                    var gridData = e.Data as GridEdge;
                    if (gridData == null) continue;
                    if (image.NotInBounds(gridData.From)) continue;
                    if (image.NotInBounds(gridData.To)) continue;

                    var u = gridData.From;
                    var v = gridData.To;

                    image.SetPixel(offset + u.x, u.y, ColorRGB.Red);
                    image.SetPixel(offset + v.x, v.y, ColorRGB.Red);
                }


            }
            */

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
