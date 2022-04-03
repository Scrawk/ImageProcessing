using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Collections.Arrays;

using ImageProcessing.Samplers;
using ImageProcessing.Pixels;

namespace ImageProcessing.Images
{

    /// <summary>
    /// Wrap mode options for when sampling a image.
    /// </summary>
    public enum WRAP_MODE {  CLAMP, WRAP, MIRROR, NONE };

    /// <summary>
    /// General interface for a 2 dimensional image.
    /// </summary>
    public interface  IImage2D : IImageSampler2D
    {
        int Width { get; }

        int Height { get; }

        new ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP);

        new ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP);

        void SetPixel(int x, int y, ColorRGB pixel, WRAP_MODE mode = WRAP_MODE.NONE);

        void SetPixel(int x, int y, ColorRGBA pixel, WRAP_MODE mode = WRAP_MODE.NONE);

    }
}
