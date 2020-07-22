using System;
using System.Collections.Generic;

using Common.Core.Numerics;

namespace ImageProcessing.Images
{
    public struct PixelIndex2D<T>
    {
        public Vector2i Index;
        public T Value;

        public PixelIndex2D(int x, int y, T value)
        {
            Index = new Vector2i(x, y);
            Value = value;
        }
    }
}
