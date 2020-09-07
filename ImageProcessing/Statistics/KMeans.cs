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
        public int Index;
        public ColorRGB Mean;
        public ColorPixelSet2D Set;

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

        public void Run(ColorImage2D image, int k, int seed)
        {
            InitializeClusters(image, k, seed);
            InitialAssignment(image);
            UpdateClusterMeans();

            while (UpdateAssignment())
                UpdateClusterMeans();
        }

        private void InitializeClusters(ColorImage2D image, int k, int seed)
        {
            if (k > image.Size.Product)
                throw new Exception("K must be equal or less than image size");

            var rnd = new Random(seed);

            for(int i = 0; i < k; i++)
            {
                var cluster = new KMeansCluster();
                cluster.Index = i;
                cluster.Set = new ColorPixelSet2D();

                do
                {
                    int x = rnd.Next(0, image.Width);
                    int y = rnd.Next(0, image.Height);
                    cluster.Mean = image[x, y];
                }
                while (Contains(cluster.Mean));

                Clusters.Add(cluster);
            }

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

                cluster.Set.Add(new PixelIndex2D<ColorRGB>(x, y, col, i));
            });
        }

        private bool UpdateAssignment()
        {
            var clusters = new List<KMeansCluster>();

            foreach (var c in Clusters)
            {
                var cluster = new KMeansCluster();
                cluster.Index = c.Index;
                cluster.Mean = c.Mean;
                cluster.Set = new ColorPixelSet2D();
                clusters.Add(cluster);
            }

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

        private bool Contains(ColorRGB col)
        {
            foreach (var cluster in Clusters)
                if (cluster.Mean == col) return true;

            return false;
        }

        private KMeansCluster Closest(ColorRGB col)
        {
            KMeansCluster closest = null;
            float dist = float.PositiveInfinity;

            foreach(var cluster in Clusters)
            {
                float d2 = (cluster.Mean - col).SqrMagnitude;
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
