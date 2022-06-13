using System;
using System.Collections.Generic;
using System.Text;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Images
{
    public interface IData2D
    {
        ColorRGBA GetPixel(int x, int y);

        void SetPixel(int x, int y, ColorRGBA pixel);   

        float GetChannel(int x, int y, int c);

        void SetChannel(int x, int y, int c, float v);

        int Channels { get; }

        int Width { get; }

        int Height { get; }

        void Clear();
    }
}
