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

		private WangTile[,] Tiles { get; set; }

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
			var tilables = new List<ColorImage2D>();

			for (int i = 0; i < exemplars.Count; i++)
			{
				var exemplar = exemplars[i];

				var tileable = ImageSynthesis.MakeTileable(exemplar.Image, exemplarSet);

				tilables.Add(tileable);

				tileable.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/tileable" + i + ".raw");
			}

			Console.WriteLine(this);
			Console.WriteLine(exemplarSet);

			CreateTiles();

			foreach(var tile in Tiles)
			{
				tile.CreateMap();
				tile.CreateMask();
				tile.FillImage(tilables);

				Console.WriteLine("Creating " + tile);

				ImageSynthesis.CreateTileImage(tile, exemplarSet);

				if (addEdgeColors)
					tile.ColorEdges(4, Colors, 0.25f);
			}

		}

		public ColorImage2D CreateTileImage()
		{
			return CreateTileImage(Tiles, false);
		}

		public ColorImage2D CreateTileImage(int numHTiles, int numVTiles, int seed)
		{
			var tiling = SequentialTiling(numHTiles, numVTiles, seed);
			return CreateTileImage(tiling, true);
		}

		public ColorImage2D CreateTileMappingImage(int numHTiles, int numVTiles, int seed)
		{
			var tiling = SequentialTiling(numHTiles, numVTiles, seed);
			return CreateMappingImage(tiling, false);
		}

		private void CreateTiles()
		{

			Tiles = new WangTile[NumHColors * NumHColors, NumVColors * NumVColors];

			for (int sEdge = 0; sEdge < NumVColors; sEdge++)
			{
				for (int eEdge = 0; eEdge < NumHColors; eEdge++)
				{
					for (int nEdge = 0; nEdge < NumVColors; nEdge++)
					{
						for (int wEdge = 0; wEdge < NumHColors; wEdge++)
						{
							var tile = new WangTile(sEdge, eEdge, nEdge, wEdge, TileSize);

							var index = TileIndex2D(sEdge, eEdge, nEdge, wEdge);
							tile.Index = index;
		
							Tiles[index.x, index.y] = tile;
						}
					}
				}
			}

		}

		private WangTile[,] SequentialTiling(int numHTiles, int numVTiles, int seed)
		{
			var tiles = new WangTile[numHTiles, numVTiles];

			var rnd = new Random(seed);

			for (int y = 0; y < numVTiles; y++)
			{
				for (int x = 0; x < numHTiles; x++)
				{

					int sEdge = rnd.Next(0, NumVColors);
					int eEdge = rnd.Next(0, NumHColors);
					int nEdge = rnd.Next(0, NumVColors);
					int wEdge = rnd.Next(0, NumHColors);

					if(tiles.GetWrapped(x - 1, y) != null)
						wEdge = tiles.GetWrapped(x - 1, y).eEdge;

					if (tiles.GetWrapped(x, y + 1) != null)
						nEdge = tiles.GetWrapped(x, y + 1).sEdge;

					if (tiles.GetWrapped(x + 1, y) != null)
						eEdge = tiles.GetWrapped(x + 1, y).wEdge;

					if (tiles.GetWrapped(x, y - 1) != null)
						sEdge = tiles.GetWrapped(x, y - 1).nEdge;

					var index = TileIndex2D(sEdge, eEdge, nEdge, wEdge);
					var tile = Tiles[index.x, index.y];

					tiles[x, y] = tile;
				}
			}

			/*
			for (int y = 0; y < numVTiles; y++)
			{
				for (int x = 0; x < numHTiles; x++)
				{
					var tile = tiles[x, y];
					if (tile == null) continue;

					Console.WriteLine("(" + x + "," + y + ")" + tile);
					Console.WriteLine(tile.wEdge + " : " + tiles.GetWrapped(x - 1, y).eEdge);
					Console.WriteLine(tile.eEdge + " : " + tiles.GetWrapped(x + 1, y).wEdge);
					Console.WriteLine(tile.nEdge + " : " + tiles.GetWrapped(x, y + 1).sEdge);
					Console.WriteLine(tile.sEdge + " : " + tiles.GetWrapped(x, y - 1).nEdge);
				}
			}
			*/

			return tiles;
		}

		private int TileIndex1D(int e1, int e2)
		{
			int result;

			if (e1 < e2)
				result = (2 * e1 + e2 * e2);
			else if (e1 == e2)
			{
				if (e1 > 0)
					result = ((e1 + 1) * (e1 + 1) - 2);
				else 
					result = 0;
			}
			else if (e2 > 0)
				result = (e1 * e1 + 2 * e2 - 1);
			else
				result = ((e1 + 1) * (e1 + 1) - 1);

			return result;
		}

		private Index2 TileIndex2D(int e0, int e1, int e2, int e3)
		{
			Index2 result;
			result.x = TileIndex1D(e3, e1);
			result.y = TileIndex1D(e2, e0);

			return result;
		}

		private ColorImage2D CreateTileImage(WangTile[,] tiling, bool flipY)
		{

			int numTilesX = tiling.GetLength(0);
			int numTilesY = tiling.GetLength(1);

			int width = TileSize * numTilesX;
			int height = TileSize * numTilesY;

			var image = new ColorImage2D(width, height);

			if (flipY)
			{

				var tmp = new WangTile[numTilesX, numTilesY];

				for (int x = 0; x < numTilesX; x++)
				{
					for (int y = 0; y < numTilesY; y++)
					{
						tmp[x, numTilesY - 1 - y] = tiling[x, y];
					}
				}

				tiling = tmp;
			}

			for (int x = 0; x < numTilesX; x++)
			{
				for (int y = 0; y < numTilesY; y++)
				{
					var tile = tiling[x, y];
					if (tile == null) continue;

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

		private ColorImage2D CreateMappingImage(WangTile[,] tiling, bool flipY)
		{

			int width = tiling.GetLength(0);
			int height = tiling.GetLength(1);

			var image = new ColorImage2D(width, height);

			if (flipY)
			{
				var tmp = new WangTile[width, height];

				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						tmp[x, height - 1 - y] = tiling[x, y];
					}
				}

				tiling = tmp;
			}


			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					var tile = tiling[x, y];

					var idx = tile.Index;
					image[x, y] = new ColorRGB(idx.x / 255.0f, idx.y / 255.0f, 0);
				}
			}

			return image;
		}

	}
}
