using System;
using System.Collections.Generic;

namespace ImageProcessing.Spectral
{
	public enum SCALING_MODE
    {
		DEFAULT,
		FORWARD,
		INVERSE
    }

	/// <summary>
	/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/spectral/dft/Dft1dDirect.java
	/// </summary>
	public class DFT1DDirect
    {

		public DFT1DDirect(int size)
		{
			Size = size;
			Mode = SCALING_MODE.DEFAULT;
			CosTable = MakeCosTable();
			SinTable = MakeSinTable();
			OutRe = new float[Size];
			OutIm = new float[Size];
		}

		public int Size { get; private set; }

		private double[] CosTable { get; set; }

		private double[] SinTable { get; set; }

		private float[] OutRe { get; set; }

		private float[] OutIm { get; set; }

		private SCALING_MODE Mode { get; set; }

		private double[] MakeCosTable()
		{
			double[] cosTable = new double[Size];
			double theta = 2 * Math.PI / Size;

			for (int i = 0; i < Size; i++)
			{
				cosTable[i] = Math.Cos(theta * i);
			}

			return cosTable;
		}

		private double[] MakeSinTable()
		{
			double[] sinTable = new double[Size];
			double theta = 2 * Math.PI / Size;

			for (int i = 0; i < Size; i++)
			{
				sinTable[i] = Math.Sin(theta * i);
			}

			return sinTable;
		}

		public void Print()
        {
			for(int i = 0; i < Size;i++)
				Console.WriteLine(OutRe[i] + ", " + OutIm[i] + "i");
        }

		public void Forward(float[] gRe, float[] gIm)
		{
			Transform(gRe, gIm, true);
		}

		public void Inverse(float[] GRe, float[] GIm)
		{
			Transform(GRe, GIm, false);
		}

		public void Transform(float[] inRe, float[] inIm, bool forward)
		{
			double scale = GetScale();

			for (int u = 0; u < Size; u++)
			{
				double sumRe = 0;
				double sumIm = 0;

				for (int m = 0; m < Size; m++)
				{
					double re = inRe[m];
					double im = (inIm == null) ? 0 : inIm[m];

					int k = (u * m) % Size;
					double cosPhi = CosTable[k];
					double sinPhi = (forward) ? -SinTable[k] : SinTable[k];

					sumRe += re * cosPhi - im * sinPhi;
					sumIm += re * sinPhi + im * cosPhi;
				}

				OutRe[u] = (float)(scale * sumRe);
				OutIm[u] = (float)(scale * sumIm);
			}

			Array.Copy(OutRe, inRe, Size);
			Array.Copy(OutIm, inIm, Size);
		}

		private double GetScale()
        {
            switch (Mode)
            {
                case SCALING_MODE.DEFAULT:
					return 1.0 / Math.Sqrt(Size);
				case SCALING_MODE.FORWARD:
					return 1.0 / (double)Size;
				case SCALING_MODE.INVERSE:
					return 1;
            }

			return 1;
        }
    }
}
