using System;
using System.Collections.Generic;

using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{
    public class ScaleSampler2D : ImageSampler2D
    {

        public ScaleSampler2D(IImageSampler2D image, float scale)
        {
            Image = image;
            Scale = scale;
        }

        public IImageSampler2D Image { get; private set; }

        public float Scale { get; set; }

        public override ColorRGBA GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return (Image.GetPixel(x, y, mode).rgb * Scale).rgb1;
        }

        public override ColorRGBA GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return (Image.GetPixel(u, v, mode).rgb * Scale).rgb1;
        }

    }
}
