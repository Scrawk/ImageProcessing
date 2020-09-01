using System;
using System.Collections.Generic;

using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{
    public interface IImageSampler2D
    {
        ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP);

        ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP);
    }
}
