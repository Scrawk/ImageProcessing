using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;
using Common.Core.Extensions;

namespace ImageProcessing.Images
{
    /// <summary>
    /// A 2D image containing RGB color values.
    /// </summary>
    public partial class VectorImage2D : Image2D<Vector2f>
    {
        /// <summary>
        /// Create a default of image.
        /// </summary>
        public VectorImage2D()
             : this(0, 0)
        {

        }

        /// <summary>
        /// Create a image of a given width and height.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public VectorImage2D(int width, int height)
        {
            Data = new Vector2f[width, height];
        }

        /// <summary>
        /// Create a image of a given size.
        /// </summary>
        /// <param name="size">The size of the image. x is the width and y is the height.</param>
        public VectorImage2D(Point2i size)
        {
            Data = new Vector2f[size.x, size.y];
        }

        /// <summary>
        /// Create a image of a given width and height and filled with a value.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="value">The value to fill the image with.</param>
        public VectorImage2D(int width, int height, Vector2f value)
        {
            Data = new Vector2f[width, height];
            Fill(value);
        }

        /// <summary>
        /// Create a image from the given data.
        /// </summary>
        /// <param name="data">The images data.</param>
        public VectorImage2D(Vector2f[,] data)
        {
            data = data.Copy();
        }

        /// <summary>
        /// The images pixels.
        /// </summary>
        private Vector2f[,] Data;

        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        public override int Count => Data.Length;

        /// <summary>
        /// The size of the arrays 1st dimention.
        /// </summary>
        public override int Width => Data.GetLength(0);

        /// <summary>
        /// The size of the arrays 2st dimention.
        /// </summary>
        public override int Height => Data.GetLength(1);

        /// <summary>
        /// The number of channels in the images pixel.
        /// </summary>
        public override int Channels => 2;

        /// <summary>
        /// Access a element at index x,y.
        /// </summary>
        public override Vector2f this[int x, int y]
        {
            get { return Data[x, y]; }
            set { Data[x, y] = value; }
        }

        /// <summary>
        /// Access a element at index x,y.
        /// </summary>
        public override Vector2f this[Point2i i]
        {
            get { return Data[i.x, i.y]; }
            set { Data[i.x, i.y] = value; }
        }

        /// <summary>
        /// Return the image description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[VectorImage2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// Sets all elements in the array to default value.
        /// </summary>
        public override void Clear()
        {
            Data.Clear();
        }

        /// <summary>
        /// Resize the array. Will clear any existing data.
        /// </summary>
        public override void Resize(int width, int height)
        {
            Data = new Vector2f[width, height];
        }

        /// <summary>
        /// Resize the array. Will clear any existing data.
        /// </summary>
        public override void Resize(Point2i size)
        {
            Data = new Vector2f[size.x, size.y];
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
            return GetVector(x, y, mode)[c];
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

            Vector2f v00, v10, v01, v11;

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
            else if (mode == WRAP_MODE.MIRROR)
            {
                v00 = GetMirrored(xi, yi);
                v10 = GetMirrored(xi + 1, yi);
                v01 = GetMirrored(xi, yi + 1);
                v11 = GetMirrored(xi + 1, yi + 1);
            }
            else
            {
                v00 = this[xi, yi];
                v10 = this[xi + 1, yi];
                v01 = this[xi, yi + 1];
                v11 = this[xi + 1, yi + 1];
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
            var vector = this[x, y];
            vector[c] = value;
            this[x, y] = vector;
        }

        /// <summary>
        /// Get a vector from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The vector at index x,y.</returns>
        public Vector2f GetVector(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
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
        /// Get a vector from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The vector at index x,y.</returns>
        public Vector2f GetVector(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float x = u * (Width - 1);
            float y = v * (Height - 1);

            int xi = (int)x;
            int yi = (int)y;

            Vector2f v00, v10, v01, v11;

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

            var vec = new Vector2f();
            vec.x = MathUtil.Blerp(v00.x, v10.x, v01.x, v11.x, x - xi, y - yi);
            vec.y = MathUtil.Blerp(v00.y, v10.y, v01.y, v11.y, x - xi, y - yi);
            return vec;
        }

        /// <summary>
        /// Set the vector at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="vector">The vector.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        public void SetVector(int x, int y, Vector2f vector, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            switch (mode)
            {
                case WRAP_MODE.NONE:
                    this[x, y] = vector;
                    break;

                case WRAP_MODE.CLAMP:
                    SetClamped(x, y, vector);
                    break;

                case WRAP_MODE.WRAP:
                    SetWrapped(x, y, vector);
                    break;

                case WRAP_MODE.MIRROR:
                    SetMirrored(x, y, vector);
                    break;

                default:
                    this[x, y] = vector;
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
            var vec = GetVector(x, y, mode);
            return new ColorRGB(vec.x, vec.y, 0);
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
            var vec = GetVector(u, v, mode);
            return new ColorRGB(vec.x, vec.y, 0);
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
                    this[x, y] = new Vector2f(pixel.r, pixel.g);
                    break;

                case WRAP_MODE.CLAMP:
                    SetClamped(x, y, new Vector2f(pixel.r, pixel.g));
                    break;

                case WRAP_MODE.WRAP:
                    SetWrapped(x, y, new Vector2f(pixel.r, pixel.g));
                    break;

                case WRAP_MODE.MIRROR:
                    SetMirrored(x, y, new Vector2f(pixel.r, pixel.g));
                    break;

                default:
                    this[x, y] = new Vector2f(pixel.r, pixel.g);
                    break;
            }
        }

        /// <summary>
        /// Return a copy of the image.
        /// </summary>
        /// <returns></returns>
        public VectorImage2D Copy()
        {
            return new VectorImage2D(Data);
        }

    }

}
