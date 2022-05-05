using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Pixels
{
    /// <summary>
    /// A set of pixel indices.
    /// Can be used to calculate some proerties of the set. 
    /// </summary>
    public class ColorPixelSet2D : PixelSet2D<ColorRGBA>
    {
        /// <summary>
        /// 
        /// </summary>
        public ColorPixelSet2D()
        {
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixels"></param>
        public ColorPixelSet2D(List<PixelIndex2D<ColorRGBA>> pixels) : base(pixels)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[ColorPixelSet2D: Count={0}]", Count);
        }

        /// <summary>
        /// The mean value of the pixels in the set.
        /// </summary>
        /// <returns>The mean value of the pixels in the set.</returns>
        public ColorRGBA CalculateMean()
        {
            ColorRGBA mean = new ColorRGBA();
            if (Count == 0) return mean;

            for (int i = 0; i < Pixels.Count; i++)
                mean += Pixels[i].Value;

            return mean / Count;
        }

    }
}
