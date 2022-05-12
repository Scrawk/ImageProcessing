﻿using System;
using System.Collections.Generic;
using System.Numerics;

using Common.Core.Numerics;

namespace ImageProcessing.Images
{
    public partial class GreyScaleImage2D
    {
        public VectorImage2D DFT()
        {
            var image = new VectorImage2D(Width, Height);

            var g = new Complex[Width];
            var G = new Complex[Width];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var v = this[x, y];
                    g[x] = new Complex(v, 0);
                }

                DFT1D(g, G, true);

                for (int x = 0; x < Width; x++)
                {
                    image[x, y] = new Vector2f(G[x].Real, G[x].Imaginary);
                }
            }

            if(Width != Height)
            {
                g = new Complex[Height];
                G = new Complex[Height];
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var v = image[x, y];
                    g[y] = new Complex(v.x, v.y);
                }

                DFT1D(g, G, true);

                for (int y = 0; y < Height; y++)
                {
                    image[x, y] = new Vector2f(G[y].Real, G[y].Imaginary);
                }
            }

            return image;
        }

    }
}
