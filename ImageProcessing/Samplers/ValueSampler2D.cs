using System;
using System.Collections.Generic;

using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Samplers
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

        public override ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return Image.GetPixel(x, y, mode) * Scale;
        }

        public override ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return Image.GetPixel(u, v, mode) * Scale;
        }

    }
}
