using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Collections.Arrays;

using ImageProcessing.Samplers;
using ImageProcessing.Pixels;

namespace ImageProcessing.Images
{

    public enum WRAP_MODE {  CLAMP, WRAP, MIRROR};

    /// <summary>
    /// General interface for a 2 dimensional image.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    public interface  IImage2D<T> : IArray2<T>, IImageSampler2D
    {
        float GetValue(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP);

        float GetValue(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP);

        //ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP);

        //ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP);

        void SetPixel(int x, int y, ColorRGB pixel);

    }
}
