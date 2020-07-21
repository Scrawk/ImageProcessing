using System;
using System.Collections.Generic;

using Common.Core.Colors;

namespace ImageProcessing.Images
{

    public abstract class ImageSampler2D : IImageSampler2D
    {
        public abstract ColorRGB GetPixelRGB(int x, int y);
    }
}
