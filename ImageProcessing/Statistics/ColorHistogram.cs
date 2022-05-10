using System;
using System.Collections.Generic;

using ImageProcessing.Images;
using ImageProcessing.Pixels;

using Common.Core.Colors;

namespace ImageProcessing.Statistics
{
    /// <summary>
    /// 
    /// </summary>
    public class ColorHistogram
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binSize"></param>
        public ColorHistogram(int binSize)
        {
            BinSize = binSize;
        }

        /// <summary>
        /// 
        /// </summary>
        public int BinSize { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Channels => Histograms != null ? Histograms.Length : 0;

        /// <summary>
        /// 
        /// </summary>
        private Histogram[] Histograms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[ColorHistogram: BinSize={0}, Channels={1}]", 
                BinSize, Channels);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            Histograms = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="inculdeAlpha"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="color"></param>
        /// <param name="background"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public ColorImage2D HistogramToImage(int channel, ColorRGBA color, ColorRGBA background, int height)
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            return Histograms[channel].HistogramToImage(color, background, height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="color"></param>
        /// <param name="background"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public ColorImage2D CumulativeHistogramToImage(int channel, ColorRGBA color, ColorRGBA background, int height)
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            return Histograms[channel].CumulativeHistogramToImage(color, background, height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
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
