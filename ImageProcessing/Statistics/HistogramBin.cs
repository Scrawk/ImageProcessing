using System;
using System.Collections.Generic;
using System.Linq;

using ImageProcessing.Images;
using ImageProcessing.Pixels;

using Common.Core.Colors;

namespace ImageProcessing.Statistics
{
    /// <summary>
    /// A bin in a histogram that holds the pixels from a image.
    /// </summary>
    public class HistogramBin
    {
        /// <summary>
        /// Create a new bin.
        /// </summary>
        public HistogramBin()
        {
            Pixels = new List<PixelIndex2D<float>>();
        }

        /// <summary>
        /// THe number of pixels in the bin.
        /// </summary>
        public int Count => Pixels.Count;

        /// <summary>
        /// THe bins pixels.
        /// </summary>
        private List<PixelIndex2D<float>> Pixels { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[HistogramBin: Count={0}]", Count);
        }

        /// <summary>
        /// Clear the bin of all pixels.
        /// </summary>
        public void Clear()
        {
            Pixels.Clear();
        }

        /// <summary>
        /// Add a pixel index to the bin.
        /// </summary>
        /// <param name="pixel"></param>
        public void Add(PixelIndex2D<float> pixel)
        {
            Pixels.Add(pixel);
        }

        /// <summary>
        /// Calculate the mean of all pixels in the bin.
        /// </summary>
        /// <returns>The mean of all pixels in the bin.</returns>
        public float CalculateMean()
        {
            if(Count == 0)
                return 0;

            float mean = 0;
            for (int i = 0; i < Count; i++)
                mean += Pixels[i].Value;

            return mean / Count;
        }

        /// <summary>
        /// Calculate the variance of all pixels in the bin.
        /// </summary>
        /// <param name="mean">The mean of all the pixels in the bin.</param>
        /// <returns>The variance of all pixels in the bin.</returns>
        public float CalculateVariance(float mean)
        {
            if (Count == 0)
                return 0;

            float v = 0;
            for (int i = 0; i < Count; i++)
            {
                float diff = Pixels[i].Value - mean;
                v += diff * diff;
            }

            return v / Count;
        }

    }
}

