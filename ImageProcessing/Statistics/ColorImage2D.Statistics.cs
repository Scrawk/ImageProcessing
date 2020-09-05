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
        /// 
        /// </summary>
        public ColorRGB Sum()
        {
            ColorRGB sum = new ColorRGB();
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
        public ColorRGB Mean()
        {
            int size = Size.Product;
            if (size == 0) return new ColorRGB();
            return Sum() / size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mean"></param>
        /// <returns></returns>
        public Matrix Covariance(ColorRGB mean)
        {
            const int dimensions = 3;
            var cv = new Matrix(dimensions, dimensions);

            int size = Size.Product;
            if (size == 0) return cv;
            
            ColorRGB[,] deviations = new ColorRGB[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    deviations[x, y] = this[x, y] - mean;
                }
            }

            for (int j = 0; j < dimensions; j++)
            {
                for (int i = 0; i < dimensions; i++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            cv[i, j] += deviations[x,y][i] * deviations[x,y][j];
                        }
                    }
                }
            }

            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                    cv[i, j] /= size;
            }

            return cv;
        }
    }
}
