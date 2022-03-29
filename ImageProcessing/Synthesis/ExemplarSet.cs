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
    public class ExemplarSet
    {

        public ExemplarSet(ColorImage2D image, int exemplarSize)
        {
            Size = exemplarSize;
            Exclude = CreateExcludeList();
            Exemplars = CreateExemplars(image, exemplarSize);
        }

        public int Count => Exemplars.Count;

        public int Size { get; private set; }

        private List<Tuple<string, string>> Exclude { get; set; }

        private List<Tuple<string, ColorImage2D>> Exemplars { get; set; }

        public Tuple<string, ColorImage2D> GetExemplar(int i)
        {
            return Exemplars[i];
        }

        public  Tuple<string, ColorImage2D> FindBestMatch(ColorImage2D image, GreyScaleImage2D mask, string previous, int startX, int startY)
        {
            Tuple<string, ColorImage2D> match = null;
            float cost = float.PositiveInfinity;

            foreach (var tuple in Exemplars)
            {
                //if (previous == tuple.Item1) continue;
                //if(IsExcluded(previous, tuple.Item1)) continue;

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
                        j = MathUtil.Wrap(j, image.Height);

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
                    match = tuple;
                }
            }

            return match;
        }

        private bool IsExcluded(string image, string exemplar)
        {
            foreach(var tuple in Exclude)
            {
                if(image == tuple.Item1 && exemplar == tuple.Item2)
                    return true;
            }

            return false;
        }

        private List<Tuple<string, string>> CreateExcludeList()
        {
            var list = new List<Tuple<string,string>>();

            //list.Add(new Tuple<string, string>("Original", "FlipHorizontal"));
            //list.Add(new Tuple<string, string>("FlipHorizontal", "Original"));

            //list.Add(new Tuple<string, string>("Rotate180", "FlipVertical"));
            //list.Add(new Tuple<string, string>("FlipVertical", "Rotate180"));

            return list;

        }

        private List<Tuple<string, ColorImage2D>> CreateExemplars(ColorImage2D image, int exemplarSize)
        {
            var croppedImages = ColorImage2D.Crop(image, image.Width / exemplarSize, image.Height / exemplarSize);

            var exemplars = new List<Tuple<string, ColorImage2D>>();

            foreach (var croppedImage in croppedImages)
            {

                var tmp = new List<Tuple<string, ColorImage2D>>();

                tmp.Add(new Tuple<string, ColorImage2D>("Original", croppedImage));
                tmp.Add(new Tuple<string, ColorImage2D>("Rotate90", ColorImage2D.Rotate90(croppedImage)));
                tmp.Add(new Tuple<string, ColorImage2D>("Rotate180", ColorImage2D.Rotate180(croppedImage)));
                tmp.Add(new Tuple<string, ColorImage2D>("Rotate270", ColorImage2D.Rotate270(croppedImage)));
                //tmp.Add(new Tuple<string, ColorImage2D>("FlipHorizontal", ColorImage2D.FlipHorizontal(croppedImage)));
                //tmp.Add(new Tuple<string, ColorImage2D>("FlipVertical", ColorImage2D.FlipVertical(croppedImage)));

                exemplars.AddRange(tmp);
            }

            exemplars.Shuffle(0);

            return exemplars;
        }
    }
}
