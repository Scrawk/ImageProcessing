using System;
using System.Collections.Generic;
using System.Linq;

using ImageProcessing.Images;
using ImageProcessing.Pixels;

using Common.Core.Colors;
using Common.Core.Numerics;
using Common.Core.Extensions;

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
        /// The width of the image the histogram was created from.
        /// </summary>
        public int ImageWidth { get; private set; }

        /// <summary>
        /// The height of the image the histogram was created from.
        /// </summary>
        public int ImageHeight { get; private set; }

        /// <summary>
        /// Has the CFD been calculated.
        /// </summary>
        public bool HasCDF => CDF != null;

        /// <summary>
        /// The histograms bins the pixels are divided up into.
        /// </summary>
        private HistogramBin[] Bins { get; set; }

        /// <summary>
        /// The CFD bins the pixels are divided up into.
        /// </summary>
        private int[] CDF { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Histogram: BinSize={0}, HasCumulativeBins={1}, ImageWidth={2}, ImageHeight={3}]", 
                BinSize, HasCDF, ImageWidth, ImageHeight);
        }

        /// <summary>
        /// Clear the histogram of all data.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Bins.Length; i++)
                Bins[i].Clear();

            CDF = null;
            ImageWidth = 0;
            ImageHeight = 0;
        }

        /// <summary>
        /// Get the cdf value for a bin in the histogram.
        /// Will create the CDF if not already created.
        /// </summary>
        /// <param name="i">The bin index.</param>
        /// <returns>The bins cdf value</returns>
        public int GetCDF(int i)
        {
            CreateCDF();
            return CDF[i];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Histogram Copy()
        {
            var copy = new Histogram(BinSize);

            for(int i = 0;i < BinSize; i++)
            {
                if(Bins[i] != null)
                    copy.Bins[i] = Bins[i].Copy(); 
            }

            if (CDF != null)
                copy.CDF = CDF.Copy();

            return copy;
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
        /// Find the minimum value in the histogram.
        /// </summary>
        /// <returns>The minimum value in the histogram.</returns>
        public float MinValue()
        {
            for (int i = 0; i < Bins.Length; i++)
            {
                int count = Bins[i].Count;

                if (count != 0)
                    return i;
            }

            return 0;
        }

        /// <summary>
        /// Find the maximum value in the histogram.
        /// </summary>
        /// <returns>The maximum value in the histogram.</returns>
        public float MaxValue()
        {
            for (int i = Bins.Length-1; i >= 0; i--)
            {
                int count = Bins[i].Count;

                if (count != 0)
                    return i;
            }

            return 0;
        }

        /// <summary>
        /// The sum of all the bin sizes.
        /// </summary>
        /// <returns>The sum of all the bin sizes.</returns>
        public int BinSum()
        {
            int length = 0;
            for (int i = 0; i < Bins.Length; i++)
                length += Bins[i].Count;

            return length;
        }

        /// <summary>
        /// The normalized sqr distance between the histograms.
        /// </summary>
        /// <param name="histo"></param>
        /// <returns>The normalized sqr distance between the histograms.</returns>
        /// <exception cref="ArgumentException">If the histograms dont have the same bin size.</exception>
        public float SqrDistance(Histogram histo)
        {
            if (BinSize != histo.BinSize)
                throw new ArgumentException("Histograms need to havethe same bin size.");

            float sqdist = 0;

            for(int i = 0;i < Bins.Length;i++)
            {
                float diff = GetBinCount(i) - histo.GetBinCount(i);
                sqdist += diff * diff;
            }

            return sqdist / BinSize;
        }

        /// <summary>
        /// Sample the histogram for a random value.
        /// Will create the histograms CFD if not already created.
        /// </summary>
        /// <param name="rng">The random generator.</param>
        /// <returns>A random value that matchs the distribution of the histogram.</returns>
        public float Sample(Random rng)
        {
            CreateCDF();
            float max = CDF.Last();
            float t = (float)rng.NextDouble();
 
            for (int i = 0; i < BinSize; i++)
            {
                float cfd = CDF[i] / max;

                if (t < cfd)
                    return i / (BinSize - 1.0f);
            }

            return 1;
        }

        /// <summary>
        /// Equalizes the histogram.
        /// Attempts to make the histograom of a uniform distribution. 
        /// </summary>
        public void Equalize()
        {
            CreateCDF();

            float n = ImageWidth * ImageHeight;
            int BinSize1 = BinSize - 1;

            var pixels = new List<PixelIndex2D<float>>(BinSize);

            for(int i = 0; i < BinSize; i++)
            {
                var bin = Bins[i];
                float v = CDF[i] * (BinSize1) / n;
                v /= BinSize1;

                for (int j = 0; j < bin.Count; j++)
                {
                    var pixel = bin.GetPixel(j); 
                    pixel.Value = v;
                    pixels.Add(pixel);  
                }
            }

            Load(pixels, ImageWidth, ImageHeight);
        }

        /// <summary>
        /// Attempts to match the hisograom to the other histogram.
        /// </summary>
        /// <param name="other">THe histogram to match.</param>
        public void Match(Histogram other)
        {
            float invBinSize1 = 1.0f / (BinSize - 1.0f);

            var func = CreateMappingFunction(other);

            var pixels = new List<PixelIndex2D<float>>(BinSize);

            for (int i = 0; i < BinSize; i++)
            {
                var bin = Bins[i];
                float v = func[i] * invBinSize1;

                for (int j = 0; j < bin.Count; j++)
                {
                    var pixel = bin.GetPixel(j);
                    pixel.Value = v;
                    pixels.Add(pixel);
                }
            }

            Load(pixels, ImageWidth, ImageHeight);
        }

        /// <summary>
        /// Load the histogram with the data from a greyScale image.
        /// </summary>
        /// <param name="image">THe greyScale image to load.</param>
        public void Load(GreyScaleImage2D image)
        {
            Clear();

            ImageWidth = image.Width;
            ImageHeight = image.Height;

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

            ImageWidth = image.Width;
            ImageHeight = image.Height;

            image.Iterate((x, y) =>
            {
                var v = image.GetChannel(x, y, channel);

                int index = (int)(v * (BinSize - 1));

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
        /// <param name="pixels"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void Load(IList<PixelIndex2D<float>> pixels, int width, int height)
        {
            Clear();

            ImageWidth = width;
            ImageHeight = height;

            for(int i = 0; i < pixels.Count; i++)   
            {
                var p = pixels[i];

                int index = (int)(p.Value * (BinSize - 1));

                if (index >= 0 && index < BinSize)
                {
                    var bin = Bins[index];
                    bin.Add(new PixelIndex2D<float>(p.x, p.y, p.Value));
                }
            };
        }

        /// <summary>
        /// Convert the histogram back into a image.
        /// </summary>
        /// <returns>The image.</returns>
        public GreyScaleImage2D ToImage()
        {
            var image = new GreyScaleImage2D(ImageWidth, ImageHeight);

            for (int i = 0; i < BinSize; i++)
            {
                var bin = Bins[i];
                float v = i / (BinSize-1.0f);

                for (int j = 0; j < bin.Count; j++)
                {
                    var p = bin.GetPixel(j);
                    image[p.x, p.y] = v;
                }
            }

            return image;
        }

        /// <summary>
        /// Fill the channel of the color image.
        /// </summary>
        /// <param name="image">The color image to fill.</param>
        /// <param name="c">The images channel to fill.</param>
        public void FillChannel(ColorImage2D image, int c)
        {
            for (int i = 0; i < BinSize; i++)
            {
                var bin = Bins[i];
                float v = i / (BinSize - 1.0f);

                for (int j = 0; j < bin.Count; j++)
                {
                    var p = bin.GetPixel(j);
                    image.SetChannel(p.x, p.y, c, v);
                }
            }
        }

        /// <summary>
        /// Normalize the histogram.
        /// The range of the histogram will be modified
        /// to start at 0 and go to the bin size.
        /// </summary>
        public void Normalize()
        {
            float min = MinValue() / (BinSize - 1.0f);
            float max = MaxValue() / (BinSize - 1.0f);

            if (max <= 0) return;

            var pixels = new List<PixelIndex2D<float>>();

            for (int i = 0; i < BinSize; i++)
            {
                var bin = Bins[i];

                for (int j = 0; j < bin.Count; j++)
                {
                    var p = bin.GetPixel(j);
                    p.Value = MathUtil.Normalize(p.Value, min, max);

                    pixels.Add(p);  
                }
            }

            Load(pixels, ImageWidth, ImageHeight);

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
            CreateCDF();

            int width = BinSize;
            int max = CDF.Last();

            var image = new ColorImage2D(width, height);
            image.Fill(background);

            for (int x = 0; x < width; x++)
            {
                float count01 = CDF[x] / (float)max;
                int count = (int)(count01 * (height - 1));

                for (int y = 0; y < count; y++)
                {
                    image[x, height - y - 1] = color;
                }
            }

            return image;
        }

        /// <summary>
        /// Creates a image with the line graph of the histograms CFD.
        /// Used for debugging.
        /// </summary>
        /// <param name="color">The bars color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="height">The images height. The width will be the bin size.</param>
        /// <returns>The line graph image.</returns>
        public ColorImage2D CreateHistogramLineGraphCFD(ColorRGBA color, ColorRGBA background, int height)
        {
            CreateCDF();

            int width = BinSize;
            int max = CDF.Last();

            var image = new ColorImage2D(width, height);
            image.Fill(background);

            float count01 = CDF[0] / (float)max;
            int y = (int)(count01 * (height - 1));

            var previosPoint = new Point2i(0, height - y - 1);

            for (int x = 1; x < width; x++)
            {
                count01 = CDF[x] / (float)max;
                y = (int)(count01 * (height - 1));

                var point = new Point2i(x, height - y - 1);

                image.DrawLine(previosPoint, point, color);

                previosPoint = point;
            }

            return image;
        }

        /// <summary>
        /// Create the cumulative function distribution (CFD).
        /// </summary>
        public void CreateCDF()
        {
            if (CDF != null && CDF.Length == BinSize)
                return;

            CDF = new int[BinSize];

            int previous = 0;
            for (int i = 0; i < Bins.Length; i++)
            {
                int count = Bins[i].Count + previous;

                CDF[i] = count;
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
                    Bins[i] = new HistogramBin(i);
                else
                    Bins[i].Clear();
            }
                
        }

        /// <summary>
        /// Get the bins as a int array.
        /// </summary>
        /// <returns>The int array where each value represents 
        /// the numper of pixels in the bin.</returns>
        private int[] ToArray()
        {
            int[] array = new int[BinSize];

            for (int i = 0; i < BinSize; i++)
            {
                array[i] = Bins[i].Count;
            }

            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <exception cref="ArgumentException"></exception>
        private int[] CreateMappingFunction(Histogram other)
        {
            if (other.BinSize != BinSize)
                throw new ArgumentException("The other histogram must have the same bin size.");

            CreateCDF();
            other.CreateCDF();

            // pixel mapping function
            int[] func = new int[BinSize];

            // compute pixel mapping function
            for (int i = 0; i < BinSize; i++)
            {
                int j = BinSize - 1;
                do
                {
                    func[i] = j;
                    j--;
                }
                while (j >= 0 && CDF[i] <= other.CDF[j]);
            }

            return func;
        }

    }
}
