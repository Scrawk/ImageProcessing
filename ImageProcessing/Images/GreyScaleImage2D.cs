using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Images
{
    /// <summary>
    /// A 2D image containing only float values.
    /// </summary>
    public partial class GreyScaleImage2D : Image2D<float>
    {

        /// <summary>
        /// Create a default of image.
        /// </summary>
        public GreyScaleImage2D()
             : base(0, 0)
        {

        }

        /// <summary>
        /// Create a image of a given width and height.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public GreyScaleImage2D(int width, int height)
            : base(width, height)
        {

        }

        /// <summary>
        /// Create a image of a given size.
        /// </summary>
        /// <param name="size">The size of the image. x is the width and y is the height.</param>
        public GreyScaleImage2D(Vector2i size)
            : base(size)
        {

        }

        /// <summary>
        /// Create a image of a given width and height and filled with a value.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="value">The value to fill the image with.</param>
        public GreyScaleImage2D(int width, int height, float value)
            : base(width, height, value)
        {

        }

        /// <summary>
        /// Create a image from the given data.
        /// </summary>
        /// <param name="data">The images data.</param>
        public GreyScaleImage2D(float[,] data)
            : base(data)
        {

        }

        /// <summary>
        /// The number of channels in the images pixel.
        /// </summary>
        public override int Channels => 1;

        /// <summary>
        /// Return the image description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[GreyScaleImage2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// Get a value from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The value at index x,y.</returns>
        public float GetValue(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            if(mode == WRAP_MODE.CLAMP)
                return GetClamped(x, y);
            else if (mode == WRAP_MODE.WRAP)
                return GetWrapped(x, y);
            else
                return GetMirrored(x, y);
        }

        /// <summary>
        /// Get a value from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The value at index x,y.</returns>
        public float GetValue(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float x = u * (Width-1);
            float y = v * (Height-1);

            int ix = (int)x;
            int iy = (int)y;

            float v00, v10, v01, v11;

            if (mode == WRAP_MODE.CLAMP)
            {
                v00 = GetClamped(ix, iy);
                v10 = GetClamped(ix + 1, iy);
                v01 = GetClamped(ix, iy + 1);
                v11 = GetClamped(ix + 1, iy + 1);
            }
            else if (mode == WRAP_MODE.WRAP)
            {
                v00 = GetWrapped(ix, iy);
                v10 = GetWrapped(ix + 1, iy);
                v01 = GetWrapped(ix, iy + 1);
                v11 = GetWrapped(ix + 1, iy + 1);
            }
            else
            {
                v00 = GetMirrored(ix, iy);
                v10 = GetMirrored(ix + 1, iy);
                v01 = GetMirrored(ix, iy + 1);
                v11 = GetMirrored(ix + 1, iy + 1);
            }

            return MathUtil.Blerp(v00, v10, v01, v11, x - ix, y - iy);
        }

        /// <summary>
        /// Set the value at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="value">The value.</param>
        public void SetValue(int x, int y, float value)
        {
            this[x, y] = value;
        }

        /// <summary>
        /// Set the value at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="value">The value.</param>
        /// <param name="mode">The blend mode used to combine value with current value.</param>
        public void SetValue(float u, float v, float value, BLEND_MODE mode)
        {
            float x = u * (Width - 1);
            float y = v * (Height - 1);

            int ix = (int)x;
            int iy = (int)y;
            float fx = x - ix;
            float fy = y - iy;

            var v00 = InBounds(ix, iy) ? this[ix, iy] : 0;
            var v10 = InBounds(ix+1, iy) ? this[ix+1, iy] : 0;
            var v01 = InBounds(ix, iy+1) ? this[ix, iy+1] : 0;
            var v11 = InBounds(ix+1, iy+1) ? this[ix+1, iy+1] : 0;

            if (mode == BLEND_MODE.BLEND)
            {
                v00 = MathUtil.Lerp(v00, value, (1 - fx) * (1 - fy));
                v10 = MathUtil.Lerp(v10, value, fx * (1 - fy));
                v01 = MathUtil.Lerp(v01, value, (1 - fx) * fy);
                v11 = MathUtil.Lerp(v11, value, fx * fy);
            }
            else if(mode == BLEND_MODE.ADDITIVE)
            {
                v00 += (1 - fx) * (1 - fy) * value;
                v10 += fx * (1 - fy) * value;
                v01 += (1 - fx) * fy * value;
                v11 += fx * fy * value;
            }
            else if (mode == BLEND_MODE.SUBTRACTIVE)
            {
                v00 -= (1 - fx) * (1 - fy) * value;
                v10 -= fx * (1 - fy) * value;
                v01 -= (1 - fx) * fy * value;
                v11 -= fx * fy * value;
            }
            else if (mode == BLEND_MODE.SUBTRACTIVE_CLAMPED)
            {
                v00 = Math.Max(0, v00 - (1 - fx) * (1 - fy) * value);
                v10 = Math.Max(0, v10 - fx * (1 - fy) * value);
                v01 = Math.Max(0, v01 - (1 - fx) * fy * value);
                v11 = Math.Max(0, v11 - fx * fy * value);
            }

            if (InBounds(ix, iy)) this[ix, iy] = v00;
            if (InBounds(ix+1, iy)) this[ix+1, iy] = v10;
            if (InBounds(ix, iy+1)) this[ix, iy+1] = v01;
            if (InBounds(ix+1, iy+1)) this[ix+1, iy+1] = v11;
        }

        /// <summary>
        /// Get a pixel from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public override ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float value = GetValue(x, y, mode);
            return new ColorRGB(value);
        }

        /// <summary>
        /// Get a pixel from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public override ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var value = GetValue(u, v, mode);
            return new ColorRGB(value);
        }

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="pixel">The pixel.</param>
        public override void SetPixel(int x, int y, ColorRGB pixel)
        {
            this[x, y] = pixel.Intensity;
        }

        /// <summary>
        /// Return a copy of the image.
        /// </summary>
        /// <returns></returns>
        public GreyScaleImage2D Copy()
        {
            return new GreyScaleImage2D(Data);
        }

        /// <summary>
        /// Get the slope from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public float GetSlope(int x, int y, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var d = GetFirstDerivative(x, y, w, mode);
            float p = d.x * d.x + d.y * d.y;
            float g = MathUtil.SafeSqrt(p);
            return MathUtil.Atan(g) * MathUtil.Rad2Deg / 90.0f;
        }

        /// <summary>
        /// Get the slope from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public float GetSlope(float u, float v, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var d = GetFirstDerivative(u, v, w, mode);
            float p = d.x * d.x + d.y * d.y;
            float g = MathUtil.SafeSqrt(p);
            return MathUtil.Atan(g) * MathUtil.Rad2Deg / 90.0f;
        }

        /// <summary>
        /// Get the normal from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public Vector3f GetNormal(int x, int y, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var d = GetFirstDerivative(x, y, w, mode);
            var n = new Vector3f(d.x, 1, d.y);
            return n.Normalized;
        }

        /// <summary>
        /// Get the normal from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public Vector3f GetNormal(float u, float v, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var d = GetFirstDerivative(u, v, w, mode);
            var n = new Vector3f(d.x, 1, d.y);
            return n.Normalized;
        }

        /// <summary>
        /// Get the frist derivative from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
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
        /// Get the first derivative from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
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
        /// Get the first and second derivative from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
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
        /// Get the first and second derivative from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
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

    }

}
