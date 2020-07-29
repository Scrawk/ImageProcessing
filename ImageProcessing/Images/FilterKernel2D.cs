﻿using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Geometry.Shapes;

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
		/// <param name="size"></param>
		/// <param name="data"></param>
		private FilterKernel2D(float[,] data, float scale) 
		{
			if(data.GetLength(0) != data.GetLength(1))
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
		public FilterKernel2D Copy()
		{
			return new FilterKernel2D(Data, Scale);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static FilterKernel2D BoxKernel(int size)
		{
			size = Math.Max(3, size);
			return new FilterKernel2D(size, 1.0f / (size*size), 1.0f);
		}

		public static FilterKernel2D GaussianKernel(int size)
		{
			size = Math.Max(3, size);
			float sigma = (size-1) * 0.5f;
			int center = (int)(3 * sigma);
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

			kernel.Data.Fill((x, y) =>
			{
				return kernel[x, y] /= sum;
			});


			return kernel;
		}

	}

}






