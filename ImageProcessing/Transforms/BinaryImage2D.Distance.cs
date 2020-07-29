using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Threading;

namespace ImageProcessing.Images
{
	/// <summary>
	/// 
	/// </summary>
	public partial class BinaryImage2D
	{

		public GreyScaleImage2D Distance(StructureElement2D b)
		{
			var image = new GreyScaleImage2D(Width, Height);
			image.Fill((x, y) => this[x, y] ? float.PositiveInfinity : 0);

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (!this[x, y]) continue;
					image[x, y] = MinDistance(x, y, image, b);
				}
			}

			for (int y = Height - 1; y >= 0; y--)
			{
				for (int x = Width - 1; x >= 0; x--)
				{
					if (!this[x, y]) continue;
					image[x, y] = MinDistance(x, y, image, b);
				}
			}

			return image;
		}

		public GreyScaleImage2D ApproxEuclideanDistance()
		{
			var image = new GreyScaleImage2D(Width, Height);
			image.Fill((x, y) => this[x, y] ? float.PositiveInfinity : 0);

			var k4 = StructureElement2D.ChessBoardElement();
			var k8 = StructureElement2D.CityBlockElement();

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (!this[x, y]) continue;

					float d4 = MinDistance(x, y, image, k4);
					float d8 = MinDistance(x, y, image, k8);

					float d1 = MathUtil.Sqr(d4 - d8);
					float d2 = MathUtil.Sqr(d8);

					image[x, y] = MathUtil.Sqrt(d1 + d2);
				}
			}

			for (int y = Height - 1; y >= 0; y--)
			{
				for (int x = Width - 1; x >= 0; x--)
				{
					if (!this[x, y]) continue;

					float d4 = MinDistance(x, y, image, k4);
					float d8 = MinDistance(x, y, image, k8);

					float d1 = MathUtil.Sqr(d4 - d8);
					float d2 = MathUtil.Sqr(d8);

					image[x, y] = MathUtil.Sqrt(d1 + d2);
				}
			}

			return image;
		}

		private static float MinDistance(int i, int j, GreyScaleImage2D a, StructureElement2D b)
		{
			int half = b.Size / 2;

			float dist = float.PositiveInfinity;

			for (int y = 0; y < b.Size; y++)
			{
				for (int x = 0; x < b.Size; x++)
				{
					int xi = MathUtil.Clamp(x + i - half, 0, a.Width - 1);
					int yj = MathUtil.Clamp(y + j - half, 0, a.Height - 1);

					dist = MathUtil.Min(dist, a[xi, yj] - b[x, y]);
				}
			}

			return dist;
		}

	}
}
