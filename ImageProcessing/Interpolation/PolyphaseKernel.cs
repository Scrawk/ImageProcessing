﻿using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Interpolation
{
    public class PolyphaseKernel
    {

        public PolyphaseKernel(InterpolationFunction func, int srcLength, int dstLength)
        {

            float scale = dstLength / (float)srcLength;
            float iscale = 1.0f / scale;

            Length = dstLength;
            Width = func.Size * iscale;
            WindowSize = (int)Math.Ceiling(Width * 2) + 1;

            if (scale > 1)
            {
                // Up Sampling.
                Samples = 1;
                scale = 1;
            }
            else
            {
                //Down Sampling
                Samples = Math.Min(32, WindowSize * 2);
            }

            Weights = new float[WindowSize * Length];

            for (int i = 0; i < Length; i++)
            {
                float center = (0.5f + i) * iscale;

                int left = (int)Math.Floor(center - Width);

                float total = 0.0f;
                for (int j = 0; j < WindowSize; j++)
                {
                    float sample = SampleWindow(func, left + j - center, scale, Samples);

                    Weights[i * WindowSize + j] = sample;
                    total += sample;
                }

                // normalize weights.
                for (int j = 0; j < WindowSize; j++)
                {
                    Weights[i * WindowSize + j] /= total;
                }
            }
        }

        public int WindowSize { get; private set; }

        public int Length { get; private set; }

        public int Samples { get; private set; }

        public float Width { get; private set; }

        public float[] Weights { get; private set; }

        public float GetWeight(int column, int x)
        {
            return Weights[column * WindowSize + x];
        }

        public void ApplyHorizontal<IMAGE>(int y, IMAGE src, IMAGE dst, WRAP_MODE mode)
            where IMAGE : IImage2D, new()
        {
            int srcWidth = src.Width;
            float scale = Length / (float)srcWidth;
            float iscale = 1.0f / scale;

            for (int i = 0; i < Length; i++)
            {
                float center = (0.5f + i) * iscale;

                int left = (int)Math.Floor(center - Width);

                ColorRGBA sum = new ColorRGBA();
                for (int j = 0; j < WindowSize; ++j)
                {
                    int x = j + left;
                    sum += src.GetPixel(x, y, mode) * GetWeight(i, j);
                }

                dst.SetPixel(i, y, sum);
            }
        }

        public void ApplyVertical<IMAGE>(int x, IMAGE src, IMAGE dst, WRAP_MODE mode)
            where IMAGE : IImage2D, new()
        {
            int srcHeight = src.Height;
            float scale = Length / (float)srcHeight;
            float iscale = 1.0f / scale;

            for (int i = 0; i < Length; i++)
            {
                float center = (0.5f + i) * iscale;

                int left = (int)Math.Floor(center - Width);

                ColorRGBA sum = new ColorRGBA();
                for (int j = 0; j < WindowSize; ++j)
                {
                    int y = j + left;
                    sum += src.GetPixel(x, y, mode) * GetWeight(i, j);
                }

                dst.SetPixel(x, i, sum);
            }
        }

        private float SampleWindow(InterpolationFunction func, float x, float scale, int samples)
        {
            float sum = 0;
            float isamples = 1.0f / samples;

            for (int s = 0; s < samples; s++)
            {
                float p = (x + (s + 0.5f) * isamples) * scale;
                float value = func.GetWeight(p);
                sum += value;
            }

            return sum * isamples;
        }


    }
}
