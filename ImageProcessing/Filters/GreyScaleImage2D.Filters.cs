using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{

	public partial class GreyScaleImage2D
	{
		/// <summary>
		/// Apply a box blur and return as a new image.
		/// </summary>
		/// <param name="image">The input image.</param>
		/// <param name="size">The size in pixels of the kernel.</param>
		/// <param name="bounds">The area to apply the filter to.</param>
		/// <param name="mask">If not null only areas where mask has a value will have the filter applied.</param>
		/// <param name="mode">The wrap mode to use.</param>
		/// <returns>The new image.</returns>
		public static GreyScaleImage2D BoxBlur(GreyScaleImage2D image, int size, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
		{
			var k = FilterKernel2D.BoxKernel(size);
			return Filter(image, k, bounds, mask, mode);
		}

		/// <summary>
		/// Apply a Gaussian blur and return as a new image.
		/// </summary>
		/// <param name="image">The input image.</param>
		/// <param name="sigma">The standard deviation of the blur kernel.</param>
		/// <param name="bounds">The area to apply the filter to.</param>
		/// <param name="mask">If not null only areas where mask has a value will have the filter applied.</param>
		/// <param name="mode">The wrap mode to use.</param>
		/// <returns></returns>
		public static GreyScaleImage2D GaussianBlur(GreyScaleImage2D image, float sigma, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
		{
			var k = FilterKernel2D.GaussianKernel(sigma);
			return Filter(image, k, bounds, mask, mode);
		}

		/// <summary>
		/// Apply a sharpen filter and return as a new image.
		/// </summary>
		/// <param name="image">The input image.</param>
		/// <param name="bounds">The area to apply the filter to.</param>
		/// <param name="mask">If not null only areas where mask has a value will have the filter applied.</param>
		/// <param name="mode">The wrap mode to use.</param>
		/// <returns>The new image.</returns>
		public static GreyScaleImage2D SharpenFilter(GreyScaleImage2D image, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
		{
			var k = FilterKernel2D.SharpenKernel();
			return Filter(image, k, bounds, mask, mode);
		}

		/// <summary>
		/// Apply a unsharpen filter and return as a new image.
		/// </summary>
		/// <param name="image">The input image.</param>
		/// <param name="bounds">The area to apply the filter to.</param>
		/// <param name="mask">If not null only areas where mask has a value will have the filter applied.</param>
		/// <param name="mode">The wrap mode to use.</param>
		/// <returns>The new image.</returns>
		public static GreyScaleImage2D UnsharpenFilter(GreyScaleImage2D image, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
		{
			var k = FilterKernel2D.UnsharpenKernel();
			return Filter(image, k, bounds, mask, mode);
		}

		/// <summary>
		/// Apply a filter and return as a new image.
		/// </summary>
		/// <param name="image">The input image.</param>
		/// <param name="k">The filter to apply.</param>
		/// <param name="bounds">The area to apply filter.</param>
		/// <param name="mask">If not null only areas where mask has a value will have the filter applied.</param>
		/// <param name="mode">The wrap mode to use.</param>
		/// <returns></returns>
		public static GreyScaleImage2D Filter(GreyScaleImage2D image, FilterKernel2D k, Box2i? bounds, GreyScaleImage2D mask, WRAP_MODE mode)
		{
			if (bounds == null)
				bounds = new Box2i(0, 0, image.Width, image.Height);

			var image2 = image.Copy();

			foreach (var p in bounds.Value.EnumerateBounds())
			{
				if (image.InBounds(p))
				{
					if (mask == null)
					{
						image2[p] = Filter(p.x, p.y, image, k, mode) * k.Scale;
					}
					else
					{
						var value1 = image.GetValue(p.x, p.y);
						var value2 = Filter(p.x, p.y, image, k, mode) * k.Scale;

						var a = MathUtil.Clamp01(mask[p]);
						image2[p] = MathUtil.Lerp(value1, value2, a);
					}
				}

			}

			return image2;
		}

		/// <summary>
		/// Apply a median filter and return as a new image.
		/// </summary>
		/// <param name="image">The input image.</param>
		/// <param name="size">The size in pixels of the kernel.</param>
		/// <returns>The new image.</returns>
		public static GreyScaleImage2D MedianFilter(GreyScaleImage2D image, int size)
		{
			var image2 = new GreyScaleImage2D(image.Size);

			var list = new List<float>(size * size);

			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					list.Clear();
					GetValues(x, y, image2, list, size);

					list.Sort();
					int count = list.Count;

					if (count % 2 != 0)
						image2[x, y] = list[count / 2];
					else
					{
						float v0 = list[count / 2 - 1];
						float v1 = list[count / 2];
						image2[x, y] = (v0 + v1) * 0.5f;
					}
				}
			}

			return image2;
		}

		/// <summary>
		/// Apply a filter to the image at index i,j.
		/// </summary>
		/// <param name="i">The first index.</param>
		/// <param name="j">The second index.</param>
		/// <param name="image">The input image.</param>
		/// <param name="k">The filter to apply.</param>
		/// <param name="mode">The wrap mode to use.</param>
		/// <returns>The filter result.</returns>
		private static float Filter(int i, int j, GreyScaleImage2D image, FilterKernel2D k, WRAP_MODE mode)
		{
			int half = k.Size / 2;

			float sum = 0;
			for (int y = 0; y < k.Size; y++)
			{
				for (int x = 0; x < k.Size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					sum += image.GetValue(xi, yj, mode) * k[x, y];
				}
			}

			return sum;
		}

		/// <summary>
		/// Get all the surrounding values from the image at location i,j.
		/// </summary>
		/// <param name="i">The first index.</param>
		/// <param name="j">The second index.</param>
		/// <param name="image">The input image.</param>
		/// <param name="list">The list of surrounding pixels.</param>
		/// <param name="size">The size of the surroundng box.</param>
		private static void GetValues(int i, int j, GreyScaleImage2D image, List<float> list, int size)
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
