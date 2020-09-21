﻿using System;
using System.Collections.Generic;

using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Samplers
{
    public class IntensitySampler2D : ImageSampler2D
    {

        public IntensitySampler2D(IImageSampler2D image)
        {
            Image = image;
        }

        public IImageSampler2D Image { get; private set; }

        public override ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return new ColorRGB(Image.GetPixel(x, y, mode).Intensity);
        }

        public override ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return new ColorRGB(Image.GetPixel(u, v, mode).Intensity);
        }

    }
}