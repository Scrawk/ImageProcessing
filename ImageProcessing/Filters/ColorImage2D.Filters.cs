using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{

	public partial class ColorImage2D
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
		public static ColorImage2D BoxBlur(ColorImage2D image, int size, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
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
		public static ColorImage2D GaussianBlur(ColorImage2D image, float sigma, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
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
		public static ColorImage2D SharpenFilter(ColorImage2D image, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
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
		public static ColorImage2D UnsharpenFilter(ColorImage2D image, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
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
		public static ColorImage2D Filter(ColorImage2D image, FilterKernel2D k, Box2i? bounds, GreyScaleImage2D mask, WRAP_MODE mode)
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
						var pixel1 = image.GetPixel(p.x, p.y);
						var pixel2 = Filter(p.x, p.y, image, k, mode) * k.Scale;

						var a = MathUtil.Clamp01(mask[p]);
						image2[p] = ColorRGB.Lerp(pixel1, pixel2, a);
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
		private static ColorRGB Filter(int i, int j, ColorImage2D image, FilterKernel2D k, WRAP_MODE mode)
		{
			int half = k.Size / 2;

			ColorRGB sum = new ColorRGB(); ;
			for (int y = 0; y < k.Size; y++)
			{
				for (int x = 0; x < k.Size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					sum += image.GetPixel(xi, yj, mode) * k[x, y];
				}
			}

			return sum;
		}

	}
}
