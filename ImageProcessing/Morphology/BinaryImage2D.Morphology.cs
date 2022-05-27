using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Threading;

using ImageProcessing.Pixels;

namespace ImageProcessing.Images
{
	/// <summary>
	/// 
	/// </summary>
	public partial class BinaryImage2D
	{

		/// <summary>
		/// Open the image by performing a erode followed by a dilate.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="size">The size of structure element. Size shoould be at least 3 and a odd number.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D Open(BinaryImage2D a, int size, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			//Size shoould be at least 3 and a odd number.
			size = Math.Min(size, 3);
			if (size % 2 != 1)
				size++;

			a = Erode(a, size, wrap);
			return Dilate(a, size, wrap);
		}

		/// <summary>
		/// Open the image by performing a erode followed by a dilate.
		/// </summary>
		/// <param name="a">The image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D Open(BinaryImage2D a, StructureElement2D b, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			a = Erode(a, b, wrap);
			return Dilate(a, b, wrap);
		}

		/// <summary>
		/// Close the image by performing a dilate followed by a erode.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="size">The size of structure element. Size shoould be at least 3 and a odd number.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D Close(BinaryImage2D a, int size, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			//Size shoould be at least 3 and a odd number.
			size = Math.Min(size, 3);
			if (size % 2 != 1)
				size++;

			a = Dilate(a, size, wrap);
			return Erode(a, size, wrap);
		}

		/// <summary>
		/// Close the image by performing a dilate followed by a erode.
		/// </summary>
		/// <param name="a">The image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D Close(BinaryImage2D a, StructureElement2D b, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			a = Dilate(a, b, wrap);
			return Erode(a, b, wrap);
		}

		/// <summary>
		/// Find the border of all connected blocks of true values in the image.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D Border(BinaryImage2D a, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			var image = Erode(a, StructureElement2D.BoxElement(3), wrap);
			image.Xor(a);
			return image;
		}

		/// <summary>
		/// Diluate all true values in a image by the structure element.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="size"></param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D Dilate(BinaryImage2D a, int size, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			var b = StructureElement2D.BoxElement(size);
			return Dilate(a, b, wrap);
		}

		/// <summary>
		/// Diluate all true values in a image by the structure element.
		/// </summary>
		/// <param name="a">The image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D Dilate(BinaryImage2D a, StructureElement2D b, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			var image = a.Copy();

			a.Iterate((x, y) =>
			{
				if (!a[x, y])
					image[x, y] = Dilate(x, y, a, b, wrap);
			});

			return image;
		}

		/// <summary>
		/// Find if a pixel in the image should be dilated.
		/// A pixel should be dilated if any surrounding pixel
		/// and the element both have a true value.
		/// The element is centered on the pixel.
		/// </summary>
		/// <param name="i">x index in source image.</param>
		/// <param name="j">y index in source image.</param>
		/// <param name="a">The source image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		private static bool Dilate(int i, int j, BinaryImage2D a, StructureElement2D b, WRAP_MODE wrap)
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

					if (a.GetValue(xi, yj, wrap) && b[x, y] == 1)
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Erode all true values in a image by the structure element.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="size"></param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D Erode(BinaryImage2D a, int size, WRAP_MODE wrap = WRAP_MODE.CLAMP)
        {
			var b = StructureElement2D.BoxElement(size);
			return Erode(a, b, wrap);
		}

		/// <summary>
		/// Erode all true values in a image by the structure element.
		/// </summary>
		/// <param name="a">The image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D Erode(BinaryImage2D a, StructureElement2D b, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			var image = a.Copy();

			a.Iterate((x, y) =>
			{
				if (a[x, y])
					image[x, y] = Erode(x, y, a, b, wrap);
			});

			return image;
		}

		/// <summary>
		/// Find if a pixel in the image should be eroded.
		/// A pixel should be eroded if any surrounding pixel
		/// has a false value and the element a true value.
		/// The element is centered on the pixel.
		/// </summary>
		/// <param name="i">x index in source image.</param>
		/// <param name="j">y index in source image.</param>
		/// <param name="a">The source image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		private static bool Erode(int i, int j, BinaryImage2D a, StructureElement2D b, WRAP_MODE wrap)
		{
			int half = b.Size / 2;

			for (int y = 0; y < b.Size; y++)
			{
				for (int x = 0; x < b.Size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					//if (xi < 0 || xi >= a.Width) continue;
					//if (yj < 0 || yj >= a.Height) continue;

					if (!a.GetValue(xi, yj, wrap) && b[x, y] == 1)
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Iteratively thin a image by removing pixels on the border
		/// if they match the structure element.
		/// Will exit early if image can not be thinned anymore.
		/// </summary>
		/// <param name="a">The image.</param>
		/// <param name="iterations">The number of times to thin the image.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D Thinning(BinaryImage2D a, WRAP_MODE wrap = WRAP_MODE.CLAMP, int iterations = int.MaxValue)
		{
			var tuple = StructureElement2D.ThinningElements();
			var b = tuple.Item1;
			var c = tuple.Item2;

			var image = a.Copy();

			//Get all the points in image with a true value.
			//Only these pixels need to be iterated.
			var points = new List<PixelIndex2D<bool>>();
			image.ToPixelIndexList(points, (v) => v == true);

			for (int i = 0; i < iterations; i++)
			{
				//Thin the image for each rotation of the element.
				bool done = true;
				points = Thinning(points, image, b, c, 0, wrap, ref done);
				points = Thinning(points, image, b, c, 1, wrap, ref done);
				points = Thinning(points, image, b, c, 2, wrap, ref done);
				points = Thinning(points, image, b, c, 3, wrap, ref done);

				//If the image did not change then it can not be 
				//thinned anymore so return.
				if (done) return image;
			}

			return image;
		}

		/// <summary>
		/// Thin the image with the two structure elements and remove 
		/// any thinned points from the image.
		/// </summary>
		/// <param name="points">The current pixels in the image with a true value.</param>
		/// <param name="a">The image to be thinned.</param>
		/// <param name="b">The structure element b.</param>
		/// <param name="c">The structure element c.</param>
		/// <param name="i">The rotation index for the elements.</param>
		/// <param name="done">If the image has not changed this iteration.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		private static List<PixelIndex2D<bool>> Thinning(List<PixelIndex2D<bool>> points, BinaryImage2D a,
			StructureElement2D b, StructureElement2D c, int i, WRAP_MODE wrap, ref bool done)
		{

			//For each point in the list determine its value depending
			//on if it matches the element b or not.
			for(int x = 0; x < points.Count; x++)
			{
				var p = points[x];
				p.Value = true ^ HitAndMiss(p.x, p.y, a, b, i, wrap);
				points[x] = p;
			}

			//Update the points by removing those with a false value
			//and keeping those with a true value.
			var points1 = new List<PixelIndex2D<bool>>(points.Count);
			for (int x = 0; x < points.Count; x++)
			{
				var p = points[x];
				if (p.Value)
					points1.Add(p);
				else
				{
					//If value is false remove from image
					//and mark that the algorithm is not done.
					a[p.x, p.y] = false;
					done = false;
				}
			}

			//For each point in the list determine its value depending
			//on if it matches the element c or not.
			for (int x = 0; x < points1.Count; x++)
			{
				var p = points1[x];
				p.Value = true ^ HitAndMiss(p.x, p.y, a, c, i, wrap);
				points1[x] = p;
			}

			var points2 = new List<PixelIndex2D<bool>>(points1.Count);
			for (int x = 0; x < points1.Count; x++)
			{
				var p = points1[x];
				if (p.Value)
					points2.Add(p);
				else
				{
					//If value is false remove from image
					//and mark that the algorithm is not done.
					a[p.x, p.y] = false;
					done = false;
				}
			}

			//return only the points remaining in the image.
			return points2;
		}

		/// <summary>
		/// Remove all pixels in the image that matchs the structure element.
		/// </summary>
		/// <param name="a">The image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D HitAndMiss(BinaryImage2D a, StructureElement2D b, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			var image = a.Copy();

			a.Iterate((x, y) =>
			{
				if (a[x, y])
					image[x, y] = HitAndMiss(x, y, a, b, 0, wrap);
			});

			return image;
		}

		/// <summary>
		/// Remove all pixels in the image that match the structure element
		/// in any of its 4 rotations.
		/// </summary>
		/// <param name="a">The image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D HitAndMiss4(BinaryImage2D a, StructureElement2D b, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			var image = a.Copy();

			a.Iterate((x, y) =>
			{
				if (a[x, y])
					image[x, y] = HitAndMiss4(x, y, a, b, wrap);
			});

			return image;
		}

		/// <summary>
		/// Remove all pixels in the image that match either of the two 
		/// structure elements in any of there 4 rotations.
		/// </summary>
		/// <param name="a">The image.</param>
		/// <param name="b">The structure element b.</param>
		/// <param name="c">The structure element c.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static BinaryImage2D HitAndMiss4(BinaryImage2D a, StructureElement2D b, StructureElement2D c, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			var image = a.Copy();

			a.Iterate((x, y) =>
			{
				if (a[x, y])
					image[x, y] = HitAndMiss4(x, y, a, b, c, wrap);
			});

			return image;
		}

		/// <summary>
		/// Find if the pixel in the image matches the structure element
		/// in any of its 4 rotations.
		/// </summary>
		/// <param name="x">The x index in the source image.</param>
		/// <param name="y">The y index in the source image.</param>
		/// <param name="a">The source image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		private static bool HitAndMiss4(int x, int y, BinaryImage2D a, StructureElement2D b, WRAP_MODE wrap)
		{

			return (HitAndMiss(x, y, a, b, 0, wrap) ||
			   HitAndMiss(x, y, a, b, 1, wrap) ||
			   HitAndMiss(x, y, a, b, 2, wrap) ||
			   HitAndMiss(x, y, a, b, 3, wrap));
		}

		/// <summary>
		/// Find if the pixel in the image matches either of the 
		/// structure elements in any of there 4 rotations.
		/// </summary>
		/// <param name="x">The x index in the source image.</param>
		/// <param name="y">The y index in the source image.</param>
		/// <param name="a">The source image.</param>
		/// <param name="b">The structure element b.</param>
		/// <param name="c">The structure element c.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		private static bool HitAndMiss4(int x, int y, BinaryImage2D a, StructureElement2D b, StructureElement2D c, WRAP_MODE wrap)
		{

			return (HitAndMiss(x, y, a, b, 0, wrap) ||
			   HitAndMiss(x, y, a, b, 1, wrap) ||
			   HitAndMiss(x, y, a, b, 2, wrap) ||
			   HitAndMiss(x, y, a, b, 3, wrap) ||
			   HitAndMiss(x, y, a, c, 0, wrap) ||
			   HitAndMiss(x, y, a, c, 1, wrap) ||
			   HitAndMiss(x, y, a, c, 2, wrap) ||
			   HitAndMiss(x, y, a, c, 3, wrap));
		}

		/// <summary>
		/// Find if all surrounding pixels in a image match the 
		/// structure element.
		/// </summary>
		/// <param name="i">The x index in the source image.</param>
		/// <param name="j">The y index in the source image.</param>
		/// <param name="a">The source image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="rotate">The element rotation.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		private static bool HitAndMiss(int i, int j, BinaryImage2D a, StructureElement2D b, int rotate, WRAP_MODE wrap)
		{
			int half = b.Size / 2;

			for (int y = 0; y < b.Size; y++)
			{
				for (int x = 0; x < b.Size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					//if (xi < 0 || xi >= a.Width) continue;
					//if (yj < 0 || yj >= a.Height) continue;

					int v = b.GetRotated(x, y, rotate);
					if (v == -1) continue;

					if (a.GetValue(xi, yj, wrap) != (v == 1))
						return false;
				}
			}

			return true;
		}


	}

}
