using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Images
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GreyScaleImage2D : Image2D<float>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public GreyScaleImage2D(int width, int height)
            : base(width, height)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public GreyScaleImage2D(Vector2i size)
            : base(size)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="vaule"></param>
        public GreyScaleImage2D(int width, int height, float value)
            : base(width, height, value)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public GreyScaleImage2D(float[,] data)
            : base(data)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[GreyScaleImage2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override float GetValue(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            if(mode == WRAP_MODE.CLAMP)
                return GetClamped(x, y);
            else if (mode == WRAP_MODE.WRAP)
                return GetWrapped(x, y);
            else
                return GetMirrored(x, y);
        }

        /// <summary>
        /// Sample the image by bilinear interpolation.
        /// </summary>
        public override float GetValue(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float x = u * (Width-1);
            float y = v * (Height-1);

            int xi = (int)x;
            int yi = (int)y;

            float v00, v10, v01, v11;

            if (mode == WRAP_MODE.CLAMP)
            {
                v00 = GetClamped(xi, yi);
                v10 = GetClamped(xi + 1, yi);
                v01 = GetClamped(xi, yi + 1);
                v11 = GetClamped(xi + 1, yi + 1);
            }
            else if (mode == WRAP_MODE.WRAP)
            {
                v00 = GetWrapped(xi, yi);
                v10 = GetWrapped(xi + 1, yi);
                v01 = GetWrapped(xi, yi + 1);
                v11 = GetWrapped(xi + 1, yi + 1);
            }
            else
            {
                v00 = GetMirrored(xi, yi);
                v10 = GetMirrored(xi + 1, yi);
                v01 = GetMirrored(xi, yi + 1);
                v11 = GetMirrored(xi + 1, yi + 1);
            }

            return MathUtil.Blerp(v00, v10, v01, v11, x - xi, y - yi);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float value = GetValue(x, y, mode);
            return new ColorRGB(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public override ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var value = GetValue(u, v, mode);
            return new ColorRGB(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pixel"></param>
        public override void SetPixel(int x, int y, ColorRGB pixel)
        {
            this[x, y] = pixel.Intensity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GreyScaleImage2D Copy()
        {
            return new GreyScaleImage2D(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public Vector3f GetNormal(int x, int y, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var d = GetFirstDerivative(x, y, w, mode);
            var n = new Vector3f(d.x, 1, d.y);
            return n.Normalized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public Vector3f GetNormal(float u, float v, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var d = GetFirstDerivative(u, v, w, mode);
            var n = new Vector3f(d.x, 1, d.y);
            return n.Normalized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public Vector2f GetFirstDerivative(int x, int y, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float z1 = GetValue(x - 1, y + 1, mode);
            float z2 = GetValue(x + 0, y + 1, mode);
            float z3 = GetValue(x + 1, y + 1, mode);
            float z4 = GetValue(x - 1, y + 0, mode);
            float z6 = GetValue(x + 1, y + 0, mode);
            float z7 = GetValue(x - 1, y - 1, mode);
            float z8 = GetValue(x + 0, y - 1, mode);
            float z9 = GetValue(x + 1, y - 1, mode);

            //p, q
            float zx = (z3 + z6 + z9 - z1 - z4 - z7) / (6.0f * w.x);
            float zy = (z1 + z2 + z3 - z7 - z8 - z9) / (6.0f * w.y);

            return new Vector2f(-zx, -zy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public Vector2f GetFirstDerivative(float u, float v, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float x1 = 1.0f / Width;
            float y1 = 1.0f / Height;
            float z1 = GetValue(u - x1, v + y1, mode);
            float z2 = GetValue(u +  0, v + y1, mode);
            float z3 = GetValue(u + x1, v + y1, mode);
            float z4 = GetValue(u - x1, v +  0, mode);
            float z6 = GetValue(u + x1, v +  0, mode);
            float z7 = GetValue(u - x1, v - y1, mode);
            float z8 = GetValue(u +  0, v - y1, mode);
            float z9 = GetValue(u + x1, v - y1, mode);

            //p, q
            float zx = (z3 + z6 + z9 - z1 - z4 - z7) / (6.0f * w.x);
            float zy = (z1 + z2 + z3 - z7 - z8 - z9) / (6.0f * w.y);

            return new Vector2f(-zx, -zy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        public (Vector2f d1, Vector3f d2) GetFirstAndSecondDerivative(int x, int y, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float wx2 = w.x * w.x;
            float wy2 = w.y * w.y;
            float wxy2 = w.SqrMagnitude;
            float z1 = GetValue(x - 1, y + 1, mode);
            float z2 = GetValue(x + 0, y + 1, mode);
            float z3 = GetValue(x + 1, y + 1, mode);
            float z4 = GetValue(x - 1, y + 0, mode);
            float z5 = GetValue(x + 0, y + 0, mode);
            float z6 = GetValue(x + 1, y + 0, mode);
            float z7 = GetValue(x - 1, y - 1, mode);
            float z8 = GetValue(x + 0, y - 1, mode);
            float z9 = GetValue(x + 1, y - 1, mode);

            //p, q
            float zx = (z3 + z6 + z9 - z1 - z4 - z7) / (6.0f * w.x);
            float zy = (z1 + z2 + z3 - z7 - z8 - z9) / (6.0f * w.y);

            //r, t, s
            float zxx = (z1 + z3 + z4 + z6 + z7 + z9 - 2.0f * (z2 + z5 + z8)) / (3.0f * wx2);
            float zyy = (z1 + z2 + z3 + z7 + z8 + z9 - 2.0f * (z4 + z5 + z6)) / (3.0f * wy2);
            float zxy = (z3 + z7 - z1 - z9) / (4.0f * wxy2);

            var d1 = new Vector2f(-zx, -zy);
            var d2 = new Vector3f(-zxx, -zyy, -zxy);

            return (d1, d2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        public (Vector2f d1, Vector3f d2) GetFirstAndSecondDerivative(float u, float v, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float x1 = 1.0f / Width;
            float y1 = 1.0f / Height;
            float wx2 = w.x * w.x;
            float wy2 = w.y * w.y;
            float wxy2 = w.SqrMagnitude;
            float z1 = GetValue(u - x1, v + y1, mode);
            float z2 = GetValue(u +  0, v + y1, mode);
            float z3 = GetValue(u + x1, v + y1, mode);
            float z4 = GetValue(u - x1, v +  0, mode);
            float z5 = GetValue(u +  0, v +  0, mode);
            float z6 = GetValue(u + x1, v +  0, mode);
            float z7 = GetValue(u - x1, v - y1, mode);
            float z8 = GetValue(u +  0, v - y1, mode);
            float z9 = GetValue(u + x1, v - y1, mode);

            //p, q
            float zx = (z3 + z6 + z9 - z1 - z4 - z7) / (6.0f * w.x);
            float zy = (z1 + z2 + z3 - z7 - z8 - z9) / (6.0f * w.y);

            //r, t, s
            float zxx = (z1 + z3 + z4 + z6 + z7 + z9 - 2.0f * (z2 + z5 + z8)) / (3.0f * wx2);
            float zyy = (z1 + z2 + z3 + z7 + z8 + z9 - 2.0f * (z4 + z5 + z6)) / (3.0f * wy2);
            float zxy = (z3 + z7 - z1 - z9) / (4.0f * wxy2);

            var d1 = new Vector2f(-zx, -zy);
            var d2 = new Vector3f(-zxx, -zyy, -zxy);

            return (d1, d2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public (float min, float max) MinMax()
        {
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float v = this[x, y];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
            }

            return (min, max);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Normalize()
        {
            var minMax = MinMax();

            ParallelModify((v) =>
            {
                v = MathUtil.Normalize(v, minMax.min, minMax.max);
                return MathUtil.Clamp01(v);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public float Sum()
        {
            float sum = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    sum += this[x, y];
                }
            }

            return sum;
        }

    }

}
