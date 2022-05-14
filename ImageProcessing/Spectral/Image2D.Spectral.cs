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

        /// <summary>
        /// Calculate the 1D DFT on a array of complex numbers.
        /// Here we are using a vector to store the complex numbers
        /// where the x component is the real part and the y component is the imaginary part.
        /// </summary>
        /// <param name="g">The untransformed complex numbers.</param>
        /// <param name="G">The transformed complex numbers.</param>
        /// <param name="forward">Is this a forward or inverse transformation.</param>
        protected static void DFT1D(Vector2f[] g, Vector2f[] G, bool forward)
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
                    double gRe = g[u].x;
                    double gIm = g[u].y;
                    double cosw = Math.Cos(phim * u);
                    double sinw = Math.Sin(phim * u);

                    if (!forward)
                        sinw = -sinw;

                    sumRe += gRe * cosw + gIm * sinw;
                    sumIm += gIm * cosw - gRe * sinw;
                }

                G[m] = new Vector2f(s * sumRe, s * sumIm);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="G"></param>
        protected static void DCT1D(float[] g, float[] G)
        {
            int M = g.Length;
            double s = Math.Sqrt(2.0 / M);
        
            for (int m = 0; m < M; m++)
            {
                double cm = 1.0;

                if (m == 0)
                    cm = 1.0 / Math.Sqrt(2);

                double sum = 0;
                for (int u = 0; u < M; u++)
                {
                    double phi = Math.PI * m * (2 * u + 1) / (2 * M);
                    sum += g[u] * cm * Math.Cos(phi);
                }

                G[m] = (float)(s * sum);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="G"></param>
        protected static void iDCT1D(float[] g, float[] G)
        {
            int M = g.Length;
            double s = Math.Sqrt(2.0 / M);

            for (int u = 0; u < M; u++)
            {
                double sum = 0;
                for (int m = 0; m < M; m++)
                {
                    double cm = 1.0;

                    if (m == 0)
                        cm = 1.0 / Math.Sqrt(2);

                    double phi = Math.PI * m * (2 * u + 1) / (2 * M);
                    sum += g[u] * cm * Math.Cos(phi);
                }

                G[u] = (float)(s * sum);
            }
        }
    }
}
