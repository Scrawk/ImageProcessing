using System;
using System.Collections.Generic;
using System.Numerics;

using Common.Core.Numerics;

namespace ImageProcessing.Images
{
    public partial class ColorImage2D
    {
        /// <summary>
        /// Calculate the 2D DFT on a greayscale image.
        /// Here we are using a vector to store the complex numbers
        /// where the x component is the real part and the y component is the imaginary part.
        /// </summary>
        /// <param name="image">The image to transform.</param>
        /// <param name="dft">A list to hold each transformed channel of the image.</param>
        public static void DFT(ColorImage2D image, List<VectorImage2D> dft)
        {
            for (int i = 0; i < image.Channels; i++)
                dft.Add(DFT(image, i));
        }

        /// <summary>
        /// Calculate the 2D DFT on a greayscale image.
        /// Here we are using a vector to store the complex numbers
        /// where the x component is the real part and the y component is the imaginary part.
        /// </summary>
        /// <param name="image">The image to transform.</param>
        /// <param name="channel">The channel to transform</param>
        /// <returns>A vector image containing the spectrum as complex numbers.</returns>
        public static VectorImage2D DFT(ColorImage2D image, int channel)
        {
            int Width = image.Width;
            int Height = image.Height;
            var dft = new VectorImage2D(Width, Height);

            var g = new Vector2f[Width];
            var G = new Vector2f[Width];

            //Transform all the rows of the image.

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var v = image[x, y][channel];
                    g[x] = new Vector2f(v, 0);
                }

                DFT1D(g, G, true);

                dft.SetRow(G, y);
            }

            if (Width != Height)
            {
                g = new Vector2f[Height];
                G = new Vector2f[Height];
            }

            //Transform all the columns of the image.

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var v = dft[x, y];
                    g[y] = new Vector2f(v.x, v.y);
                }

                DFT1D(g, G, true);

                dft.SetColumn(G, x);
            }

            return dft;
        }
    }
}
