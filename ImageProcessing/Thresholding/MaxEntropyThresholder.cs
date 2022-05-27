using System;
using System.Collections.Generic;

using ImageProcessing.Images;
using ImageProcessing.Statistics;

using Common.Core.Numerics;

namespace ImageProcessing.Thresholding
{
    public class MaxEntropyThresholder : GlobalThresholder
	{
		/// <summary>
		/// Maximum entropy thresholder modeled after Kapur et al. (1985).
		/// 
		/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/threshold/global/MaxEntropyThresholder.java
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		protected override int GetThreshold(GreyScaleImage2D image)
		{
			Histogram histo = new Histogram(image, 256);

			int K = histo.BinSize;

			// normalized histogram (probabilities)
			double[] p = Normalize(histo);

			double EPSILON = 1E-12;

			// initialize S0, S1
			MakeTables(p, out double[] S0, out double[] S1);

			double P0 = 0, P1;
			int qMax = -1;
			double Hmax = double.NegativeInfinity;

			for (int q = 0; q <= K - 1; q++)
			{
				// one more step for logging
				P0 = P0 + p[q];
				P1 = 1 - P0;

				double H0 = (P0 > EPSILON) ? -S0[q] / P0 + Math.Log(P0) : 0;
				double H1 = (P1 > EPSILON) ? -S1[q] / P1 + Math.Log(P1) : 0;
				double H01 = H0 + H1;

				if (H01 > Hmax)
				{
					Hmax = H01;
					qMax = q;
				}
			}

			return qMax;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="h"></param>
		/// <returns></returns>
		private double[] Normalize(Histogram h)
		{
			int K = h.BinSize;
			int N = h.BinSum();

			double[] nh = new double[K];
			for (int i = 0; i < K; i++)
			{
				nh[i] = ((double)h[i]) / N;
			}

			return nh;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		/// <param name="S0"></param>
		/// <param name="S1"></param>
		private void MakeTables(double[] p, out double[] S0, out double[] S1)
		{
			double EPSILON = 1E-12;
			int K = p.Length;

			// make tables S0[], S1[]
			S0 = new double[K];
			S1 = new double[K];

			double s0 = 0;
			for (int i = 0; i < K; i++)
			{
				if (p[i] > EPSILON)
				{
					s0 = s0 + p[i] * Math.Log(p[i]);
				}
				S0[i] = s0;
			}

			double s1 = 0;
			for (int i = K - 1; i >= 0; i--)
			{
				S1[i] = s1;
				if (p[i] > EPSILON)
				{
					s1 = s1 + p[i] * Math.Log(p[i]);
				}
			}
		}
	}
}
