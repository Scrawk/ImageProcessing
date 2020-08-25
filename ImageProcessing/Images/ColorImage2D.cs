using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ColorImage2D : Image2D<ColorRGB>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public ColorImage2D(int width, int height)
            : base(width, height)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public ColorImage2D(Vector2i size)
            : base(size)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="vaule"></param>
        public ColorImage2D(int width, int height, ColorRGB value)
            : base(width, height, value)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public ColorImage2D(ColorRGB[,] data)
            : base(data)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[ColorImage2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override float GetValue(int x, int y)
        {
            return GetPixel(x, y).Intensity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public override float GetValue(float u, float v)
        {
            return GetPixel(u, v).Intensity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override ColorRGB GetPixel(int x, int y)
        {
            return GetClamped(x, y);
        }

        /// <summary>
        /// Sample the image by clamped bilinear interpolation.
        /// </summary>
        public override ColorRGB GetPixel(float u, float v)
        {
            float x = u * (Width-1);
            float y = v * (Height-1);

            int xi = (int)x;
            int yi = (int)y;

            var v00 = GetClamped(xi, yi);
            var v10 = GetClamped(xi + 1, yi);
            var v01 = GetClamped(xi, yi + 1);
            var v11 = GetClamped(xi + 1, yi + 1);

            var col = new ColorRGB();
            col.r = MathUtil.Blerp(v00.r, v10.r, v01.r, v11.r, x - xi, y - yi);
            col.g = MathUtil.Blerp(v00.g, v10.g, v01.g, v11.g, x - xi, y - yi);
            col.b = MathUtil.Blerp(v00.b, v10.b, v01.b, v11.b, x - xi, y - yi);
            return col;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pixel"></param>
        public override void SetPixel(int x, int y, ColorRGB pixel)
        {
            this[x, y] = pixel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ColorImage2D Copy()
        {
            return new ColorImage2D(Data);
        }

    }

}
