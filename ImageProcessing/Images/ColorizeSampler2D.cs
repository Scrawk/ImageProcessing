using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Collections.Arrays;

namespace ImageProcessing.Images
{
    public class ColorizeSampler2D<T> : ImageSampler2D
    {

        private ColorArray1 m_posGradient, m_negGradient, m_gradient;

        public ColorizeSampler2D(IImage2D<T> image, float exponent)
            : this(image, exponent, true, 1.0f)
        {

        }

        public ColorizeSampler2D(IImage2D<T> image, float exponent, bool nonNegative)
            : this(image, exponent, nonNegative, 1.0f)
        {

        }

        public ColorizeSampler2D(IImage2D<T> image, float exponent, bool nonNegative, float scale)
        {
            Image = image;
            Exponent = exponent;
            Scale = scale;
            NonNegative = nonNegative;
        }

        public IImage2D<T> Image { get; private set; }

        public float Exponent { get; private set; }

        public float Scale { get; private set; }

        public bool NonNegative { get; private set; }

        public override ColorRGB GetPixel(int x, int y)
        {
            return Colorize(Image.GetValue(x, y) * Scale);
        }

        public override ColorRGB GetPixel(float u, float v)
        {
            return Colorize(Image.GetValue(u, v) * Scale);
        }

        public ColorRGB Colorize(float v)
        {
            if (m_gradient == null)
                CreateGradients(true);

            if (Exponent > 0)
            {
                float sign = MathUtil.SignOrZero(v);
                float pow = MathUtil.Pow(10, Exponent);
                float log = MathUtil.Log(1.0f + pow * MathUtil.Abs(v));

                v = sign * log;
            }

            if (NonNegative)
                return m_gradient.GetBilinear01(v);
            else
            {
                if (v > 0)
                    return m_posGradient.GetBilinear01(v);
                else
                    return m_negGradient.GetBilinear01(-v);
            }
        }

        public void CreateGradients(bool colored)
        {
            if (colored)
            {
                m_gradient = CreateGradient("COOL_WARM");
                m_posGradient = CreateGradient("WARM");
                m_negGradient = CreateGradient("COOL");
            }
            else
            {
                m_gradient = CreateGradient("BLACK_WHITE");
                m_posGradient = CreateGradient("GREY_WHITE");
                m_negGradient = CreateGradient("GREY_BLACK");
            }
        }

        private ColorArray1 CreateGradient(string g)
        {
            switch (g)
            {
                case "WARM":
                    return CreateWarmGradient();

                case "COOL":
                    return CreateCoolGradient();

                case "COOL_WARM":
                    return CreateCoolToWarmGradient();

                case "GREY_WHITE":
                    return CreateGreyToWhiteGradient();

                case "GREY_BLACK":
                    return CreateGreyToBlackGradient();

                case "BLACK_WHITE":
                    return CreateBlackToWhiteGradient();
            }

            return null;
        }

        private const float SCALE = 1.0f / 255.0f;

        private ColorArray1 CreateWarmGradient()
        {
            var gradient = new ColorArray1(5);
            gradient[0] = new ColorRGB(80, 230, 80) * SCALE;
            gradient[1] = new ColorRGB(180, 230, 80) * SCALE;
            gradient[2] = new ColorRGB(230, 230, 80) * SCALE;
            gradient[3] = new ColorRGB(230, 180, 80) * SCALE;
            gradient[4] = new ColorRGB(230, 80, 80) * SCALE;
            return gradient;
        }

        private ColorArray1 CreateCoolGradient()
        {
            var gradient = new ColorArray1(5);
            gradient[0] = new ColorRGB(80, 230, 80) * SCALE;
            gradient[1] = new ColorRGB(80, 230, 180) * SCALE;
            gradient[2] = new ColorRGB(80, 230, 230) * SCALE;
            gradient[3] = new ColorRGB(80, 180, 230) * SCALE;
            gradient[4] = new ColorRGB(80, 80, 230) * SCALE;
            return gradient;
        }

        private ColorArray1 CreateCoolToWarmGradient()
        {
            var gradient = new ColorArray1(9);
            gradient[0] = new ColorRGB(80, 80, 230) * SCALE;
            gradient[1] = new ColorRGB(80, 180, 230) * SCALE;
            gradient[2] = new ColorRGB(80, 230, 230) * SCALE;
            gradient[3] = new ColorRGB(80, 230, 180) * SCALE;
            gradient[4] = new ColorRGB(80, 230, 80) * SCALE;
            gradient[5] = new ColorRGB(180, 230, 80) * SCALE;
            gradient[6] = new ColorRGB(230, 230, 80) * SCALE;
            gradient[7] = new ColorRGB(230, 180, 80) * SCALE;
            gradient[8] = new ColorRGB(230, 80, 80) * SCALE;
            return gradient;
        }

        private ColorArray1 CreateGreyToWhiteGradient()
        {
            var gradient = new ColorArray1(3);
            gradient[0] = new ColorRGB(128, 128, 128) * SCALE;
            gradient[1] = new ColorRGB(192, 192, 192) * SCALE;
            gradient[2] = new ColorRGB(255, 255, 255) * SCALE;
            return gradient;
        }

        private ColorArray1 CreateGreyToBlackGradient()
        {
            var gradient = new ColorArray1(3);
            gradient[0] = new ColorRGB(128, 128, 128) * SCALE;
            gradient[1] = new ColorRGB(64, 64, 64) * SCALE;
            gradient[2] = new ColorRGB(0, 0, 0);
            return gradient;
        }

        private ColorArray1 CreateBlackToWhiteGradient()
        {
            var gradient = new ColorArray1(5);
            gradient[0] = new ColorRGB(0, 0, 0);
            gradient[1] = new ColorRGB(64, 64, 64) * SCALE;
            gradient[2] = new ColorRGB(128, 128, 128) * SCALE;
            gradient[3] = new ColorRGB(192, 192, 192) * SCALE;
            gradient[4] = new ColorRGB(255, 255, 255) * SCALE;
            return gradient;
        }

    }
}
