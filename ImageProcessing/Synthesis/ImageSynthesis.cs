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
        public static (ColorImage2D, ColorImage2D) CreateSeamlessImageTest(ColorImage2D source, int width, int height, int exemplarSize, int overlap)
        {
            var exemplars = CreateExemplars(source, exemplarSize);
            exemplars.Shuffle(0);

            var image1 = new ColorImage2D(width, height);
            var image2 = new ColorImage2D(width, height);
            var mask = new GreyScaleImage2D(width, height);

            int offset = exemplarSize - overlap;
            int count = width / (exemplarSize - overlap);
            int startX = 0;

            var matches = new ColorImage2D[count];

            for (int j = 0; j < count; j++)
            {
                ColorImage2D match = null;

                if(j == 0)
                {
                    match = exemplars[0].Item2;
                }
                else
                {
                    match = FindBestMatch(image1, mask, startX, 0, exemplars);
                }

                for (int y = 0; y < image1.Height; y++)
                {
                    for (int x = 0; x < match.Width; x++)
                    {
                        int i = startX + x;
                        i = MathUtil.Wrap(i, image1.Width);

                        image1[i, y] = match[x, y];
                        mask[i, y] = 1;
                    }
                }

                startX += offset;
                matches[j] = match;
            }

            for(int j = 0; j < count; j++)
            {
                startX -= offset;

                var match = matches[count-1-j];

                for (int y = 0; y < image2.Height; y++)
                {
                    for (int x = 0; x < match.Width; x++)
                    {
                        int i = startX + x;
                        i = MathUtil.Wrap(i, image2.Width);

                        image2[i, y] = match[x, y];
                    }
                }
               
            }

            startX = 0;

            for (int j = 0; j < count; j++)
            {
                var bounds = new Box2i(startX, 0, startX + overlap, height);
                
                var graph = CreateGraph(image1, image2, bounds);
                var path = FindBestCut(graph, 4);

                if(j != 0)
                {
                    foreach (var p in path)
                        for (int i = 0; i < p.x; i++)
                            image1[startX + i, p.y] = image2[startX + i, p.y];
                }
                else
                {
                    foreach (var p in path)
                        for (int i = p.x; i < overlap; i++)
                            image1[startX + i, p.y] = image2[startX + i, p.y];
                }

                //DrawPath(graph, path, image1, ColorRGB.Red, startX);

                image1 = BlurSeams(image1, graph, path, bounds);

                startX += offset;
            }

            

            return (image1, image2);
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

        private static void DrawPath(GridGraph graph, List<Point2i> path, ColorImage2D image, ColorRGB col, int offset)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var p = path[i];
                image.SetPixel(offset + p.x, p.y, col);
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


        public static List<Tuple<string, ColorImage2D>> CreateExemplars(ColorImage2D image, int exemplarSize)
        {

            var exemplars = ColorImage2D.Crop(image, image.Width / exemplarSize, image.Height / exemplarSize);

            var images = new List<Tuple<string, ColorImage2D>>();

            foreach (var exemplar in exemplars)
            {
                images.Add(new Tuple<string, ColorImage2D>("Original", exemplar));
                images.Add(new Tuple<string, ColorImage2D>("Rotate90", ColorImage2D.Rotate90(exemplar)));
                images.Add(new Tuple<string, ColorImage2D>("Rotate180", ColorImage2D.Rotate180(exemplar)));
                images.Add(new Tuple<string, ColorImage2D>("Rotate270", ColorImage2D.Rotate270(exemplar)));
                images.Add(new Tuple<string, ColorImage2D>("FlipHorizontal", ColorImage2D.FlipHorizontal(exemplar)));
                images.Add(new Tuple<string, ColorImage2D>("FlipVertical", ColorImage2D.FlipVertical(exemplar)));
            }

            return images;
        }

        public static ColorImage2D FindBestMatch(ColorImage2D image, GreyScaleImage2D mask, int startX, int startY, List<Tuple<string, ColorImage2D>> exemplars)
        {
            ColorImage2D match = null;
            float cost = float.PositiveInfinity;

            foreach (var tuple in exemplars)
            {
                var exemplar = tuple.Item2;

                float c = 0;
                int count = 0;

                for (int x = 0; x < exemplar.Width; x++)
                {
                    for (int y = 0; y < exemplar.Height; y++)
                    {
                        int i = startX + x;
                        i = MathUtil.Wrap(i, image.Width);

                        int j = startY + y;

                        if (mask[i, j] == 0) continue;
                        
                        var pixel1 = image[i, j];
                        var pixel2 = exemplar[x, y];

                        c += ColorRGB.SqrDistance(pixel1, pixel2);
                        count++;
                    }
                }

                if (count == 0) continue;

                c /= count;

                if (c < cost)
                {
                    cost = c;
                    match = exemplar;
                }
            }

            return match;
        }


        public static ColorImage2D FindBestMatch(ColorImage2D image, List<ColorImage2D> images, int overlap)
        {

            int width = image.Width;

            ColorImage2D match = null;
            float cost = float.PositiveInfinity;

            foreach (var image2 in images)
            {
                if (image2 == image) continue;
                if(image2.Size != image.Size) continue;

                float c = 0;

                for(int x = 0; x < overlap; x++)
                {
                    for(int y = 0; y < image.Height; y++)
                    {
                        var pixel1 = image[width - overlap + x, y];
                        var pixel2 = image2[x, y];

                        c += ColorRGB.SqrDistance(pixel1, pixel2);
                    }
                }

                if(c == 0) continue;

                if(c < cost)
                {
                    cost = c;
                    match = image2;
                }
            }

            return match;
        }

        public static (ColorImage2D, int) FindBestMatch(ColorImage2D image, List<ColorImage2D> images, int shift, int overlap)
        {
            int width = image.Width;

            ColorImage2D best_match = null;
            float best_cost = float.PositiveInfinity;
            int best_overlap = 0;

            foreach (var image2 in images)
            {
                if (image2 == image) continue;
                if (image2.Height != image.Height) continue;

                for (int s = 0; s < shift; s++)
                {
                    float cost = 0;
                    int samples = 0;
                    int buffer = overlap - s;

                    for (int x = 0; x < buffer; x++)
                    {
                        for (int y = 0; y < image.Height; y++)
                        {
                            var pixel1 = image[width - buffer + x, y];
                            var pixel2 = image2[x, y];

                            cost += ColorRGB.SqrDistance(pixel1, pixel2);
                            samples++;
                        }
                    }

                    if (cost == 0) continue;
                    if (samples == 0) continue;

                    cost /= samples;

                    if (cost < best_cost)
                    {
                        best_cost = cost;
                        best_match = image2;
                        best_overlap = buffer;
                    }

                }

            }

            return (best_match, best_overlap);
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
