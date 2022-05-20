using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Extensions;

namespace ImageProcessing.Spectral
{
	/// <summary>
	/// https://github.com/imagingbook/imagingbook-common/blob/master/src/main/java/imagingbook/common/spectral/dft/Dft2d.java
	/// </summary>
	public class DFT2D
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mode"></param>
		public DFT2D(SCALING_MODE mode)
        {
			Mode = mode;
        }

		/// <summary>
		/// 
		/// </summary>
		private SCALING_MODE Mode { get; set; }

		/// <summary>
		/// Performs an "in-place" 2D DFT forward transformation on the supplied data.
		/// The input signal is replaced by the associated DFT spectrum.
		/// </summary>
		/// <param name="gRe">gRe real part of the signal (modified)</param>
		/// <param name="gIm">gIm imaginary part of the signal (modified)</param>
		public void Forward(float[,] gRe, float[,] gIm)
		{
			Transform(gRe, gIm, true);
		}

		/// <summary>
		/// Performs an "in-place" 2D DFT inverse transformation on the supplied spectrum.
		/// The input spectrum is replaced by the associated signal.
		/// </summary>
		/// <param name="GRe">real part of the spectrum (modified)</param>
		/// <param name="GIm">imaginary part of the spectrum (modified)</param>
		public void Inverse(float[,] GRe, float[,] GIm)
		{
			Transform(GRe, GIm, false);
		}

		/// <summary>
		/// Transforms the given 2D arrays 'in-place'. Separate arrays of identical size
		/// must be supplied for the real and imaginary parts of the signal(forward)
		/// or spectrum(inverse), neither of which may be null.
		/// </summary>
		/// <param name="inRe">real part of the input signal or spectrum (modified)</param>
		/// <param name="inIm">imaginary part of the input signal or spectrum (modified)</param>
		/// <param name="forward">forward transformation if true, inverse transformation if false</param>
		public void Transform(float[,] inRe, float[,] inIm, bool forward)
		{
			int width = inRe.GetLength(0);
			int height = inRe.GetLength(1);

			// transform each row (in place):
			float[] rowRe = new float[width];
			float[] rowIm = new float[width];

			var dftRow = new DFT1DDirect(width, Mode);

			for (int v = 0; v < height; v++)
			{
				extractRow(inRe, v, rowRe);
				extractRow(inIm, v, rowIm);
				dftRow.Transform(rowRe, rowIm, forward);

				insertRow(inRe, v, rowRe);
				insertRow(inIm, v, rowIm);
			}

			// transform each column (in place):
			float[] colRe = new float[height];
			float[] colIm = new float[height];

			var dftCol = new DFT1DDirect(height, Mode);

			for (int u = 0; u < width; u++)
			{
				extractCol(inRe, u, colRe);
				extractCol(inIm, u, colIm);
				dftCol.Transform(colRe, colIm, forward);

				insertCol(inRe, u, colRe);
				insertCol(inIm, u, colIm);
			}
		}

		/// <summary>
		/// extract the values of row 'v' of 'g' into 'row'
		/// </summary>
		/// <param name="g"></param>
		/// <param name="v"></param>
		/// <param name="row"></param>
		private void extractRow(float[,] g, int v, float[] row)
		{
			if (g == null)
			{
				// TODO: check if needed
				row.Fill(0);
			}
			else
			{
				for (int u = 0; u < row.Length; u++)
				{
					row[u] = g[u,v];
				}
			}
		}

		/// <summary>
		/// insert 'row' into row 'v' of 'g'
		/// </summary>
		/// <param name="g"></param>
		/// <param name="v"></param>
		/// <param name="row"></param>
		private void insertRow(float[,] g, int v, float[] row)
		{
			for (int u = 0; u < row.Length; u++)
			{
				g[u,v] = row[u];
			}
		}

		/// <summary>
		/// extract the values of column 'u' of 'g' into 'cols'
		/// </summary>
		/// <param name="g"></param>
		/// <param name="u"></param>
		/// <param name="col"></param>
		private void extractCol(float[,] g, int u, float[] col)
		{
			if (g == null)
			{           
				// TODO: check if needed
				col.Fill(0);
			}
			else
			{
				for (int v = 0; v < col.Length; v++)
				{
					col[v] = g[u,v];
				}
			}
		}

		/// <summary>
		/// insert 'col' into column 'u' of 'g'
		/// </summary>
		/// <param name="g"></param>
		/// <param name="u"></param>
		/// <param name="col"></param>
		private void insertCol(float[,] g, int u, float[] col)
		{
			for (int v = 0; v < col.Length; v++)
			{
				g[u,v] = col[v];
			}
		}
	}
}
