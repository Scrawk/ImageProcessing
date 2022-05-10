using System;
using System.Collections.Generic;
using System.Linq;

using ImageProcessing.Images;
using ImageProcessing.Pixels;

using Common.Core.Colors;

namespace ImageProcessing.Statistics
{

    public class HistogramBin
    {
        public HistogramBin()
        {
            Pixels = new List<PixelIndex2D<float>>();
        }

        public int Count => Pixels.Count;

        private List<PixelIndex2D<float>> Pixels { get; set; }

        public override string ToString()
        {
            return string.Format("[HistogramBin: Count={0}]", Count);
        }

        public void Clear()
        {
            Pixels.Clear();
        }

        public void Add(PixelIndex2D<float> pixel)
        {
            Pixels.Add(pixel);
        }

    }
}

