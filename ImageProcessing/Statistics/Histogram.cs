using System;
using System.Collections.Generic;
using System.Linq;

using ImageProcessing.Images;
using ImageProcessing.Pixels;

using Common.Core.Colors;

namespace ImageProcessing.Statistics
{

    public class Histogram
    {

        public Histogram(int bins)
        {
            CreateBins(bins);
        }

        public int BinSize => Bins.Length;

        public bool HasCulumativeHistogram => CumulativeBins != null;

        private HistogramBin[] Bins { get; set; }

        private int[] CumulativeBins { get; set; }

        public override string ToString()
        {
            return string.Format("[Histogram: BinSize={0}, HasCumulativeBins={1}]", 
                BinSize, HasCulumativeHistogram);
        }

        public void Clear()
        {
            for (int i = 0; i < Bins.Length; i++)
                Bins[i].Clear();

            CumulativeBins = null;
        }

        public int GetBinCount(int index)
        {
            return Bins[index].Count;
        }

        public int MaxBinCount()
        {
            int max = -1;
            for (int i = 0; i < Bins.Length; i++)
            {
                int count = Bins[i].Count;
                if(count > max)
                    max = count;    
            }

            return max;
        }

        public void Load(GreyScaleImage2D image)
        {
            Clear();

            image.Iterate((x, y) =>
            {
                var v = image[x, y];

                int index = (int)(v * BinSize - 1);

                if(index >= 0 && index < BinSize)
                {
                    var bin = Bins[index];

                    bin.Add(new PixelIndex2D<float>(x, y, v));
                }

            });
        }

        public void Load(ColorImage2D image, int channel)
        {
            Clear();

            image.Iterate((x, y) =>
            {
                var v = image.GetChannel(x, y, channel);

                int index = (int)(v * BinSize - 1);

                if (index >= 0 && index < BinSize)
                {
                    var bin = Bins[index];

                    bin.Add(new PixelIndex2D<float>(x, y, v));
                }

            });
        }

        public ColorImage2D HistogramToImage(ColorRGBA color, ColorRGBA background, int height)
        {
            int width = BinSize;
            int max = MaxBinCount();

            var image = new ColorImage2D(width, height);  
            image.Fill(background);

            for(int x = 0; x < width; x++)
            {
                float count01 = Bins[x].Count / (float)max;
                int count = (int)(count01 * (height-1));

                for (int y = 0; y < count; y++)
                {
                    image[x, height - y - 1] = color;
                }
            }

            return image;
        }

        public ColorImage2D CumulativeHistogramToImage(ColorRGBA color, ColorRGBA background, int height)
        {
            CreateCumulativeHistogram();

            int width = BinSize;
            int max = CumulativeBins.Last();

            var image = new ColorImage2D(width, height);
            image.Fill(background);

            for (int x = 0; x < width; x++)
            {
                float count01 = CumulativeBins[x] / (float)max;
                int count = (int)(count01 * (height - 1));

                for (int y = 0; y < count; y++)
                {
                    image[x, height - y - 1] = color;
                }
            }

            return image;
        }

        public void CreateCumulativeHistogram()
        {
            CumulativeBins = new int[BinSize];

            int previous = 0;
            for (int i = 0; i < Bins.Length; i++)
            {
                int count = Bins[i].Count + previous;

                CumulativeBins[i] = count;
                previous = count;
            }

        }

        private void CreateBins(int bins)
        {
            Bins = new HistogramBin[bins];

            for (int i = 0; i < Bins.Length; i++)    
                Bins[i] = new HistogramBin();
        }

    }
}
