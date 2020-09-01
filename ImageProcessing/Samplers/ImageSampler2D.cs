﻿using System;
using System.Collections.Generic;

using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{

    public abstract class ImageSampler2D : IImageSampler2D
    {
        public abstract ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP);

        public abstract ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP);
    }
}
