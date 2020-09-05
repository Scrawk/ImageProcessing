using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Images
{
    public partial class GreyScaleImage2D
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public (float min, float max) MinMax()
        {
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float v = this[x, y];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
            }

            return (min, max);
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
        public float Mean()
        {
            int size = Size.Product;
            if (size == 0) return 0;
            return Sum() / size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mean"></param>
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
