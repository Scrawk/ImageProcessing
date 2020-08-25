using System;
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
	public partial class Image2D<T>
	{

		public float GetInterpolatedValue(float u, float v, InterpolationFunction func)
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

					p += GetValue(xi, yj) * func.GetWeight(x - xi);
				}

				q += p * func.GetWeight(y - yj);
			}

			return q;
		}

		public ColorRGB GetInterpolatedPixel(float u, float v, InterpolationFunction func)
		{
			float x = u * (Width - 1);
			float y = v * (Height - 1);
			int n = func.Size;
			int N = 2 * n - 1;

			ColorRGB q = new ColorRGB();
			for (int j = 0; j <= N; j++)
			{
				int yj = (int)Math.Floor(y) - n + 1 + j;

				ColorRGB p = new ColorRGB();
				for (int i = 0; i <= N; i++)
				{
					int xi = (int)Math.Floor(x) - n + 1 + i;

					p += GetPixel(xi, yj) * func.GetWeight(x - xi);
				}

				q += p * func.GetWeight(y - yj);
			}

			return q;
		}

	}
}
