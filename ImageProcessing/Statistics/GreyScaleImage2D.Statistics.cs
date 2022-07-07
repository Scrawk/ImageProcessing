using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Images
{
    public partial class GreyScaleImage2D
    {
        /// <summary>
        /// The minimum and maximum values in the image.
        /// </summary>
        /// <returns></returns>
        public void MinMaxValue(out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float v = this[x, y];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
            }
        }

        /// <summary>
        /// Find the min value in image.
        /// </summary>
        /// <returns>The min value.</returns>
        public float MinValue()
        {
            MinMaxValue(out float min, out float max);
            return min; 
        }

        /// <summary>
        /// Find the max value in image.
        /// </summary>
        /// <returns>The max value.</returns>
        public float MaxValue()
        {
            MinMaxValue(out float min, out float max);
            return max;
        }

        /// <summary>
        /// The sum of all values in the image.
        /// </summary>
        public float Sum()
        {
            float sum = 0;
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
        /// The mean of the values in the image.
        /// </summary>
        /// <returns></returns>
        public float Mean()
        {
            int size = Size.Product;
            if (size == 0) return 0;
            return Sum() / size;
        }

        /// <summary>
        /// The variance of the values in the image.
        /// </summary>
        /// <param name="mean">The mean of the values in the image.</param>
        /// <returns></returns>
        public float Variance(float mean)
        {
            int size = Size.Product;
            if (size == 0) return 0;

            float v = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float diff = this[x, y] - mean;
                    v += diff * diff;
                }
            }

            return v / size;
        }
    }
}
