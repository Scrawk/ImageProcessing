using System;
using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BinaryImage2D : Image2D<bool>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BinaryImage2D(int width, int height)
            : base(width, height)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public BinaryImage2D(Vector2i size)
            : base(size)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="value"></param>
        public BinaryImage2D(int width, int height, bool value)
             : base(width, height, value)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="data"></param>
        public BinaryImage2D(bool[,] data)
            : base(data)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[BinaryImage2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override float GetValue(int x, int y)
        {
            return GetClamped(x, y) ? 1 : 0;
        }

        /// <summary>
        /// Sample the image by clamped bilinear interpolation.
        /// </summary>
        public override float GetValue(float u, float v)
        {
            float x = u * (Width-1);
            float y = v * (Height-1);

            int xi = (int)x;
            int yi = (int)y;

            var v00 = GetValue(xi, yi);
            var v10 = GetValue(xi + 1, yi);
            var v01 = GetValue(xi, yi + 1);
            var v11 = GetValue(xi + 1, yi + 1);

            return MathUtil.Blerp(v00, v10, v01, v11, x - xi, y - yi);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override ColorRGB GetPixel(int x, int y)
        {
            var value = GetValue(x, y);
            return new ColorRGB(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public override ColorRGB GetPixel(float u, float v)
        {
            var value = GetValue(u, v);
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
            this[x, y] = pixel.Intensity > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BinaryImage2D Copy()
        {
            return new BinaryImage2D(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float[,] ToFloatArray()
        {
            var array = new float[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    array[x, y] = this[x, y] ? 1 : 0;
                }
            }

            return array;
        }
    }

}
