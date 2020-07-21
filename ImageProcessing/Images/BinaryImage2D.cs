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
            return this[x, y] ? 1 : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override ColorRGB GetPixel(int x, int y)
        {
            var v = this[x, y] ? 1 : 0;
            return new ColorRGB(v, v, v);
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
