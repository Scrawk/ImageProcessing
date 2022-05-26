using System;
using System.Collections.Generic;

using ImageProcessing.Images;
using ImageProcessing.Statistics;

using Common.Core.Numerics;


namespace ImageProcessing.Images
{

	public partial class GreyScaleImage2D
	{
		/// <summary>
		/// Minimum Error thresholder after Kittler and Illingworth (1986).
		/// 
		/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/threshold/global/MinErrorThresholder.java
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public static BinaryImage2D MinErrorThreshold(GreyScaleImage2D image)
		{
			Histogram histo = new Histogram(image, 256);
			
			int K = histo.BinSize;

			// set up S2_0, S2_1, N
			int N = MakeSigmaTables(histo, out double[] S2_0, out double[] S2_1); 

			int n0 = 0, n1;
			int qMin = -1;
			double eMin = Double.PositiveInfinity;

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
					{       // minimize e;
						eMin = e;
						qMin = q;
					}
				}
			}

			float threshold = MathUtil.Clamp01(qMin / 255.0f);
			return image.ToBinaryImage(threshold);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="h"></param>
		/// <param name="S2_0"></param>
		/// <param name="S2_1"></param>
		/// <returns></returns>
		private static int MakeSigmaTables(Histogram h, out double[] S2_0, out double[] S2_1)
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

		/// <summary>
		/// Maximum entropy thresholder modeled after Kapur et al. (1985).
		/// 
		/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/threshold/global/MaxEntropyThresholder.java
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public static BinaryImage2D MaxEntropyThreshold(GreyScaleImage2D image)
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

			float threshold = MathUtil.Clamp01(qMax / 255.0f);
			return image.ToBinaryImage(threshold);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="h"></param>
		/// <returns></returns>
		private static double[] Normalize(Histogram h)
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
		private static void MakeTables(double[] p, out double[] S0, out double[] S1)
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

		/// <summary>
		/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/threshold/global/MeanThresholder.java
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public static BinaryImage2D MeanThreshold(GreyScaleImage2D image)
		{
			Histogram histo = new Histogram(image, 256);

			// calculate mean of entire image:
			int K = histo.BinSize;
			int cnt = 0;
			double sum = 0;
			for (int i = 0; i < K; i++)
			{
				cnt += histo[i];
				sum += i * histo[i];
			}

			int mean = (int)Math.Round(sum / cnt);

			// count resulting background pixels:
			int n0 = 0;
			for (int i = 0; i <= mean; i++)
			{
				n0 += histo[i];
			}

			// determine if background or foreground is empty:
			int q = (n0 < cnt) ? mean : -1;

			float threshold = MathUtil.Clamp01(q / 255.0f);
			return image.ToBinaryImage(threshold);
		}

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
		public static BinaryImage2D IsodataThreshold(GreyScaleImage2D image)
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

			float threshold = MathUtil.Clamp01(q / 255.0f);
			return image.ToBinaryImage(threshold);
		}

		/// <summary>
		/// Thresholder as described in N. Otsu, "A threshold selection method from gray-level histograms",
		/// IEEE Transactions on Systems, Man, and Cybernetics 9(1), 62-66 (1979).
		/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/threshold/global/OtsuThresholder.java
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public static BinaryImage2D OtusThreshold(GreyScaleImage2D image)
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

			float threshold = MathUtil.Clamp01(qMax / 255.0f);
			return image.ToBinaryImage(threshold);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="h"></param>
		/// <param name="M0"></param>
		/// <param name="M1"></param>
		/// <returns></returns>
		private static int MakeMeanTables(Histogram h, out double[] M0, out double[] M1)
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
