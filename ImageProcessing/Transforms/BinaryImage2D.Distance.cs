using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Threading;

namespace ImageProcessing.Images
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BinaryImage2D
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GreyScaleImage2D DistanceTransform()
        {
            var image = new GreyScaleImage2D(Width, Height);


			return image;
		}

    }
}
