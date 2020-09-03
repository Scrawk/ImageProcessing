using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Threading;

using ImageProcessing.Interpolation;

namespace ImageProcessing.Images
{
	/// <summary>
	/// 
	/// </summary>
	public partial class ColorImage2D
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static ColorImage2D BilinearRescale(ColorImage2D image, int width, int height)
		{
			return Rescale(image, width, height, LinearInterpolation.Default);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static ColorImage2D BicubicRescale(ColorImage2D image, int width, int height)
		{
			return Rescale(image, width, height, CubicInterpolation.Default);
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static ColorImage2D BSplineRescale(ColorImage2D image, int width, int height)
		{
			return Rescale(image, width, height, SplineInterpolation.MitchellNetravli);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="func"></param>
		/// <returns></returns>
		public static ColorImage2D Rescale(ColorImage2D image, int width, int height, InterpolationFunction func)
		{
			var image2 = new ColorImage2D(width, height);
			var tmp = new ColorImage2D(width, image.Height);

			var xKernel = new PolyphaseKernel(func, image.Width, width);
			var yKernel = new PolyphaseKernel(func, image.Height, height);

			for (int y = 0; y < image.Height; y++)
				xKernel.ApplyHorizontal(y, image, tmp);

			for (int x = 0; x < width; x++)
				yKernel.ApplyVertical(x, tmp, image2);

			return image2;
		}

	}
}
