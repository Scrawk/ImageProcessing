using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Images
{

	public partial class ColorImage2D
	{
		/// <summary>
		/// Apply a box blur and return as a new image.
		/// </summary>
		/// <param name="image">The input image.</param>
		/// <param name="size">The size in pixels of the kernel.</param>
		/// <returns>The new image.</returns>
		public static ColorImage2D BoxBlur(ColorImage2D image, int size)
		{
			var k = FilterKernel2D.BoxKernel(size);
			return Filter(image, k);
		}

		/// <summary>
		/// Apply a Gaussian blur and return as a new image.
		/// </summary>
		/// <param name="image">The input image.</param>
		/// <param name="sigma">The standard deviation of the blur kernel.</param>
		/// <returns>The new image.</returns>
		public static ColorImage2D GaussianBlur(ColorImage2D image, float sigma)
		{
			var k = FilterKernel2D.GaussianKernel(sigma);
			return Filter(image, k);
		}

		/// <summary>
		/// Apply a sharpen filter and return as a new image.
		/// </summary>
		/// <param name="image">The input image.</param>
		/// <returns>The new image.</returns>
		public static ColorImage2D SharpenFilter(ColorImage2D image)
		{
			var k = FilterKernel2D.SharpenKernel();
			return Filter(image, k);
		}

		/// <summary>
		/// Apply a unsharpen filter and return as a new image.
		/// </summary>
		/// <param name="image">The input image.</param>
		/// <returns>The new image.</returns>
		public static ColorImage2D UnsharpenFilter(ColorImage2D image)
		{
			var k = FilterKernel2D.UnsharpenKernel();
			return Filter(image, k);
		}

		/// <summary>
		/// Apply a filter and return as a new image.
		/// </summary>
		/// <param name="image">The input image.</param>
		/// <param name="k">The filter to apply.</param>
		/// <returns>The new image.</returns>
		public static ColorImage2D Filter(ColorImage2D image, FilterKernel2D k)
		{
			var image2 = new ColorImage2D(image.Size);

			image2.Fill((x, y) =>
			{
				return Filter(x, y, image, k) * k.Scale;
			});

			return image2;
		}

		/// <summary>
		/// Apply a filter to the image at index i,j.
		/// </summary>
		/// <param name="i">The first index.</param>
		/// <param name="j">The second index.</param>
		/// <param name="image">The input image.</param>
		/// <param name="k">The filter to apply.</param>
		/// <returns>The filter result.</returns>
		private static ColorRGB Filter(int i, int j, ColorImage2D image, FilterKernel2D k)
		{
			int half = k.Size / 2;

			ColorRGB sum = new ColorRGB(); ;
			for (int y = 0; y < k.Size; y++)
			{
				for (int x = 0; x < k.Size; x++)
				{
					int xi = MathUtil.Clamp(x + i - half, 0, image.Width - 1);
					int yj = MathUtil.Clamp(y + j - half, 0, image.Height - 1);

					sum += image[xi, yj] * k[x, y];
				}
			}

			return sum;
		}

		/// <summary>
		/// Get all the surrounding pixels from the image at location i,j.
		/// </summary>
		/// <param name="i">The first index.</param>
		/// <param name="j">The second index.</param>
		/// <param name="image">The input image.</param>
		/// <param name="list">The list of surrounding pixels.</param>
		/// <param name="size">The size of the surroundng box.</param>
		private static void GetColors(int i, int j, ColorImage2D image, List<ColorRGB> list, int size)
		{
			int half = size / 2;

			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					if (image.InBounds(xi, yj))
						list.Add(image[xi, yj]);
				}
			}

		}

	}
}
