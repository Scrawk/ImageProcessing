using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Geometry.Shapes;

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
        /// <param name="data"></param>
        public GreyScaleImage2D(float[,] data)
                  : base(data)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="value"></param>
        public GreyScaleImage2D(int width, int height, float value)
          : base(width, height)
        {
            Data.Fill(value);
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
        /// Sample the image by clamped bilinear interpolation.
        /// </summary>
        public float GetBilinear01(float u, float v)
        {
            float x = u * Width;
            float y = v * Height;
            return GetBilinear(x, y);
        }

        /// <summary>
        /// Sample the image by clamped bilinear interpolation.
        /// </summary>
        public float GetBilinear(float x, float y)
        {
            int xi = (int)x;
            int yi = (int)y;

            var v00 = GetClamped(xi, yi);
            var v10 = GetClamped(xi + 1, yi);
            var v01 = GetClamped(xi, yi + 1);
            var v11 = GetClamped(xi + 1, yi + 1);

            return MathUtil.Blerp(v00, v10, v01, v11, x - xi, y - yi);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GreyScaleImage2D Copy()
        {
            return new GreyScaleImage2D(Data);
        }

    }

}
