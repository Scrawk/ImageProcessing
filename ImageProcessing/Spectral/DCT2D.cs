using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Extensions;

namespace ImageProcessing.Spectral
{
	/// <summary>
	/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/spectral/dct/Dct2d.java
	/// </summary>
	public class DCT2D
	{


		public DCT2D()
		{
		}

		/// <summary>
		/// Performs an "in-place" 2D DCT forward transformation on the supplied data.
		/// The input signal is replaced by the associated DCT spectrum.
		/// </summary>
		/// <param name="g">the signal to be transformed (modified)</param>
		public void Forward(float[,] g)
		{
			Transform(g, true);
		}

		/// <summary>
		///Performs an "in-place" 2D DCT inverse transformation on the supplied spectrum.
		/// The input spectrum is replaced by the associated signal.
		/// </summary>
		/// <param name="G">the spectrum to be transformed (modified)</param>
		public void Inverse(float[,] G)
		{
			Transform(G, false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="forward"></param>
		private void Transform(float[,] data, bool forward)
		{
			int width = data.GetLength(0);
			int height = data.GetLength(1);

			// do the rows:
			float[] row = new float[width];
			var dct1R = new DCT1DDirect(width);

			for (int v = 0; v < height; v++)
			{
				extractRow(data, v, row);

				if (forward)
					dct1R.Forward(row);
				else
					dct1R.Inverse(row);

				insertRow(data, v, row);
			}

			// do the columns:
			float[] col = new float[height];
			var dct1C = new DCT1DDirect(height);

			for (int u = 0; u < width; u++)
			{
				extractCol(data, u, col);

				if (forward)
					dct1C.Forward(col);
				else
					dct1C.Inverse(col);

				insertCol(data, u, col);
			}
		}

		private void extractRow(float[,] data, int v, float[] row)
		{
			int width = data.GetLength(0);
			for (int u = 0; u < width; u++)
			{
				row[u] = data[u, v];
			}
		}

		private void insertRow(float[,] data, int v, float[] row)
		{
			int width = data.GetLength(0);
			for (int u = 0; u < width; u++)
			{
				data[u, v] = (float)row[u];
			}
		}

		private void extractCol(float[,] data, int u, float[] column)
		{
			int height = data.GetLength(1);
			for (int v = 0; v < height; v++)
			{
				column[v] = data[u, v];
			}
		}

		private void insertCol(float[,] data, int u, float[] column)
		{
			int height = data.GetLength(1);
			for (int v = 0; v < height; v++)
			{
				data[u, v] = (float)column[v];
			}
		}
	}

}