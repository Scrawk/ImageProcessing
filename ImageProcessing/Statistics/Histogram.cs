using System;
using System.Collections.Generic;
using System.Linq;

using ImageProcessing.Images;
using ImageProcessing.Pixels;

using Common.Core.Colors;
using Common.Core.Numerics;

namespace ImageProcessing.Statistics
{
    /// <summary>
    /// A histogram of a single channel of values in a image.
    /// </summary>
    public class Histogram
    {
        /// <summary>
        /// Create a new histogram.
        /// </summary>
        /// <param name="bins">The number of bins in the histogram.</param>
        public Histogram(int bins)
        {
            CreateBins(bins);
        }

        /// <summary>
        /// Create a new histogram.
        /// </summary>
        /// <param name="image">The image to load.</param>
        /// <param name="bins">The number of bins in the histogram.</param>
        public Histogram(GreyScaleImage2D image,  int bins)
        {
            CreateBins(bins);
            Load(image);
        }

        /// <summary>
        /// The number of bins in the histogram.
        /// </summary>
        public int BinSize => Bins.Length;

        /// <summary>
        /// Has the CFD been calculated.
        /// </summary>
        public bool HasCulumativeHistogram => CumulativeBins != null;

        /// <summary>
        /// The histograms bins the pixels are divided up into.
        /// </summary>
        private HistogramBin[] Bins { get; set; }

        /// <summary>
        /// The CFD bins the pixels are divided up into.
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
        /// Clear the histogram of all data.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Bins.Length; i++)
                Bins[i].Clear();

            CumulativeBins = null;
        }

        /// <summary>
        /// Get the number of pixels in the bin at the index.
        /// </summary>
        /// <param name="index">The bin index.</param>
        /// <returns>The number of pixels in the bin.</returns>
        public int GetBinCount(int index)
        {
            return Bins[index].Count;
        }

        /// <summary>
        /// Find the bin with the most pixels in it.
        /// </summary>
        /// <returns>The largest bin size.</returns>
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
        /// The sum of all the bin sizes.
        /// </summary>
        /// <returns>The sum of all the bin sizes.</returns>
        public int BinLength()
        {
            int length = 0;
            for (int i = 0; i < Bins.Length; i++)
                length += Bins[i].Count;

            return length;
        }

        /// <summary>
        /// Load the histogram with the data from a greyScale image.
        /// </summary>
        /// <param name="image">THe greyScale image to load.</param>
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
        /// Load the histogram with the data from a color images channel.
        /// </summary>
        /// <param name="image">The color image.</param>
        /// <param name="channel">The channel to load.</param>
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
        /// Creates a image with the bar graph of the histogram.
        /// Used for debugging.
        /// </summary>
        /// <param name="color">The bars color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="height">The images height. The width will be the bin size.</param>
        /// <returns>The bar graph image.</returns>
        public ColorImage2D CreateHistogramBarGraph(ColorRGBA color, ColorRGBA background, int height)
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
        /// Creates a image with the line graph of the histogram.
        /// Used for debugging.
        /// </summary>
        /// <param name="color">The bars color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="height">The images height. The width will be the bin size.</param>
        /// <returns>The line graph image.</returns>
        public ColorImage2D CreateHistogramLineGraph(ColorRGBA color, ColorRGBA background, int height)
        {
            int width = BinSize;
            int max = MaxBinCount();

            var image = new ColorImage2D(width, height);
            image.Fill(background);

            float count01 = Bins[0].Count / (float)max;
            int y = (int)(count01 * (height - 1));

            var previosPoint = new Point2i(0, height - y - 1);

            for (int x = 1; x < width; x++)
            {
                count01 = Bins[x].Count / (float)max;
                y = (int)(count01 * (height - 1));

                var point = new Point2i(x, height - y - 1);

                image.DrawLine(previosPoint, point, color);

                previosPoint = point;
            }

            return image;
        }

        /// <summary>
        /// Creates a image with the bar graph of the histograms CFD.
        /// Used for debugging.
        /// </summary>
        /// <param name="color">The bars color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="height">The images height. The width will be the bin size.</param>
        /// <returns>The bar graph image.</returns>
        public ColorImage2D CreateHistogramBarGraphCFD(ColorRGBA color, ColorRGBA background, int height)
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
        /// Create the cumulative function distribution (CFD).
        /// </summary>
        public void CreateCumulativeHistogram()
        {
            if (CumulativeBins != null && CumulativeBins.Length == BinSize)
                return;

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
        /// Creates the histograms bins.
        /// </summary>
        /// <param name="bins"></param>
        private void CreateBins(int bins)
        {
            if(Bins == null || Bins.Length != bins)
                Bins = new HistogramBin[bins];

            for (int i = 0; i < Bins.Length; i++)
            {
                if (Bins[i] == null)
                    Bins[i] = new HistogramBin();
                else
                    Bins[i].Clear();
            }
                
        }

    }
}
