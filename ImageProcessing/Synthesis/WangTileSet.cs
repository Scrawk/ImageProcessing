using System;
using System.Collections.Generic;

using Common.Core.Colors;
using Common.Core.Numerics;
using Common.Core.Shapes;
using Common.Core.Extensions;
using Common.Core.Threading;
using Common.GraphTheory.GridGraphs;

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

			TileCount = numHColors * numHColors * numVColors * numVColors;

			TileSize = tileSize;

		}

		public int TileCount { get; private set; }

		public int TileSize { get; private set; }

		public int NumHColors { get; private set; }

		public int NumVColors { get; private set; }

		private WangTile[] Tiles { get; set; }

		public override string ToString()
		{
			return String.Format("[WangTileSet: NumHColors={0}, NumVColors={1}, TileCount={2}, TileSize={3}]", 
				NumHColors, NumVColors, TileCount, TileSize);
		}

		private static ColorRGB[] Colors = new ColorRGB[]
		{
			ColorRGB.Red,
			ColorRGB.Green,
			ColorRGB.Blue,
			ColorRGB.Yellow
		};

		public void CreateTiles(ColorImage2D source, int seed, bool addEdgeColors = false)
		{

			var exemplarSet = new ExemplarSet(TileSize);
			exemplarSet.CreateExemplarFromRandom(source, seed, 32);

			var exemplars = exemplarSet.GetRandomExemplars(Math.Max(NumHColors, NumVColors) + 4, seed);

			var pairs = new List<ValueTuple<ColorImage2D, float>>();

			for (int i = 0; i < exemplars.Count; i++)
			{
				var exemplar = exemplars[i];

				var pair = ImageSynthesis.MakeTileable(exemplar.Image, exemplarSet);

				var tileable = pair.Item1;
				var cost = pair.Item2;

				pairs.Add(pair);

				//tileable.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/tileable" + i + ".raw");
            }

			pairs.Sort((a,b) => a.Item2.CompareTo(b.Item2));

			var tilables = new List<ColorImage2D>();
			for(int i = 0; i < pairs.Count; i++)
            {
				tilables.Add(pairs[i].Item1);
            }

			//Console.WriteLine(this);
			//Console.WriteLine(exemplarSet);

			CreateTiles();

			for(int i = 0; i < TileCount; i++)
			{
				var tile = Tiles[i];
				tile.CreateMap();
				tile.CreateMask();
				tile.FillImage(tilables);

				Console.WriteLine("Creating " + tile);

				ImageSynthesis.CreateTileImage(tile, exemplarSet);

				if(addEdgeColors)
					tile.ColorEdges(4, Colors, 0.25f);
			}
		}

		public ColorImage2D CreateTilesImage()
        {
			var tiling = OrthogonalTiling();
			return CreateTileImage(tiling);
		}

		public ColorImage2D CreateTileMappingImage(int numHTiles, int numVTiles, int seed)
		{
			var tiling = SequentialTiling(numHTiles, numVTiles, seed);
			return CreateTileImage(tiling);
		}

		private int GetIndex(int e0, int e1, int e2, int e3)
		{
			return (e0 * (NumHColors * NumVColors * NumHColors) + e1 * (NumVColors * NumHColors) + e2 * (NumHColors) + e3);
		}

		private int GetIndex(int numHColors, int numVColors, WangTile tile)
		{
			return (tile.sEdge * (numHColors * numVColors * numHColors) + tile.eEdge * (numVColors * numHColors) + tile.nEdge * (numHColors) + tile.wEdge);
		}

		private void CreateTiles()
        {
			Tiles = new WangTile[TileCount];

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

		private Index2[,] SequentialTiling(int numHTiles, int numVTiles, int seed)
		{

			var edges = new Index4[numHTiles, numVTiles];
			edges.Fill((x,y) => new Index4(-1,-1,-1,-1));

			var indices = new Index2[numHTiles, numVTiles];
			indices.Fill((x, y) => new Index2(-1, -1));

			var rnd = new Random(seed);

			for (int j = 0; j < numVTiles; j++)
			{
				for (int i = 0; i < numHTiles; i++)
				{
					/*
					var possible = GetPossibleTiles(i, j, edges);

					var count = possible.Count;
					if (count == 0) continue;

					int e0 = edges.GetWrapped(i, j - 1)[2];
					int e1 = edges.GetWrapped(i + 1, j)[3];
					int e2 = edges.GetWrapped(i, j + 1)[0];
					int e3 = edges.GetWrapped(i - 1, j)[1];

					int attempts = 0;
					Index4 index = new Index4();

					while (attempts++ < 10)
					{
						index = possible[rnd.Next(0, count)];
	
						if (e0 != -1) index[0] = e0;
						if (e1 != -1) index[1] = e1;
						if (e2 != -1) index[2] = e2;
						if (e3 != -1) index[3] = e3;

						if (possible.Contains(index))
							break;
					}

					edges[i, j] = index;
					indices[i, j] = GetIndex(index[2], index[1], index[0], index[3]);
					*/

					int e0 = edges.GetWrapped(i, j - 1)[2];
					int e1 = edges.GetWrapped(i + 1, j)[3];
					int e2 = edges.GetWrapped(i, j + 1)[0];
					int e3 = edges.GetWrapped(i - 1, j)[1];

					if (e0 < 0) e0 = rnd.Next(0, NumVColors);
					if (e1 < 0) e1 = rnd.Next(0, NumHColors);
					if (e2 < 0) e2 = rnd.Next(0, NumVColors);
					if (e3 < 0) e3 = rnd.Next(0, NumHColors);

					edges[i, j] = new Index4(e0, e1, e2, e3);

					var index = GetIndex(e2, e1, e0, e3);

					int xi = index % 4;
					int yi = index / 4;

					indices[i, j] = new Index2(xi, yi);
				}
			}

			return indices;
		}

		private List<Index4> GetPossibleTiles(int i, int j, Index4[,] edges)
        {
			var list = new List<Index4>();

			for (int e0 = 0; e0 < NumVColors; e0++)
			{
				for (int e1 = 0; e1 < NumHColors; e1++)
				{
					for (int e2 = 0; e2 < NumVColors; e2++)
					{
						for (int e3 = 0; e3 < NumHColors; e3++)
						{
							var index = new Index4(e0, e1, e2, e3);

							if (AreSame(index, i - 1, j, edges)) continue;
							if (AreSame(index, i, j + 1, edges)) continue;
							if (AreSame(index, i + 1, j, edges)) continue;
							if (AreSame(index, i, j - 1, edges)) continue;

							list.Add(index);
						}
					}
				}
			}

			return list;
		}

		private bool AreSame(Index4 index, int x, int y, Index4[,] edges)
        {
			int nullCount = 0;
			for(int i = 0; i < 4; i++)
            {
				int e = edges.GetWrapped(x, y)[i];
				if (e == -1)
				{
					nullCount++;
					continue;
				}

				if(e != index[i]) return false;
            }

			if (nullCount == 4)
				return false;
			else
				return true;
        }


		private int[,] OrthogonalTiling()
		{

			int width = NumHColors * NumHColors;
			int height = NumVColors * NumVColors;

			var result = new int[width, height];

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
					result[x, y] = index;
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

		private ColorImage2D CreateTileImage(int[,] tiling)
		{

			int numTilesY = tiling.GetLength(1);
			int numTilesX = tiling.GetLength(0);

			int height = TileSize * numTilesY;
			int width = TileSize * numTilesX;

			var image = new ColorImage2D(width, height);

			for (int x = 0; x < numTilesX; x++)
			{
				for (int y = 0; y < numTilesY; y++)
				{
					int idx = tiling[x, y];
					if (idx < 0) continue;

					var tile = Tiles[idx];

					for (int i = 0; i < TileSize; i++)
					{
						for (int j = 0; j < TileSize; j++)
						{
							int xi = x * TileSize + i;
							int yj = y * TileSize + j;

							image[xi, yj] = tile.Image[i, j];
						}
					}
				}
			}

			return image;
		}

		private ColorImage2D CreateTileImage(Index2[,] tiling)
		{

			int numTilesY = tiling.GetLength(1);
			int numTilesX = tiling.GetLength(0);

			int height = TileSize * numTilesY;
			int width = TileSize * numTilesX;

			var image = new ColorImage2D(width, height);

			for (int x = 0; x < numTilesX; x++)
			{
				for (int y = 0; y < numTilesY; y++)
				{
					var idx = tiling[x, y];
	
					var tile = Tiles[idx.x + idx.y * 4];

					for (int i = 0; i < TileSize; i++)
					{
						for (int j = 0; j < TileSize; j++)
						{
							int xi = x * TileSize + i;
							int yj = y * TileSize + j;

							image[xi, yj] = tile.Image[i, j];
						}
					}
				}
			}

			return image;
		}

		private ColorImage2D CreateMappingImage(Index2[,] tiling)
		{

			int width = tiling.GetLength(1);
			int height = tiling.GetLength(0);

			var image = new ColorImage2D(width, height);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var idx = tiling[x, y];
			
					image[x, y] = new ColorRGB(idx.x / 4.0f, idx.y / 4.0f, 0);

				}
			}

			return image;
		}


	}
}
