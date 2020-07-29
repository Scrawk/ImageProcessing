using System;
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

        public void ApplyHorizontal(int y, GreyScaleImage2D src, GreyScaleImage2D dst)
        {
            int srcWidth = src.Width;
            float scale = Length / (float)srcWidth;
            float iscale = 1.0f / scale;

            for (int i = 0; i < Length; i++)
            {
                float center = (0.5f + i) * iscale;

                int left = (int)Math.Floor(center - Width);

                float sum = 0;
                for (int j = 0; j < WindowSize; ++j)
                {
                    int x = MathUtil.Clamp(j + left, 0, srcWidth - 1);
                    sum += src[x, y] * GetWeight(i, j);
                }

                dst[i, y] = sum;
            }
        }

        public void ApplyHorizontal(int y, ColorImage2D src, ColorImage2D dst)
        {
            int srcWidth = src.Width;
            float scale = Length / (float)srcWidth;
            float iscale = 1.0f / scale;

            for (int i = 0; i < Length; i++)
            {
                float center = (0.5f + i) * iscale;

                int left = (int)Math.Floor(center - Width);

                ColorRGB sum = new ColorRGB();
                for (int j = 0; j < WindowSize; ++j)
                {
                    int x = MathUtil.Clamp(j + left, 0, srcWidth - 1);
                    sum += src[x, y] * GetWeight(i, j);
                }

                dst[i, y] = sum;
            }
        }

        public void ApplyVertical(int x, GreyScaleImage2D src, GreyScaleImage2D dst)
        {
            int srcHeight = src.Height;
            float scale = Length / (float)srcHeight;
            float iscale = 1.0f / scale;

            for (int i = 0; i < Length; i++)
            {
                float center = (0.5f + i) * iscale;

                int left = (int)Math.Floor(center - Width);

                float sum = 0;
                for (int j = 0; j < WindowSize; ++j)
                {
                    int y = MathUtil.Clamp(j + left, 0, srcHeight - 1);
                    sum += src[x, y] * GetWeight(i, j);
                }

                dst[x, i] = sum;
            }
        }

        public void ApplyVertical(int x, ColorImage2D src, ColorImage2D dst)
        {
            int srcHeight = src.Height;
            float scale = Length / (float)srcHeight;
            float iscale = 1.0f / scale;

            for (int i = 0; i < Length; i++)
            {
                float center = (0.5f + i) * iscale;

                int left = (int)Math.Floor(center - Width);

                ColorRGB sum = new ColorRGB();
                for (int j = 0; j < WindowSize; ++j)
                {
                    int y = MathUtil.Clamp(j + left, 0, srcHeight - 1);
                    sum += src[x, y] * GetWeight(i, j);
                }

                dst[x, i] = sum;
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
