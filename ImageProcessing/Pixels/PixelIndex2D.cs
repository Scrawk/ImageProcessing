using System;
using System.Collections.Generic;

using Common.Core.Numerics;

namespace ImageProcessing.Pixels
{
    public struct PixelIndex2D<T>
    {
        public Point2i Index;

        public T Value;

        public int Tag;

        public PixelIndex2D(int x, int y, T value)
        {
            Index = new Point2i(x, y);
            Value = value;
            Tag = 0;
        }

        public PixelIndex2D(int x, int y, T value, int tag)
        {
            Index = new Point2i(x, y);
            Value = value;
            Tag = tag;
        }

        public override string ToString()
        {
            return string.Format("[PixelIndex2D: Index={0}, Value={1}]", Index, Value);
        }
    }
}
