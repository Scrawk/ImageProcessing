using System;
using System.Collections.Generic;

using Common.Core.Colors;
using Common.Core.Numerics;
using Common.Core.Shapes;
using ImageProcessing.Images;

namespace ImageProcessing.Synthesis
{
	public class WangTileSet
	{

		public WangTileSet(int numHColors, int numVColors, int tileSize)
		{
			NumHColors = numHColors;
			NumVColors = numVColors;

			Count = numHColors * numHColors * numVColors * numVColors;

			TileSize = tileSize;
			BorderSize = 8;

		}

		public int Count { get; private set; }

		public int TileSize { get; private set; }

		public int BorderSize { get; private set; }

		public int NumHColors { get; private set; }

		public int NumVColors { get; private set; }

		private WangTile[] Tiles { get; set; }

		public override string ToString()
		{
			return String.Format("[WangTileSet: Count={0}]", Count);
		}

		ColorRGB[] m_colors = new ColorRGB[]
			{
			new ColorRGB(1,0,0), new ColorRGB(0,1,0), new ColorRGB(0,0,1),
			new ColorRGB(1,1,0), new ColorRGB(1,0,1), new ColorRGB(0,1,1)
			};

		public void Test()
		{
			Console.WriteLine(this);

			Tiles = new WangTile[Count];

			int index = 0;
			for (int sEdge = 0; sEdge < NumVColors; sEdge++)
			{
				for (int eEdge = 0; eEdge < NumHColors; eEdge++)
				{
					for (int nEdge = 0; nEdge < NumVColors; nEdge++)
					{
						for (int wEdge = 0; wEdge < NumHColors; wEdge++)
						{
							var tile = new WangTile(index, sEdge, eEdge, nEdge, wEdge, TileSize);

							AddEdgeColor(tile);

							Tiles[index++] = tile;
						}
					}
				}
			}

			var compaction = OrthogonalCompaction();

			var image = CreateTileImage(compaction);

			Console.WriteLine(image);

			image.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/Image.raw");

		}

		int GetIndex(int e0, int e1, int e2, int e3)
		{
			return (e0 * (NumHColors * NumVColors * NumHColors) + e1 * (NumVColors * NumHColors) + e2 * (NumHColors) + e3);
		}

		public int GetIndex(int numHColors, int numVColors, WangTile tile)
		{
			return (tile.sEdge * (numHColors * numVColors * numHColors) + tile.eEdge * (numVColors * numHColors) + tile.nEdge * (numHColors) + tile.wEdge);
		}

		void AddEdgeColor(WangTile tile)
		{
			int border = BorderSize;
			int size = TileSize;

			var min = new Point2i(border, 0);
			var max = min + new Point2i(size - border * 2, border);	
			var box = new Box2i(min, max);

			tile.Image.DrawBox(box, m_colors[tile.nEdge], true);

			min = new Point2i(border, size - border);
			max = min + new Point2i(size - border * 2, size);
			box = new Box2i(min, max);

			tile.Image.DrawBox(box, m_colors[tile.sEdge], true);

			min = new Point2i(0, border);
			max = min + new Point2i(border, size - border * 2);
			box = new Box2i(min, max);

			tile.Image.DrawBox(box, m_colors[tile.wEdge], true);

			min = new Point2i(size - border, border);
			max = min + new Point2i(size, size - border * 2);
			box = new Box2i(min, max);

			tile.Image.DrawBox(box, m_colors[tile.eEdge], true);
		}

		WangTile[,] OrthogonalCompaction()
		{

			int width = NumHColors * NumHColors;
			int height = NumVColors * NumVColors;

			var result = new WangTile[width, height];

			var travelHEdges = TravelEdges(0, NumHColors - 1);
			var travelVEdges = TravelEdges(0, NumVColors - 1);

			for (int j = 0; j < height; j++)
			{
				for (int i = 0; i < width; i++)
				{
					int hIndex0 = i % (NumHColors * NumHColors);
					int hIndex1 = hIndex0 + 1;

					int vIndex0 = j % (NumVColors * NumVColors);
					int vIndex1 = vIndex0 + 1;

					int e0 = travelVEdges[vIndex1];
					int e1 = travelHEdges[hIndex1];
					int e2 = travelVEdges[vIndex0];
					int e3 = travelHEdges[hIndex0];
					
					int index = GetIndex(e0, e1, e2, e3);

					result[i, j] = Tiles[index].Copy();
				}
			}

			return result;
		}

		int[] TravelEdges(int startNode, int endNode)
		{
			int numNodes = (endNode - startNode + 1);
			var result = new int[numNodes * numNodes + 1];

			for (int i = startNode; i <= endNode; i++)
			{
				for (int j = startNode; j <= endNode; j++)
				{
					int index = EdgeOrdering(i - startNode, j - startNode);

					result[index] = i - startNode;
					result[index + 1] = j - startNode;
				}
			}

			return result;
		}

		static int EdgeOrdering(int x, int y)
		{
			if (x < y)
				return (2 * x + y * y);
			else if (x == y)
			{
				if (x > 0)
					return ((x + 1) * (x + 1) - 2);
				else
					return 0;
			}
			else
			{
				if (y > 0)
					return (x * x + 2 * y - 1);
				else
					return ((x + 1) * (x + 1) - 1);
			}
		}

		ColorImage2D CreateTileImage(WangTile[,] compaction)
		{

			int numTilesY = compaction.GetLength(1);
			int numTilesX = compaction.GetLength(0);

			Console.WriteLine("numTilesX " + numTilesX);
			Console.WriteLine("numTilesY " + numTilesY);

			int height = TileSize * numTilesY;
			int width = TileSize * numTilesX;

			ColorRGB[] tileData = new ColorRGB[width * height];

			for (int x = 0; x < numTilesX; x++)
			{
				for (int y = 0; y < numTilesY; y++)
				{
					int idx = GetIndex(NumHColors, NumVColors, compaction[x, y]);

					for (int i = 0; i < TileSize; i++)
					{
						for (int j = 0; j < TileSize; j++)
						{
							tileData[(x * TileSize + i) + (y * TileSize + j) * width] = Tiles[idx].Image[i, j];
						}
					}
				}
			}

			var tileTexture = new ColorImage2D(width, height);
			tileTexture.Fill(tileData);

			return tileTexture;
		}

	}
}
