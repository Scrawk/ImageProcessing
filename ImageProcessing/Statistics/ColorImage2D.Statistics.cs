using System;
using System.Collections.Generic;
using System.Text;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Images
{
    public partial class ColorImage2D
    {
        /// <summary>
        /// The minimum and maximum values in the image.
        /// </summary>
        /// <returns></returns>
        public void MinMaxRGBA(out ColorRGBA min, out ColorRGBA max)
        {
            min = new ColorRGBA(float.PositiveInfinity, float.PositiveInfinity);
            max = new ColorRGBA(float.NegativeInfinity, float.NegativeInfinity);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var pixel = this[x, y];

                    for(int i = 0; i < 4; i++)
                    {
                        float v = pixel[i];
                        if (v < min[i]) min[i] = v;
                        if (v > max[i]) max[i] = v;
                    }

                }
            }
        }

        /// <summary>
        /// The minimum and maximum values in the image.
        /// </summary>
        /// <returns></returns>
        public void MinMaxIntensity(out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float v = this[x, y].Intensity;
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
            }
        }

        /// <summary>
        /// The sum of all pixels in the image.
        /// </summary>
        public ColorRGBA Sum()
        {
            ColorRGBA sum = new ColorRGBA();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    sum += this[x, y];
                }
            }

            return sum;
        }

        /// <summary>
        /// The mean of the pixels in the image.
        /// </summary>
        /// <returns></returns>
        public ColorRGBA Mean()
        {
            int size = Size.Product;
            if (size == 0) return new ColorRGBA();
            return Sum() / size;
        }

    }
}
