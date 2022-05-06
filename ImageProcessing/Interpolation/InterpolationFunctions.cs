using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Threading;

namespace ImageProcessing.Interpolation
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class InterpolationFunction
	{
		/// <summary>
		/// 
		/// </summary>
		public abstract int Size { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public abstract float GetWeight(float x);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static float Pow2(float x)
		{
			return x * x;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static float Pow3(float x)
		{
			return x * x * x;
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public class LinearInterpolation : InterpolationFunction
	{
		/// <summary>
		/// 
		/// </summary>
		public static InterpolationFunction Default { get; private set; } = new LinearInterpolation();

		/// <summary>
		/// 
		/// </summary>
		public override int Size => 1;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public override float GetWeight(float x)
		{
			if (Math.Abs(x) < 1)
				return 1 - x;
			else
				return 0;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class CubicInterpolation : InterpolationFunction
	{
		/// <summary>
		/// 
		/// </summary>
		public static InterpolationFunction Default { get; private set; } = new CubicInterpolation(1);

		/// <summary>
		/// 
		/// </summary>
		public static InterpolationFunction Smooth { get; private set; } = new CubicInterpolation(0.25f);

		/// <summary>
		/// 
		/// </summary>
		public static InterpolationFunction Sharp { get; private set; } = new CubicInterpolation(1.25f);

		/// <summary>
		/// 
		/// </summary>
		public override int Size => 2;

		/// <summary>
		/// 
		/// </summary>
		private float a;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		public CubicInterpolation(float a = 1)
		{
			this.a = a;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public override float GetWeight(float x)
		{
			return GetWeight(x, a);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		private static float GetWeight(float x, float a)
		{
			x = Math.Abs(x);

			if (x < 1)
				return (-a + 2) * Pow3(x) + (a - 3) * Pow2(x) + 1;
			else if (x < 2)
				return -a * Pow3(x) + 5 * a * Pow2(x) - 8 * a * x + 4 * a;
			else
				return 0;
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public class SplineInterpolation : InterpolationFunction
	{
		/// <summary>
		/// 
		/// </summary>
		public static InterpolationFunction CatmullRom { get; private set; } = new SplineInterpolation(0.5f, 0);

		/// <summary>
		/// 
		/// </summary>
		public static InterpolationFunction CubicBSpline { get; private set; } = new SplineInterpolation(0, 1);

		/// <summary>
		/// 
		/// </summary>
		public static InterpolationFunction MitchellNetravli { get; private set; } = new SplineInterpolation(1 / 3.0f, 1 / 3.0f);

		/// <summary>
		/// 
		/// </summary>
		public override int Size => 2;

		/// <summary>
		/// 
		/// </summary>
		private float a, b;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public SplineInterpolation(float a = 0, float b = 1)
		{
			this.a = a;
			this.b = b;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public override float GetWeight(float x)
		{
			return GetWeight(x, a, b);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static float GetWeight(float x, float a, float b)
		{
			x = Math.Abs(x);
			float v = 0;

			if (x < 1)
				v = (-6 * a - 9 * b + 12) * Pow3(x) + (6 * a + 12 * b - 18) * Pow2(x) - 2 * b + 6;
			else if (x < 2)
				v = (-6 * a - b) * Pow3(x) + (30 * a + 6 * b) * Pow2(x) + (-48 * a - 12 * b) * x + 24 * a + 8 * b;

			return v * (1 / 6.0f);
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public class LanzcosInterpolation : InterpolationFunction
	{
		/// <summary>
		/// 
		/// </summary>
		public static InterpolationFunction Default2 { get; private set; } = new LanzcosInterpolation(2);

		/// <summary>
		/// 
		/// </summary>
		public static InterpolationFunction Default3 { get; private set; } = new LanzcosInterpolation(3);

		/// <summary>
		/// 
		/// </summary>
		public static InterpolationFunction Default4 { get; private set; } = new LanzcosInterpolation(4);

		/// <summary>
		/// 
		/// </summary>
		public override int Size => n;

		/// <summary>
		/// 
		/// </summary>
		private int n;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="n"></param>
		public LanzcosInterpolation(int n)
		{
			this.n = n;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public override float GetWeight(float x)
		{
			return GetWeight(x, n);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		private static float GetWeight(float x, float n)
		{
			float ax = Math.Abs(x);

			if (ax == 0)
				return 0;
			else if (0 < ax && ax < n)
			{
				float factor = MathUtil.PI_32 * x / n;
				return MathUtil.Sin(factor) / factor;
			}
			else
				return 0;
		}

	}
}
