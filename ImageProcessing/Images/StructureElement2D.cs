using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{
	/// <summary>
	/// 
	/// </summary>
	public class StructureElement2D
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		public StructureElement2D(int size)
		{
			Size = size;
			Data = new int[size, size];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <param name="value"></param>
		public StructureElement2D(int size, int value)
		{
			Size = size;
			Data = new int[size, size];
			Data.Fill(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		private StructureElement2D(int[,] data) 
		{
			if(data.GetLength(0) != data.GetLength(1))
				throw new Exception("Data array must be square.");

			Size = data.GetLength(0);
			Data = data.Copy();
		}

		public int[,] Data { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int this[int x, int y]
        {
			get => Data[x, y];
			set => Data[x, y] = value;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[StructureElement2D: Size={0}]", Size);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public StructureElement2D Copy()
		{
			return new StructureElement2D(Data);
		}

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
			var data = new int[,]
			{
				{ -1, 1, -1},
				{ 0,  1,  1},
				{ 0,  0, -1}
			};

			return new StructureElement2D(data);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static (StructureElement2D, StructureElement2D) ThinningElements()
		{
			var data1 = new int[,]
			{
				{  0, 0,  0},
				{ -1, 1, -1},
				{  1, 1,  1}
			};

			var data2 = new int[,]
			{
				{ -1, 0,  0},
				{  1, 1,  0},
				{ -1, 1, -1}
			};

			return (new StructureElement2D(data1), new StructureElement2D(data2));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static StructureElement2D CityBlockElement()
		{
			var data = new int[,]
			{
				{ -1, -1, -1},
				{ -1,  0, -1},
				{ -1, -1, -1}
			};

			return new StructureElement2D(data);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static StructureElement2D ChessBoardElement()
		{
			var data = new int[,]
			{
				{ -2, -1, -2},
				{ -1,  0, -1},
				{ -2, -1, -2}
			};

			return new StructureElement2D(data);
		}

	}

}
