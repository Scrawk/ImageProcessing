using System;
using System.Collections.Generic;

using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{
    public class HSVSampler2D : ImageSampler2D
    {

        public HSVSampler2D(IImageSampler2D image)
        {
            Image = image;
        }

        public IImageSampler2D Image { get; private set; }

        public override ColorRGBA GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var hsv = Image.GetPixel(x, y, mode).hsv;
            return new ColorRGBA(hsv.h, hsv.s, hsv.v, 1);
        }

        public override ColorRGBA GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var hsv = Image.GetPixel(u, v, mode).hsv;
            return new ColorRGBA(hsv.h, hsv.s, hsv.v, 1);
        }

    }
}
