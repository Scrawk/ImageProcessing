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
        /// The sum of all pixels in the image.
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
        /// The mean of the pixels in the image.
        /// </summary>
        /// <returns></returns>
        public ColorRGB Mean()
        {
            int size = Size.Product;
            if (size == 0) return new ColorRGB();
            return Sum() / size;
        }

    }
}
