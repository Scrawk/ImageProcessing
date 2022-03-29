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
                        i = MathUtil.Wrap(i, image1.Width);

                        int j = startY + y;
                        j = MathUtil.Wrap(j, image1.Height);

                        image1[i, j] = match[x, y];
                        mask[i, j] = 1;
                    }
                }

                matches[k] = tuple;
            }

            for (int k = 0; k < count; k++)
            {
                int startX = offset * (count-1) - offset * k;
 
                var match = matches[count-1-k];

                for (int y = 0; y < exemplars.Size; y++)
                {
                    for (int x = 0; x < match.Item2.Width; x++)
                    {
                        int i = startX + x;
                        i = MathUtil.Wrap(i, image2.Width);

                        int j = startY + y;
                        j = MathUtil.Wrap(j, image2.Height);

                        image2[i, j] = match.Item2[x, y];
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
                var path = FindBestCut(graph, 4);

                if (j != 0)
                {
                    foreach (var p in path)
                        for (int i = 0; i < p.x; i++)
                            image1[startX + i, startY + p.y] = image2[startX + i, startY + p.y];
                }
                else
                {
                    foreach (var p in path)
                        for (int i = p.x; i < overlap; i++)
                            image1[startX + i, startY + p.y] = image2[startX + i, startY + p.y];
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

            image1.DrawBox(bounds, ColorRGB.Red, true);

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

                var col1 = image1[bounds.Min.x + x, bounds.Min.y + y];
                var col2 = image2[bounds.Min.x + x, bounds.Min.y + y];

                var w1 = ColorRGB.SqrDistance(col1, col2);

                if (graph.InBounds(xi, yi))
                {
                    var col1i = image1[bounds.Min.x + xi, bounds.Min.y + yi];
                    var col2i = image2[bounds.Min.x + xi, bounds.Min.y + yi];

                    var w2 = ColorRGB.SqrDistance(col1i, col2i);
                    var w = Math.Max(1, (w1 + w2) * 255);

                    graph.AddDirectedWeightedEdge(x, y, i, w);
                }
            });

            return graph;
        }

        public static List<Point2i> FindBestCut(GridGraph graph, int samples)
        {

            var search = new GridSearch(graph.Width, graph.Height);
            var path = new List<Point2i>();
            float cost = float.PositiveInfinity;

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

            return path;
        }

    }
}
