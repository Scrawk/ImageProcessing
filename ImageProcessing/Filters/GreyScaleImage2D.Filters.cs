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
		public GreyScaleImage2D BoxBlur(int size)
		{
			var k = FilterKernel2D.BoxKernel(size);
			return Filter(k);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sigma"></param>
		/// <returns></returns>
		public GreyScaleImage2D GaussianBlur(float sigma)
		{
			var k = FilterKernel2D.GaussianKernel(sigma);
			return Filter(k);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public GreyScaleImage2D SharpenFilter()
		{
			var k = FilterKernel2D.SharpenKernel();
			return Filter(k);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public GreyScaleImage2D UnsharpenFilter()
		{
			var k = FilterKernel2D.UnsharpenKernel();
			return Filter(k);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public GreyScaleImage2D Filter(FilterKernel2D k)
		{
			var image = new GreyScaleImage2D(Width, Height);

			image.ParallelFill((x, y) =>
			{
				return Filter(x, y, this, k) * k.Scale;
			});

			return image;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="a"></param>
		/// <param name="k"></param>
		/// <returns></returns>
		private static float Filter(int i, int j, GreyScaleImage2D a, FilterKernel2D k)
		{
			int half = k.Size / 2;

			float sum = 0;
			for (int y = 0; y < k.Size; y++)
			{
				for (int x = 0; x < k.Size; x++)
				{
					int xi = MathUtil.Clamp(x + i - half, 0, a.Width - 1);
					int yj = MathUtil.Clamp(y + j - half, 0, a.Height - 1);

					sum += a[xi, yj] * k[x, y];
				}
			}

			return sum;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public GreyScaleImage2D MedianFilter(int size)
		{
			var image = new GreyScaleImage2D(Width, Height);

			int blockSize = BlockSize();
			var blocks = ThreadingBlock2D.CreateBlocks(Width, Height, blockSize);
			Parallel.ForEach(blocks, (block) =>
			{
				var list = new List<float>(size * size);

				for (int y = block.Min.y; y < block.Max.y; y++)
				{
					for (int x = block.Min.x; x < block.Max.x; x++)
					{
						list.Clear();
						GetValues(x, y, this, list, size);

						list.Sort();
						int count = list.Count;

						if (count % 2 != 0)
							image[x, y] = list[count / 2];
						else
						{
							float v0 = list[count / 2 - 1];
							float v1 = list[count / 2];
							image[x, y] = (v0 + v1) * 0.5f;
						}
					}
				}
			});

			return image;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="a"></param>
		/// <param name="list"></param>
		/// <param name="size"></param>
		private static void GetValues(int i, int j, GreyScaleImage2D a, List<float> list, int size)
		{
			int half = size / 2;

			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					if (xi < 0 || xi >= a.Width) continue;
					if (yj < 0 || yj >= a.Height) continue;

					list.Add(a[xi, yj]);
				}
			}

		}

	}
}
