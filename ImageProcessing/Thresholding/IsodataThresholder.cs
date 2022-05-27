using System;
using System.Collections.Generic;

using ImageProcessing.Images;
using ImageProcessing.Statistics;

using Common.Core.Numerics;

namespace ImageProcessing.Thresholding
{
    public class IsodataThresholder : GlobalThresholder
	{
		/// <summary>
		///  This thresholder implements the algorithm proposed by Ridler and Calvard (1978),
		/// T.W.Ridler, S.Calvard, Picture thresholding using an iterative selection method,
		/// IEEE Trans.System, Man and Cybernetics, SMC-8 (August 1978) 630-632.
		/// described in Glasbey/Horgan: "Image Analysis for the Biological Sciences" (Ch. 4).
		///
		/// Fast version using tables of background and foreground means.
		/// 
		/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/threshold/global/IsodataThresholder.java
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		protected override int GetThreshold(GreyScaleImage2D image)
		{
			Histogram histo = new Histogram(image, 256);

			int K = histo.BinSize;
			int MAX_ITERATIONS = 100;
			var N = MakeMeanTables(histo, out double[] M0, out double[] M1);

			// start with total mean
			int q = (int)M0[K - 1];
			int q_;

			int i = 0;
			do
			{
				i++;

				// background or foreground is empty
				if (M0[q] < 0 || M1[q] < 0)
					break;

				q_ = q;
				q = (int)((M0[q] + M1[q]) / 2);
			}
			while (q != q_ && i < MAX_ITERATIONS);
;
			return q;
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
