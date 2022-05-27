using System;
using System.Collections.Generic;

using ImageProcessing.Images;
using ImageProcessing.Statistics;

using Common.Core.Numerics;

namespace ImageProcessing.Thresholding
{

	public enum GLOBAL_THRESHOLD
    {
		ISODATA,
		MAXENTROPY,
		MEAN,
		MINERROR,
		OTUS
    }

    public abstract class GlobalThresholder
    {

        protected abstract int GetThreshold(GreyScaleImage2D image);


		public BinaryImage2D Threshold(GreyScaleImage2D image)
		{
			int q = GetThreshold(image);
			if (q > 0)
			{
				float threshold = MathUtil.Clamp01(q / 255.0f);
				return image.ToBinaryImage(threshold);
			}
			else
			{
				return new BinaryImage2D(image.Width, image.Height);
			}
		}

	}
}
