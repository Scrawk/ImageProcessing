using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;
using Common.Core.Directions;

using ImageProcessing.Images;

namespace ImageProcessing.Synthesis
{
    public class ExemplarSet
    {

        public ExemplarSet(int exemplarSize)
        {
            Size = exemplarSize;
            Exemplars = new List<Exemplar>();
        }

        public int Count => Exemplars.Count;

        public int Size { get; private set; }

        private List<Exemplar> Exemplars { get; set; }

        public override string ToString()
        {
            return String.Format("[ExemplarSet: Count={0}, Size={1}]", Count, Size);
        }

        public Exemplar GetExemplar(int i)
        {
            return Exemplars[i];
        }

        public Exemplar FindBestMatch(ColorImage2D image, GreyScaleImage2D mask, Exemplar[,] previous, Point2i current, Point2i start)
        {
            Exemplar match = null;
            float cost = float.PositiveInfinity;

            foreach (var exemplar in Exemplars)
            {
                if (IsNeighbour(exemplar, previous, current))
                    continue;

                if (exemplar.Used > 0)
                    continue;

                float c = 0;
                int count = 0;

                for (int x = 0; x < exemplar.Width; x++)
                {
                    for (int y = 0; y < exemplar.Height; y++)
                    {
                        int i = start.x + x;
                        int j = start.y + y;

                        if (mask.GetValue(i, j, WRAP_MODE.WRAP) == 0) continue;

                        var pixel1 = image.GetPixel(i, j, WRAP_MODE.WRAP);
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

        private bool IsNeighbour(Exemplar exemplar, Exemplar[,] previous, Point2i current)
        {
            int width = previous.GetLength(0);
            int height = previous.GetLength(1);

            for(int i = 0; i < 4; i++)
            {
                int x = current.x + D4.OFFSETS[i, 0];
                int y = current.y + D4.OFFSETS[i, 1];

                x = MathUtil.Wrap(x, width);
                y = MathUtil.Wrap(y, height);

                if (previous[x, y] == exemplar)
                    return true;
            }

            return false;
        }

        public void CreateExemplarFromCrop(ColorImage2D image)
        {
            var croppedImages = ColorImage2D.Crop(image, image.Width / Size, image.Height / Size);
            Exemplars = CreateVariants(croppedImages);
        }

        public void CreateExemplarFromRandom(ColorImage2D image, int seed, int count)
        {
            var mask = new BinaryImage2D(image.Width, image.Height);
            var exemplars = new List<Exemplar>();

            var rnd = new Random(seed);
            int fails = 0;

            while(exemplars.Count < count && fails < 1000)
            {
                int x = rnd.Next(0, image.Width - Size - 1);
                int y = rnd.Next(0, image.Height - Size - 1);

                var coverage = GetCoverage(mask, x, y);

                if (coverage > 0.5f)
                {
                    fails++;
                    continue;
                }

                AddCoverage(mask, x, y);

                var exemplar = ColorImage2D.Crop(image, new Box2i(x, y, x + Size, y + Size));
                exemplars.Add(new Exemplar(exemplar));
            }

            Exemplars = CreateVariants(exemplars);
        }

        private float GetCoverage(BinaryImage2D mask, int x, int y)
        {
            int count = 0;

            for(int j = 0; j < Size; j++)
            {
                for (int i = 0; i < Size; i++)
                {
                    if(mask[x + i, y + j])
                        count++;
                }
            }

            return count / (float)(Size * Size);
        }

        private void AddCoverage(BinaryImage2D mask, int x, int y)
        {
            for (int j = 0; j < Size; j++)
            {
                for (int i = 0; i < Size; i++)
                {
                    mask[x + i, y + j] = true;
                }
            }
        }

        private List<Exemplar> CreateVariants(List<ColorImage2D> images)
        {
            var variants = new List<Exemplar>();

            foreach (var image in images)
            {
                var exemplar = new Exemplar(image);
                var v = exemplar.CreateVariants();

                variants.Add(exemplar);
                variants.AddRange(v);
            }

            return variants;
        }

        private List<Exemplar> CreateVariants(List<Exemplar> exemplars)
        {
            var variants = new List<Exemplar>();

            foreach (var exemplar in exemplars)
            {
                var v = exemplar.CreateVariants();

                variants.Add(exemplar);
                variants.AddRange(v);
            }

            return variants;
        }
    }
}
