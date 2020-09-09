using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Common.Core.Numerics;
using Common.Core.Threading;

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
		/// <param name="size"></param>
		/// <returns></returns>
		public static GreyScaleImage2D BoxBlur(GreyScaleImage2D image, int size)
		{
			var k = FilterKernel2D.BoxKernel(size);
			return Filter(image, k);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sigma"></param>
		/// <returns></returns>
		public static GreyScaleImage2D GaussianBlur(GreyScaleImage2D image, float sigma)
		{
			var k = FilterKernel2D.GaussianKernel(sigma);
			return Filter(image, k);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static GreyScaleImage2D SharpenFilter(GreyScaleImage2D image)
		{
			var k = FilterKernel2D.SharpenKernel();
			return Filter(image, k);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static GreyScaleImage2D UnsharpenFilter(GreyScaleImage2D image)
		{
			var k = FilterKernel2D.UnsharpenKernel();
			return Filter(image, k);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public static GreyScaleImage2D Filter(GreyScaleImage2D image, FilterKernel2D k)
		{
			var image2 = new GreyScaleImage2D(image.Size);

			image2.Fill((x, y) =>
			{
				return Filter(x, y, image, k) * k.Scale;
			});

			return image2;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="image"></param>
		/// <param name="k"></param>
		/// <returns></returns>
		private static float Filter(int i, int j, GreyScaleImage2D image, FilterKernel2D k)
		{
			int half = k.Size / 2;

			float sum = 0;
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

		/*

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static GreyScaleImage2D MedianFilter(GreyScaleImage2D image, int size)
		{
			var image2 = new GreyScaleImage2D(image.Size);

			int blockSize = image2.BlockSize();
			var blocks = ThreadingBlock2D.CreateBlocks(image2.Width, image2.Height, blockSize);
			Parallel.ForEach(blocks, (block) =>
			{
				var list = new List<float>(size * size);

				for (int y = block.Min.y; y < block.Max.y; y++)
				{
					for (int x = block.Min.x; x < block.Max.x; x++)
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
			});

			return image2;
		}

		*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="image"></param>
		/// <param name="list"></param>
		/// <param name="size"></param>
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
