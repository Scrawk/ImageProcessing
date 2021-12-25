using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{
    /// <summary>
    /// A 2D image containing RGB color values.
    /// </summary>
    public partial class ColorImage2D : Image2D<ColorRGB>
    {
        /// <summary>
        /// Create a default of image.
        /// </summary>
        public ColorImage2D()
             : base(0, 0)
        {

        }

        /// <summary>
        /// Create a image of a given width and height.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public ColorImage2D(int width, int height)
            : base(width, height)
        {

        }

        /// <summary>
        /// Create a image of a given size.
        /// </summary>
        /// <param name="size">The size of the image. x is the width and y is the height.</param>
        public ColorImage2D(Point2i size)
            : base(size)
        {

        }

        /// <summary>
        /// Create a image of a given width and height and filled with a value.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="value">The value to fill the image with.</param>
        public ColorImage2D(int width, int height, ColorRGB value)
            : base(width, height, value)
        {

        }

        /// <summary>
        /// Create a image from the given data.
        /// </summary>
        /// <param name="data">The images data.</param>
        public ColorImage2D(ColorRGB[,] data)
            : base(data)
        {

        }

        /// <summary>
        /// The number of channels in the images pixel.
        /// </summary>
        public override int Channels => 3;

        /// <summary>
        /// Return the image description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[ColorImage2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// Get a channels value from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="c">The channel index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The value at index x,y.</returns>
        public float GetChannel(int x, int y, int c, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return GetPixel(x, y, mode)[c];
        }

        /// <summary>
        /// Get a channels value from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="c">The channel index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The value at index x,y.</returns>
        public float GetChannel(float u, float v, int c, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float x = u * (Width - 1);
            float y = v * (Height - 1);

            int xi = (int)x;
            int yi = (int)y;

            ColorRGB v00, v10, v01, v11;

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

            return MathUtil.Blerp(v00[c], v10[c], v01[c], v11[c], x - xi, y - yi);
        }

        /// <summary>
        /// Set the channel value at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="c">The channel index.</param>
        /// <param name="value">The value.</param>
        public void SetChannel(int x, int y, int c, float value)
        {
            var pixel = this[x, y];
            pixel[c] = value;
            this[x, y] = pixel;
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
            if (mode == WRAP_MODE.CLAMP)
                return GetClamped(x, y);
            else if (mode == WRAP_MODE.WRAP)
                return GetWrapped(x, y);
            else
                return GetMirrored(x, y);
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
            float x = u * (Width-1);
            float y = v * (Height-1);

            int xi = (int)x;
            int yi = (int)y;

            ColorRGB v00, v10, v01, v11;

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

            var col = new ColorRGB();
            col.r = MathUtil.Blerp(v00.r, v10.r, v01.r, v11.r, x - xi, y - yi);
            col.g = MathUtil.Blerp(v00.g, v10.g, v01.g, v11.g, x - xi, y - yi);
            col.b = MathUtil.Blerp(v00.b, v10.b, v01.b, v11.b, x - xi, y - yi);
            return col;
        }

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="pixel">The pixel.</param>
        public override void SetPixel(int x, int y, ColorRGB pixel)
        {
            this[x, y] = pixel;
        }

        /// <summary>
        /// Set the pixel at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="pixel">The value.</param>
        /// <param name="mode">The blend mode used to combine value with current value.</param>
        public void SetPixel(float u, float v, ColorRGB pixel, BLEND_MODE mode = BLEND_MODE.ADDITIVE)
        {
            float x = u * (Width - 1);
            float y = v * (Height - 1);

            int ix = (int)x;
            int iy = (int)y;
            float fx = x - ix;
            float fy = y - iy;

            var v00 = InBounds(ix, iy) ? this[ix, iy] : ColorRGB.Black;
            var v10 = InBounds(ix + 1, iy) ? this[ix + 1, iy] : ColorRGB.Black;
            var v01 = InBounds(ix, iy + 1) ? this[ix, iy + 1] : ColorRGB.Black;
            var v11 = InBounds(ix + 1, iy + 1) ? this[ix + 1, iy + 1] : ColorRGB.Black;

            if (mode == BLEND_MODE.BLEND)
            {
                v00 = ColorRGB.Lerp(v00, pixel, (1 - fx) * (1 - fy));
                v10 = ColorRGB.Lerp(v10, pixel, fx * (1 - fy));
                v01 = ColorRGB.Lerp(v01, pixel, (1 - fx) * fy);
                v11 = ColorRGB.Lerp(v11, pixel, fx * fy);
            }
            else if (mode == BLEND_MODE.ADDITIVE)
            {
                v00 += (1 - fx) * (1 - fy) * pixel;
                v10 += fx * (1 - fy) * pixel;
                v01 += (1 - fx) * fy * pixel;
                v11 += fx * fy * pixel;
            }
            else if (mode == BLEND_MODE.SUBTRACTIVE)
            {
                v00 -= (1 - fx) * (1 - fy) * pixel;
                v10 -= fx * (1 - fy) * pixel;
                v01 -= (1 - fx) * fy * pixel;
                v11 -= fx * fy * pixel;
            }
            else if (mode == BLEND_MODE.SUBTRACTIVE_CLAMPED)
            {
                v00 = ColorRGB.Max(v00 - (1 - fx) * (1 - fy) * pixel, 0);
                v10 = ColorRGB.Max(v10 - fx * (1 - fy) * pixel, 0);
                v01 = ColorRGB.Max(v01 - (1 - fx) * fy * pixel, 0);
                v11 = ColorRGB.Max(v11 - fx * fy * pixel, 0);
            }

            if (InBounds(ix, iy)) this[ix, iy] = v00;
            if (InBounds(ix + 1, iy)) this[ix + 1, iy] = v10;
            if (InBounds(ix, iy + 1)) this[ix, iy + 1] = v01;
            if (InBounds(ix + 1, iy + 1)) this[ix + 1, iy + 1] = v11;
        }

        /// <summary>
        /// Return a copy of the image.
        /// </summary>
        /// <returns></returns>
        public ColorImage2D Copy()
        {
            return new ColorImage2D(Data);
        }

    }

}
