using System;
using System.Collections.Generic;

using ImageProcessing.Images;
using ImageProcessing.Statistics;

using Common.Core.Numerics;

namespace ImageProcessing.Thresholding
{
    public class OtusThresholder : GlobalThresholder
    {

		/// <summary>
		/// Thresholder as described in N. Otsu, "A threshold selection method from gray-level histograms",
		/// IEEE Transactions on Systems, Man, and Cybernetics 9(1), 62-66 (1979).
		/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/threshold/global/OtsuThresholder.java
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		protected override int GetThreshold(GreyScaleImage2D image)
		{

			Histogram histo = new Histogram(image, 256);

			int K = histo.BinSize;

			var N = MakeMeanTables(histo, out double[] M0, out double[] M1);

			double sigma2Bmax = 0;
			int qMax = -1;
			int n0 = 0;

			// examine all possible threshold values q:
			for (int q = 0; q <= K - 2; q++)
			{
				n0 = n0 + histo.GetBinCount(q);
				int n1 = N - n0;

				if (n0 > 0 && n1 > 0)
				{
					double meanDiff = M0[q] - M1[q];
					double sigma2B = meanDiff * meanDiff * n0 * n1;
					// (1/N^2) has been omitted

					if (sigma2B > sigma2Bmax)
					{
						sigma2Bmax = sigma2B;
						qMax = q;
					}
				}
			}

			return qMax;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="h"></param>
		/// <param name="M0"></param>
		/// <param name="M1"></param>
		/// <returns></returns>
		private int MakeMeanTables(Histogram h, out double[] M0, out double[] M1)
		{
			int K = h.BinSize;
			int n0 = 0;
			long s0 = 0;

			M0 = new double[K];
			M1 = new double[K];

			for (int q = 0; q < K; q++)
			{
				n0 = n0 + h.GetBinCount(q);
				s0 = s0 + q * h.GetBinCount(q);

				M0[q] = (n0 > 0) ? ((double)s0) / n0 : -1;
			}

			int n1 = 0;
			long s1 = 0;
			M1[K - 1] = 0;

			for (int q = h.BinSize - 2; q >= 0; q--)
			{
				n1 = n1 + h.GetBinCount(q + 1);
				s1 = s1 + (q + 1) * h.GetBinCount(q + 1);

				M1[q] = (n1 > 0) ? ((double)s1) / n1 : -1;
			}

			return n0;
		}
	}
}
