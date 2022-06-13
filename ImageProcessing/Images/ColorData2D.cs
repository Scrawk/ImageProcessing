using System;
using System.Collections.Generic;
using System.Text;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Extensions;

namespace ImageProcessing.Images
{
    public class ColorData2D : IData2D
    {

        private ColorRGBA[,] data;

        public ColorData2D(int width, int height)
        {
            data = new ColorRGBA[width, height];
        }

        public int Channels => 4;

        public int Width => data.GetLength(0);

        public int Height => data.GetLength(1);

        public void Clear()
        {
            data.Fill(ColorRGBA.Black);
        }

        public ColorRGBA GetPixel(int x, int y)
        {
            return data[x, y];
        }

        public void SetPixel(int x, int y, ColorRGBA pixel)
        {
            data[x, y] = pixel;
        }

        public float GetChannel(int x, int y, int c)
        {
            return data[x, y][c];
        }

        public void SetChannel(int x, int y, int c, float v)
        {
            data[x, y][c] = v;
        }
    }
}
