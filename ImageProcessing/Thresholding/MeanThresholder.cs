using System;
using System.Collections.Generic;

using ImageProcessing.Images;
using ImageProcessing.Statistics;

using Common.Core.Numerics;

namespace ImageProcessing.Thresholding
{
    public class MeanThresholder : GlobalThresholder
	{
		/// <summary>
		/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/threshold/global/MeanThresholder.java
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		protected override int GetThreshold(GreyScaleImage2D image)
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

			return q;
		}
	}
}
