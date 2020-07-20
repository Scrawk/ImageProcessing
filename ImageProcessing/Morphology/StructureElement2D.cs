using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Geometry.Shapes;
using ImageProcessing.Images;

namespace ImageProcessing.Morphology
{
	/// <summary>
	/// 
	/// </summary>
	public class StructureElement2D : Image2D<int>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		public StructureElement2D(int size)
			: base(size, size)
		{
			Size = size;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <param name="value"></param>
		public StructureElement2D(int size, int value)
			: base(size, size)
		{
			Size = size;
			Data.Fill(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <param name="data"></param>
		private StructureElement2D(int size, int[,] data) 
			: base(data)
		{
			Size = size;
		}

		/// <summary>
		/// 
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="shape"></param>
		/// <param name="center"></param>
		public void Fill(IShape2f shape, bool center = true)
		{
			for (int y = 0; y < Size; y++)
			{
				for (int x = 0; x < Size; x++)
				{
					var p = new Vector2f(x, y);
					if (center) p += 0.5f;

					if (shape.Contains(p))
						this[x, y] = 1;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public StructureElement2D Copy()
		{
			return new StructureElement2D(Size, Data);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public float[,] ToFloatArray()
		{
			var array = new float[Size, Size];

			for (int y = 0; y < Size; y++)
			{
				for (int x = 0; x < Size; x++)
				{
					array[x, y] = this[x, y];
				}
			}

			return array;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="rotate"></param>
		/// <returns></returns>
		public int GetRotated(int x, int y, int rotate)
		{
			switch (rotate)
			{
				case 0:
					return GetRotated0(x, y);

				case 1:
					return GetRotated90(x, y);

				case 2:
					return GetRotated180(x, y);

				case 3:
					return GetRotated270(x, y);
			}

			throw new Exception("Undefined rotation");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int GetRotated0(int x, int y)
		{
			return this[x, y];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int GetRotated90(int x, int y)
		{
			return this[y, Size - 1 - x];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int GetRotated180(int x, int y)
		{
			return this[Size - 1 - x, Size - 1 - y];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int GetRotated270(int x, int y)
		{
			return this[Size - 1 - y, x];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static StructureElement2D BoxElement(int size)
		{
			return new StructureElement2D(size, 1);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static StructureElement2D CircleElement(int size)
		{
			float half = size / 2.0f;
			var circle = new Circle2f((half, half), half);
			var e = new StructureElement2D(size);
			e.Fill(circle, true);
			return e;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static StructureElement2D HitMissCornerElement()
		{
			var e = new StructureElement2D(3, -1);
			e[0, 0] = 0; e[1, 0] = 0; e[0, 1] = 0;
			e[1, 1] = 1; e[2, 1] = 1; e[1, 2] = 1;
			return e;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static (StructureElement2D, StructureElement2D) ThinningElements()
		{
			var b = new StructureElement2D(3, -1);
			b[0, 2] = 0; b[1, 2] = 0; b[2, 2] = 0;
			b[0, 0] = 1; b[1, 0] = 1; b[2, 0] = 1; b[1, 1] = 1;

			var c = new StructureElement2D(3, -1);
			c[2, 2] = 0; c[1, 2] = 0; c[2, 1] = 0;
			c[1, 1] = 1; c[1, 0] = 1; c[0, 1] = 1;

			return (b, c);
		}

	}

}
