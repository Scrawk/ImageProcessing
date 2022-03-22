using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Threading;

using ImageProcessing.Interpolation;

namespace ImageProcessing.Images
{

	public enum RESCALE
    {
		BILINEAR,
		BICUBIC,
		BSPLINE,
		POINT
    }

	public static class RescaleColorImage2D
	{
		public static ColorImage2D Rescale(this ColorImage2D image, int scale, RESCALE method)
		{
			int width = image.Width * scale;
			int height = image.Height * scale;
			return Rescale(image, width, height, method);
		}

		public static ColorImage2D Rescale(this ColorImage2D image, int width, int height, RESCALE method)
        {
			ColorImage2D image2 = null;

            switch (method)
            {
                case RESCALE.BILINEAR:
					image = ColorImage2D.BilinearRescale(image, width, height);
					break;

                case RESCALE.BICUBIC:
					image = ColorImage2D.BicubicRescale(image, width, height);
					break;

                case RESCALE.BSPLINE:
					image = ColorImage2D.BSplineRescale(image, width, height);
					break;

                case RESCALE.POINT:
					image = ColorImage2D.PointRescale(image, width, height);
					break;
            }

            return image2;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class ColorImage2D
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="image"></param>
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
		/// <param name="image"></param>
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
		/// <param name="image"></param>
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
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static ColorImage2D PointRescale(ColorImage2D image, int width, int height)
		{
			var image2 = new ColorImage2D(width, height);

			int scaleX = image2.Width / image.Width;
			int scaleY = image2.Height / image.Height;

			image2.Fill((x, y) =>
			{
				int X = x / scaleX;
				int Y = y / scaleY;

				return image.GetPixel(X, Y, WRAP_MODE.CLAMP);
			});

			return image2;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="image"></param>
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
