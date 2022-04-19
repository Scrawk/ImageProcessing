using System;
using System.Collections.Generic;
using System.Text;

using Common.Core.Numerics;
using Common.Core.Shapes;

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
                var x = Pixels[i].x;
                var y = Pixels[i].y;

                if (x < min.x) min.x = x;
                if (y < min.y) min.y = y;
                if (x > max.x) max.x = x;
                if (y > max.y) max.y = y;
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
                centroid.x += Pixels[i].x;
                centroid.y += Pixels[i].y;
            }

            return centroid / Count;
        }

    }
}
