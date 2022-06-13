using System;
using System.Collections.Generic;
using System.Text;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Extensions;

namespace ImageProcessing.Images
{
    public class ColorDataPlanar2D : IData2D
    {

        private float[][] data;

        private Func<int, int, float>[] getters;

        private Action<int, int, float>[] setters;

        public ColorDataPlanar2D(int width, int height, int channels)
        {
            Width = width;
            Height = height;
            Channels = channels;

            data = new float[channels][];
            data[0] = new float[width * height];

            if (channels > 1)
                data[1] = new float[width * height];

            if (channels > 2)
                data[2] = new float[width * height];

            if (channels > 3)
                data[3] = new float[width * height];

            CreateGetters();
            CreateSetters();
        }

        public int Channels { get; private set; }

        public int Width { get; private set; } 

        public int Height { get; private set; }

        public void Clear()
        {
            for(int i = 0; i < Channels; i++)
                data[i].Clear();
        }

        public ColorRGBA GetPixel(int x, int y)
        {
            int i = x + y * Width;
            float r = getters[0](i, 0);
            float g = getters[1](i, 1);
            float b = getters[2](i, 2);
            float a = getters[3](i, 3);

            return new ColorRGBA(r, g, b, a);
        }

        public void SetPixel(int x, int y, ColorRGBA pixel)
        {
            int i = x + y * Width;
            setters[0](i, 0, pixel.r);
            setters[1](i, 1, pixel.g);
            setters[2](i, 2, pixel.b);
            setters[3](i, 3, pixel.a);
        }

        public float GetChannel(int x, int y, int c)
        {
            int i = x + y * Width;
            return getters[c](i, c);
        }

        public void SetChannel(int x, int y, int c, float v)
        {
            int i = x + y * Width;
            setters[c](i, c, v);
        }

        private void CreateGetters()
        {
            getters = new Func<int, int, float>[4];
            getters[0] = GetChannel;

            if (Channels > 1)
                getters[1] = GetChannel;
            else
                getters[1] = GetChannelR;

            if (Channels > 2)
                getters[2] = GetChannel;
            else
            {
                if (Channels == 2)
                    getters[2] = GetZero;
                else
                    getters[2] = GetChannelR;
            }

            if (Channels > 3)
                getters[3] = GetChannel;
            else
                getters[3] = GetOne;
        }

        private void CreateSetters()
        {
            setters = new Action<int, int, float>[4];
            setters[0] = SetChannel;

            if (Channels > 1)
                setters[1] = SetChannel;
            else
                setters[1] = SetIgnore;

            if (Channels > 2)
                setters[2] = SetChannel;
            else
                setters[1] = SetIgnore;

            if (Channels > 3)
                setters[3] = SetChannel;
            else
                setters[3] = SetIgnore;
        }

        private float GetChannel(int i, int c)
        {
            return data[c][i];
        }

        private float GetChannelR(int i, int c)
        {
            return data[0][i];
        }

        private float GetOne(int i, int c)
        {
            return 1;
        }

        private float GetZero(int i, int c)
        {
            return 0;
        }

        private void SetChannel(int i, int c, float v)
        {
            data[c][i] = v;
        }

        private void SetIgnore(int i, int c, float v)
        {
          
        }
    }
}
