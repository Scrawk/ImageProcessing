using System;
using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{
    /// <summary>
    /// A 2D image containing only true or false values.
    /// </summary>
    public partial class BinaryImage2D : Image2D<bool>
    {
        /// <summary>
        /// Create a default of image.
        /// </summary>
        public BinaryImage2D()
            : base(0, 0)
        {

        }

        /// <summary>
        /// Create a image of a given width and height.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public BinaryImage2D(int width, int height)
            : base(width, height)
        {

        }

        /// <summary>
        /// Create a image of a given size.
        /// </summary>
        /// <param name="size">The size of the image. x is the width and y is the height.</param>
        public BinaryImage2D(Vector2i size)
            : base(size)
        {

        }

        /// <summary>
        /// Create a image of a given width and height and filled with a value.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="value">The value to fill the image with.</param>
        public BinaryImage2D(int width, int height, bool value)
             : base(width, height, value)
        {

        }

        /// <summary>
        /// Create a image from the given data.
        /// </summary>
        /// <param name="data">The images data.</param>
        public BinaryImage2D(bool[,] data)
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
            return string.Format("[BinaryImage2D: Width={0}, Height={1}]", Width, Height);
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
            if (mode == WRAP_MODE.CLAMP)
                return GetClamped(x, y) ? 1 : 0;
            else if (mode == WRAP_MODE.WRAP)
                return GetWrapped(x, y) ? 1 : 0;
            else
                return GetMirrored(x, y) ? 1 : 0;
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

            var v00 = GetValue(ix, iy, mode);
            var v10 = GetValue(ix + 1, iy, mode);
            var v01 = GetValue(ix, iy + 1, mode);
            var v11 = GetValue(ix + 1, iy + 1, mode);

            return MathUtil.Blerp(v00, v10, v01, v11, x - ix, y - iy);
        }

        /// <summary>
        /// Set the value at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="value">The value.</param>
        public void SetValue(int x, int y, bool value)
        {
            this[x, y] = value;
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
            var value = GetValue(x, y, mode);
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
            this[x, y] = pixel.Intensity > 0;
        }

        /// <summary>
        /// Return a copy of the image.
        /// </summary>
        /// <returns></returns>
        public BinaryImage2D Copy()
        {
            return new BinaryImage2D(Data);
        }

        /// <summary>
        /// Return the image as a float array.
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
