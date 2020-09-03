using System;
using System.Collections.Generic;

using Common.Core.Numerics;

namespace ImageProcessing.Images
{
	/// <summary>
	/// 
	/// </summary>
	public partial class BinaryImage2D
	{

		public static GreyScaleImage2D CityBlockDistance(BinaryImage2D image)
        {
			var e = StructureElement2D.CityBlockElement();
			return Distance(image, e);
		}

		public static GreyScaleImage2D ChessBoardDistance(BinaryImage2D image)
		{
			var e = StructureElement2D.ChessBoardElement();
			return Distance(image, e);
		}

		public static GreyScaleImage2D Distance(BinaryImage2D image, StructureElement2D b)
		{
			var image2 = new GreyScaleImage2D(image.Size);
			image2.Fill((x, y) => image[x, y] ? float.PositiveInfinity : 0);

			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					if (!image[x, y]) continue;
					image2[x, y] = MinDistance(x, y, image2, b);
				}
			}

			for (int y = image.Height - 1; y >= 0; y--)
			{
				for (int x = image.Width - 1; x >= 0; x--)
				{
					if (!image[x, y]) continue;
					image2[x, y] = MinDistance(x, y, image2, b);
				}
			}

			return image2;
		}

		public static GreyScaleImage2D ApproxEuclideanDistance(BinaryImage2D image)
		{
			var image2 = new GreyScaleImage2D(image.Size);
			image2.Fill((x, y) => image[x, y] ? float.PositiveInfinity : 0);

			var k4 = StructureElement2D.ChessBoardElement();
			var k8 = StructureElement2D.CityBlockElement();

			for (int y = 0; y < image2.Height; y++)
			{
				for (int x = 0; x < image2.Width; x++)
				{
					if (!image[x, y]) continue;

					float d4 = MinDistance(x, y, image2, k4);
					float d8 = MinDistance(x, y, image2, k8);

					float d1 = MathUtil.Sqr(d4 - d8);
					float d2 = MathUtil.Sqr(d8);

					image2[x, y] = MathUtil.SafeSqrt(d1 + d2);
				}
			}

			for (int y = image2.Height - 1; y >= 0; y--)
			{
				for (int x = image2.Width - 1; x >= 0; x--)
				{
					if (!image[x, y]) continue;

					float d4 = MinDistance(x, y, image2, k4);
					float d8 = MinDistance(x, y, image2, k8);

					float d1 = MathUtil.Sqr(d4 - d8);
					float d2 = MathUtil.Sqr(d8);

					image2[x, y] = MathUtil.SafeSqrt(d1 + d2);
				}
			}

			return image2;
		}

		private static float MinDistance(int i, int j, GreyScaleImage2D a, StructureElement2D b)
		{
			int half = b.Size / 2;

			float dist = float.PositiveInfinity;

			for (int y = 0; y < b.Size; y++)
			{
				for (int x = 0; x < b.Size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					float v = 0;

					if (a.InBounds(xi, yj))
						v = a[xi, yj];

					dist = MathUtil.Min(dist, v - b[x, y]);
				}
			}

			return dist;
		}

	}
}
