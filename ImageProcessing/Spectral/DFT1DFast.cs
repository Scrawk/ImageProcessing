using System;
using System.Collections.Generic;

namespace ImageProcessing.Spectral
{
	internal class DFT1DFast
	{

		internal DFT1DFast()
		{

		}

		public void forward(float[] gRe, float[] gIm)
		{
			transform(gRe, gIm, true);
		}

		public void inverse(float[] GRe, float[] GIm)
		{
			transform(GRe, GIm, false);
		}

		public void transform(float[] inRe, float[] inIm, bool forward)
		{
			float scale = GetScale(inRe.Length, forward);
			//setupA(inRe, inIm, A);

			//if (forward)
			//	fft.complexForward(A);
			//else
			//	fft.complexInverse(A, false);

			//decompA(A, inRe, inIm, scale);
		}

		private void setupA(float[] re, float[] im, float[] A)
		{
			for (int i = 0; i < A.Length; i++)
			{
				A[2 * i] = re[i];
				A[2 * i + 1] = im[i];
			}
		}

		private void decompA(float[] A, float[] re, float[] im, float scale)
		{
			for (int i = 0; i < A.Length; i++)
			{
				re[i] = A[2 * i] * scale;
				im[i] = A[2 * i + 1] * scale;
			}
		}

		private float GetScale(int length, bool forward)
        {
			return 1;
        }

	}
}
