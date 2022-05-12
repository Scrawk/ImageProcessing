using System;
using System.Collections.Generic;

using ImageProcessing.Images;
using ImageProcessing.Pixels;

using Common.Core.Colors;

namespace ImageProcessing.Statistics
{
    /// <summary>
    /// A histogram for color images which consists 
    /// of a float histogram for each channel in the image.
    /// </summary>
    public class ColorHistogram
    {
        /// <summary>
        /// Create a new histogram.
        /// </summary>
        /// <param name="bins">The number of bins in the histogram.</param>
        public ColorHistogram(int bins)
        {
            BinSize = bins;
        }

        /// <summary>
        /// Create a new histogram.
        /// </summary>
        /// <param name="image">The image to load.</param>
        /// <param name="bins">The number of bins in the histogram.</param>
        public ColorHistogram(ColorImage2D image, int bins)
        {
            BinSize = bins;
            Load(image);
        }

        /// <summary>
        /// The number of bins in the histogram.
        /// </summary>
        public int BinSize { get; private set; }

        /// <summary>
        /// The number of channels in the histogram.
        /// </summary>
        public int Channels => Histograms != null ? Histograms.Length : 0;

        /// <summary>
        /// The histograms, one for each channel.
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
        /// Clear the histogram of all data.
        /// </summary>
        public void Clear()
        {
            Histograms = null;
        }

        /// <summary>
        /// Get the histogram for a channel.
        /// </summary>
        /// <param name="i">The channels index.</param>
        /// <returns>The histogram for channel i.</returns>
        /// <exception cref="NullReferenceException">If the channels have not been created.</exception>
        public Histogram GetChannel(int i)
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            return Histograms[i];
        }

        /// <summary>
        /// The normalized sqr distance between the histograms.
        /// </summary>
        /// <param name="histo"></param>
        /// <returns>The normalized sqr distance between the histograms.</returns>
        /// <exception cref="ArgumentException">If the histograms dont have the same bin size or number of channels.</exception>
        public float SqrDistance(ColorHistogram histo)
        {
            if (BinSize != histo.BinSize)
                throw new ArgumentException("Histograms need to havethe same bin size.");

            if (Channels != histo.Channels)
                throw new ArgumentException("Histograms need to havethe same number of channels.");

            float sqdist = 0;

            for (int i = 0; i < Channels; i++)
                sqdist += Histograms[i].SqrDistance(histo.GetChannel(i));
            
            return sqdist / Channels;
        }

        /// <summary>
        /// Sample the histogram for a random value.
        /// Will create the histograms CFD if not already created.
        /// </summary>
        /// <param name="rng">The random number generator.</param>
        /// <returns>A random value that matchs the distribution of the histogram.</returns>
        public ColorRGBA Sample(Random rng)
        {
            ColorRGBA color = new ColorRGBA(0,0,0,1);

            for (int i = 0; i < Channels; i++)
            {
                color[i] = Histograms[i].Sample(rng);
            }

            return color;
        }

        /// <summary>
        /// Load the color image into the histogram.
        /// </summary>
        /// <param name="image">The color image.</param>
        /// <param name="inculdeAlpha">Should a histogram of the alpha channel be created.</param>
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
        /// Normalize the histogram.
        /// </summary>
        public void Normalize()
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            for (int i = 0; i < Histograms.Length; i++)
            {
                Histograms[i].Normalize();
            }
        }

        /// <summary>
        /// Convert the histogram back into a image.
        /// </summary>
        /// <param name="width">The images width.</param>
        /// <param name="height">The images height.</param>
        /// <returns>The image.</returns>
        public ColorImage2D ToImage(int width, int height)
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            var image = new ColorImage2D(width, height);

            for (int i = 0; i < Histograms.Length; i++)
            {
                Histograms[i].FillChannel(image, i);
            }

            return image;
        }

        /// <summary>
        /// Creates a image with the bar graph of the histogram.
        /// Used for debugging.
        /// </summary>
        /// <param name="channel">The channel to create the graph from.</param>
        /// <param name="color">The bars color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="height">The images height. The width will be the bin size.</param>
        /// <returns>The bar graph image.</returns>
        /// <exception cref="NullReferenceException">If no data has been loaded into the histogram.</exception>
        public ColorImage2D CreateHistogramBarGraph(int channel, ColorRGBA color, ColorRGBA background, int height)
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            return Histograms[channel].CreateHistogramBarGraph(color, background, height);
        }

        /// <summary>
        /// Creates a image with the line graph of the histogram.
        /// Used for debugging.
        /// </summary>
        /// <param name="colors">The color array for each channel.</param>
        /// <param name="background">The background color.</param>
        /// <param name="height">The images height. The width will be the bin size.</param>
        /// <returns>The line graph image.</returns>
        /// <exception cref="NullReferenceException">If no data has been loaded into the histogram.</exception>
        public ColorImage2D CreateHistogramLineGraph(ColorRGBA[] colors, ColorRGBA background, int height)
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            var image0 = Histograms[0].CreateHistogramLineGraph(colors[0], background, height);

            for(int i = 1; i < Histograms.Length; i++)
            {
                var image = Histograms[i].CreateHistogramLineGraph(colors[i], ColorRGBA.Clear, height);
                image0.Add(image);
            }

            return image0;  
        }

        /// <summary>
        /// Creates a image with the line graph of the histogram.
        /// Used for debugging.
        /// </summary>
        /// <param name="channel">The channel to create the graph from.</param>
        /// <param name="color">The lines color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="height">The images height. The width will be the bin size.</param>
        /// <returns>The line graph image.</returns>
        /// <exception cref="NullReferenceException">If no data has been loaded into the histogram.</exception>
        public ColorImage2D CreateHistogramLineGraph(int channel, ColorRGBA color, ColorRGBA background, int height)
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            return Histograms[channel].CreateHistogramLineGraph(color, background, height);
        }

        /// <summary>
        /// Creates a image with the line graph of the histogram CFD.
        /// Used for debugging.
        /// </summary>
        /// <param name="colors">The color array for each channel.</param>
        /// <param name="background">The background color.</param>
        /// <param name="height">The images height. The width will be the bin size.</param>
        /// <returns>The line graph image.</returns>
        /// <exception cref="NullReferenceException">If no data has been loaded into the histogram.</exception>
        public ColorImage2D CreateHistogramLineGraphCFD(ColorRGBA[] colors, ColorRGBA background, int height)
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            var image0 = Histograms[0].CreateHistogramLineGraphCFD(colors[0], background, height);

            for (int i = 1; i < Histograms.Length; i++)
            {
                var image = Histograms[i].CreateHistogramLineGraphCFD(colors[i], ColorRGBA.Clear, height);
                image0.Add(image);
            }

            return image0;
        }

        /// <summary>
        /// Creates a image with the bar graph of the histograms CFD.
        /// Used for debugging.
        /// </summary>
        /// <param name="channel">The channel to create the graph from.</param>
        /// <param name="color">The bars color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="height">The images height. The width will be the bin size.</param>
        /// <returns>The bar graph image.</returns>
        /// <exception cref="NullReferenceException">If no data has been loaded into the histogram.</exception>
        public ColorImage2D CreateHistogramBarGraphCFD(int channel, ColorRGBA color, ColorRGBA background, int height)
        {
            if (Histograms == null)
                throw new NullReferenceException("Histograms have not been created.");

            return Histograms[channel].CreateHistogramBarGraphCFD(color, background, height);
        }

        /// <summary>
        /// Create the cumulative function distribution (CFD).
        /// </summary>
        /// <exception cref="NullReferenceException">If no data has been loaded into the histogram.</exception>
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
