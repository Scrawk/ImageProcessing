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

		public readonly int id, e0, e1, e2, e3;

		public WangTile(int id, int e0, int e1, int e2, int e3, int tileSize)
		{
			this.id = id;
			this.e0 = e0;
			this.e1 = e1;
			this.e2 = e2;
			this.e3 = e3;

			Image = new ColorImage2D(tileSize, tileSize);
		}

		public int Size => Image.Width;

		public ColorImage2D Image { get; private set; }

		public WangTile Copy()
        {
			var tile = new WangTile(id, e0, e1, e2, e3, Size);
			tile.Image.Fill(Image);
			return tile;
        }

	}
}
