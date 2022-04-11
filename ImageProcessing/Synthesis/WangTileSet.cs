﻿using System;
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

		private WangTile[,] Tiles2 { get; set; }

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

			var exemplars = exemplarSet.GetRandomExemplars(Math.Max(NumHColors, NumVColors), seed);

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

			pairs.Sort((a, b) => a.Item2.CompareTo(b.Item2));

			var tilables = new List<ColorImage2D>();
			for (int i = 0; i < pairs.Count; i++)
			{
				tilables.Add(pairs[i].Item1);
			}

			Console.WriteLine(this);
			Console.WriteLine(exemplarSet);

			CreateTiles();
			var tiling = OrthogonalTiling();
			RepackTiles(tiling);

			for (int i = 0; i < TileCount; i++)
			{
				var tile = Tiles[i];
				tile.CreateMap();
				tile.CreateMask();
				tile.FillImage(tilables);

				Console.WriteLine("Creating " + tile);

				//ImageSynthesis.CreateTileImage(tile, exemplarSet);

				if (addEdgeColors)
					tile.ColorEdges(4, Colors, 0.25f);
			}

		}

		public ColorImage2D CreateTilesImage()
		{
			return CreateTileImage(Tiles2);
		}

		public ColorImage2D CreateTileMappingImage(int numHTiles, int numVTiles, int seed)
		{
			//var tiling = SequentialTiling(numHTiles, numVTiles, seed);
			return CreateMappingImage(Tiles2);
		}

		private int GetIndex(int e0, int e1, int e2, int e3)
		{
			return (e0 * (NumHColors * NumVColors * NumHColors) + e1 * (NumVColors * NumHColors) + e2 * (NumHColors) + e3);
		}

		private void CreateTiles()
		{
			Tiles = new WangTile[TileCount];

			Tiles2 = new WangTile[NumHColors * NumHColors, NumVColors * NumVColors];

			int index = 0;
			for (int sEdge = 0; sEdge < NumVColors; sEdge++)
			{
				for (int eEdge = 0; eEdge < NumHColors; eEdge++)
				{
					for (int nEdge = 0; nEdge < NumVColors; nEdge++)
					{
						for (int wEdge = 0; wEdge < NumHColors; wEdge++)
						{
							var tile = new WangTile(sEdge, eEdge, nEdge, wEdge, TileSize);
							tile.Index = GetIndex(sEdge, eEdge, nEdge, wEdge);

							Tiles[index++] = tile;
						}
					}
				}
			}

			index = 0;
			for (int y = 0; y < Tiles2.GetLength(1); y++)
			{
				for (int x = 0; x < Tiles2.GetLength(0); x++)
				{
					Tiles2[x, y] = Tiles[index++];
					Tiles2[x, y].Index2 = new Index2(x, y);
				}
			}
		}

		private void RepackTiles(int[,] tiles)
		{
			WangTile[,] repacked = new WangTile[Tiles2.GetLength(0), Tiles2.GetLength(1)];

			for (int y = 0; y < Tiles2.GetLength(1); y++)
			{
				for (int x = 0; x < Tiles2.GetLength(0); x++)
				{
					var idx = tiles[x, y];
					repacked[x, y] = Tiles[idx];
					repacked[x, y].Index2 = new Index2(x, y);

					//int e0 = repacked[x, y].Edges[0];
					//int e1 = repacked[x, y].Edges[1];
					//int e2 = repacked[x, y].Edges[2];
					//int e3 = repacked[x, y].Edges[3];

					//var i1 = repacked[x, y].Index2;
					//var i2 = TileIndex2D(e0, e1, e2, e3);

					//Console.WriteLine(i1 + " " + i2 + " " + i1.Equals(i2));
				}
			}

			Tiles2 = repacked;
		}

		private Index2[,] SequentialTiling(int numHTiles, int numVTiles, int seed)
		{

			var edges = new Index4[numHTiles, numVTiles];
			edges.Fill((x, y) => new Index4(-1, -1, -1, -1));

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

					//var index = GetIndex(e2, e1, e0, e3);
					//var tile = Tiles[index];

					indices[i, j] = TileIndex2D(e0, e1, e2, e3);
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
			for (int i = 0; i < 4; i++)
			{
				int e = edges.GetWrapped(x, y)[i];
				if (e == -1)
				{
					nullCount++;
					continue;
				}

				if (e != index[i]) return false;
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

		int TileIndex1D(int e1, int e2)
		{
			int result;

			if (e1 < e2)
				result = (2 * e1 + e2 * e2);
			else if (e1 == e2)
			{
				if (e1 > 0)
					result = ((e1 + 1) * (e1 + 1) - 2);
				else result = 0;
			}
			else if (e2 > 0)
				result = (e1 * e1 + 2 * e2 - 1);
			else
				result = ((e1 + 1) * (e1 + 1) - 1);

			return result;
		}

		Index2 TileIndex2D(int e0, int e1, int e2, int e3)
		{
			Index2 result;
			result.x = TileIndex1D(e3, e1);
			result.y = TileIndex1D(e2, e0);

			return result;
		}

		private ColorImage2D CreateTileImage(WangTile[,] tiling)
		{

			int numTilesX = tiling.GetLength(0);
			int numTilesY = tiling.GetLength(1);

			int width = TileSize * numTilesX;
			int height = TileSize * numTilesY;

			var image = new ColorImage2D(width, height);

			for (int x = 0; x < numTilesX; x++)
			{
				for (int y = 0; y < numTilesY; y++)
				{
					var tile = tiling[x, y];

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
			int numTilesX = tiling.GetLength(0);
			int numTilesY = tiling.GetLength(1);
			
			int width = TileSize * numTilesX;
			int height = TileSize * numTilesY;
			
			var image = new ColorImage2D(width, height);

			for (int x = 0; x < numTilesX; x++)
			{
				for (int y = 0; y < numTilesY; y++)
				{
					var idx = tiling[x, y];
					var tile = Tiles2[idx.x, idx.y];

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

		private ColorImage2D CreateMappingImage(WangTile[,] tiling)
		{

			int width = tiling.GetLength(0);
			int height = tiling.GetLength(1);

			var image = new ColorImage2D(width, height);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int e0 = tiling[x, y].Edges[0];
					int e1 = tiling[x, y].Edges[1];
					int e2 = tiling[x, y].Edges[2];
					int e3 = tiling[x, y].Edges[3];

					var idx = TileIndex2D(e0, e1, e2, e3);

					Console.WriteLine(idx);

					image[x, y] = new ColorRGB(idx.x / 255.0f, idx.y / 255.0f, 0);
				}
			}

			return image;
		}

		private ColorImage2D CreateMappingImage(Index2[,] tiling)
		{

			int width = tiling.GetLength(0);
			int height = tiling.GetLength(1);

			var image = new ColorImage2D(width, height);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var idx = tiling[x, y];
	
					image[x, y] = new ColorRGB(idx.x / 255.0f, idx.y / 255.0f, 0);
				}
			}

			return image;
		}


	}
}
