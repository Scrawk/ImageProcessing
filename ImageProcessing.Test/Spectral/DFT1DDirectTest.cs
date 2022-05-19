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
    public class DFT1DDirectTest
    {
		/// <summary>
		/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/spectral/dft/Dft1dDirect.java
		/// </summary>
		[TestMethod]
        public void Transform1D()
        {

			float[] re = { 1, 2, 3, 4, 5, 6, 7, 8 };
			float[] im = new float[re.Length];

			var dft = new DFT1DDirect(re.Length, SCALING_MODE.DEFAULT);

			var expected = new float[]
			{
				12.7279f, 0.0f,
				-1.41421f, 3.41421f,
				-1.41421f, 1.41421f,
				-1.41421f, 0.585786f,
				-1.41421f, 0.0f,
				-1.41421f, -0.585786f,
				-1.41421f, -1.41421f,
				-1.41421f, -3.41421f
			};

			dft.Forward(re, im);

			for (int i = 0; i < dft.Size; i++)
            {
				//Console.WriteLine(expected[i * 2 + 0] + "  " + re[i]);
				//Console.WriteLine(expected[i * 2 + 1] + "  " + im[i]);

				Assert.IsTrue(MathUtil.AlmostEqual(expected[i * 2 + 0], re[i], 1e-2f));
				Assert.IsTrue(MathUtil.AlmostEqual(expected[i * 2 + 1], im[i], 1e-2f));
			}

			dft.Inverse(re, im);

			for (int i = 0; i < dft.Size; i++)
			{
				//Console.WriteLine(expected[i * 2 + 0] + "  " + re[i]);
				//Console.WriteLine(expected[i * 2 + 1] + "  " + im[i]);

				Assert.IsTrue(MathUtil.AlmostEqual(i + 1, re[i], 1e-2f));
				Assert.IsTrue(MathUtil.AlmostEqual(0, im[i], 1e-2f));
			}

		}

		[TestMethod]
		public void Transform2D()
        {
			float[,] re = 
			{
				{ 1, 2, 3, 4 },
				{ 1, 2, 3, 4 },
				{ 1, 2, 3, 4 },
				{ 1, 2, 3, 4 }
			};

			float[,] im = new float[4,4];

			var image = new GreyScaleImage2D(re);

			re = image.ToFloatArray(0);
			
			var dft = new DFT2D(false, SCALING_MODE.DEFAULT);

			dft.Forward(re, im);
			dft.Inverse(re, im);

			image.FillChannel(re, 0);

			foreach(var v in re)
            {
				Console.WriteLine(v);
            }
		}

	}
}
