using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Extensions;

namespace ImageProcessing.Spectral
{
	/// <summary>
	/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/spectral/dct/Dct1dDirect.java
	/// </summary>
	public class DCT1DDirect
	{
		public DCT1DDirect(int size)
		{
			Size = size;
			Size4 = 4 * size;
			Scale = Math.Sqrt(2.0 / size);
			CosTable = MakeCosineTable();
		}

		private double CM0 = 1.0 / Math.Sqrt(2);

		public int Size { get; private set; }

		private int Size4 { get; set; }

		private double Scale { get; set; }

		private double[] CosTable { get; set; }

		private double[] MakeCosineTable()
		{
			// we need a table of size 4*M
			double[] table = new double[Size4];  
			
			for (int j = 0; j < Size4; j++)
			{       
				// j is equivalent to (m * (2 * u + 1)) % 4M
				//double phi = j * Math.PI / (2 * M);
				double phi = 2 * j * Math.PI / Size4;
				table[j] = Math.Cos(phi);
			}
			return table;
		}

		public void Forward(float[] g)
		{

			float[] G = new float[Size];

			for (int m = 0; m < Size; m++)
			{
				double cm = (m == 0) ? CM0 : 1.0;
				double sum = 0;

				for (int u = 0; u < Size; u++)
				{
					sum += g[u] * cm * CosTable[(m * (2 * u + 1)) % Size4];
				}

				G[m] = (float)(Scale * sum);
			}

			Array.Copy(G, g, Size);
		}

		public void Inverse(float[] G)
		{

			float[] g = new float[Size];

			for (int u = 0; u < Size; u++)
			{
				double sum = 0;
				for (int m = 0; m < Size; m++)
				{
					double cm = (m == 0) ? CM0 : 1.0;
					sum += cm * G[m] * CosTable[(m * (2 * u + 1)) % Size4];
				}

				g[u] = (float)(Scale * sum);
			}

			Array.Copy(g, G, Size);
		}

	}

}
