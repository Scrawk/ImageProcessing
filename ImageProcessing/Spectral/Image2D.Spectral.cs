using System;
using System.Collections.Generic;
using System.Numerics;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Extensions;

namespace ImageProcessing.Images
{

    public partial class Image2D<T>
    {
        protected void DFT1D(Complex[] g, Complex[] G, bool forward)
        {
            int M = g.Length;
            double s = 1 / Math.Sqrt(M);

            for (int m = 0; m < M; m++)
            {
                double sumRe = 0;
                double sumIm = 0;
                double phim = 2 * Math.PI * m / M;

                for (int u = 0; u < M; u++)
                {
                    double gRe = g[u].Real;
                    double gIm = g[u].Imaginary;
                    double cosw = Math.Cos(phim * u);
                    double sinw = Math.Sin(phim * u);

                    if (!forward)
                        sinw = -sinw;

                    sumRe += gRe * cosw + gIm * sinw;
                    sumIm += gIm * cosw - gRe * sinw;
                }

                G[m] = new Complex(s * sumRe, s * sumIm);
            }
        }
    }
}
