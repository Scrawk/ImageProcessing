using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;
using Common.Core.Directions;
using Common.GraphTheory.AdjacencyGraphs;
using Common.GraphTheory.GridGraphs;

using ImageProcessing.Images;

namespace ImageProcessing.Synthesis
{

    public static class ImageSynthesis
    {
        public static (ColorImage2D, ColorImage2D, GreyScaleImage2D) CreateSeamlessImageTest(ExemplarSet exemplars, int imageSize, int overlap)
        {

            int tilesX = imageSize / exemplars.Size;
            int tilesY = imageSize / exemplars.Size;

            int width = imageSize - tilesX * overlap;
            int height = imageSize - tilesY * overlap;

            var image1 = new ColorImage2D(width, height);
            var image2 = new ColorImage2D(width, height);
            var mask = new GreyScaleImage2D(width, height);

            int countX = image1.Width / (exemplars.Size - overlap);
            int countY = image1.Height / (exemplars.Size - overlap);

            int cutSamples = 4;

            var matches = new ColorImage2D[countX, countY];

            for (int y = 0; y <= countY; y++)
            {
                if(y == 0)
                {
                    TileHorizontally(image1, image2, mask, y, exemplars, matches, overlap);
                    GraphCutVertical(image1, image2, y, exemplars.Size, overlap, cutSamples);
                }
                else if(y == countY)
                {
                    GraphCutHorizontally(image1, image2, 0, exemplars.Size, overlap, cutSamples);
                }
                else
                {
                    TileHorizontally(image1, image2, mask, y, exemplars, matches, overlap);
                    GraphCutVertical(image1, image2, y, exemplars.Size, overlap, cutSamples);
                    GraphCutHorizontally(image1, image2, y, exemplars.Size, overlap, cutSamples);
                }
            }

            Console.WriteLine(image1);

            return (image1, image2, mask);

        }

        private static void TileHorizontally(ColorImage2D image1, ColorImage2D image2, GreyScaleImage2D mask, int indexY, ExemplarSet exemplars, ColorImage2D[,] matches, int overlap)
        {

            if (exemplars.Count == 0)
                throw new ArgumentException("Exemplar set empty");

            int offset = exemplars.Size - overlap;
            int count = image1.Width / (exemplars.Size - overlap);
            Point2i start = new Point2i(0, offset * indexY);
 
            for (int k = 0; k < count; k++)
            {
                start.x = offset * k;

                ColorImage2D match = null;

                if(k == 0 && indexY == 0)
                {
                    match = exemplars.GetExemplar(0);
                }
                else
                {
                    var current = new Point2i(k, indexY);
                    match = exemplars.FindBestMatch(image1, mask, matches, current, start);
                }

                for (int y = 0; y < exemplars.Size; y++)
                {
                    for (int x = 0; x < match.Width; x++)
                    {
                        int i = start.x + x;
                        int j = start.y + y;
   
                        image1.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                        mask.SetPixel(i, j, ColorRGB.White, WRAP_MODE.WRAP);
                    }
                }

                matches[k, indexY] = match;
            }

            for (int k = 0; k < count; k++)
            {
                start.x = offset * k;
                var match = matches[k, indexY];

                if(indexY == 0)
                {
                    for (int y = 0; y < exemplars.Size; y++)
                    {
                        if (k == 0)
                        {
                            for (int x = 0; x < match.Width; x++)
                            {
                                int i = start.x + x;
                                int j = start.y + y;
                                image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        if (k == count - 1)
                        {
                            for (int x = overlap; x < match.Width - overlap; x++)
                            {
                                int i = start.x + x;
                                int j = start.y + y;
                                image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        else
                        {
                            for (int x = overlap; x < match.Width; x++)
                            {
                                int i = start.x + x;
                                int j = start.y + y;
                                image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                            }
                        }
                    }
                }
                else if (indexY == count-1)
                {
                    for (int y = overlap; y < exemplars.Size - overlap; y++)
                    {
                        if (k == 0)
                        {
                            for (int x = 0; x < match.Width; x++)
                            {
                                int i = start.x + x;
                                int j = start.y + y;
                                image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        if (k == count - 1)
                        {
                            for (int x = overlap; x < match.Width - overlap; x++)
                            {
                                int i = start.x + x;
                                int j = start.y + y;
                                image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        else
                        {
                            for (int x = overlap; x < match.Width; x++)
                            {
                                int i = start.x + x;
                                int j = start.y + y;
                                image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                            }
                        }
                    }
                }
                else
                {
                    for (int y = overlap; y < exemplars.Size; y++)
                    {
                        if (k == 0)
                        {
                            for (int x = 0; x < match.Width; x++)
                            {
                                int i = start.x + x;
                                int j = start.y + y;
                                image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        if (k == count - 1)
                        {
                            for (int x = overlap; x < match.Width - overlap; x++)
                            {
                                int i = start.x + x;
                                int j = start.y + y;
                                image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        else
                        {
                            for (int x = overlap; x < match.Width; x++)
                            {
                                int i = start.x + x;
                                int j = start.y + y;
                                image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                            }
                        }
                    }
                }
               
            }

        }

        private static void GraphCutVertical(ColorImage2D image1, ColorImage2D image2, int indexY, int exemplarsSize, int overlap, int samples)
        {
            int offset = exemplarsSize - overlap;
            int count = image1.Width / (exemplarsSize - overlap);
            int startY = offset * indexY;

            for (int j = 0; j < count; j++)
            {
                int startX = offset * j;

                var bounds = new Box2i(startX, startY, startX + overlap, startY + exemplarsSize);
                var graph = CreateGraph(image1, image2, bounds);
                var path = FindBestCut(graph, samples, true);

                if (j != 0)
                {
                    foreach (var p in path)
                    {
                        for (int i = 0; i < p.x; i++)
                        {
                            var pixel = image2.GetPixel(startX + i, startY + p.y, WRAP_MODE.WRAP);
                            image1.SetPixel(startX + i, startY + p.y, pixel, WRAP_MODE.WRAP);
                        }
                    }
                            
                }
                else
                {
                    foreach (var p in path)
                    {
                        for (int i = p.x; i < overlap; i++)
                        {
                            var pixel = image2.GetPixel(startX + i, startY + p.y, WRAP_MODE.WRAP);
                            image1.SetPixel(startX + i, startY + p.y, pixel, WRAP_MODE.WRAP);
                        }
                    }       
                }

                BlurSeams(image1, path, bounds);
                //DrawPath(path, image1, ColorRGB.Red, startX, startY);
            }
        }

        private static void GraphCutHorizontally(ColorImage2D image1, ColorImage2D image2, int indexY, int exemplarsSize, int overlap, int samples)
        {
            int offset = exemplarsSize - overlap;
            int startY = offset * indexY;

            var bounds = new Box2i(0, startY, image1.Width, startY + overlap);
            var graph = CreateGraph(image1, image2, bounds);
            var path = FindBestCut(graph, samples, false);

            if(indexY != 0)
            {
                foreach (var p in path)
                {
                    for (int i = 0; i < p.y; i++)
                    {
                        var pixel = image2.GetPixel(p.x, startY + i, WRAP_MODE.WRAP);
                        image1.SetPixel(p.x, startY + i, pixel, WRAP_MODE.WRAP);
                    }
                }
            }
            else
            {
                foreach (var p in path)
                {
                    for (int i = p.y; i < overlap; i++)
                    {
                        var pixel = image2.GetPixel(p.x, startY + i, WRAP_MODE.WRAP);
                        image1.SetPixel(p.x, startY + i, pixel, WRAP_MODE.WRAP);
                    }
                }
            }

            BlurSeams(image1, path, bounds);
            //DrawPath(path, image1, ColorRGB.Red, 0, startY);

        }

        private static void BlurSeams(ColorImage2D image, List<Point2i> path, Box2i bounds)
        {

            var binary = new BinaryImage2D(image.Width, image.Height);
            DrawPath(path, binary, ColorRGB.White, bounds.Min.x, bounds.Min.y);

            binary = BinaryImage2D.Dilate(binary, 3);

            var mask = binary.ToGreyScaleImage();
            mask = GreyScaleImage2D.GaussianBlur(mask, 0.5f, bounds);

            var blurred = ColorImage2D.GaussianBlur(image, 0.75f, bounds, mask);
            blurred.CopyTo(image);
        }

        private static void DrawPath(List<Point2i> path, ColorImage2D image, ColorRGB col, int offsetX, int offsetY)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var p = path[i];
                image.SetPixel(offsetX + p.x, offsetY + p.y, col);
            }
        }

        private static void DrawPath(List<Point2i> path, BinaryImage2D image, ColorRGB col, int offsetX, int offsetY)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var p = path[i];
                image.SetPixel(offsetX + p.x, offsetY + p.y, col, WRAP_MODE.WRAP);
            }
        }

        public static GridGraph CreateGraph(ColorImage2D image1, ColorImage2D image2, Box2i bounds)
        {
            var graph = new GridGraph(bounds.Width, bounds.Height);

            graph.Iterate((x, y, i) =>
            {
                int xi = x + D8.OFFSETS[i, 0];
                int yi = y + D8.OFFSETS[i, 1];

                var col1 = image1.GetPixel(bounds.Min.x + x, bounds.Min.y + y, WRAP_MODE.WRAP);
                var col2 = image2.GetPixel(bounds.Min.x + x, bounds.Min.y + y, WRAP_MODE.WRAP);

                var w1 = ColorRGB.SqrDistance(col1, col2);

                if (graph.InBounds(xi, yi))
                {
                    var col1i = image1.GetPixel(bounds.Min.x + xi, bounds.Min.y + yi, WRAP_MODE.WRAP);
                    var col2i = image2.GetPixel(bounds.Min.x + xi, bounds.Min.y + yi, WRAP_MODE.WRAP);

                    var w2 = ColorRGB.SqrDistance(col1i, col2i);
                    var w = Math.Max(1, (w1 + w2) * 255);

                    graph.AddDirectedWeightedEdge(x, y, i, w);
                }
            });

            return graph;
        }

        public static List<Point2i> FindBestCut(GridGraph graph, int samples, bool vertical)
        {
            //if (samples > 1 && MathUtil.IsOdd(samples))
             //   samples++;

            var search = new GridSearch(graph.Width, graph.Height);
            var path = new List<Point2i>();
            float cost = float.PositiveInfinity;
          
            for (int x = 0; x < samples; x++)
            {
                for (int y = 0; y < samples; y++)
                {
                    int incrementS, incrementT;
                    Point2i source, target;

                    if (vertical)
                    {
                        if (samples <= 1)
                        {
                            source = new Point2i(graph.Width / 2, 0);
                            target = new Point2i(graph.Width / 2, graph.Height - 1);
                        }
                        else
                        {
                            incrementS = (graph.Width / samples) * x;
                            incrementT = (graph.Width / samples) * y;
                            source = new Point2i(incrementS, 0);
                            target = new Point2i(incrementT, graph.Height - 1);
                        }
                    }
                    else
                    {
                        if (samples <= 1)
                        {
                            source = new Point2i(0, graph.Height / 2);
                            target = new Point2i(graph.Height - 1, graph.Height / 2);
                        }
                        else
                        {
                            incrementS = (graph.Height / samples) * x;
                            incrementT = (graph.Height / samples) * y;

                            source = new Point2i(0, incrementS);
                            target = new Point2i(graph.Width - 1, incrementT);
                        }
                    }

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

            return path;
        }

    }
}
