using System;
using System.Collections.Generic;

using ImageProcessing.Images;
using ImageProcessing.Statistics;

using Common.Core.Numerics;
using ImageProcessing.Thresholding;

namespace ImageProcessing.Images
{

	public partial class GreyScaleImage2D
	{

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="image"></param>
        /// <returns></returns>
		public static BinaryImage2D Threshold(GLOBAL_THRESHOLD method, GreyScaleImage2D image)
        {
            GlobalThresholder thresholder = null;

            switch (method)
            {
                case GLOBAL_THRESHOLD.ISODATA:
                    thresholder = new IsodataThresholder();
                    break;

                case GLOBAL_THRESHOLD.MAXENTROPY:
                    thresholder = new MaxEntropyThresholder();
                    break;

                case GLOBAL_THRESHOLD.MEAN:
                    thresholder = new MeanThresholder();
                    break;

                case GLOBAL_THRESHOLD.MINERROR:
                    thresholder = new MinErrorThresholder();
                    break;

                case GLOBAL_THRESHOLD.OTUS:
                    thresholder = new OtusThresholder();
                    break;
            }

            return thresholder.Threshold(image);
        }

    }
}
