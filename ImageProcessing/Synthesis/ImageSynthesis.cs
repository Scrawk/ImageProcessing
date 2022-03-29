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
        public static (ColorImage2D, ColorImage2D, GreyScaleImage2D) CreateSeamlessImageTest(ColorImage2D source, int exemplarSize, int overlap)
        {

            var exemplars = new ExemplarSet(source, exemplarSize);

            int tilesX = source.Width / exemplars.Size;
            int tilesY = source.Height / exemplars.Size;

            int width = source.Width - tilesX * overlap;
            int height = source.Height - tilesY * overlap;

            var image1 = new ColorImage2D(width, height);
            var image2 = new ColorImage2D(width, height);
            var mask = new GreyScaleImage2D(width, height);

            int y = 0;

            TileHorizontally(image1, image2, mask, y, exemplars, overlap);
            GraphCutVertical(image1, image2, y, exemplars.Size, overlap);

            y = 1;

            TileHorizontally(image1, image2, mask, y, exemplars, overlap);
            GraphCutVertical(image1, image2, y, exemplars.Size, overlap);
            GraphCutHorizontally(image1, image2, y, exemplars.Size, overlap);

            y = 2;

            TileHorizontally(image1, image2, mask, y, exemplars, overlap);
            GraphCutVertical(image1, image2, y, exemplars.Size, overlap);
            GraphCutHorizontally(image1, image2, y, exemplars.Size, overlap);

            y = 3;

            TileHorizontally(image1, image2, mask, y, exemplars, overlap);
            GraphCutVertical(image1, image2, y, exemplars.Size, overlap);
            GraphCutHorizontally(image1, image2, y, exemplars.Size, overlap);

            y = 0;

            GraphCutHorizontally(image1, image2, y, exemplars.Size, overlap);

            return (image1, image2, mask);

        }

        private static void TileHorizontally(ColorImage2D image1, ColorImage2D image2, GreyScaleImage2D mask, int indexY, ExemplarSet exemplars, int overlap)
        {

            if (exemplars.Count == 0)
                throw new ArgumentException("Exemplar set empty");

            int offset = exemplars.Size - overlap;
            int count = image1.Width / (exemplars.Size - overlap);
            int startY = offset * indexY;

            var matches = new Tuple<string, ColorImage2D>[count];

            for (int k = 0; k < count; k++)
            {
                int startX = offset * k;

                Tuple<string, ColorImage2D> tuple = null;

                if(k == 0 && indexY == 0)
                {
                    tuple = exemplars.GetExemplar(0);
                }
                else
                {
                    //var prevous = matches[j - 1].Item1;
                    tuple = exemplars.FindBestMatch(image1, mask, "", startX, startY);
                }

                var match = tuple.Item2;

                for (int y = 0; y < exemplars.Size; y++)
                {
                    for (int x = 0; x < match.Width; x++)
                    {
                        int i = startX + x;
                        int j = startY + y;
   
                        image1.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                        mask.SetPixel(i, j, ColorRGB.White, WRAP_MODE.WRAP);
                    }
                }

                matches[k] = tuple;
            }

            for (int k = 0; k < count; k++)
            {
                int startX = offset * k;
                var match = matches[k];

                if(indexY == 0)
                {
                    for (int y = 0; y < exemplars.Size; y++)
                    {
                        if (k == 0)
                        {
                            for (int x = 0; x < match.Item2.Width; x++)
                            {
                                int i = startX + x;
                                int j = startY + y;
                                image2.SetPixel(i, j, matches[k].Item2[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        if (k == count - 1)
                        {
                            for (int x = overlap; x < match.Item2.Width - overlap; x++)
                            {
                                int i = startX + x;
                                int j = startY + y;
                                image2.SetPixel(i, j, matches[k].Item2[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        else
                        {
                            for (int x = overlap; x < match.Item2.Width; x++)
                            {
                                int i = startX + x;
                                int j = startY + y;
                                image2.SetPixel(i, j, matches[k].Item2[x, y], WRAP_MODE.WRAP);
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
                            for (int x = 0; x < match.Item2.Width; x++)
                            {
                                int i = startX + x;
                                int j = startY + y;
                                image2.SetPixel(i, j, matches[k].Item2[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        if (k == count - 1)
                        {
                            for (int x = overlap; x < match.Item2.Width - overlap; x++)
                            {
                                int i = startX + x;
                                int j = startY + y;
                                image2.SetPixel(i, j, matches[k].Item2[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        else
                        {
                            for (int x = overlap; x < match.Item2.Width; x++)
                            {
                                int i = startX + x;
                                int j = startY + y;
                                image2.SetPixel(i, j, matches[k].Item2[x, y], WRAP_MODE.WRAP);
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
                            for (int x = 0; x < match.Item2.Width; x++)
                            {
                                int i = startX + x;
                                int j = startY + y;
                                image2.SetPixel(i, j, matches[k].Item2[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        if (k == count - 1)
                        {
                            for (int x = overlap; x < match.Item2.Width - overlap; x++)
                            {
                                int i = startX + x;
                                int j = startY + y;
                                image2.SetPixel(i, j, matches[k].Item2[x, y], WRAP_MODE.WRAP);
                            }
                        }
                        else
                        {
                            for (int x = overlap; x < match.Item2.Width; x++)
                            {
                                int i = startX + x;
                                int j = startY + y;
                                image2.SetPixel(i, j, matches[k].Item2[x, y], WRAP_MODE.WRAP);
                            }
                        }
                    }
                }
               
            }

        }

        private static void GraphCutVertical(ColorImage2D image1, ColorImage2D image2, int indexY, int exemplarsSize, int overlap)
        {
            int offset = exemplarsSize - overlap;
            int count = image1.Width / (exemplarsSize - overlap);
            int startY = offset * indexY;

            for (int j = 0; j < count; j++)
            {
                int startX = offset * j;

                var bounds = new Box2i(startX, startY, startX + overlap, startY + exemplarsSize);

                var graph = CreateGraph(image1, image2, bounds);
                var path = FindBestCut(graph, 4, true);

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

                //DrawPath(graph, path, image1, ColorRGB.Red, startX, startY);
                //image1 = BlurSeams(image1, graph, path, bounds);
            }
        }

        private static void GraphCutHorizontally(ColorImage2D image1, ColorImage2D image2, int indexY, int exemplarsSize, int overlap)
        {
            int offset = exemplarsSize - overlap;
            int startY = offset * indexY;

            var bounds = new Box2i(0, startY, image1.Width, startY + overlap);

            //image1.DrawBox(bounds, ColorRGB.Red, true);

            var graph = CreateGraph(image1, image2, bounds);
            var path = FindBestCut(graph, 4, false);

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

            //DrawPath(graph, path, image1, ColorRGB.Red, 0, startY);

        }

        private static ColorImage2D BlurSeams(ColorImage2D image, GridGraph graph, List<Point2i> path, Box2i bounds)
        {

            var binary = new BinaryImage2D(image.Width, image.Height);
            DrawPath(graph, path, binary, ColorRGB.White, bounds.Min.x);

            binary = BinaryImage2D.Dilate(binary, 2);

            var mask = binary.ToGreyScaleImage();
            mask = GreyScaleImage2D.GaussianBlur(mask, 0.75f, bounds);

            return ColorImage2D.GaussianBlur(image, 0.75f, bounds, mask);
        }

        private static void DrawPath(GridGraph graph, List<Point2i> path, ColorImage2D image, ColorRGB col, int offsetX, int offsetY)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var p = path[i];
                image.SetPixel(offsetX + p.x, offsetY + p.y, col);
            }
        }

        private static void DrawPath(GridGraph graph, List<Point2i> path, BinaryImage2D image, ColorRGB col, int offset)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var p = path[i];
                image.SetPixel(offset + p.x, p.y, col);
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
