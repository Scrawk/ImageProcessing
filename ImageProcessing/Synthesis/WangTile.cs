using System;
using System.Collections.Generic;

using Common.Core.Colors;
using Common.Core.Numerics;
using Common.Core.Shapes;
using ImageProcessing.Images;

namespace ImageProcessing.Synthesis
{
    public class WangTile
    {

		public readonly int id, sEdge, eEdge, nEdge, wEdge;

		public WangTile(int id, int sEdge, int eEdge, int nEdge, int wEdge, int tileSize)
		{
			this.id = id;
			this.sEdge = sEdge;
			this.eEdge = eEdge;
			this.nEdge = nEdge;
			this.wEdge = wEdge;

			Image = new ColorImage2D(tileSize, tileSize);
		}

		public int Size => Image.Width;

		public ColorImage2D Image { get; private set; }

		public WangTile Copy()
        {
			var tile = new WangTile(id, sEdge, eEdge, nEdge, wEdge, Size);
			tile.Image.Fill(Image);
			return tile;
        }

	}
}
