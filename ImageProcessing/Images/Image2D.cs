using System;
using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Collections.Arrays;
using Common.Core.Shapes;

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
        public Image2D(Point2i size)
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
        public Point2i Size => new Point2i(Width, Height);

        /// <summary>
        /// The size of the image as a box.
        /// </summary>
        public Box2i Bounds => new Box2i((0, 0), Size);

        /// <summary>
        /// The images description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Image2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// Get a pixel from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public abstract ColorRGB GetPixel(int x, int y, WRAP_MODE mode);

        /// <summary>
        /// Get a pixel from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public abstract ColorRGB GetPixel(float u, float v, WRAP_MODE mode);

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="pixel">The pixel.</param>
        public abstract void SetPixel(int x, int y, ColorRGB pixel);

        /// <summary>
        /// Return a index list of all pixels that match the predicate.
        /// </summary>
        /// <param name="list">The list the pixels will be added to.</param>
        /// <param name="predicate">The predicate that decides what pixels to include.</param>
        public void ToIndexList(List<Point2i> list, Func<T, bool> predicate)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var v = this[x, y];
                    if (predicate(v))
                        list.Add(new Point2i(x, y));
                }
            }
        }

        /// <summary>
        /// Return a index list of all pixels that match the predicate.
        /// </summary>
        /// <param name="list">The list the pixels will be added to.</param>
        /// <param name="predicate">The predicate that decides what pixels to include.</param>
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
        /// Fill the image with the values at the provided indices.
        /// </summary>
        /// <param name="indices">The indices and value to fill.</param>
        public void Fill(IList<PixelIndex2D<T>> indices)
        {
            for (int i = 0; i < indices.Count; i++)
            {
                var p = indices[i];
                this[p.Index] = p.Value;
            }
        }

        /// <summary>
        /// Fill the image with the value at the provided indices.
        /// </summary>
        /// <param name="indices">The indices to fill.</param>
        /// <param name="value">The value to fill.</param>
        public void Fill(IList<Point2i> indices, T value)
        {
            for (int i = 0; i < indices.Count; i++)
            {
                var j = indices[i];
                this[j] = value;
            }
        }

        /// <summary>
        /// Copy this image to another image of the same size.
        /// </summary>
        /// <param name="dest">The image to copy to.</param>
        public void CopyTo(Image2D<T> dest)
        {
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
