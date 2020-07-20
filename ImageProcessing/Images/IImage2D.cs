using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Collections.Arrays;

namespace ImageProcessing.Images
{
    /// <summary>
    /// General interface for a 2 dimensional image.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    public interface  IImage2D<T> : IArray2<T>
    {
        List<PixelIndex2D<T>> ToPixelIndexList();
    }
}
