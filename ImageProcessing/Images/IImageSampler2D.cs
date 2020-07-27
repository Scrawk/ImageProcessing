using System;
using System.Collections.Generic;

using Common.Core.Colors;

namespace ImageProcessing.Images
{
    public interface IImageSampler2D
    {
        ColorRGB GetPixel(int x, int y);

        ColorRGB GetPixel(float u, float v);
    }
}
