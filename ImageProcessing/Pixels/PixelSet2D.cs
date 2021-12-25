using System;
using System.Collections.Generic;
using System.Text;

using Common.Core.Numerics;
using Common.Geometry.Shapes;

namespace ImageProcessing.Pixels
{

    public class PixelSet2D<T> : IComparable<PixelSet2D<T>>
    {

        public PixelSet2D()
        {
            Pixels = new List<PixelIndex2D<T>>();
        }

        public PixelSet2D(List<PixelIndex2D<T>> pixels)
        {
            Pixels = new List<PixelIndex2D<T>>(pixels);
        }

        public int Count => Pixels.Count;

        public List<PixelIndex2D<T>> Pixels { get; private set; }

        public override string ToString()
        {
            return string.Format("[PixelSet2D: Count={0}]", Count);
        }

        public void Add(PixelIndex2D<T> pixel)
        {
            Pixels.Add(pixel);
        }

        public int CompareTo(PixelSet2D<T> other)
        {
            return Pixels.Count.CompareTo(other.Pixels.Count);
        }

        public float Area()
        {
            return Pixels.Count;
        }

        public Box2i Bounds()
        {
            Point2i min = Point2i.MaxValue;
            Point2i max = Point2i.MinValue;

            for(int i = 0; i < Pixels.Count; i++)
            {
                var idx = Pixels[i].Index;
                if (idx.x < min.x) min.x = idx.x;
                if (idx.y < min.y) min.y = idx.y;
                if (idx.x > max.x) max.x = idx.x;
                if (idx.y > max.y) max.y = idx.y;
            }

            return new Box2i(min, max);
        }

        public float Roundness(float perimeter)
        {
            float area = Area();
            float p2 = perimeter * perimeter;
            float pi = MathUtil.PI_32;

            return MathUtil.Clamp01(pi * 4.0f * area / p2);
        }

        public Point2f Centroid()
        {
            Point2f centroid = new Point2f();

            for (int i = 0; i < Pixels.Count; i++)
            {
                var idx = Pixels[i].Index;
                centroid.x += idx.x;
                centroid.y += idx.y;
            }

            return centroid / Count;
        }

    }
}
