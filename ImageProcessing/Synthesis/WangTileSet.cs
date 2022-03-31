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

		public WangTileSet(int numColors, int tileSize)
		{
			NumHColors = numColors;
			NumVColors = numColors;

			Count = (int)(Math.Pow(numColors, 4));

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
			return String.Format("[WangTileSet: Size={0}]", Count);
		}

		ColorRGB[] m_colors = new ColorRGB[]
			{
			new ColorRGB(1,0,0), new ColorRGB(0,1,0), new ColorRGB(0,0,1),
			new ColorRGB(1,1,0), new ColorRGB(1,0,1), new ColorRGB(0,1,1)
			};

		public void Test()
		{
			Tiles = new WangTile[Count];

			int index = 0;
			for (int sEdge = 0; sEdge < NumHColors; sEdge++)
			{
				for (int eEdge = 0; eEdge < NumVColors; eEdge++)
				{
					for (int nEdge = 0; nEdge < NumHColors; nEdge++)
					{
						for (int wEdge = 0; wEdge < NumVColors; wEdge++)
						{
							var tile = new WangTile(index, sEdge, eEdge, nEdge, wEdge, TileSize);

							AddEdgeColor(nEdge, eEdge, sEdge, wEdge, tile);

							Tiles[index++] = tile;
						}
					}
				}
			}

			var compaction = OrthogonalCompaction();

			var image = CreateTileImage(compaction);

			image.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/Image.raw");

		}

		int GetIndex(int e0, int e1, int e2, int e3)
		{
			return (e0 * (NumVColors * NumHColors * NumVColors) + e1 * (NumHColors * NumVColors) + e2 * (NumVColors) + e3);
		}

		public int GetIndex(int numHColors, int numVColors, int e0, int e1, int e2, int e3)
		{
			return (e0 * (numVColors * numHColors * numVColors) + e1 * (numHColors * numVColors) + e2 * (numVColors) + e3);
		}

		public int GetIndex(int numHColors, int numVColors, WangTile tile)
		{
			return (tile.sEdge * (numVColors * numHColors * numVColors) + tile.eEdge * (numHColors * numVColors) + tile.nEdge * (numVColors) + tile.wEdge);
		}

		void AddEdgeColor(int sEdge, int eEdge, int nEdge, int wEdge, WangTile tile)
		{
			int border = BorderSize;
			int size = TileSize;
			for (int i = border; i < size - border; i++)
			{
				for (int j = 0; j < border; j++)
				{
					tile.Image[i, j] = m_colors[nEdge];

					tile.Image[i, j + size - border] = m_colors[sEdge];

					tile.Image[j, i] = m_colors[eEdge];

					tile.Image[j + size - border, i] = m_colors[wEdge];
				}
			}
		}

		WangTile[,] OrthogonalCompaction()
		{
			int numHColors = NumHColors;
			int numVColors = NumHColors;

			int height = numHColors * numHColors;
			int width = numVColors * numVColors;

			var result = new WangTile[height, width];

			var travelHEdges = TravelEdges(0, numHColors - 1);
			var travelVEdges = TravelEdges(0, numVColors - 1);

			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					int hIndex0 = i % (numHColors * numHColors);
					int hIndex2 = hIndex0 + 1;
					int vIndex1 = j % (numVColors * numVColors);
					int vIndex3 = vIndex1 + 1;

					int e0 = travelHEdges[hIndex0];
					int e3 = travelVEdges[vIndex1];
					int e2 = travelHEdges[hIndex2];
					int e1 = travelVEdges[vIndex3];

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

			int numTilesY = compaction.GetLength(0);
			int numTilesX = compaction.GetLength(1);

			int height = TileSize * numTilesY;
			int width = TileSize * numTilesX;

			ColorRGB[] tileData = new ColorRGB[width * height];

			for (int x = 0; x < numTilesX; x++)
			{
				for (int y = 0; y < numTilesY; y++)
				{
					int idx = GetIndex(NumHColors, NumVColors, compaction[y, x]);

					for (int i = 0; i < TileSize; i++)
					{
						for (int j = 0; j < TileSize; j++)
						{
							tileData[(x * TileSize + i) + (y * TileSize + j) * width] = Tiles[idx].Image[TileSize - 1 - i, j];
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
