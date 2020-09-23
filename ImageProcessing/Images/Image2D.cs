using System;
using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Collections.Arrays;
using Common.Geometry.Shapes;

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
        /// Create a new image.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public Image2D(int width, int height)
            : base(width, height)
        {

        }

        /// <summary>
        /// Create a new image.
        /// </summary>
        /// <param name="size">The size of the image.</param>
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
        /// Create a new image.
        /// </summary>
        /// <param name="data">The data for the image. Will be deep copied.</param>
        public Image2D(T[,] data)
            : base(data)
        {

        }

        /// <summary>
        /// The number of channels in the images pixel.
        /// </summary>
        public abstract int Channels { get; }

        /// <summary>
        /// The size of the image as a vector.
        /// </summary>
        public Vector2i Size => new Vector2i(Width, Height);

        /// <summary>
        /// The size of the image as a box.
        /// </summary>
        public Box2i Bounds => new Box2i((0, 0), Size);

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
        /// <param name="mode"></param>
        /// <returns></returns>
        public abstract float GetValue(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public abstract float GetValue(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public abstract ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public abstract ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP);

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
        public void ToIndexList(List<Vector2i> list, Func<T, bool> predicate)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var v = this[x, y];
                    if (predicate(v))
                        list.Add(new Vector2i(x, y));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void ToPixelIndexList(List<PixelIndex2D<T>>  list, Func<T, bool> predicate)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var v = this[x, y];
                    if (predicate(v))
                        list.Add(new PixelIndex2D<T>(x, y, v));
                }
            }
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
