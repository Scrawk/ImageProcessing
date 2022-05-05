using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

using ImageProcessing.Images;
using ImageProcessing.Pixels;

namespace ImageProcessing.Statistics
{
    /// <summary>
    /// 
    /// </summary>
    public class KMeansCluster
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public KMeansCluster(int index)
        {
            Index = index;
            Set = new ColorPixelSet2D();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="mean"></param>
        public KMeansCluster(int index, ColorRGBA mean)
        {
            Index = index;
            Mean = mean;
            Set = new ColorPixelSet2D();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ColorRGBA Mean { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public ColorPixelSet2D Set { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[KMeansCluster: Mean={0}, Count={1}]", Mean, Set.Count);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class KMeans
    {
        /// <summary>
        /// 
        /// </summary>
        public KMeans()
        {
            Clusters = new List<KMeansCluster>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<KMeansCluster> Clusters { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="k"></param>
        /// <param name="seed"></param>
        /// <param name="weighted"></param>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="k"></param>
        /// <param name="seed"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="k"></param>
        /// <param name="seed"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="weights"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="weights"></param>
        /// <param name="t"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <exception cref="NullReferenceException"></exception>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        private void UpdateClusterMeans()
        {
            foreach(var cluster in Clusters)
                cluster.Mean = cluster.Set.CalculateMean();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
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
