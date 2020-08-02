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

        public PixelSet2D(Vector2i root)
        {
            Root = root;
            Pixels = new List<PixelIndex2D<T>>();
        }

        public PixelSet2D(Vector2i root, List<PixelIndex2D<T>> pixels)
        {
            Root = root;
            Pixels = pixels;
        }

        public Vector2i Root { get; private set; }

        public List<PixelIndex2D<T>> Pixels { get; private set; }

        public int CompareTo(PixelSet2D<T> other)
        {
            return Pixels.Count.CompareTo(other.Pixels.Count);
        }

        public float CalculateArea()
        {
            return Pixels.Count;
        }

        public Box2i CalculateBounds()
        {
            Vector2i min = Vector2i.MaxInt;
            Vector2i max = Vector2i.MinInt;

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

        public float CalculateRoundness(float perimeter)
        {
            float area = CalculateArea();
            float p2 = perimeter * perimeter;
            float pi = MathUtil.PI;

            return MathUtil.Clamp01(pi * 4.0f * area / p2);
        }

        public Vector2f CalculateCentroid()
        {
            Vector2f centroid = new Vector2f();

            for (int i = 0; i < Pixels.Count; i++)
            {
                var idx = Pixels[i].Index;
                centroid.x += idx.x;
                centroid.y += idx.y;
            }

            return centroid / Pixels.Count;
        }


    }
}
