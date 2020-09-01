using System;
using Common.Core.Colors;

using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{
    public class ConstSampler2D : ImageSampler2D
    {

        public ConstSampler2D(ColorRGB color)
        {
            Color = color;
        }

        public ConstSampler2D(float value)
        {
            Color = new ColorRGB(value);
        }

        public ColorRGB Color;

        public override ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return Color;
        }

        public override ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return Color;
        }
    }
}
