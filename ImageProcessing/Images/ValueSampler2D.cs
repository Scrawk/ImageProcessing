using System;
using System.Collections.Generic;

using Common.Core.Colors;

namespace ImageProcessing.Images
{
    public class ValueSampler2D<T> : ImageSampler2D
    {

        public ValueSampler2D(IImage2D<T> image, float scale)
        {
            Image = image;
            Scale = scale;
        }

        public IImage2D<T> Image { get; private set; }

        public float Scale { get; private set; }

        public override ColorRGB GetPixelRGB(int x, int y)
        {
            return Image.GetPixelRGB(x, y) * Scale;
        }

    }
}
