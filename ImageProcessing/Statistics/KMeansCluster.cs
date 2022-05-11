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
}
