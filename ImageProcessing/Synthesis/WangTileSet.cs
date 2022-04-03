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

			if (numHColors < 2 || numHColors > 4)
				throw new ArgumentException("Number of edge colors must be >= 2 annd <= 4.");

			if (numVColors < 2 || numVColors > 4)
				throw new ArgumentException("Number of edge colors must be >= 2 annd <= 4.");

			NumHColors = numHColors;
			NumVColors = numVColors;

			NumTiles = numHColors * numHColors * numVColors * numVColors;

			TileSize = tileSize;
			BorderSize = 8;

		}

		public int NumTiles { get; private set; }

		public int TileSize { get; private set; }

		public int BorderSize { get; private set; }

		public int NumHColors { get; private set; }

		public int NumVColors { get; private set; }

		private WangTile[] Tiles { get; set; }

		public override string ToString()
		{
			return String.Format("[WangTileSet: NumHColors={0}, NumVColors={1}, NumTiles={2}, TileSize={3}]", 
				NumHColors, NumVColors, NumTiles, TileSize);
		}

		private static ColorRGB[] Colors = new ColorRGB[]
		{
			ColorRGB.Red,
			ColorRGB.Green,
			ColorRGB.Blue,
			ColorRGB.Yellow
		};

		public void Test(ColorImage2D source)
		{

			var exemplars = new ExemplarSet(TileSize);
			exemplars.CreateExemplarFromRandom(source, 0, 32);

			Console.WriteLine(this);
			Console.WriteLine(exemplars);

			CreateTiles();

			AddEdgeColor();

			var compaction = OrthogonalCompaction();

			var image = CreateImage(compaction);

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

		private void CreateTiles()
        {
			Tiles = new WangTile[NumTiles];

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
							Tiles[index++] = tile;
						}
					}
				}
			}
		}

		private void AddEdgeColor()
        {
			foreach (var tile in Tiles)
            {
				tile.DrawMask(Colors);
				tile.ColorEdges(BorderSize, Colors, 0.5f);
			}
				
        }

		private WangTile[,] OrthogonalCompaction()
		{

			int width = NumHColors * NumHColors;
			int height = NumVColors * NumVColors;

			var result = new WangTile[width, height];

			var travelHEdges = TravelEdges(0, NumHColors - 1);
			var travelVEdges = TravelEdges(0, NumVColors - 1);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int hIndex0 = x % (NumHColors * NumHColors);
					int hIndex1 = hIndex0 + 1;

					int vIndex0 = y % (NumVColors * NumVColors);
					int vIndex1 = vIndex0 + 1;

					int e0 = travelVEdges[vIndex1];
					int e1 = travelHEdges[hIndex1];
					int e2 = travelVEdges[vIndex0];
					int e3 = travelHEdges[hIndex0];
					
					int index = GetIndex(e0, e1, e2, e3);
					var tile = Tiles[index];

					result[x, y] = tile.Copy();
				}
			}

			return result;
		}

		private int[] TravelEdges(int startNode, int endNode)
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

		private int EdgeOrdering(int x, int y)
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

		private ColorImage2D CreateImage(WangTile[,] compaction)
		{

			int numTilesY = compaction.GetLength(1);
			int numTilesX = compaction.GetLength(0);

			int height = TileSize * numTilesY;
			int width = TileSize * numTilesX;

			var image = new ColorImage2D(width, height);

			for (int x = 0; x < numTilesX; x++)
			{
				for (int y = 0; y < numTilesY; y++)
				{
					int idx = GetIndex(NumHColors, NumVColors, compaction[x, y]);

					var tile = Tiles[idx];

					for (int i = 0; i < TileSize; i++)
					{
						for (int j = 0; j < TileSize; j++)
						{
							image[x * TileSize + i, y * TileSize + j] = tile.Image[i, j];
						}
					}
				}
			}

			//image = ColorImage2D.FlipVertical(image);

			return image;
		}

	}
}
