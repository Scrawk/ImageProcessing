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

        public static ColorImage2D CreateSeamlessImage(ColorImage2D source, int exemplarSize, int overlap, int shift)
        {
            const int pathSamples = 4;

            var exemplars = CreateExemplars(source, exemplarSize);
            var exemplar = exemplars[0];

            var image = new ColorImage2D(512, exemplarSize);

            int totalOffset = 0;

            for (int j = 0; j < 3; j++)
            {
                var match_pair = FindBestMatch(exemplar, exemplars, shift, overlap);
                var match = match_pair.Item1;
                var new_overlap = match_pair.Item2;

                int offset = exemplarSize - new_overlap;
                totalOffset += offset;

                if (j == 0)
                {
                    exemplar.Iterate((x, y) =>
                    {
                        image[x, y] = exemplar[x, y];
                    });
                }

                match.Iterate((x, y) =>
                {
                    image[totalOffset + x, y] = match[x, y];
                });

                var cut_pair = FindBestCut(exemplar, match, new_overlap, pathSamples);

                var path = cut_pair.Item1;
                var graph = cut_pair.Item2;

                foreach (var p in path)
                {
                    for (int i = 0; i < p.x; i++)
                    {
                        var pixel = exemplar[offset + i, p.y];
                        image.SetPixel(totalOffset + i, p.y, pixel);
                    }
                }

                image = BlurSeams(image, graph, path, totalOffset, exemplarSize);

                exemplar = match;
            }

            

            return image;

        }

        private static ColorImage2D BlurSeams(ColorImage2D image, GridGraph graph, List<Point2i> path, int offset, int exemplarSize)
        {
            var min = new Point2i(offset, 0);
            var max = new Point2i(offset + exemplarSize, exemplarSize);
            var bounds = new Box2i(min, max);

            var binary = new BinaryImage2D(image.Width, image.Height);
            DrawPath(graph, path, binary, ColorRGB.White, offset);

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


        public static List<ColorImage2D> CreateExemplars(ColorImage2D image, int exemplarSize)
        {

            var exemplars = ColorImage2D.Crop(image, image.Width / exemplarSize, image.Height / exemplarSize);

            var images = new List<ColorImage2D>();

            foreach (var exemplar in exemplars)
            {
                images.Add(exemplar);
                images.Add(ColorImage2D.Rotate90(exemplar));
                images.Add(ColorImage2D.Rotate180(exemplar));
                images.Add(ColorImage2D.Rotate270(exemplar));
                //images.Add(ColorImage2D.FlipHorizontal(exemplar));
                //images.Add(ColorImage2D.FlipVertical(exemplar));
            }

            return images;
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

        public static bool HasDuplicates(List<ColorImage2D> images)
        {
            for (int x = 0; x < images.Count; x++)
            {
                for (int y = 0; y < images.Count; y++)
                {
                    if(x == y) continue;
                    
                    var image1 = images[x];
                    var image2 = images[y];

                    if(image1.AreEqual(image2))
                        return true;

                }
            }

            return false;

        }

        public static (List<Point2i>, GridGraph) FindBestCut(ColorImage2D image, ColorImage2D match, int overlap, int samples)
        {
            var graph = new GridGraph(overlap, image.Height);
            int offset = image.Width - overlap;

            graph.Iterate((x, y, i) =>
            {
                int xi = x + D8.OFFSETS[i, 0];
                int yi = y + D8.OFFSETS[i, 1];

                var col1 = image[offset + x, y];
                var col2 = match[x, y];

                var w1 = ColorRGB.SqrDistance(col1, col2);

                if (graph.InBounds(xi, yi))
                {
                    var col1i = image[offset + xi, yi];
                    var col2i = match[xi, yi];

                    var w2 = ColorRGB.SqrDistance(col1i, col2i);
                    var w = Math.Max(1, (w1 + w2) * 255);

                    graph.AddDirectedWeightedEdge(x, y, i, w);
                }
            });

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

            return (path, graph);
        }

    }
}
