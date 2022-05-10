using System;
using System.Collections.Generic;
using System.Linq;

using ImageProcessing.Images;
using ImageProcessing.Pixels;

using Common.Core.Colors;
using Common.Core.RandomNum;

namespace ImageProcessing.Statistics
{
    /// <summary>
    /// 
    /// </summary>
    public class Histogram
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bins"></param>
        public Histogram(int bins)
        {
            CreateBins(bins);
        }

        /// <summary>
        /// 
        /// </summary>
        public int BinSize => Bins.Length;

        /// <summary>
        /// 
        /// </summary>
        public bool HasCulumativeHistogram => CumulativeBins != null;

        /// <summary>
        /// 
        /// </summary>
        private HistogramBin[] Bins { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private int[] CumulativeBins { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Histogram: BinSize={0}, HasCumulativeBins={1}]", 
                BinSize, HasCulumativeHistogram);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Bins.Length; i++)
                Bins[i].Clear();

            CumulativeBins = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetBinCount(int index)
        {
            return Bins[index].Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int BinLength()
        {
            int length = 0;
            for (int i = 0; i < Bins.Length; i++)
                length += Bins[i].Count;

            return length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="channel"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="background"></param>
        /// <param name="height"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="background"></param>
        /// <param name="height"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bins"></param>
        private void CreateBins(int bins)
        {
            Bins = new HistogramBin[bins];

            for (int i = 0; i < Bins.Length; i++)    
                Bins[i] = new HistogramBin();
        }

    }
}
