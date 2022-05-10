using System;
using System.Collections.Generic;

using ImageProcessing.Images;
using ImageProcessing.Pixels;

using Common.Core.Colors;

namespace ImageProcessing.Statistics
{

    public class ColorHistogram
    {

        public ColorHistogram(int binSize)
        {
            BinSize = binSize;
        }

        public int BinSize { get; private set; }

        public int Channels => Histograms != null ? Histograms.Length : 0;

        private Histogram[] Histograms { get; set; }

        public override string ToString()
        {
            return string.Format("[ColorHistogram: BinSize={0}, Channels={1}]", 
                BinSize, Channels);
        }

        public void Clear()
        {
            Histograms = null;
        }

        public void Load(ColorImage2D image, bool inculdeAlpha = false)
        {
            Clear();

            if(inculdeAlpha)
                Histograms = new Histogram[4];
            else
                Histograms = new Histogram[3];

            for(int i = 0; i < Histograms.Length; i++)
            {
                Histograms[i] = new Histogram(BinSize);
                Histograms[i].Load(image, i);
            }
                
        }

        public ColorImage2D HistogramToImage(int channel, ColorRGBA color, ColorRGBA background, int height)
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            return Histograms[channel].HistogramToImage(color, background, height);
        }

        public ColorImage2D CumulativeHistogramToImage(int channel, ColorRGBA color, ColorRGBA background, int height)
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            return Histograms[channel].CumulativeHistogramToImage(color, background, height);
        }

        public void CreateCumulativeHistograms()
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            for (int i = 0; i < Histograms.Length; i++)
            {
                Histograms[i].CreateCumulativeHistogram();
            }
        }

    }

    
}
