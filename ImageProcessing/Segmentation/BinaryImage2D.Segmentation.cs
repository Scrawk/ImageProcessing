﻿using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Directions;
using Common.Collections.Sets;
using Common.GraphTheory.AdjacencyGraphs;

using ImageProcessing.Pixels;

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
		public PixelSegmentation2D<bool> Segmentation()
        {
			var segmentation = new PixelSegmentation2D<bool>(this);

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

					var root = set.FindParent(x, y);
					var pixel = new PixelIndex2D<bool>(x, y, true);

					segmentation.AddPixel(root, pixel);
				}
			}

			return segmentation;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public GraphForest MinimumSpanningForest(Func<Vector2i, Vector2i, float> weights = null)
        {
			if (weights == null)
				weights = (a, b) => (float)Vector2i.Distance(a, b);

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
				var a = graph.Vertices[j].Index;
				var idx = pixel.Index;

				for (int i = 0; i < 8; i++)
				{
					int xi = idx.x + D8.OFFSETS[i, 0];
					int yi = idx.y + D8.OFFSETS[i, 1];

					if (xi < 0 || xi >= Width) continue;
					if (yi < 0 || yi >= Height) continue;
					var idx2 = new Vector2i(xi, yi);

					if (table.TryGetValue(idx2, out int b) && !graph.ContainsEdge(a, b))
                    {
						float weight = weights(idx, idx2);
						graph.AddEdge(a, b, weight);
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

			var colors = PixelSegmentation2D.SegmentationColors(seed, roots);
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
