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

        public static (ColorImage2D, float) MakeTileable(ColorImage2D image, ExemplarSet set)
        {
            int width = image.Width;
            int height = image.Height;
            int cutOffset = 2;
            int sinkOffsetX = width / 2 - 48;
            int sinkOffsetY = height / 2 - 48;
            int matchOffset = Math.Min(2, cutOffset);

            var tileable = ColorImage2D.Offset(image, width / 2, height / 2);

            var horzontalLine = new Segment2f(0, height / 2, width, height / 2);
            var verticalLine = new Segment2f(width / 2, 0, width / 2, height);
            BlurSeams(tileable, horzontalLine, verticalLine);

            var cutBounds = new Box2i(cutOffset, cutOffset, width - 1 - cutOffset, height - 1 - cutOffset);
            var sinkBounds = new Box2i(sinkOffsetX, sinkOffsetY, width - 1 - sinkOffsetX, height - 1 - sinkOffsetY);

            var mask = new GreyScaleImage2D(width, height);
            mask.DrawBox(cutBounds, ColorRGBA.White, true);
            mask.DrawBox(sinkBounds, ColorRGBA.Black, true);

            var pair = set.FindBestMatch(tileable, mask, matchOffset);
            pair.Item1.IncrementUsed();

            var exemplar = pair.Item1;
            exemplar.IncrementUsed();
            var match = exemplar.Image;
            var offset = pair.Item2;

            match = ColorImage2D.Offset(match, offset.x, offset.y);

            var graph = CreateGraph(tileable, mask, match, cutBounds, sinkBounds);
            var cost = PerformGraphCut(graph, tileable, match, cutOffset);

            var points = graph.FindBoundaryPoints();
            BlurSeams(tileable, points, cutOffset);

            return (tileable, cost);
        }

        public static void Test(WangTile tile, ExemplarSet set)
        {
            if(tile.IsConst)
                return;   

            int width = tile.Size;
            int height = tile.Size;
            var map = tile.Map;
            var mask = tile.Mask;

            var sourceBounds = new Box2i(0, 0, width - 1, height - 1);
            var sinkBounds = new Box2i(20, 20, width - 1 - 20, height - 1 - 20);

            foreach (var p in sourceBounds.EnumeratePerimeter())
            {
                mask[p.x,p.y] = true;
            }

            foreach (var p in sinkBounds.EnumerateBounds())
            {
                mask[p.x, p.y] = true;
            }

            var pair = set.FindBestMatch(tile.Image, mask);
            //pair.Item1.IncrementUsed();

            var match = pair.Item1.Image;

            var graph = CreateGraph(tile.Image, match, mask, map);

            graph.Calculate();

            var image = tile.Image;

            image.Iterate((x, y) =>
            {
                //if (graph.IsSource(x, y))
                //    image[x, y] = ColorRGB.Red;
                //else if (graph.IsSink(x, y))
                //    image[x, y] = ColorRGB.Green;

                if (graph.IsSink(x, y))
                    image[x, y] = match[x, y];
            });
        }

        private static void BlurSeams(ColorImage2D image, Segment2f horzontal, Segment2f vertical)
        {
            int width = image.Width;
            int height = image.Height;

            var binary = new BinaryImage2D(width, height);
            binary.DrawLine(horzontal, ColorRGBA.White);
            binary.DrawLine(vertical, ColorRGBA.White);

            binary = BinaryImage2D.Dilate(binary, 2);

            var mask = binary.ToGreyScaleImage();
            mask = GreyScaleImage2D.GaussianBlur(mask, 0.5f, null, null, WRAP_MODE.WRAP);

            var blurred = ColorImage2D.GaussianBlur(image, 0.5f, null, mask, WRAP_MODE.WRAP);
            image.Fill(blurred);
        }

        private static void BlurSeams(ColorImage2D image, List<Point2i> points, int offset)
        {
            int width = image.Width;
            int height = image.Height;

            var binary = new BinaryImage2D(width, height);

            foreach (var p in points)
                binary[p.x + offset, p.y + offset] = true;

            binary = BinaryImage2D.Dilate(binary, 2);

            var mask = binary.ToGreyScaleImage();
            mask = GreyScaleImage2D.GaussianBlur(mask, 0.5f, null, null, WRAP_MODE.WRAP);

            var blurred = ColorImage2D.GaussianBlur(image, 0.5f, null, mask, WRAP_MODE.WRAP);
            image.Fill(blurred);
        }

        private static float PerformGraphCut(GridFlowGraph graph, ColorImage2D image, ColorImage2D match, int cutOffset)
        {
            graph.Calculate();

            float cost = 0;
            int count = 0;

            graph.Iterate((x, y) =>
            {
                int xo = x + cutOffset;
                int yo = y + cutOffset;

                if (graph.IsSink(x, y))
                {
                    cost += ColorRGB.SqrDistance(image[xo, yo], match[xo, yo]);
                    count++;

                    image[xo, yo] = match[xo, yo];
                }   
            });

            if(count > 0)
                cost /= count;

            return cost;
        }

        private static GridFlowGraph CreateGraph(ColorImage2D image, GreyScaleImage2D mask, ColorImage2D match, Box2i cutBounds, Box2i sinkBounds)
        {
            int cutOffset = cutBounds.Min.x;
            var graph = new GridFlowGraph(cutBounds.Width + 1, cutBounds.Height + 1);

            graph.Iterate((x, y) =>
            {
                int xo = x + cutOffset;
                int yo = y + cutOffset;

                if (mask[xo, yo] != 0)
                {
                    var col1 = match[xo, yo];
                    var col2 = image[xo, yo];

                    var w1 = ColorRGB.SqrDistance(col1, col2) * 255;

                    for (int i = 0; i < 8; i++)
                    {
                        int xi = xo + D8.OFFSETS[i, 0];
                        int yi = yo + D8.OFFSETS[i, 1];

                        if (xi < 0 || xi >= graph.Width) continue;
                        if (yi < 0 || yi >= graph.Height) continue;
                        if (mask[xi, yi] == 0) continue;

                        var col1i = match[xi, yi];
                        var col2i = image[xi, yi];

                        var w2 = ColorRGB.SqrDistance(col1i, col2i) * 255;

                        var w = MathUtil.Max(1, w1, w2);

                        graph.SetCapacity(x, y, i, w);

                    }
                }
            });

            foreach (var p in cutBounds.EnumeratePerimeter())
            {
                graph.SetSource(p.x - cutOffset, p.y - cutOffset, 255);
            }

            var expanded = sinkBounds;
            expanded.Min -= 1;
            expanded.Max += 2;
            foreach (var p in expanded.EnumerateBounds())
            {
                graph.SetSink(p.x - cutOffset, p.y - cutOffset, 255);
            }

            return graph;
        }

        private static GridFlowGraph CreateGraph(ColorImage2D image1, ColorImage2D image2, BinaryImage2D mask, GreyScaleImage2D map)
        {
            var graph = new GridFlowGraph(image1.Width, image1.Height);

            var sourceBounds = new Box2i(0, 0, graph.Width - 1, graph.Height - 1);
            var sinkBounds = new Box2i(20, 20, graph.Width - 1 - 20, graph.Height - 1 - 20);

            graph.Iterate((x, y) =>
            {
                //if (mask[x, y])
                {
                    var col1 = image1[x, y];
                    var col2 = image2[x, y];

                    var w1 = ColorRGB.SqrDistance(col1, col2) * 255;

                    for (int i = 0; i < 8; i++)
                    {
                        int xi = x + D8.OFFSETS[i, 0];
                        int yi = y + D8.OFFSETS[i, 1];

                        if (xi < 0 || xi >= graph.Width) continue;
                        if (yi < 0 || yi >= graph.Height) continue;
                        //if (!mask[xi, yi]) continue;

                        var col1i = image1[xi, yi];
                        var col2i = image2[xi, yi];

                        var w2 = ColorRGB.SqrDistance(col1i, col2i) * 255;

                        var w = MathUtil.Max(1, w1, w2);

                        graph.SetCapacity(x, y, i, w);
                    }
                }

            });

            foreach (var p in sourceBounds.EnumeratePerimeter())
            {
                graph.SetSource(p.x, p.y, 255);
            }

            var expanded = sinkBounds;
            expanded.Min -= 1;
            expanded.Max += 1;
            foreach (var p in expanded.EnumerateBounds())
            {
                graph.SetSink(p.x, p.y, 255);
            }

            return graph;
        }

    }
}
