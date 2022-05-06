using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{
	public partial class GreyScaleImage2D
	{

		/// <summary>
		/// Open the image by performing a erode followed by a dilate.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="size"></param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static GreyScaleImage2D Open(GreyScaleImage2D a, int size, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
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
		public static GreyScaleImage2D Open(GreyScaleImage2D a, StructureElement2D b, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			a = Erode(a, b, wrap);
			return Dilate(a, b, wrap);
		}

		/// <summary>
		/// Close the image by performing a dilate followed by a erode.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="size"></param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static GreyScaleImage2D Close(GreyScaleImage2D a, int size, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
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
		public static GreyScaleImage2D Close(GreyScaleImage2D a, StructureElement2D b, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			a = Dilate(a, b, wrap);
			return Erode(a, b, wrap);
		}

		/// <summary>
		/// Dilate all values in a image by the structure element.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="size"></param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static GreyScaleImage2D Dilate(GreyScaleImage2D a, int size, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			var b = StructureElement2D.BoxElement(size);
			return Dilate(a, b, wrap);
		}

		/// <summary>
		/// Dilate all values in a image by the structure element.
		/// </summary>
		/// <param name="a">The image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static GreyScaleImage2D Dilate(GreyScaleImage2D a, StructureElement2D b, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			var image = a.Copy();

			a.Iterate((x, y) =>
			{
				image[x, y] = DilateFindMax(x, y, a, b, wrap);
			});

			return image;
		}

		/// <summary>
		/// Erode all values in a image by the structure element.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="size"></param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static GreyScaleImage2D Erode(GreyScaleImage2D a, int size, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			var b = StructureElement2D.BoxElement(size);
			return Erode(a, b, wrap);
		}

		/// <summary>
		/// Erode all values in a image by the structure element.
		/// </summary>
		/// <param name="a">The image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		public static GreyScaleImage2D Erode(GreyScaleImage2D a, StructureElement2D b, WRAP_MODE wrap = WRAP_MODE.CLAMP)
		{
			var image = a.Copy();

			a.Iterate((x, y) =>
			{
				image[x, y] = ErodeFindMin(x, y, a, b, wrap);
			});

			return image;
		}

		/// <summary>
		/// Find the max value around the pixel at i,j.
		/// </summary>
		/// <param name="i">x index in source image.</param>
		/// <param name="j">y index in source image.</param>
		/// <param name="a">The source image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		private static float DilateFindMax(int i, int j, GreyScaleImage2D a, StructureElement2D b, WRAP_MODE wrap)
		{
			int half = b.Size / 2;
			float max = float.NegativeInfinity;

			for (int y = 0; y < b.Size; y++)
			{
				for (int x = 0; x < b.Size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					if (xi < 0 || xi >= a.Width) continue;
					if (yj < 0 || yj >= a.Height) continue;

					if (b[x, y] == 1)
                    {
						var v = a.GetValue(xi, yj, wrap);

						if (v > max)
							max = v;
					}
				}
			}

			if (max == float.NegativeInfinity)
				max = 0;

			return max;
		}

		/// <summary>
		/// Find the min value around the pixel at i,j.
		/// </summary>
		/// <param name="i">x index in source image.</param>
		/// <param name="j">y index in source image.</param>
		/// <param name="a">The source image.</param>
		/// <param name="b">The structure element.</param>
		/// <param name="wrap">The wrap mode for out of bounds indices, Defaluts to clamp.</param>
		/// <returns></returns>
		private static float ErodeFindMin(int i, int j, GreyScaleImage2D a, StructureElement2D b, WRAP_MODE wrap)
		{
			int half = b.Size / 2;
			float min = float.PositiveInfinity;

			for (int y = 0; y < b.Size; y++)
			{
				for (int x = 0; x < b.Size; x++)
				{
					int xi = x + i - half;
					int yj = y + j - half;

					if (xi < 0 || xi >= a.Width) continue;
					if (yj < 0 || yj >= a.Height) continue;

					if (b[x, y] == 1)
					{
						var v = a.GetValue(xi, yj, wrap);

						if (v < min)
							min = v;
					}
				}
			}

			if (min == float.PositiveInfinity)
				min = 0;

			return min;
		}
	}
}
