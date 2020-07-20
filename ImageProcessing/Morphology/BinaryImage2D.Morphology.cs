using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Common.Core.Numerics;
using Common.Core.Threading;
using Common.Geometry.Shapes;

using ImageProcessing.Morphology;
using System;

namespace ImageProcessing.Images
{
	/// <summary>
	/// 
	/// </summary>
	public partial class BinaryImage2D
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static BinaryImage2D Open(BinaryImage2D a, StructureElement2D b)
		{
			return Dilate(Erode(a, b), b);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static BinaryImage2D Close(BinaryImage2D a, StructureElement2D b)
		{
			return Erode(Dilate(a, b), b);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static BinaryImage2D Border(BinaryImage2D a)
        {
			var image = Erode(a, StructureElement2D.BoxElement(3));
			image.Xor(a);
			return image;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static BinaryImage2D Dilate(BinaryImage2D a, StructureElement2D b)
		{
			var image = a.Copy();

			int blockSize = Math.Max(64, Math.Max(a.Width, a.Height) / 16);
			ThreadingBlock2D.ParallelAction(a.Width, a.Height, blockSize, (x, y) =>
			{
				if (!a[x, y])
					image[x, y] = Dilate(x, y, a, b);
			});

			return image;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static bool Dilate(int i, int j, BinaryImage2D a, StructureElement2D b)
		{
			int half = b.Size / 2;

			for (int y = 0; y < b.Size; y++)
			{
				for (int x = 0; x < b.Size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					if (xi < 0 || xi >= a.Width) continue;
					if (yj < 0 || yj >= a.Height) continue;

					if (a[xi, yj] && b[x, y] == 1)
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static BinaryImage2D Erode(BinaryImage2D a, StructureElement2D b)
		{
			var image = a.Copy();

			int blockSize = Math.Max(64, Math.Max(a.Width, a.Height) / 16);
			ThreadingBlock2D.ParallelAction(a.Width, a.Height, blockSize, (x, y) =>
			{
				if (a[x, y])
					image[x, y] = Erode(x, y, a, b);
			});

			return image;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static bool Erode(int i, int j, BinaryImage2D a, StructureElement2D b)
		{
			int half = b.Size / 2;

			for (int y = 0; y < b.Size; y++)
			{
				for (int x = 0; x < b.Size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					if (xi < 0 || xi >= a.Width) continue;
					if (yj < 0 || yj >= a.Height) continue;

					if (!a[xi, yj] && b[x, y] == 1)
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="iterations"></param>
		/// <returns></returns>
		public static BinaryImage2D Thinning(BinaryImage2D a, int iterations = int.MaxValue)
		{
			var tuple = StructureElement2D.ThinningElements();
			var b = tuple.Item1;
			var c = tuple.Item2;

			var image = a.Copy();
			var points = image.ToPixelIndexList();

			for (int i = 0; i < iterations; i++)
			{
				bool done = true;
				points = Thinning(points, image, b, c, 0, ref done);
				points = Thinning(points, image, b, c, 1, ref done);
				points = Thinning(points, image, b, c, 2, ref done);
				points = Thinning(points, image, b, c, 3, ref done);

				if (done) return image;
			}

			return image;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="points"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="i"></param>
		/// <param name="done"></param>
		/// <returns></returns>
		private static List<PixelIndex2D<bool>> Thinning(List<PixelIndex2D<bool>> points, BinaryImage2D a, 
			StructureElement2D b, StructureElement2D c, int i, ref bool done)
		{

			int blockSize = Math.Max(256, points.Count / 16);
			ThreadingBlock1D.ParallelAction(points.Count, blockSize, (x) =>
			{
				var p = points[x];
				p.value = true ^ HitAndMiss(p.x, p.y, a, b, i);
				points[x] = p;
			});

			var points1 = new List<PixelIndex2D<bool>>(points.Count);
			for (int x = 0; x < points.Count; x++)
			{
				var p = points[x];
				if (p.value)
					points1.Add(p);
				else
				{
					a[p.x, p.y] = false;
					done = false;
				}
            }

			blockSize = Math.Max(256, points1.Count / 16);
			ThreadingBlock1D.ParallelAction(points1.Count, blockSize, (x) =>
			{
				var p = points1[x];
				p.value = true ^ HitAndMiss(p.x, p.y, a, c, i);
				points1[x] = p;
			});

			var points2 = new List<PixelIndex2D<bool>>(points1.Count);
			for (int x = 0; x < points1.Count; x++)
			{
				var p = points1[x];
				if (p.value)
					points2.Add(p);
				else
				{
					a[p.x, p.y] = false;
					done = false;
				}
			}

			return points2;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static BinaryImage2D HitAndMiss1(BinaryImage2D a, StructureElement2D b)
		{
			var image = a.Copy();

			int blockSize = Math.Max(64, Math.Max(a.Width, a.Height) / 16);
			ThreadingBlock2D.ParallelAction(a.Width, a.Height, blockSize, (x, y) =>
			{
				if (a[x, y])
					image[x, y] = HitAndMiss(x, y, a, b);
			});

			return image;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static BinaryImage2D HitAndMiss4(BinaryImage2D a, StructureElement2D b)
		{
			var image = a.Copy();

			int blockSize = Math.Max(64, Math.Max(a.Width, a.Height) / 16);
			ThreadingBlock2D.ParallelAction(a.Width, a.Height, blockSize, (x, y) =>
			{
				if (a[x, y])
					image[x, y] = HitAndMiss4(x, y, a, b);
			});

			return image;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		public static BinaryImage2D HitAndMiss4(BinaryImage2D a, StructureElement2D b, StructureElement2D c)
		{
			var image = a.Copy();

			for (int y = 0; y < a.Height; y++)
			{
				for (int x = 0; x < a.Width; x++)
				{
					if (a[x, y])
						image[x, y] = HitAndMiss4(x, y, a, b, c);
				}
			}

			return image;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static bool HitAndMiss4(int x, int y, BinaryImage2D a, StructureElement2D b)
		{

			return (HitAndMiss(x, y, a, b, 0) ||
			   HitAndMiss(x, y, a, b, 1) ||
			   HitAndMiss(x, y, a, b, 2) ||
			   HitAndMiss(x, y, a, b, 3));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		private static bool HitAndMiss4(int x, int y, BinaryImage2D a, StructureElement2D b, StructureElement2D c)
		{

			return (HitAndMiss(x, y, a, b, 0) ||
			   HitAndMiss(x, y, a, b, 1) ||
			   HitAndMiss(x, y, a, b, 2) ||
			   HitAndMiss(x, y, a, b, 3) ||
			   HitAndMiss(x, y, a, c, 0) ||
			   HitAndMiss(x, y, a, c, 1) ||
			   HitAndMiss(x, y, a, c, 2) ||
			   HitAndMiss(x, y, a, c, 3));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="rotate"></param>
		/// <returns></returns>
		private static bool HitAndMiss(int i, int j, BinaryImage2D a, StructureElement2D b)
		{
			int half = b.Size / 2;

			for (int y = 0; y < b.Size; y++)
			{
				for (int x = 0; x < b.Size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					if (xi < 0 || xi >= a.Width) continue;
					if (yj < 0 || yj >= a.Height) continue;

					int v = b[x, y];
					if (v == -1) continue;

					if (a[xi, yj] != (v == 1))
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="rotate"></param>
		/// <returns></returns>
		private static bool HitAndMiss(int i, int j, BinaryImage2D a, StructureElement2D b, int rotate)
		{
			int half = b.Size / 2;

			for (int y = 0; y < b.Size; y++)
			{
				for (int x = 0; x < b.Size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					if (xi < 0 || xi >= a.Width) continue;
					if (yj < 0 || yj >= a.Height) continue;

					int v = b.GetRotated(x, y, rotate);
					if (v == -1) continue;

					if (a[xi, yj] != (v == 1))
						return false;
				}
			}

			return true;
		}


	}

}
