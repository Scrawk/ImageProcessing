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
	public partial class GreyScaleImage2D
	{

		public GreyScaleImage2D BilinearRescale(int width, int height)
		{
			return Rescale(width, height, LinearInterpolation.Default);
		}

		public GreyScaleImage2D BicubicRescale(int width, int height)
		{
			return Rescale(width, height, CubicInterpolation.Default);
		}

		public GreyScaleImage2D BSplineRescale(int width, int height)
		{
			return Rescale(width, height, SplineInterpolation.MitchellNetravli);
		}

		public GreyScaleImage2D Rescale(int width, int height, InterpolationFunction func)
		{
			var image = new GreyScaleImage2D(width, height);
			var tmp = new GreyScaleImage2D(width, Height);

			var xKernel = new PolyphaseKernel(func, Width, width);
			var yKernel = new PolyphaseKernel(func, Height, height);

			for (int y = 0; y < Height; y++)
				xKernel.ApplyHorizontal(y, this, tmp);

			for (int x = 0; x < width; x++)
				yKernel.ApplyVertical(x, tmp, image);

			return image;
		}

	}
}
