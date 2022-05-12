using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Images
{
    public partial class VectorImage2D
    {
        /// <summary>
        /// The minimum and maximum values in the image.
        /// </summary>
        /// <returns></returns>
        public void MinMax(out Vector2f min, out Vector2f max)
        {
            min = Vector2f.PositiveInfinity;
            max = Vector2f.NegativeInfinity;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Vector2f v = this[x, y];

                    if (v.x < min.x) min.x = v.x;
                    if (v.y < min.y) min.y = v.y;

                    if (v.x > max.x) max.x = v.x;
                    if (v.y > max.y) max.y = v.y;
                }
            }
        }

        /// <summary>
        /// The sum of all values in the image.
        /// </summary>
        public Vector2f Sum()
        {
            Vector2f sum = Vector2f.Zero;
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
        public Vector2f Mean()
        {
            int size = Size.Product;
            if (size == 0) return Vector2f.Zero;
            return Sum() / size;
        }

        /// <summary>
        /// The variance of the values in the image.
        /// </summary>
        /// <param name="mean">The mean of the values in the image.</param>
        /// <returns></returns>
        public Vector2f Variance(float mean)
        {
            int size = Size.Product;
            if (size == 0) return Vector2f.Zero;

            Vector2f v = Vector2f.Zero;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Vector2f diff = this[x, y] - mean;
                    v += diff * diff;
                }
            }

            return v / size;
        }
    }
}
