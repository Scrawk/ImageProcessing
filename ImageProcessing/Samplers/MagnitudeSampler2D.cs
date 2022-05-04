using System;
using System.Collections.Generic;

using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{
    public class MagnitudeSampler2D : ImageSampler2D
    {

        public MagnitudeSampler2D(IImageSampler2D image, float scale = 1)
        {
            Image = image;
            Scale = scale;
        }

        public IImageSampler2D Image { get; private set; }

        public float Scale { get; private set; }

        public override ColorRGBA GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return new ColorRGBA(Image.GetPixel(x, y, mode).Magnitude * Scale, 1);
        }

        public override ColorRGBA GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return new ColorRGBA(Image.GetPixel(u, v, mode).Magnitude * Scale, 1);
        }

    }
}
