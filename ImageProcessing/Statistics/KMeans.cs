using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

using ImageProcessing.Images;
using ImageProcessing.Pixels;

namespace ImageProcessing.Statistics
{
    public class KMeansCluster
    {
        public KMeansCluster(int index)
        {
            Index = index;
            Set = new ColorPixelSet2D();
        }

        public KMeansCluster(int index, ColorRGBA mean)
        {
            Index = index;
            Mean = mean;
            Set = new ColorPixelSet2D();
        }

        public int Index { get; private set; }

        public ColorRGBA Mean { get; internal set; }

        public ColorPixelSet2D Set { get; private set; }

        public override string ToString()
        {
            return string.Format("[KMeansCluster: Mean={0}, Count={1}]", Mean, Set.Count);
        }
    }

    public class KMeans
    {

        public KMeans()
        {
            Clusters = new List<KMeansCluster>();
        }

        public List<KMeansCluster> Clusters { get; private set; }

        public void Run(ColorImage2D image, int k, int seed, bool weighted = true)
        {
            if (k > image.Size.Product)
                throw new Exception("K must be equal or less than image size");

            if(weighted)
                InitializeClustersWeighted(image, k, seed);
            else
                InitializeClustersRandom(image, k, seed);

            InitialAssignment(image);
            UpdateClusterMeans();

            while (UpdateAssignment())
                UpdateClusterMeans();
        }

        private void InitializeClustersRandom(ColorImage2D image, int k, int seed)
        {
            var rnd = new Random(seed);

            for(int i = 0; i < k; i++)
            {
                var cluster = new KMeansCluster(i);

                int x = rnd.Next(0, image.Width);
                int y = rnd.Next(0, image.Height);
                cluster.Mean = image[x, y];

                Clusters.Add(cluster);
            }
        }

        private void InitializeClustersWeighted(ColorImage2D image, int k, int seed)
        {
            var rnd = new Random(seed);
            int x = rnd.Next(0, image.Width);
            int y = rnd.Next(0, image.Height);

            Clusters.Add(new KMeansCluster(0, image[x, y]));

            var weights = new GreyScaleImage2D(image.Size);

            for (int i = 1; i < k; i++)
            {
                UpdateWeights(image, weights);

                double t = rnd.NextDouble();
                var col = ChoosePixel(image, weights, t);

                Clusters.Add(new KMeansCluster(i, col));
            }

        }

        private void UpdateWeights(ColorImage2D image, GreyScaleImage2D weights)
        {
            float sum = 0;
            weights.Fill((x, y) =>
            {
                var col = image[x, y];
                var closest = Closest(col);

                float d = ColorRGBA.SqrDistance(closest.Mean, col);
                sum += d;
                return d;
            });

            weights.Modify( x => x / sum);
        }

        private ColorRGBA ChoosePixel(ColorImage2D image, GreyScaleImage2D weights, double t)
        {
            double sum = 0;
            var size = image.Size;
   
            for(int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    double w = weights[x, y];

                    if (t >= sum && t < sum + w)
                        return image[x, y];

                    sum += w;
                }
            }

            return image[size - 1];
        }

        private void InitialAssignment(ColorImage2D image)
        {
            image.Iterate((x, y) =>
            {
                var col = image[x, y];

                var cluster = Closest(col);
                if (cluster == null)
                    throw new NullReferenceException("Cluster is null");

                int i = cluster.Index;

                cluster.Set.Add(new PixelIndex2D<ColorRGBA>(x, y, col, i));
            });
        }

        private bool UpdateAssignment()
        {
            var clusters = new List<KMeansCluster>();

            foreach (var c in Clusters)
                clusters.Add(new KMeansCluster(c.Index, c.Mean));
            
            bool changed = false;

            foreach (var cluster in Clusters)
            {
                for(int i = 0; i < cluster.Set.Count; i++)
                {
                    var pixel = cluster.Set.Pixels[i];
                    var closest = Closest(pixel.Value);
                    int j = closest.Index;

                    if (j != pixel.Tag)
                        changed = true;

                    pixel.Tag = j;
                    clusters[j].Set.Add(pixel);
                }
            }

            Clusters = clusters;

            return changed;
        }

        private void UpdateClusterMeans()
        {
            foreach(var cluster in Clusters)
                cluster.Mean = cluster.Set.Mean();
        }

        private KMeansCluster Closest(ColorRGBA col)
        {
            KMeansCluster closest = null;
            float dist = float.PositiveInfinity;

            foreach(var cluster in Clusters)
            {
                float d2 = ColorRGBA.SqrDistance(cluster.Mean, col);
                if(d2 < dist)
                {
                    dist = d2;
                    closest = cluster;
                }
            }

            return closest;
        }

    }
}
