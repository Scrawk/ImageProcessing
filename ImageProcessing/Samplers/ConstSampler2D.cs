using System;
using Common.Core.Colors;

using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{
    public class ConstSampler2D : ImageSampler2D
    {

        public ConstSampler2D(ColorRGBA color)
        {
            Color = color;
        }

        public ConstSampler2D(float value)
        {
            Color = new ColorRGBA(value, 1);
        }

        public ColorRGBA Color;

        public override ColorRGBA GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return Color;
        }

        public override ColorRGBA GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return Color;
        }
    }
}
