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
		public abstract int Size { get; }

		public abstract float GetWeight(float x);

		protected static float Pow2(float x)
		{
			return x * x;
		}

		protected static float Pow3(float x)
		{
			return x * x * x;
		}

	}

	public class LinearInterpolation : InterpolationFunction
	{
		public static InterpolationFunction Default { get; private set; } = new LinearInterpolation();

		public override int Size => 1;

		public override float GetWeight(float x)
		{
			if (Math.Abs(x) < 1)
				return 1 - x;
			else
				return 0;
		}
	}

	public class CubicInterpolation : InterpolationFunction
	{
		public static InterpolationFunction Default { get; private set; } = new CubicInterpolation(1);

		public static InterpolationFunction Smooth { get; private set; } = new CubicInterpolation(0.25f);

		public static InterpolationFunction Sharp { get; private set; } = new CubicInterpolation(1.25f);

		public override int Size => 2;

		private float a;

		public CubicInterpolation(float a = 1)
		{
			this.a = a;
		}

		public override float GetWeight(float x)
		{
			return GetWeight(x, a);
		}

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

	public class SplineInterpolation : InterpolationFunction
	{
		public static InterpolationFunction CatmullRom { get; private set; } = new SplineInterpolation(0.5f, 0);

		public static InterpolationFunction CubicBSpline { get; private set; } = new SplineInterpolation(0, 1);

		public static InterpolationFunction MitchellNetravli { get; private set; } = new SplineInterpolation(1 / 3.0f, 1 / 3.0f);

		public override int Size => 2;

		private float a, b;

		public SplineInterpolation(float a = 0, float b = 1)
		{
			this.a = a;
			this.b = b;
		}

		public override float GetWeight(float x)
		{
			return GetWeight(x, a, b);
		}

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

	public class LanzcosInterpolation : InterpolationFunction
	{
		public static InterpolationFunction Default2 { get; private set; } = new LanzcosInterpolation(2);

		public static InterpolationFunction Default3 { get; private set; } = new LanzcosInterpolation(3);

		public static InterpolationFunction Default4 { get; private set; } = new LanzcosInterpolation(4);

		public override int Size => n;

		private int n;

		public LanzcosInterpolation(int n)
		{
			this.n = n;
		}

		public override float GetWeight(float x)
		{
			return GetWeight(x, n);
		}

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
