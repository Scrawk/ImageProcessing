using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{
	/// <summary>
	/// 
	/// </summary>
	public class FilterKernel2D
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="size"></param>
		public FilterKernel2D(int size, float scale)
		{
			Size = size;
			Scale = scale;
			Data = new float[size, size];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="size"></param>
		/// <param name="value"></param>
		public FilterKernel2D(int size, float scale, float value)
		{
			Size = size;
			Scale = scale;
			Data = new float[size, size];
			Data.Fill(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="data"></param>
		private FilterKernel2D(float[,] data, float scale)
		{
			if (data.GetLength(0) != data.GetLength(1))
				throw new Exception("Data array must be square.");

			Size = data.GetLength(0);
			Scale = scale;
			Data = data.Copy();
		}

		/// <summary>
		/// 
		/// </summary>
		public float[,] Data { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public float Scale { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public float this[int x, int y]
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
			return string.Format("[FilterKernel2D: Size={0}, Scale={1}]", Size, Scale);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public FilterKernel2D Copy()
		{
			return new FilterKernel2D(Data, Scale);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public float Sum()
		{
			float sum = 0;
			for (int y = 0; y < Size; y++)
			{
				for (int x = 0; x < Size; x++)
				{
					sum += this[x, y];
				}
			}

			return sum;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static FilterKernel2D BoxKernel(int size)
		{
			size = Math.Max(3, size);
			return new FilterKernel2D(size, 1.0f / (size * size), 1.0f);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sigma"></param>
		/// <returns></returns>
		public static FilterKernel2D GaussianKernel(float sigma)
		{
			//sigma 0.5  == size 3
			//sigma 0.75 == size 5
			//sigma 1.0  == size 7

			int center = (int)(sigma * 3);
			int size = 2 * center + 1;
			float sigma2 = sigma * sigma;
			float sum = 0;

			var kernel = new FilterKernel2D(size, 1.0f);

			kernel.Data.Fill((x, y) =>
			{
				float rx = center - x;
				float ry = center - y;
				float g = MathUtil.Exp(-0.5f * (rx * rx + ry * ry) / sigma2);

				sum += g;
				return g;
			});

			kernel.Data.Fill((x, y) => kernel[x, y] / sum);

			return kernel;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static FilterKernel2D SharpenKernel()
		{
			float[,] data = new float[,]
			{
				{0,-1,0},
				{-1,5,-1},
				{0,-1,0}
			};

			return new FilterKernel2D(data, 1.0f);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static FilterKernel2D UnsharpenKernel()
		{
			float[,] data = new float[,]
			{
				{1,4,6,4,1},
				{4,16,24,16,4},
				{6,24,-476,24,6},
				{4,16,24,16,4},
				{1,4,6,4,1}
			};

			return new FilterKernel2D(data, -1.0f / 256.0f);
		}

	}

}






