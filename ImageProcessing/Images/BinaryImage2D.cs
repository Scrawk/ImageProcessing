using System;
using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{
    /// <summary>
    /// A 2D image containing only true or false values.
    /// </summary>
    public partial class BinaryImage2D : Image2D<bool>
    {


        int m_width, m_height;

        /// <summary>
        /// Create a default of image.
        /// </summary>
        public BinaryImage2D()
            : this(0, 0)
        {
        }

        /// <summary>
        /// Create a image of a given width and height.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public BinaryImage2D(int width, int height)
        {
            m_width = width;
            m_height = height;
            Data = new BitArray(width * height);
        }

        /// <summary>
        /// Create a image of a given size.
        /// </summary>
        /// <param name="size">The size of the image. x is the width and y is the height.</param>
        public BinaryImage2D(Point2i size)
        {
            m_width = size.x;
            m_height = size.y;
            Data = new BitArray(size.x * size.y);
        }

        /// <summary>
        /// Create a image of a given width and height and filled with a value.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="value">The value to fill the image with.</param>
        public BinaryImage2D(int width, int height, bool value)
        {
            m_width = width;
            m_height = height;
            Data = new BitArray(width * height);
            Fill(value);
        }

        /// <summary>
        /// The images pixels.
        /// </summary>
        private BitArray Data;

        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        public override int Count => Data.Length;

        /// <summary>
        /// The size of the arrays 1st dimention.
        /// </summary>
        public override int Width => m_width;

        /// <summary>
        /// The size of the arrays 2st dimention.
        /// </summary>
        public override int Height => m_height;

        /// <summary>
        /// The number of channels in the images pixel.
        /// </summary>
        public override int Channels => 1;

        /// <summary>
        /// Access a element at index x,y.
        /// </summary>
        public override bool this[int x, int y]
        {
            get {  return Data[x + y * m_width]; } 
            set {  Data[x + y * m_width] = value; }
        }

        /// <summary>
        /// Access a element at index x,y.
        /// </summary>
        public override bool this[Point2i i]
        {
            get { return Data[i.x + i.y * m_width]; }
            set { Data[i.x + i.y * m_width] = value; }
        }

        /// <summary>
        /// Return the image description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[BinaryImage2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// Sets all elements in the array to default value.
        /// </summary>
        public override void Clear()
        {
            Data.SetAll(false);
        }

        /// <summary>
        /// Resize the array. Will clear any existing data.
        /// </summary>
        public override void Resize(int width, int height)
        {
            m_width = width;
            m_height = height;
            Data = new BitArray(width * height);
        }

        /// <summary>
        /// Resize the array. Will clear any existing data.
        /// </summary>
        public override void Resize(Point2i size)
        {
            m_width = size.x;
            m_height = size.y;
            Data = new BitArray(size.x * size.y);
        }

        /// <summary>
        /// Get a value from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The value at index x,y.</returns>
        public bool GetValue(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            switch (mode)
            {
                case WRAP_MODE.CLAMP:
                    return GetClamped(x, y);

                case WRAP_MODE.WRAP:
                    return GetWrapped(x, y);

                case WRAP_MODE.MIRROR:
                    return GetMirrored(x, y);

                case WRAP_MODE.NONE:
                    return this[x, y];

                default:
                    return this[x, y];
            }
        }

        /// <summary>
        /// Get a value from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The value at index x,y.</returns>
        public bool GetValue(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float x = u * (Width-1);
            float y = v * (Height-1);

            int ix = (int)x;
            int iy = (int)y;

            var v00 = GetValue(ix, iy, mode) ? 1.0f : 0.0f;
            var v10 = GetValue(ix + 1, iy, mode) ? 1.0f : 0.0f;
            var v01 = GetValue(ix, iy + 1, mode) ? 1.0f : 0.0f;
            var v11 = GetValue(ix + 1, iy + 1, mode) ? 1.0f : 0.0f;

            return MathUtil.Blerp(v00, v10, v01, v11, x - ix, y - iy) > 0;
        }

        /// <summary>
        /// Set the value at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="value">The value.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        public void SetValue(int x, int y, bool value, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            switch (mode)
            {
                case WRAP_MODE.NONE:
                    this[x, y] = value;
                    break;

                case WRAP_MODE.CLAMP:
                    SetClamped(x, y, value);
                    break;

                case WRAP_MODE.WRAP:
                    SetWrapped(x, y, value);
                    break;

                case WRAP_MODE.MIRROR:
                    SetMirrored(x, y, value);
                    break;

                default:
                    this[x, y] = value;
                    break;
            }
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
            var value = GetValue(x, y, mode) ? 1.0f : 0.0f; ;
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
            var value = GetValue(u, v, mode) ? 1.0f : 0.0f;
            return new ColorRGB(value);
        }

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="pixel">The pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        public override void SetPixel(int x, int y, ColorRGB pixel, WRAP_MODE mode = WRAP_MODE.NONE)
        {
 
            switch (mode)
            {
                case WRAP_MODE.NONE:
                    this[x, y] = pixel.Intensity > 0;
                    break;

                case WRAP_MODE.CLAMP:
                    SetClamped(x, y, pixel.Intensity > 0);
                    break;

                case WRAP_MODE.WRAP:
                    SetWrapped(x, y, pixel.Intensity > 0);
                    break;

                case WRAP_MODE.MIRROR:
                    SetMirrored(x, y, pixel.Intensity > 0);
                    break;

                default:
                    this[x, y] = pixel.Intensity > 0;
                    break;
            }
        }

        /// <summary>
        /// Return a copy of the image.
        /// </summary>
        /// <returns></returns>
        public BinaryImage2D Copy()
        {
            var copy = new BinaryImage2D(Width, Height);
            copy.Fill((x, y) =>
            {
                return this[x, y];
            });

            return copy;
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

        /// <summary>
        /// Convert to a greyscale image.
        /// </summary>
        /// <returns>The greayscale image.</returns>
        public GreyScaleImage2D ToGreyScaleImage()
        {
            var image = new GreyScaleImage2D(Width, Height);

            image.Fill((x, y) =>
            {
                return this[x, y] ? 1 : 0;
            });

            return image;
        }
    }

}
