using System;
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
		public static PixelSegmentation2D<bool> Segmentation(BinaryImage2D image)
        {
			var segmentation = new PixelSegmentation2D<bool>(image);

			var set = new DisjointGridSet2(image.Width, image.Height);

			for(int y = 0; y < image.Height; y++)
            {
				for (int x = 0; x < image.Width; x++)
				{
					if (!image[x, y]) continue;
					set.Add(x, y, x, y);
				}
			}

			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					if (!image[x, y]) continue;
					
					for(int i = 0; i < 8; i++)
                    {
						int xi = x + D8.OFFSETS[i, 0];
						int yi = y + D8.OFFSETS[i, 1];

						if (image.NotInBounds(xi, yi)) continue;
						if (!image[xi, yi]) continue;

						set.Union(xi, yi, x, y);
					}
				}
			}

			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					if (!image[x, y]) continue;

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
		public static GraphForest MinimumSpanningForest(BinaryImage2D image, Func<Point2i, Point2i, float> weights = null)
        {
			if (weights == null)
				weights = (a, b) => (float)Point2i.Distance(a, b);

			var pixels = new List<Point2i>();
			image.ToIndexList(pixels, (v) => v == true);

			var table = new Dictionary<Point2i, int>();
			var graph = new UndirectedGraph(pixels.Count);
			for (int i = 0; i < pixels.Count; i++)
			{
				var pixel = pixels[i];

				graph.Vertices[i].Data = pixel;
				table.Add(pixel, i);
			}
			
			for (int j = 0; j < pixels.Count; j++)
            {
				var pixel = pixels[j];
				var a = graph.Vertices[j].Index;

				for (int i = 0; i < 8; i++)
				{
					int xi = pixel.x + D8.OFFSETS[i, 0];
					int yi = pixel.y + D8.OFFSETS[i, 1];

					if (image.NotInBounds(xi, yi)) continue;
					var idx2 = new Point2i(xi, yi);

					if (table.TryGetValue(idx2, out int b) && !graph.ContainsEdge(a, b))
                    {
						float weight = weights(pixel, idx2);
						graph.AddUndirectedEdge(a, b, weight);
                    }
				}
			}

			return graph.KruskalsMinimumSpanningForest();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="image"></param>
		/// <param name="seed"></param>
		/// <param name="forest"></param>
		/// <returns></returns>
		public static ColorImage2D ColorizeForest(BinaryImage2D image, int seed, GraphForest forest)
		{
			var roots = new List<Point2i>();
			foreach(var tree in forest.Trees)
            {
				int root = tree.Root;
				var pixel = tree.Graph.GetVertexData<PixelIndex2D<bool>>(root);
				roots.Add(pixel.Index);
            }

			var colors = SegmentationColors.Generate(seed, roots);
			var pixels = new List<PixelIndex2D<bool>>();
			var image2 = new ColorImage2D(image.Size);

			foreach (var tree in forest.Trees)
			{
				int root = tree.Root;
				var pixel = tree.Graph.GetVertexData<PixelIndex2D<bool>>(root);
				var color = colors[pixel.Index];

				pixels.Clear();
				tree.GetData(pixels);

				foreach (var p in pixels)
					image2[p.Index] = color;

			}

			return image2;
		}


	}
}
