using System;
using System.Collections.Generic;

using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{
    public class IntensitySampler2D : ImageSampler2D
    {

        public IntensitySampler2D(IImageSampler2D image)
        {
            Image = image;
        }

        public IImageSampler2D Image { get; private set; }

        public override ColorRGBA GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return new ColorRGBA(Image.GetPixel(x, y, mode).Intensity, 1);
        }

        public override ColorRGBA GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return new ColorRGBA(Image.GetPixel(u, v, mode).Intensity, 1);
        }

    }
}
