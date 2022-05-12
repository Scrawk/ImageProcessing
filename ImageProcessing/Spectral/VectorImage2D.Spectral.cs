using System;
using System.Collections.Generic;
using System.Numerics;

using Common.Core.Numerics;

namespace ImageProcessing.Images
{
    public partial class VectorImage2D
    {
        /// <summary>
        /// Calculate the 2D inverse DFT on a vector image.
        /// Here we are using a vector to store the complex numbers
        /// where the x component is the real part and the y component is the imaginary part.
        /// </summary>
        /// <returns>THe real part of the transform in a greyscale image.</returns>
        public GreyScaleImage2D iDFT()
        {
            var image = new GreyScaleImage2D(Width, Height);

            var g = new Vector2f[Width];
            var G = new Vector2f[Width];

            //Transform all the rows of the image.

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var v = this[x, y];
                    g[x] = new Vector2f(v.x, v.y);
                }

                DFT1D(g, G, false);

                SetRow(G, y);
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
                    var v = this[x, y];
                    g[y] = new Vector2f(v.x, v.y);
                }

                DFT1D(g, G, false);

                for (int y = 0; y < Height; y++)
                {
                    image[x, y] = G[y].x;
                }
            }

            return image;
        }
    }
}
