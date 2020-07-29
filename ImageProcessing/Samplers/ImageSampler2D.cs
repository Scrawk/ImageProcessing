using System;
using System.Collections.Generic;

using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{

    public abstract class ImageSampler2D : IImageSampler2D
    {
        public abstract ColorRGB GetPixel(int x, int y);

        public abstract ColorRGB GetPixel(float u, float v);
    }
}
