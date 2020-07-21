using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Images
{
    public struct PixelIndex2D<T>
    {
        public int x, y;
        public T value;

        public PixelIndex2D(int x, int y, T value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }
    }
}
