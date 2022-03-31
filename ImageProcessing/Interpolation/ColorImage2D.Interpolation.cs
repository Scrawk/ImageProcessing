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

    /// <summary>
    /// 
    /// </summary>
    public partial class ColorImage2D
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		public void Rescale(int scale, RESCALE method = RESCALE.BICUBIC)
		{
			int width = Width * scale;
			int height = Height * scale;
			Rescale(width, height, method);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		public void Rescale(int width, int height, RESCALE method = RESCALE.BICUBIC)
		{
			ColorImage2D image2 = null;

			switch (method)
			{
				case RESCALE.BILINEAR:
					image2 = ColorImage2D.BilinearRescale(this, width, height);
					break;

				case RESCALE.BICUBIC:
					image2 = ColorImage2D.BicubicRescale(this, width, height);
					break;

				case RESCALE.BSPLINE:
					image2 = ColorImage2D.BSplineRescale(this, width, height);
					break;

				case RESCALE.POINT:
					image2 = ColorImage2D.PointRescale(this, width, height);
					break;
			}

			Resize(image2.Width, image2.Height);
			Fill(image2);
		}

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
			if (image.Width == width && image.Height == height)
				return image.Copy();

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
			if (image.Width == width && image.Height == height)
				return image.Copy();

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
