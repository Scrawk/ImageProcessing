using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;
using Common.Core.Extensions;

using ImageProcessing.Images;
using ImageProcessing.Spectral;

namespace ImageProcessing.Test.Spectral
{
	[TestClass]
	public class DCTTest
	{
		/// <summary>
		/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/spectral/dct/Dct1dDirect.java
		/// </summary>
		[TestMethod]
		public void Transform1D()
		{

			float[] data = { 1, 2, 3, 4, 5, 3, 0 };

			var dct = new DCT1DDirect(data.Length);

			var expected = new float[]
			{
				6.803f, -0.361f, -3.728f, 1.692f, -0.888f, -0.083f, 0.167f
			};

			dct.Forward(data);

			for (int i = 0; i < dct.Size; i++)
			{
				//Console.WriteLine(expected[i] + "  " + data[i]);
				Assert.IsTrue(MathUtil.AlmostEqual(expected[i], data[i], 1e-2f));
			}

			expected = new float[]
			{
				1, 2, 3, 4, 5, 3, 0
			};

			dct.Inverse(data);

			for (int i = 0; i < dct.Size; i++)
			{
				Assert.IsTrue(MathUtil.AlmostEqual(expected[i], data[i], 1e-2f));
			}

		}

		[TestMethod]
		public void Transform2D()
		{
			float[,] data =
			{
				{ 1, 2, 3, 4 },
				{ 1, 2, 3, 4 },
				{ 1, 2, 3, 4 },
				{ 1, 2, 3, 4 }
			};

			var image = new GreyScaleImage2D(data);

			data = image.ToFloatArray(0);

			var dft = new DCT2D();

			dft.Forward(data);
			dft.Inverse(data);

			image.FillChannel(data, 0);

			float[,] expected =
			{
				{ 1, 2, 3, 4 },
				{ 1, 2, 3, 4 },
				{ 1, 2, 3, 4 },
				{ 1, 2, 3, 4 }
			};

			for (int i = 0; i < data.GetLength(0); i++)
			{
				for (int j = 0; j < data.GetLength(1); j++)
				{
					Assert.IsTrue(MathUtil.AlmostEqual(data[i,j], expected[i,j], 1e-2f));
				}
			}
		}

	}
}
