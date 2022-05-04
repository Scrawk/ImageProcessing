using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{
    public class ChannelSampler2D : ImageSampler2D
    {

        public ChannelSampler2D(IImageSampler2D image, int channel)
        {
            Image = image;
            Channel = channel;
        }

        public IImageSampler2D Image { get; private set; }

        public int Channel { get; set; }

        public override ColorRGBA GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            int c = MathUtil.Clamp(Channel, 0, 2);
            return new ColorRGBA(Image.GetPixel(x, y, mode)[c]);
        }

        public override ColorRGBA GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            int c = MathUtil.Clamp(Channel, 0, 2);
            return new ColorRGBA(Image.GetPixel(u, v, mode)[c]);
        }

    }
}
