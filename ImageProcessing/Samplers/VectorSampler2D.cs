using System;
using System.Collections.Generic;

using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{
    public class VectorSampler2D : ImageSampler2D
    {

        public VectorSampler2D(IImageSampler2D image, float scale = 1)
        {
            Image = image;
            Scale = scale;
        }

        public IImageSampler2D Image { get; private set; }

        public float Scale { get; private set; }

        public override ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var vec = Image.GetPixel(x, y, mode) * Scale;
            return new ColorRGB(vec.r * 0.5f + 0.5f, vec.g * 0.5f + 0.5f, 0);
        }

        public override ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var vec = Image.GetPixel(u, v, mode) * Scale;
            return new ColorRGB(vec.r * 0.5f + 0.5f, vec.g * 0.5f + 0.5f, 0);
        }

    }
}
