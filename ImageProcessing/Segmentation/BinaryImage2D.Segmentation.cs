using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Directions;
using Common.Collections.Sets;
using Common.GraphTheory.AdjacencyGraphs;

namespace ImageProcessing.Images
{
	/// <summary>
	/// 
	/// </summary>
	public partial class BinaryImage2D
	{

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<Vector2i, List<PixelIndex2D<bool>>> Segmentation()
        {
			var segments = new Dictionary<Vector2i, List<PixelIndex2D<bool>>>();

			var set = new DisjointGridSet2(Width, Height);

			for(int y = 0; y < Height; y++)
            {
				for (int x = 0; x < Width; x++)
				{
					if (!this[x, y]) continue;
					set.Add(x, y, x, y);
				}
			}

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (!this[x, y]) continue;
					
					for(int i = 0; i < 8; i++)
                    {
						int xi = x + D8.OFFSETS[i, 0];
						int yi = y + D8.OFFSETS[i, 1];

						if (xi < 0 || xi >= Width) continue;
						if (yi < 0 || yi >= Height) continue;
						if (!this[xi, yi]) continue;

						set.Union(xi, yi, x, y);
					}
				}
			}

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (!this[x, y]) continue;

					var p = set.FindParent(x, y);

					List<PixelIndex2D<bool>> list;
					if(!segments.TryGetValue(p, out list))
                    {
						list = new List<PixelIndex2D<bool>>();
						segments.Add(p, list);
                    }

					list.Add(new PixelIndex2D<bool>(x, y, this[x, y]));
				}
			}


			return segments;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		public static Dictionary<Vector2i, ColorRGB> SegmentationColors(int seed, IEnumerable<Vector2i> keys)
        {
			var rnd = new Random(seed);
			var colors = new Dictionary<Vector2i, ColorRGB>();
			
			foreach(var p in keys)
            {
				colors.TryAdd(p, rnd.NextColorRGB());
            }

			return colors;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="segmentation"></param>
		/// <returns></returns>
		public ColorImage2D ColorizeSegmentation(int seed, Dictionary<Vector2i, List<PixelIndex2D<bool>>> segmentation)
        {
			var colors = SegmentationColors(seed, segmentation.Keys);

			var image = new ColorImage2D(Width, Height);

			foreach (var kvp in segmentation)
			{
				var idx = kvp.Key;
				var list = kvp.Value;
				var color = colors[idx];

				foreach (var p in list)
					image[p.Index] = color;
			}

			return image;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public GraphForest MinimumSpanningForest()
        {
			var pixels = ToPixelIndexList((v) => v == true);

			var table = new Dictionary<Vector2i, int>();
			var graph = new UndirectedGraph(pixels.Count);

			for (int i = 0; i < pixels.Count; i++)
			{
				var pixel = pixels[i];

				graph.Vertices[i].Data = pixel;
				table.Add(pixel.Index, i);
			}
			
			for (int j = 0; j < pixels.Count; j++)
            {
				var pixel = pixels[j];
				var from = graph.Vertices[j].Index;
				var idx = pixel.Index;

				for (int i = 0; i < 8; i++)
				{
					int xi = idx.x + D8.OFFSETS[i, 0];
					int yi = idx.y + D8.OFFSETS[i, 1];

					if (xi < 0 || xi >= Width) continue;
					if (yi < 0 || yi >= Height) continue;

					if (table.TryGetValue((xi, yi), out int to) && !graph.ContainsEdge(from, to))
                    {
						float w = Vector2f.Distance(idx, (xi, yi));
						graph.AddEdge(from, to, w, w);
                    }
				}
			}

			return graph.KruskalsMinimumSpanningForest();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="segmentation"></param>
		/// <returns></returns>
		public ColorImage2D ColorizeForest(int seed, GraphForest forest)
		{
			var roots = new List<Vector2i>();
			foreach(var tree in forest.Trees)
            {
				int root = tree.Root;
				var pixel = tree.Graph.GetVertexData<PixelIndex2D<bool>>(root);
				roots.Add(pixel.Index);
            }

			var colors = SegmentationColors(seed, roots);
			var pixels = new List<PixelIndex2D<bool>>();
			var image = new ColorImage2D(Width, Height);

			foreach (var tree in forest.Trees)
			{
				int root = tree.Root;
				var pixel = tree.Graph.GetVertexData<PixelIndex2D<bool>>(root);
				var color = colors[pixel.Index];

				pixels.Clear();
				tree.GetData(pixels);

				foreach (var p in pixels)
					image[p.Index] = color;

			}

			return image;
		}


	}
}
