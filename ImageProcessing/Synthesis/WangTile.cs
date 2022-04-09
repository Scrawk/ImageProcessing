using System;
using System.Collections.Generic;

using Common.Core.Colors;
using Common.Core.Numerics;
using Common.Core.Shapes;
using Common.Core.Extensions;
using ImageProcessing.Images;

namespace ImageProcessing.Synthesis
{
    public class WangTile
    {

		public WangTile(int sEdge, int eEdge, int nEdge, int wEdge, int tileSize)
		{
			Edges = new int[]
			{
				sEdge, eEdge, nEdge, wEdge
			};

			Image = new ColorImage2D(tileSize, tileSize);
			Map = new GreyScaleImage2D(tileSize, tileSize);
			Mask = new BinaryImage2D(tileSize, tileSize);
		}

		public int Index { get; set; }

		public Index2 Index2 { get; set; }

		public int sEdge => Edges[0];

		public int eEdge => Edges[1];

		public int nEdge => Edges[2];

		public int wEdge => Edges[3];

		public int[] Edges { get; private set; }

		public int Size => Image.Width;

		public ColorImage2D Image { get; private set; }

		public GreyScaleImage2D Map { get; private set; }

		public BinaryImage2D Mask { get; private set; }

		public bool IsConst
        {
			get
			{
				var first = Edges[0];
				for(int i = 1; i < 4; i++)
					if(Edges[i] != first)
						return false;

				return true;
			}
        }

		public override string ToString()
		{
			return String.Format("[WangTile: Index={0}, sEdge={1}, eEdge={2}, nEdge={3}, wEdge={4}]",
				Index, sEdge, eEdge, nEdge, wEdge);
		}

		public WangTile Copy()
        {
			var tile = new WangTile(sEdge, eEdge, nEdge, wEdge, Size);
			tile.Index = Index;
			tile.Index2 = Index2;
			tile.Image.Fill(Image);
			tile.Map.Fill(Map);
			tile.Mask.Fill(Mask);
			return tile;
        }

		public void FillImage(List<ColorImage2D> images)
        {
			Image.Fill((x, y) =>
			{
				var index = (int)Map[x, y];
				var pixel = images[index][x,y];

				return pixel;
			});
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

		public void CreateMap()
        {
			if (IsConst)
			{
				Map.Fill(sEdge);
			}
			else
			{
				var c00 = new Point2i(0, 0);
				var c01 = new Point2i(0, Size);
				var c10 = new Point2i(Size, 0);
				var c11 = new Point2i(Size, Size);
				var mid = new Point2i(Size / 2, Size / 2);

				Map.DrawTriangle(mid, c01, c11, new ColorRGBA(Edges[0], 1), true);
				Map.DrawTriangle(mid, c10, c11, new ColorRGBA(Edges[1], 1), true);
				Map.DrawTriangle(mid, c00, c10, new ColorRGBA(Edges[2], 1), true);
				Map.DrawTriangle(mid, c00, c01, new ColorRGBA(Edges[3], 1), true);
				
			}

		}

		public void CreateMask()
		{
			if (IsConst) return;

			var mid = new Point2i(Size / 2, Size / 2);

			if(sEdge != eEdge)
				Mask.DrawLine(mid, new Point2i(Size, Size), ColorRGBA.White);

			if (eEdge != nEdge)
				Mask.DrawLine(mid, new Point2i(Size, 0), ColorRGBA.White);

			if (nEdge != wEdge)
				Mask.DrawLine(mid, new Point2i(0, 0), ColorRGBA.White);

			if (wEdge != sEdge)
				Mask.DrawLine(mid, new Point2i(0, Size), ColorRGBA.White);

			Mask = BinaryImage2D.Dilate(Mask, 5);

		}

	}
}
