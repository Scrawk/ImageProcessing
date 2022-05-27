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
		LANZCOS,
		POINT
	}

	public enum INTERPOLATION
	{
		BILINEAR,
		BICUBIC,
		BSPLINE,
		LANZCOS,
		POINT
	}

	/// <summary>
	/// 
	/// </summary>
	public partial class Image2D<T>
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="u"></param>
		/// <param name="v"></param>
		/// <param name="method"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public ColorRGBA GetPixelInterpolated(float u, float v, INTERPOLATION method, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
			switch (method)
			{
				case INTERPOLATION.BILINEAR:
					return GetPixelInterpolated(u, v, LinearInterpolation.Default, mode);

				case INTERPOLATION.BICUBIC:
					return GetPixelInterpolated(u, v, CubicInterpolation.Default, mode);

				case INTERPOLATION.BSPLINE:
					return GetPixelInterpolated(u, v, SplineInterpolation.MitchellNetravli, mode);

				case INTERPOLATION.LANZCOS:
					return GetPixelInterpolated(u, v, LanzcosInterpolation.Default4, mode);

				case INTERPOLATION.POINT:
					{
						int x = (int)(u * (Width - 1));
						int y = (int)(v * (Height - 1));
						return GetPixel(x, y, mode);
					}

				default:
					return GetPixelInterpolated(u, v, LinearInterpolation.Default, mode);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="u"></param>
		/// <param name="v"></param>
		/// <param name="c"></param>
		/// <param name="method"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public float GetChannelInterpolated(float u, float v, int c, INTERPOLATION method, WRAP_MODE mode = WRAP_MODE.CLAMP)
		{
			switch (method)
			{
				case INTERPOLATION.BILINEAR:
					return GetChannelInterpolated(u, v, c, LinearInterpolation.Default, mode);

				case INTERPOLATION.BICUBIC:
					return GetChannelInterpolated(u, v, c, CubicInterpolation.Default, mode);

				case INTERPOLATION.BSPLINE:
					return GetChannelInterpolated(u, v, c, SplineInterpolation.MitchellNetravli, mode);

				case INTERPOLATION.LANZCOS:
					return GetChannelInterpolated(u, v, c, LanzcosInterpolation.Default4, mode);

				case INTERPOLATION.POINT:
					{
						int x = (int)(u * (Width - 1));
						int y = (int)(v * (Height - 1));
						return GetChannel(x, y, c, mode);
					}

				default:
					return GetChannelInterpolated(u, v, c, LinearInterpolation.Default, mode);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="u"></param>
		/// <param name="v"></param>
		/// <param name="func"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public ColorRGBA GetPixelInterpolated(float u, float v, InterpolationFunction func, WRAP_MODE mode = WRAP_MODE.CLAMP)
		{
			float x = u * (Width - 1);
			float y = v * (Height - 1);
			int n = func.Size;
			int N = 2 * n - 1;

			ColorRGBA q = new ColorRGBA();
			for (int j = 0; j <= N; j++)
			{
				int yj = (int)Math.Floor(y) - n + 1 + j;

				ColorRGBA p = new ColorRGBA();
				for (int i = 0; i <= N; i++)
				{
					int xi = (int)Math.Floor(x) - n + 1 + i;

					p += GetPixel(xi, yj, mode) * func.GetWeight(x - xi);
				}

				q += p * func.GetWeight(y - yj);
			}

			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="u"></param>
		/// <param name="v"></param>
		/// <param name="c"></param>
		/// <param name="func"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public float GetChannelInterpolated(float u, float v, int c, InterpolationFunction func, WRAP_MODE mode = WRAP_MODE.CLAMP)
		{
			float x = u * (Width - 1);
			float y = v * (Height - 1);
			int n = func.Size;
			int N = 2 * n - 1;

			float q = 0;
			for (int j = 0; j <= N; j++)
			{
				int yj = (int)Math.Floor(y) - n + 1 + j;

				float p = 0;
				for (int i = 0; i <= N; i++)
				{
					int xi = (int)Math.Floor(x) - n + 1 + i;

					p += GetChannel(xi, yj, c, mode) * func.GetWeight(x - xi);
				}

				q += p * func.GetWeight(y - yj);
			}

			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="IMAGE"></typeparam>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="mode"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		public static IMAGE Rescale<IMAGE>(IMAGE image, int width, int height, RESCALE method = RESCALE.BICUBIC, WRAP_MODE mode = WRAP_MODE.CLAMP)
			where IMAGE : Image2D<T>, new()
		{

			switch (method)
			{
				case RESCALE.BILINEAR:
					return Rescale(image, width, height, LinearInterpolation.Default, mode);

				case RESCALE.BICUBIC:
					return Rescale(image, width, height, CubicInterpolation.Default, mode);

				case RESCALE.BSPLINE:
					return Rescale(image, width, height, SplineInterpolation.MitchellNetravli, mode);

				case RESCALE.POINT:
					return PointRescale(image, width, height, mode);

				case RESCALE.LANZCOS:
					return Rescale(image, width, height, LanzcosInterpolation.Default4, mode);

				default:
					return Rescale(image, width, height, LinearInterpolation.Default, mode);
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="IMAGE"></typeparam>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static IMAGE PointRescale<IMAGE>(IMAGE image, int width, int height, WRAP_MODE mode = WRAP_MODE.CLAMP)
			where IMAGE : IImage2D, new()
		{
			var image2 = new IMAGE();
			image2.Resize(width, height);

			int scaleX = image2.Width / image.Width;
			int scaleY = image2.Height / image.Height;

			for (int y = 0; y < image2.Height; y++)
			{
				for (int x = 0; x < image2.Width; x++)
				{
					int X = x / scaleX;
					int Y = y / scaleY;

					var pixel = image.GetPixel(X, Y, mode);
					image2.SetPixel(x, y, pixel);
				}
			}

			return image2;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="IMAGE"></typeparam>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="func"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static IMAGE Rescale<IMAGE>(IMAGE image, int width, int height, InterpolationFunction func, WRAP_MODE mode = WRAP_MODE.CLAMP)
			where IMAGE : IImage2D, new()
		{
			var image2 = new IMAGE();
			image2.Resize(width, height);

			var tmp = new IMAGE();
			tmp.Resize(width, image.Height);

			var xKernel = new PolyphaseKernel(func, image.Width, width);
			var yKernel = new PolyphaseKernel(func, image.Height, height);

			for (int y = 0; y < image.Height; y++)
				xKernel.ApplyHorizontal<IMAGE>(y, image, tmp, mode);

			for (int x = 0; x < width; x++)
				yKernel.ApplyVertical<IMAGE>(x, tmp, image2, mode);

			return image2;
		}

	}
}
