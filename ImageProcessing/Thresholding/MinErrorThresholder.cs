using System;
using System.Collections.Generic;

using ImageProcessing.Images;
using ImageProcessing.Statistics;

using Common.Core.Numerics;

namespace ImageProcessing.Thresholding
{
    public class MinErrorThresholder : GlobalThresholder
    {
		/// <summary>
		/// Minimum Error thresholder after Kittler and Illingworth (1986).
		/// 
		/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/threshold/global/MinErrorThresholder.java
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		protected override int GetThreshold(GreyScaleImage2D image)
		{
			Histogram histo = new Histogram(image, 256);

			int K = histo.BinSize;

			// set up S2_0, S2_1, N
			int N = MakeSigmaTables(histo, out double[] S2_0, out double[] S2_1);

			int n0 = 0, n1;
			int qMin = -1;
			double eMin = double.PositiveInfinity;

			for (int q = 0; q <= K - 2; q++)
			{
				n0 = n0 + histo[q];
				n1 = N - n0;

				if (n0 > 0 && n1 > 0)
				{
					// could use n0, n1 instead
					double P0 = (double)n0 / N;
					double P1 = (double)n1 / N;

					double e = P0 * Math.Log(S2_0[q]) + P1 * Math.Log(S2_1[q]) - 2 * (P0 * Math.Log(P0) + P1 * Math.Log(P1));

					if (e < eMin)
					{   
						// minimize e;
						eMin = e;
						qMin = q;
					}
				}
			}

			return qMin;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="h"></param>
		/// <param name="S2_0"></param>
		/// <param name="S2_1"></param>
		/// <returns></returns>
		private int MakeSigmaTables(Histogram h, out double[] S2_0, out double[] S2_1)
		{
			int K = h.BinSize;

			// variance of a uniform distribution in unit interval
			double unitVar = 1d / 12;
			S2_0 = new double[K];
			S2_1 = new double[K];

			int n0 = 0;
			long A0 = 0;
			long B0 = 0;

			for (int q = 0; q < K; q++)
			{
				// need a long type to avoid overflows
				long ql = q;
				n0 = n0 + h[q];
				A0 = A0 + h[q] * ql;
				B0 = B0 + h[q] * ql * ql;

				S2_0[q] = (n0 > 0) ? unitVar + ((double)B0 - (double)A0 * A0 / n0) / n0 : 0;
			}

			int n1 = 0;
			long A1 = 0;
			long B1 = 0;
			S2_1[K - 1] = 0;

			for (int q = K - 2; q >= 0; q--)
			{
				long qp1 = q + 1;
				n1 = n1 + h[q + 1];
				A1 = A1 + h[q + 1] * qp1;
				B1 = B1 + h[q + 1] * qp1 * qp1;

				S2_1[q] = (n1 > 0) ? unitVar + ((double)B1 - (double)A1 * A1 / n1) / n1 : 0;
			}

			return n0;
		}
	}
}
