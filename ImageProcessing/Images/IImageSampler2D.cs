using System;
using System.Collections.Generic;

using Common.Core.Colors;

namespace ImageProcessing.Images
{
    public interface IImageSampler2D
    {
        ColorRGB GetPixelRGB(int x, int y);
    }
}
