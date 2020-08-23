using System;
using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Collections.Arrays;

using ImageProcessing.Pixels;

namespace ImageProcessing.Images
{
    /// <summary>
    /// Base class for 2D images.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    public abstract partial class Image2D<T> : Array2<T>, IImage2D<T>
    {

        /// <summary>
        /// Create a new array.
        /// </summary>
        /// <param name="width">The size of the arrays 1st dimention.</param>
        /// <param name="height">The size of the arrays 2st dimention.</param>
        public Image2D(int width, int height)
            : base(width, height)
        {

        }

        /// <summary>
        /// Create a new array.
        /// </summary>
        /// <param name="size">The size of the array.</param>
        public Image2D(Vector2i size)
             : base(size)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="value"></param>
        public Image2D(int width, int height, T value)
            : base(width, height)
        {
            Data.Fill(value);
        }

        /// <summary>
        /// Create a new array.
        /// </summary>
        /// <param name="data">The data form the array. Will be deep copied.</param>
        public Image2D(T[,] data)
            : base(data)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Image2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract float GetValue(int x, int y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public abstract float GetValue(float u, float v);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract ColorRGB GetPixel(int x, int y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public abstract ColorRGB GetPixel(float u, float v);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pixel"></param>
        public abstract void SetPixel(int x, int y, ColorRGB pixel);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<PixelIndex2D<T>> ToPixelIndexList(Func<T, bool> predicate)
        {
            var pixel = new List<PixelIndex2D<T>>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var v = this[x, y];
                    if (predicate(v))
                        pixel.Add(new PixelIndex2D<T>(x, y, v));
                }
            }

            return pixel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void Fill(IList<PixelIndex2D<T>> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                this[p.Index] = p.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="value"></param>
        public void Fill(IList<PixelIndex2D<T>> points, T value)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                this[p.Index] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        public void CopyTo(Image2D<T> dest)
        {
            if (dest.Width != Width)
                throw new ArgumentException("dest.Width != Width");

            if (dest.Height != Height)
                throw new ArgumentException("dest.Height != Height");

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    dest[x, y] = this[x, y];
                }
            }
        }


    }
}
