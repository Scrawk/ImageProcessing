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

    public class ImageSynthesis
    {

        public ImageSynthesis(ExemplarSet exemplars, int imageSize, int overlap)
        {
            ImageSize = imageSize;
            Overlap = overlap;
            Exemplars = exemplars;
            BlurWidth = 3;
            BlurStrength = 0.75f;
        }

        public int ImageSize { get; set; }

        public int Overlap { get; set; }

        public int BlurWidth { get; set; }

        public float BlurStrength { get; set; }

        public ExemplarSet Exemplars { get; set; }

        public ColorImage2D Image => Image1;

        private ColorImage2D Image1 { get; set; }

        private ColorImage2D Image2 { get; set; }

        private GreyScaleImage2D Mask { get; set; }

        public void CreateSeamlessImage()
        {

            Image1 = new ColorImage2D(ImageSize, ImageSize);
            Image2 = new ColorImage2D(ImageSize, ImageSize);
            Mask = new GreyScaleImage2D(ImageSize, ImageSize);

            int countX = Image1.Width / (Exemplars.Size - Overlap);
            int countY = Image1.Height / (Exemplars.Size - Overlap);

            var matches = new Exemplar[countX, countY];

            for (int y = 0; y <= countY; y++)
            {
                if(y == 0)
                {
                    TileHorizontally(y, matches);
                    CopyToImage2(y, matches);
                    GraphCutVertical(y);
                }
                else if(y == countY)
                {
                    GraphCutHorizontally(0);
                }
                else
                {
                    TileHorizontally(y, matches);
                    CopyToImage2(y, matches);
                    GraphCutVertical(y);
                    GraphCutHorizontally( y);
                }
            }

            Console.WriteLine(Image1);

        }

        private void TileHorizontally(int indexY, Exemplar[,] matches)
        {

            if (Exemplars.Count == 0)
                throw new ArgumentException("Exemplar set empty");

            int offset = Exemplars.Size - Overlap;
            int count = Image1.Width / (Exemplars.Size - Overlap);
            Point2i start = new Point2i(0, offset * indexY);
 
            for (int k = 0; k < count; k++)
            {
                start.x = offset * k;

                Exemplar match = null;

                if(k == 0 && indexY == 0)
                {
                    match = Exemplars.GetExemplar(0);
                }
                else
                {
                    var current = new Point2i(k, indexY);
                    match = Exemplars.FindBestMatch(Image1, Mask, matches, current, start);
                }

                for (int y = 0; y < Exemplars.Size; y++)
                {
                    for (int x = 0; x < match.Width; x++)
                    {
                        int i = start.x + x;
                        int j = start.y + y;
   
                        Image1.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                        Mask.SetPixel(i, j, ColorRGB.White, WRAP_MODE.WRAP);
                    }
                }

                match.IncrementUsed();
                matches[k, indexY] = match;
            }

        }

        private void CopyToImage2(int indexY, Exemplar[,] matches)
        {
            int offset = Exemplars.Size - Overlap;
            int count = Image1.Width / (Exemplars.Size - Overlap);
            Point2i start = new Point2i(0, offset * indexY);
 
            for (int k = 0; k < count; k++)
            {
                start.x = offset * k;
                var match = matches[k, indexY];

                if (indexY == 0)
                {
                    CopyToImage2Horizontal(0, Exemplars.Size, k, match, start);
                }
                else if (indexY == count - 1)
                {
                    CopyToImage2Horizontal(Overlap, Exemplars.Size - Overlap, k, match, start);
                }
                else
                {
                    CopyToImage2Horizontal(Overlap, Exemplars.Size, k, match, start);
                }

            }
        }

        private void CopyToImage2Horizontal(int ystart, int yend, int k, Exemplar match, Point2i start)
        {
            int count = Image1.Width / (Exemplars.Size - Overlap);

            for (int y = ystart; y < yend; y++)
            {
                if (k == 0)
                {
                    for (int x = 0; x < match.Width; x++)
                    {
                        int i = start.x + x;
                        int j = start.y + y;
                        Image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                    }
                }
                if (k == count - 1)
                {
                    for (int x = Overlap; x < match.Width - Overlap; x++)
                    {
                        int i = start.x + x;
                        int j = start.y + y;
                        Image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                    }
                }
                else
                {
                    for (int x = Overlap; x < match.Width; x++)
                    {
                        int i = start.x + x;
                        int j = start.y + y;
                        Image2.SetPixel(i, j, match[x, y], WRAP_MODE.WRAP);
                    }
                }
            }
        }

        private void GraphCutVertical(int indexY)
        {
            int offset = Exemplars.Size - Overlap;
            int count = Image1.Width / (Exemplars.Size - Overlap);
            int startY = offset * indexY;

            for (int j = 0; j < count; j++)
            {
                int startX = offset * j;

                var bounds = new Box2i(startX, startY, startX + Overlap, startY + Exemplars.Size);
                var graph = CreateGraph(bounds);
                var path = FindBestCut(graph, true);

                if (j != 0)
                {
                    foreach (var p in path)
                    {
                        for (int i = 0; i < p.x; i++)
                        {
                            var pixel = Image2.GetPixel(startX + i, startY + p.y, WRAP_MODE.WRAP);
                            Image1.SetPixel(startX + i, startY + p.y, pixel, WRAP_MODE.WRAP);
                        }
                    }
                            
                }
                else
                {
                    foreach (var p in path)
                    {
                        for (int i = p.x; i < Overlap; i++)
                        {
                            var pixel = Image2.GetPixel(startX + i, startY + p.y, WRAP_MODE.WRAP);
                            Image1.SetPixel(startX + i, startY + p.y, pixel, WRAP_MODE.WRAP);
                        }
                    }       
                }

                BlurSeams(Image1, path, bounds);
                //DrawPath(path, Image1, ColorRGB.Red, startX, startY);
            }
        }

        private void GraphCutHorizontally(int indexY)
        {
            int offset = Exemplars.Size - Overlap;
            int startY = offset * indexY;

            var bounds = new Box2i(0, startY, Image1.Width, startY + Overlap);
            var graph = CreateGraph(bounds);
            var path = FindBestCut(graph, false);

            if(indexY != 0)
            {
                foreach (var p in path)
                {
                    for (int i = 0; i < p.y; i++)
                    {
                        var pixel = Image2.GetPixel(p.x, startY + i, WRAP_MODE.WRAP);
                        Image1.SetPixel(p.x, startY + i, pixel, WRAP_MODE.WRAP);
                    }
                }
            }
            else
            {
                foreach (var p in path)
                {
                    for (int i = p.y; i < Overlap; i++)
                    {
                        var pixel = Image2.GetPixel(p.x, startY + i, WRAP_MODE.WRAP);
                        Image1.SetPixel(p.x, startY + i, pixel, WRAP_MODE.WRAP);
                    }
                }
            }

            BlurSeams(Image1, path, bounds);
            //DrawPath(path, Image1, ColorRGB.Red, 0, startY);

        }

        private void BlurSeams(ColorImage2D image, List<Point2i> path, Box2i bounds)
        {

            var binary = new BinaryImage2D(image.Width, image.Height);
            DrawPath(path, binary, ColorRGB.White, bounds.Min.x, bounds.Min.y);

            binary = BinaryImage2D.Dilate(binary, BlurWidth);

            var mask = binary.ToGreyScaleImage();
            mask = GreyScaleImage2D.GaussianBlur(mask, 0.5f, bounds, null, WRAP_MODE.WRAP);

            var blurred = ColorImage2D.GaussianBlur(image, BlurStrength, bounds, mask, WRAP_MODE.WRAP);
            blurred.CopyTo(image);
        }

        private void DrawPath(List<Point2i> path, ColorImage2D image, ColorRGB col, int offsetX, int offsetY)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var p = path[i];
                image.SetPixel(offsetX + p.x, offsetY + p.y, col, WRAP_MODE.WRAP);
            }
        }

        private void DrawPath(List<Point2i> path, BinaryImage2D image, ColorRGB col, int offsetX, int offsetY)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var p = path[i];
                image.SetPixel(offsetX + p.x, offsetY + p.y, col, WRAP_MODE.WRAP);
            }
        }

        public GridGraph CreateGraph(Box2i bounds)
        {
            var graph = new GridGraph(bounds.Width, bounds.Height);

            graph.Iterate((x, y, i) =>
            {
                int xi = x + D8.OFFSETS[i, 0];
                int yi = y + D8.OFFSETS[i, 1];

                var col1 = Image1.GetPixel(bounds.Min.x + x, bounds.Min.y + y, WRAP_MODE.WRAP);
                var col2 = Image2.GetPixel(bounds.Min.x + x, bounds.Min.y + y, WRAP_MODE.WRAP);

                var w1 = ColorRGB.SqrDistance(col1, col2);

                if (graph.InBounds(xi, yi))
                {
                    var col1i = Image1.GetPixel(bounds.Min.x + xi, bounds.Min.y + yi, WRAP_MODE.WRAP);
                    var col2i = Image2.GetPixel(bounds.Min.x + xi, bounds.Min.y + yi, WRAP_MODE.WRAP);

                    var w2 = ColorRGB.SqrDistance(col1i, col2i);
                    var w = Math.Max(1, (w1 + w2) * 255);

                    graph.AddDirectedWeightedEdge(x, y, i, w);
                }
            });

            return graph;
        }

        public List<Point2i> FindBestCut(GridGraph graph, bool vertical)
        {
            var search = new GridSearch(graph.Width, graph.Height);
            var path = new List<Point2i>();
            float cost = float.PositiveInfinity;

            Point2i source, target;

            if (vertical)
            {
                source = new Point2i(graph.Width / 2, 0);
                target = new Point2i(graph.Width / 2, graph.Height - 1);
            }
            else
            {
                source = new Point2i(0, graph.Height / 2);
                target = new Point2i(graph.Width - 1, graph.Height / 2);
            }

            search.Clear();
            graph.PrimsMinimumSpanningTree(search, source.x, source.y);

            var p = search.GetPath(target);

            float c = search.GetPathCost(p, graph);

            if (c < cost)
            {
                cost = c;
                path = p;
            }

            return path;
        }

    }
}
