using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Pixels
{
    public class ColorPixelSet2D : PixelSet2D<ColorRGBA>
    {
        public ColorPixelSet2D()
        {
        
        }

        public ColorPixelSet2D(List<PixelIndex2D<ColorRGBA>> pixels) : base(pixels)
        {
            
        }

        public override string ToString()
        {
            return string.Format("[ColorPixelSet2D: Count={0}]", Count);
        }

        public ColorRGBA Mean()
        {
            ColorRGBA mean = new ColorRGBA();
            if (Count == 0) return mean;

            for (int i = 0; i < Pixels.Count; i++)
                mean += Pixels[i].Value;

            return mean / Count;
        }

    }
}
