using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{

	public partial class Image2D<T>
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
		public static IMAGE BoxBlur<IMAGE>(IMAGE image, int size, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
			where IMAGE : IImage2D, new()
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
		public static IMAGE GaussianBlur<IMAGE>(IMAGE image, float sigma, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
			where IMAGE : IImage2D, new()
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
		public static IMAGE SharpenFilter<IMAGE>(IMAGE image, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
			where IMAGE : IImage2D, new()
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
		public static IMAGE UnsharpenFilter<IMAGE>(IMAGE image, Box2i? bounds = null, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.CLAMP)
			where IMAGE : IImage2D, new()
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
		public static IMAGE Filter<IMAGE>(IMAGE image, FilterKernel2D k, Box2i? bounds, GreyScaleImage2D mask, WRAP_MODE mode)
			where IMAGE : IImage2D, new()
		{
			int width = image.Width;
			int height = image.Height;

			if (bounds == null)
				bounds = new Box2i(0, 0, width-1, height-1);

			var image2 = new IMAGE();
			image2.Resize(width, height);

			foreach (var p in bounds.Value.EnumerateBounds())
			{
				ColorRGB pixel;

				if (mask == null)
				{
					pixel = Filter(p.x, p.y, image, k, mode) * k.Scale;
				}
				else
				{
					var pixel1 = image.GetPixel(p.x, p.y);
					var pixel2 = Filter(p.x, p.y, image, k, mode) * k.Scale;

					var a = MathUtil.Clamp01(mask[p]);
					pixel = ColorRGB.Lerp(pixel1, pixel2, a);
				}

				image2.SetPixel(p.x, p.y, pixel, mode);
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
		private static ColorRGB Filter<IMAGE>(int i, int j, IMAGE image, FilterKernel2D k, WRAP_MODE mode)
			where IMAGE : IImage2D, new()
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
