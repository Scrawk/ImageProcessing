using System;
using System.Collections.Generic;
using System.Text;

using Common.Core.Numerics;

namespace ImageProcessing.Pixels
{
    public class PixelSet2D<T>
    {

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
    }
}
