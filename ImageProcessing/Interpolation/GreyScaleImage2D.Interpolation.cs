﻿using System;
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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="method"></param>
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
		public void Rescale(int width, int height, RESCALE method = RESCALE.BICUBIC)
		{
			GreyScaleImage2D image2 = null;

			switch (method)
			{
				case RESCALE.BILINEAR:
					image2 = GreyScaleImage2D.BilinearRescale(this, width, height);
					break;

				case RESCALE.BICUBIC:
					image2 = GreyScaleImage2D.BicubicRescale(this, width, height);
					break;

				case RESCALE.BSPLINE:
					image2 = GreyScaleImage2D.BSplineRescale(this, width, height);
					break;

				case RESCALE.POINT:
					image2 = GreyScaleImage2D.PointRescale(this, width, height);
					break;
			}

			Resize(image2.Width, image2.Height);
			image2.CopyTo(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static GreyScaleImage2D BilinearRescale(GreyScaleImage2D image, int width, int height)
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
		public static GreyScaleImage2D BicubicRescale(GreyScaleImage2D image, int width, int height)
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
		public static GreyScaleImage2D BSplineRescale(GreyScaleImage2D image, int width, int height)
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
		public static GreyScaleImage2D PointRescale(GreyScaleImage2D image, int width, int height)
		{
			if (image.Width == width && image.Height == height)
				return image.Copy();

			var image2 = new GreyScaleImage2D(width, height);

			int scaleX = image2.Width / image.Width;
			int scaleY = image2.Height / image.Height;

			image2.Fill((x, y) =>
			{
				int X = x / scaleX;
				int Y = y / scaleY;

				return image.GetValue(X, Y, WRAP_MODE.CLAMP);
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
		public static GreyScaleImage2D Rescale(GreyScaleImage2D image, int width, int height, InterpolationFunction func)
		{
			if (image.Width == width && image.Height == height)
				return image.Copy();

			var image2 = new GreyScaleImage2D(width, height);
			var tmp = new GreyScaleImage2D(width, image.Height);

			var xKernel = new PolyphaseKernel(func, image.Width, width);
			var yKernel = new PolyphaseKernel(func, image.Height, height);

			for (int y = 0; y < image.Height; y++)
				xKernel.ApplyHorizontal(y, image, tmp);

			for (int x = 0; x < width; x++)
				yKernel.ApplyVertical(x, tmp, image2);

			return image2;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="u"></param>
		/// <param name="v"></param>
		/// <param name="func"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public float GetInterpolatedValue(float u, float v, InterpolationFunction func, WRAP_MODE mode = WRAP_MODE.CLAMP)
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
					p += GetValue(xi, yj, mode) * func.GetWeight(x - xi);
				}

				q += p * func.GetWeight(y - yj);
			}

			return q;
		}

	}
}
