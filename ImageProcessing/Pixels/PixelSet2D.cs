using System;
using System.Collections.Generic;
using System.Text;

using Common.Core.Numerics;
using Common.Core.Shapes;

namespace ImageProcessing.Pixels
{
    /// <summary>
    /// A set of pixel indices.
    /// Can be used to calculate some proerties of the set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PixelSet2D<T> : IComparable<PixelSet2D<T>>
    {
        /// <summary>
        /// /
        /// </summary>
        public PixelSet2D()
        {
            Pixels = new List<PixelIndex2D<T>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixels"></param>
        public PixelSet2D(List<PixelIndex2D<T>> pixels)
        {
            Pixels = new List<PixelIndex2D<T>>(pixels);
        }

        /// <summary>
        /// The number of pixels in the set.
        /// </summary>
        public int Count => Pixels.Count;

        /// <summary>
        /// 
        /// </summary>
        public List<PixelIndex2D<T>> Pixels { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[PixelSet2D: Count={0}]", Count);
        }

        /// <summary>
        /// Add a new pixel index to the set.
        /// </summary>
        /// <param name="pixel"></param>
        public void Add(PixelIndex2D<T> pixel)
        {
            Pixels.Add(pixel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PixelSet2D<T> other)
        {
            return Pixels.Count.CompareTo(other.Pixels.Count);
        }

        /// <summary>
        /// Calculate the bounding box of the set.
        /// </summary>
        /// <returns></returns>
        public Box2i CalculateBounds()
        {
            Point2i min = Point2i.MaxValue;
            Point2i max = Point2i.MinValue;

            for(int i = 0; i < Pixels.Count; i++)
            {
                var x = Pixels[i].x;
                var y = Pixels[i].y;

                if (x < min.x) min.x = x;
                if (y < min.y) min.y = y;
                if (x > max.x) max.x = x;
                if (y > max.y) max.y = y;
            }

            return new Box2i(min, max);
        }

        /// <summary>
        /// Calcuate the roundness of the set.
        /// </summary>
        /// <param name="perimeter"></param>
        /// <returns></returns>
        public float CalculateRoundness(float perimeter)
        {
            float area = Count;
            float p2 = perimeter * perimeter;
            float pi = MathUtil.PI_32;

            return MathUtil.Clamp01(pi * 4.0f * area / p2);
        }

        /// <summary>
        /// Calcuate the centriod of the set.
        /// </summary>
        /// <returns></returns>
        public Point2f CalculateCentroid()
        {
            Point2f centroid = new Point2f();

            for (int i = 0; i < Pixels.Count; i++)
            {
                centroid.x += Pixels[i].x;
                centroid.y += Pixels[i].y;
            }

            return centroid / Count;
        }

    }
}
