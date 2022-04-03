using System;
using System.Collections.Generic;

using Common.Core.Colors;
using Common.Core.Numerics;
using Common.Core.Shapes;
using Common.Core.Bits;
using ImageProcessing.Images;

namespace ImageProcessing.Synthesis
{
    public class WangTile
    {

		public readonly int index, sEdge, eEdge, nEdge, wEdge;

		public WangTile(int index, int sEdge, int eEdge, int nEdge, int wEdge, int tileSize)
		{
			this.index = index;
			this.sEdge = sEdge;
			this.eEdge = eEdge;
			this.nEdge = nEdge;
			this.wEdge = wEdge;

			Image = new ColorImage2D(tileSize, tileSize);
		}

		public int Size => Image.Width;

		public ColorImage2D Image { get; private set; }

		public override string ToString()
		{
			return String.Format("[WangTile: Index={0}, sEdge={1}, eEdge={2}, nEdge={3}, wEdge={4}]",
				index, sEdge, eEdge, nEdge, wEdge);
		}

		public WangTile Copy()
        {
			var tile = new WangTile(index, sEdge, eEdge, nEdge, wEdge, Size);
			tile.Image.Fill(Image);
			return tile;
        }

		public void ColorEdges(int thickness, ColorRGB[] colors, float alpha)
		{

			var min = new Point2i(thickness, 0);
			var max = min + new Point2i(Size - thickness * 2, thickness);
			Image.DrawBox(min, max, new ColorRGBA(colors[nEdge], alpha), true);

			min = new Point2i(thickness, Size - thickness);
			max = min + new Point2i(Size - thickness * 2, Size);
			Image.DrawBox(min, max, new ColorRGBA(colors[sEdge], alpha), true);

			min = new Point2i(0, thickness);
			max = min + new Point2i(thickness, Size - thickness * 2);
			Image.DrawBox(min, max, new ColorRGBA(colors[wEdge], alpha), true);

			min = new Point2i(Size - thickness, thickness);
			max = min + new Point2i(Size, Size - thickness * 2);
			Image.DrawBox(min, max, new ColorRGBA(colors[eEdge], alpha), true);
		}

		public void DrawMask(ColorRGB[] colors)
        {
			var c00 = new Point2i(0, 0);
			var c01 = new Point2i(0, Size);
			var c10 = new Point2i(Size, 0);
			var c11 = new Point2i(Size, Size);
			var mid = new Point2i(Size/2, Size/2);

			Image.DrawTriangle(mid, c01, c11, new ColorRGBA(colors[sEdge], 1), true);
			Image.DrawTriangle(mid, c00, c10, new ColorRGBA(colors[nEdge], 1), true);

			Image.DrawTriangle(mid, c00, c01, new ColorRGBA(colors[wEdge], 1), true);
			Image.DrawTriangle(mid, c10, c11, new ColorRGBA(colors[eEdge], 1), true);

		}

	}
}
