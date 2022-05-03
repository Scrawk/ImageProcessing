using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{

	public partial class GreyScaleImage2D
	{

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
