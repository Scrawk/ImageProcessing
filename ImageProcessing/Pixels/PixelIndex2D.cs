using System;
using System.Collections.Generic;

using Common.Core.Numerics;

namespace ImageProcessing.Pixels
{
    public struct PixelIndex2D<T>
    {
        public int x, y;

        public T Value;

        public int Tag;

        public PixelIndex2D(int x, int y, T value)
        {
            this.x = x;
            this.y = y;
            Value = value;
            Tag = 0;
        }

        public PixelIndex2D(int x, int y, T value, int tag)
        {
            this.x = x;
            this.y = y;
            Value = value;
            Tag = tag;
        }

        public override string ToString()
        {
            return string.Format("[PixelIndex2D: x={0}, y={1}, Value={2}]", x, y, Value);
        }
    }
}
