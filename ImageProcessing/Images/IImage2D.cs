﻿using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Geometry.Shapes;
using Common.Collections.Arrays;

namespace ImageProcessing.Images
{
    /// <summary>
    /// General interface for a 2 dimensional image.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    public interface  IImage2D<T> : IArray2<T>, IImageSampler2D
    {
        float GetValue(int x, int y);

        //ColorRGB GetPixel(int x, int y);

        void SetPixel(int x, int y, ColorRGB pixel);

        List<PixelIndex2D<T>> ToPixelIndexList(Func<T, bool> predicate);

        void Fill(IList<PixelIndex2D<T>> points);
    }
}
