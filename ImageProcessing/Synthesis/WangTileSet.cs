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

		public WangTileSet(int numColors)
		{
			m_numHColors = numColors;
			m_numVColors = numColors;

			Size = (int)(Math.Pow(numColors, 4));
		}

		public int Size { get; private set; }

		public int m_numHColors { get; private set; }

		public int m_numVColors { get; private set; }

		private WangTile[] Tiles { get; set; }

		public override string ToString()
		{
			return String.Format("[WangTileSet: Size={0}]", Size);
		}

		int m_tileSize = 128;

		int m_borderSize = 8;

		ColorRGB[] m_colors = new ColorRGB[]
			{
			new ColorRGB(1,0,0), new ColorRGB(0,1,0), new ColorRGB(0,0,1),
			new ColorRGB(1,1,0), new ColorRGB(1,0,1), new ColorRGB(0,1,1)
			};

		WangTile[,] m_tileCompaction = null;

		public void Test()
		{
			Tiles = new WangTile[Size];

			int id = 0;
			for (int e0 = 0; e0 < m_numHColors; e0++)
			{
				for (int e1 = 0; e1 < m_numVColors; e1++)
				{
					for (int e2 = 0; e2 < m_numHColors; e2++)
					{
						for (int e3 = 0; e3 < m_numVColors; e3++)
						{
							int index = GetIndex(e0, e1, e2, e3);

							Tiles[index] = new WangTile(id++, e0, e1, e2, e3, 128);
						}
					}
				}
			}

			List<ColorRGB[,]> colorTiles = null;

			Create(ref colorTiles);

			OrthogonalCompaction(ref m_tileCompaction);

			ColorImage2D m_tileTexture = null;

			CreateTileTexture(ref m_tileTexture, colorTiles);

			m_tileTexture.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/Image.raw");

		}

		bool Create(ref List<ColorRGB[,]> colorTiles)
		{

			List<ColorRGB[,]> resultTiles = new List<ColorRGB[,]>(m_numVColors * m_numVColors * m_numHColors * m_numHColors);

			for (int x = 0; x < Size; x++)
			{
				resultTiles.Add(new ColorRGB[m_numHColors, m_numHColors]);
			}

			_WangTileSet(ref resultTiles);

			ColorWangTileSet(ref colorTiles);

			return true;
		}

		void ColorWangTileSet(ref List<ColorRGB[,]> colorTiles)
		{

			colorTiles = new List<ColorRGB[,]>(m_numVColors * m_numVColors * m_numHColors * m_numHColors);

			for (int x = 0; x < colorTiles.Capacity; x++)
			{
				colorTiles.Add(new ColorRGB[m_tileSize, m_tileSize]);
			}

			for (int sEdge = 0; sEdge < m_numHColors; sEdge++)
			{
				for (int eEdge = 0; eEdge < m_numVColors; eEdge++)
				{
					for (int nEdge = 0; nEdge < m_numHColors; nEdge++)
					{
						for (int wEdge = 0; wEdge < m_numVColors; wEdge++)
						{
							int idx = GetIndex(sEdge, eEdge, nEdge, wEdge);

							ColorRGB[,] result = new ColorRGB[m_tileSize, m_tileSize];

							AddEdgeColor(nEdge, eEdge, sEdge, wEdge, ref result, m_tileSize);

							colorTiles[idx] = result;
						}
					}
				}
			}
		}

		int GetIndex(int e0, int e1, int e2, int e3)
		{
			return (e0 * (m_numVColors * m_numHColors * m_numVColors) + e1 * (m_numHColors * m_numVColors) + e2 * (m_numVColors) + e3);
		}

		public int GetIndex(int numHColors, int numVColors, int e0, int e1, int e2, int e3)
		{
			return (e0 * (numVColors * numHColors * numVColors) + e1 * (numHColors * numVColors) + e2 * (numVColors) + e3);
		}

		public int GetIndex(int numHColors, int numVColors, WangTile tile)
		{
			return (tile.e0 * (numVColors * numHColors * numVColors) + tile.e1 * (numHColors * numVColors) + tile.e2 * (numVColors) + tile.e3);
		}

		void _WangTileSet(ref List<ColorRGB[,]> resultTiles)
		{

			for (int sEdge = 0; sEdge < m_numHColors; sEdge++)
			{
				for (int eEdge = 0; eEdge < m_numVColors; eEdge++)
				{
					for (int nEdge = 0; nEdge < m_numHColors; nEdge++)
					{
						for (int wEdge = 0; wEdge < m_numVColors; wEdge++)
						{
							int idx = GetIndex(sEdge, eEdge, nEdge, wEdge);

							ColorRGB[,] result = new ColorRGB[m_tileSize, m_tileSize];

							AddEdgeColor(nEdge, eEdge, sEdge, wEdge, ref result, m_tileSize);

							resultTiles[idx] = result;
						}
					}
				}
			}

		}

		void AddEdgeColor(int sEdge, int eEdge, int nEdge, int wEdge, ref ColorRGB[,] col, int size)
		{
			int border = m_borderSize;

			for (int i = border; i < size - border; i++)
			{
				for (int j = 0; j < border; j++)
				{
					col[i, j] += m_colors[nEdge];

					col[i, j + size - border] += m_colors[sEdge];

					col[j, i] += m_colors[eEdge];

					col[j + size - border, i] += m_colors[wEdge];
				}
			}
		}

		 bool OrthogonalCompaction(ref WangTile[,] result)
		{
			int numHColors = m_numHColors;
			int numVColors = m_numHColors;

			int height = numHColors * numHColors;
			int width = numVColors * numVColors;

			result = new WangTile[height, width];

			List<int> travelHEdges = new List<int>();
			List<int> travelVEdges = new List<int>();

			if (!TravelEdges(0, numHColors - 1, ref travelHEdges))
			{
				Console.WriteLine("WangTileSet::OrthogonalCompaction - Travel horizontal edges failed");
				return false;
			}

			if (!TravelEdges(0, numVColors - 1, ref travelVEdges))
			{
				Console.WriteLine("WangTileSet::OrthogonalCompaction - Travel vertical edges failed");
				return false;
			}

			// put the tiles
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

			// done
			return true;
		}

		bool TravelEdges(int startNode, int endNode, ref List<int> result)
		{
			if (startNode > endNode)
			{
				Console.WriteLine("WangTileSet::TravelEdges - start node is greater than end node");
				return false;
			}

			if (startNode != 0)
			{
				Console.WriteLine("WangTileSet::TravelEdges - start node is not 0");
				return false;
			}

			int numNodes = (endNode - startNode + 1);

			result = new List<int>(numNodes * numNodes + 1);

			for (int i = 0; i < result.Capacity; i++)
			{
				result.Add(-1);
			}

			for (int i = startNode; i <= endNode; i++)
			{
				for (int j = startNode; j <= endNode; j++)
				{
					int index = EdgeOrdering(i - startNode, j - startNode);

					if ((index < 0) || (index >= (numNodes * numNodes)))
					{
						Console.WriteLine("WangTileSet::TravelEdges - capacity error");
						return false;
					}

					result[index] = i - startNode;
					result[index + 1] = j - startNode;
				}
			}

			return true;
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

		bool CreateTileTexture(ref ColorImage2D tileTexture, List<ColorRGB[,]> tiles)
		{

			int tileTextureHeight = m_tileCompaction.GetLength(0);
			int tileTextureWidth = m_tileCompaction.GetLength(1);

			int glTileTextureHeight = m_tileSize * tileTextureHeight;
			int glTileTextureWidth = m_tileSize * tileTextureWidth;

			ColorRGB[] tileData = new ColorRGB[glTileTextureWidth * glTileTextureHeight];

			for (int x = 0; x < tileTextureWidth; x++)
			{
				for (int y = 0; y < tileTextureHeight; y++)
				{
					int idx = GetIndex(m_numHColors, m_numVColors, m_tileCompaction[y, x]);

					for (int i = 0; i < m_tileSize; i++)
					{
						for (int j = 0; j < m_tileSize; j++)
						{
							tileData[(x * m_tileSize + i) + (y * m_tileSize + j) * glTileTextureWidth] = tiles[idx][m_tileSize - 1 - i, j];
						}
					}
				}
			}

			tileTexture = new ColorImage2D(glTileTextureWidth, glTileTextureHeight);
			tileTexture.Fill(tileData);

			return true;
		}

	}
}
