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

    }
}

