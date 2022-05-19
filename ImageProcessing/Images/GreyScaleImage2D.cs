using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Extensions;

namespace ImageProcessing.Images
{
    /// <summary>
    /// A 2D image containing only float values.
    /// </summary>
    public partial class GreyScaleImage2D : Image2D<float>
    {

        /// <summary>
        /// Create a default of image.
        /// </summary>
        public GreyScaleImage2D()
             : this(1, 1)
        {

        }

        /// <summary>
        /// Create a image of a given width and height and filled with a value.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="value">The value to fill the image with.</param>
        public GreyScaleImage2D(int width, int height, float value)
            : this(width, height)
        {
            FillWithValue(value);
        }

        /// <summary>
        /// Create a image of a given width and height.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public GreyScaleImage2D(int width, int height)
        {
            Data = new float[width, height];
        }

        /// <summary>
        /// Create a image of a given size.
        /// </summary>
        /// <param name="size">The size of the image. x is the width and y is the height.</param>
        public GreyScaleImage2D(Point2i size)
        {
            Data = new float[size.x, size.y];
        }

        /// <summary>
        /// Create a image from the given data.
        /// </summary>
        /// <param name="data">The images data.</param>
        public GreyScaleImage2D(float[,] data)
        {
            Data = data.Copy();
        }

        /// <summary>
        /// The images pixels.
        /// </summary>
        private float[,] Data;

        /// <summary>
        /// The images mipmaps.
        /// CreateMipmaps must be called for the image to have mipmaps.
        /// </summary>
        private GreyScaleImage2D[] Mipmaps { get; set; }

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
        public override int Channels => 1;

        /// <summary>
        /// The number of mipmap levels in image.
        /// CreateMipmaps must be called for the image to have mipmaps.
        /// </summary>
        public override int MipmapLevels => (Mipmaps != null) ? Mipmaps.Length : 0;

        /// <summary>
        /// Access a element at index x,y.
        /// </summary>
        public override float this[int x, int y]
        {
            get { return Data[x, y]; }
            set { Data[x, y] = value; }
        }

        /// <summary>
        /// Access a element at index x,y and mipmap m.
        /// </summary>
        public float this[int x, int y, int m]
        {
            get { return GetMipmap(m).Data[x, y]; }
            set { GetMipmap(m).Data[x, y] = value; }
        }

        /// <summary>
        /// Return the image description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[GreyScaleImage2D: Name={0}, Width={1}, Height={2}, Channels={3}, Mipmaps={4}]",
                Name, Width, Height, Channels, MipmapLevels);
        }

        /// <summary>
        /// Sets all elements in the array to default value.
        /// </summary>
        public override void Clear()
        {
            Data.Clear();
            ClearMipmaps();
        }

        /// <summary>
        /// Clear the image of all mipmaps.
        /// </summary>
        public override void ClearMipmaps()
        {
            Mipmaps = null;
        }

        /// <summary>
        /// Resize the array. Will clear any existing data.
        /// </summary>
        public override void Resize(int width, int height)
        {
            Data = new float[width, height];
        }

        /// <summary>
        /// Get a value from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The value at index x,y.</returns>
        public override float GetValue(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            Indices(ref x, ref y, mode);
            return this[x, y];
        }

        /// <summary>
        /// Get a value from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The value at index x,y.</returns>
        public override float GetValue(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float x = u * (Width - 1);
            float y = v * (Height - 1);

            int xi = (int)x;
            int yi = (int)y;
            int xi1 = xi + 1;
            int yi1 = yi + 1;

            Indices(ref xi, ref yi, mode);
            Indices(ref xi1, ref yi1, mode);

            float v00 = this[xi, yi];
            float v10 = this[xi1, yi];
            float v01 = this[xi, yi1];
            float v11 = this[xi1, yi1];

            return MathUtil.BLerp(v00, v10, v01, v11, x - xi, y - yi);
        }

        /// <summary>
        /// Set the value at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="value">The value.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        public override void SetValue(int x, int y, float value, WRAP_MODE mode = WRAP_MODE.NONE)
        {
            Indices(ref x, ref y, mode);
            this[x, y] = value;
        }

        /// <summary>
        /// Get a pixel from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public override ColorRGBA GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float value = GetValue(x, y, mode);
            return new ColorRGBA(value, 1);
        }

        /// <summary>
        /// Get a pixel from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public override ColorRGBA GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var value = GetValue(u, v, mode);
            return new ColorRGBA(value, 1);
        }

        /// <summary>
        /// Get a pixels channel value from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="c">The pixels channel index (0-2).</param>
        /// <param name="mode">The wrap mode for indices outside image bounds</param>
        /// <returns>The pixels channel at index x,y,c.</returns>
        public override float GetChannel(int x, int y, int c, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return GetValue(x, y, mode);
        }

        /// <summary>
        /// Get a pixels channel value from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="c">The pixels channel index (0-2).</param>
        /// <param name="mode">The wrap mode for indices outside image bounds</param>
        /// <returns>The pixels channel at index x,y,c.</returns>
        public override float GetChannel(float u, float v, int c, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return GetValue(u, v, mode);
        }

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="pixel">The pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <param name="blend">The mode pixels are blended based on there alpha value. 
        /// Only applies to images with a alpha channel.</param>
        public override void SetPixel(int x, int y, ColorRGBA pixel, WRAP_MODE mode = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            Indices(ref x, ref y, mode);

            switch (blend)
            {
                case BLEND_MODE.ALPHA:
                    this[x, y] = pixel.Intensity * pixel.a;
                    break;

                case BLEND_MODE.NONE:
                    this[x, y] = pixel.Intensity;
                    break;

                default:
                    this[x, y] = pixel.Intensity;
                    break;
            }
        }

        /// <summary>
        /// Set the pixels channel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="c">The pixels channel index (0-2).</param>
        /// <param name="value">The pixels channel value.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        public override void SetChannel(int x, int y, int c, float value, WRAP_MODE mode = WRAP_MODE.NONE)
        {
            SetValue(x, y, value, mode);
        }

        /// <summary>
        /// Get the values of a row in the image. 
        /// </summary>
        /// <param name="row">The array to hold the values.</param>
        /// <param name="y">The column index.</param>
        /// <exception cref="ArgumentException">Thrown if the row length does not match the images width.</exception>
        public void GetRow(float[] row, int y)
        {
            if (row.Length != Width)
                throw new ArgumentException("The row length must be equal to the images width.");

            for(int x = 0; x < Width; x++)
            {
                row[x] = this[x, y];
            }
        }

        /// <summary>
        /// Set the values of a row in the image. 
        /// </summary>
        /// <param name="row">The array to holding the values.</param>
        /// <param name="y">The column index.</param>
        /// <exception cref="ArgumentException">Thrown if the row length does not match the images width.</exception>
        public void SetRow(float[] row, int y)
        {
            if (row.Length != Width)
                throw new ArgumentException("The row length must be equal to the images width.");

            for (int x = 0; x < Width; x++)
            {
                this[x, y] = row[x];
            }
        }

        /// <summary>
        /// Get the values of a column in the image. 
        /// </summary>
        /// <param name="column">The array to hold the values.</param>
        /// <param name="x">The column index.</param>
        /// <exception cref="ArgumentException">Thrown if the column length does not match the images height.</exception>
        public void GetColumn(float[] column, int x)
        {
            if (column.Length != Height)
                throw new ArgumentException("The column length must be equal to the images height.");

            for (int y = 0; y < Height; y++)
            {
                column[y] = this[x, y];
            }
        }

        /// <summary>
        /// Set the values of a column in the image. 
        /// </summary>
        /// <param name="column">The array to holding the values.</param>
        /// <param name="x">The column index.</param>
        /// <exception cref="ArgumentException">Thrown if the column length does not match the images height.</exception>
        public void SetColumn(float[] column, int x)
        {
            if (column.Length != Height)
                throw new ArgumentException("The column length must be equal to the images height.");

            for (int y = 0; y < Height; y++)
            {
                this[x, y] = column[y];
            }
        }

        /// <summary>
        /// Return a copy of the image.
        /// </summary>
        /// <returns></returns>
        public GreyScaleImage2D Copy()
        {
            var copy = new GreyScaleImage2D(Data);
            base.Copy(copy);

            if (HasMipmaps)
            {
                copy.Mipmaps = new GreyScaleImage2D[Mipmaps.Length];
                for (int i = 0; i < Mipmaps.Length; i++)
                    copy.Mipmaps[i] = Mipmaps[i].Copy();
            }

            return copy;
        }

        /// <summary>
        /// Get the slope from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public float GetSlope(int x, int y, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var d = GetFirstDerivative(x, y, w, mode);
            float p = d.x * d.x + d.y * d.y;
            float g = MathUtil.SafeSqrt(p);
            return MathUtil.Atan(g) * MathUtil.RAD_TO_DEG_32 / 90.0f;
        }

        /// <summary>
        /// Get the slope from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public float GetSlope(float u, float v, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var d = GetFirstDerivative(u, v, w, mode);
            float p = d.x * d.x + d.y * d.y;
            float g = MathUtil.SafeSqrt(p);
            return MathUtil.Atan(g) * MathUtil.RAD_TO_DEG_32 / 90.0f;
        }

        /// <summary>
        /// Get the normal from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public Vector3f GetNormal(int x, int y, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var d = GetFirstDerivative(x, y, w, mode);
            var n = new Vector3f(d.x, 1, d.y);
            return n.Normalized;
        }

        /// <summary>
        /// Get the normal from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public Vector3f GetNormal(float u, float v, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var d = GetFirstDerivative(u, v, w, mode);
            var n = new Vector3f(d.x, 1, d.y);
            return n.Normalized;
        }

        /// <summary>
        /// Get the frist derivative from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public Vector2f GetFirstDerivative(int x, int y, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float z1 = GetValue(x - 1, y + 1, mode);
            float z2 = GetValue(x + 0, y + 1, mode);
            float z3 = GetValue(x + 1, y + 1, mode);
            float z4 = GetValue(x - 1, y + 0, mode);
            float z6 = GetValue(x + 1, y + 0, mode);
            float z7 = GetValue(x - 1, y - 1, mode);
            float z8 = GetValue(x + 0, y - 1, mode);
            float z9 = GetValue(x + 1, y - 1, mode);

            //p, q
            float zx = (z3 + z6 + z9 - z1 - z4 - z7) / (6.0f * w.x);
            float zy = (z1 + z2 + z3 - z7 - z8 - z9) / (6.0f * w.y);

            return new Vector2f(-zx, -zy);
        }

        /// <summary>
        /// Get the first derivative from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public Vector2f GetFirstDerivative(float u, float v, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float x1 = Width > 1 ? 1.0f / Width : 0;
            float y1 = Height > 1 ? 1.0f / Height : 0;
            float z1 = GetValue(u - x1, v + y1, mode);
            float z2 = GetValue(u +  0, v + y1, mode);
            float z3 = GetValue(u + x1, v + y1, mode);
            float z4 = GetValue(u - x1, v +  0, mode);
            float z6 = GetValue(u + x1, v +  0, mode);
            float z7 = GetValue(u - x1, v - y1, mode);
            float z8 = GetValue(u +  0, v - y1, mode);
            float z9 = GetValue(u + x1, v - y1, mode);

            //p, q
            float zx = (z3 + z6 + z9 - z1 - z4 - z7) / (6.0f * w.x);
            float zy = (z1 + z2 + z3 - z7 - z8 - z9) / (6.0f * w.y);

            return new Vector2f(-zx, -zy);
        }

        /// <summary>
        /// Get the first and second derivative from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public (Vector2f d1, Vector3f d2) GetFirstAndSecondDerivative(int x, int y, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float wx2 = w.x * w.x;
            float wy2 = w.y * w.y;
            float wxy2 = w.SqrMagnitude;
            float z1 = GetValue(x - 1, y + 1, mode);
            float z2 = GetValue(x + 0, y + 1, mode);
            float z3 = GetValue(x + 1, y + 1, mode);
            float z4 = GetValue(x - 1, y + 0, mode);
            float z5 = GetValue(x + 0, y + 0, mode);
            float z6 = GetValue(x + 1, y + 0, mode);
            float z7 = GetValue(x - 1, y - 1, mode);
            float z8 = GetValue(x + 0, y - 1, mode);
            float z9 = GetValue(x + 1, y - 1, mode);

            //p, q
            float zx = (z3 + z6 + z9 - z1 - z4 - z7) / (6.0f * w.x);
            float zy = (z1 + z2 + z3 - z7 - z8 - z9) / (6.0f * w.y);

            //r, t, s
            float zxx = (z1 + z3 + z4 + z6 + z7 + z9 - 2.0f * (z2 + z5 + z8)) / (3.0f * wx2);
            float zyy = (z1 + z2 + z3 + z7 + z8 + z9 - 2.0f * (z4 + z5 + z6)) / (3.0f * wy2);
            float zxy = (z3 + z7 - z1 - z9) / (4.0f * wxy2);

            var d1 = new Vector2f(-zx, -zy);
            var d2 = new Vector3f(-zxx, -zyy, -zxy);

            return (d1, d2);
        }

        /// <summary>
        /// Get the first and second derivative from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="w">The size of the pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns></returns>
        public (Vector2f d1, Vector3f d2) GetFirstAndSecondDerivative(float u, float v, Vector2f w, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float x1 = Width > 1 ? 1.0f / Width : 0;
            float y1 = Height > 1 ? 1.0f / Height : 0;

            float wx2 = w.x * w.x;
            float wy2 = w.y * w.y;
            float wxy2 = w.SqrMagnitude;
            float z1 = GetValue(u - x1, v + y1, mode);
            float z2 = GetValue(u +  0, v + y1, mode);
            float z3 = GetValue(u + x1, v + y1, mode);
            float z4 = GetValue(u - x1, v +  0, mode);
            float z5 = GetValue(u +  0, v +  0, mode);
            float z6 = GetValue(u + x1, v +  0, mode);
            float z7 = GetValue(u - x1, v - y1, mode);
            float z8 = GetValue(u +  0, v - y1, mode);
            float z9 = GetValue(u + x1, v - y1, mode);

            //p, q
            float zx = (z3 + z6 + z9 - z1 - z4 - z7) / (6.0f * w.x);
            float zy = (z1 + z2 + z3 - z7 - z8 - z9) / (6.0f * w.y);

            //r, t, s
            float zxx = (z1 + z3 + z4 + z6 + z7 + z9 - 2.0f * (z2 + z5 + z8)) / (3.0f * wx2);
            float zyy = (z1 + z2 + z3 + z7 + z8 + z9 - 2.0f * (z4 + z5 + z6)) / (3.0f * wy2);
            float zxy = (z3 + z7 - z1 - z9) / (4.0f * wxy2);

            var d1 = new Vector2f(-zx, -zy);
            var d2 = new Vector3f(-zxx, -zyy, -zxy);

            return (d1, d2);
        }

        /// <summary>
        /// Get the mipmap at index i.
        /// </summary>
        /// <param name="i">The mipmap level.</param>
        /// <returns>The mipmap at index i.</returns>
        /// <exception cref="IndexOutOfRangeException">If the index is out of bounds or if there are no mipmaps.</exception>
        public GreyScaleImage2D GetMipmap(int i)
        {
            if (i < 0 || i >= MipmapLevels)
                throw new IndexOutOfRangeException("The mipmap level " + i + "is out of range.");

            return Mipmaps[i];
        }

        /// <summary>
        /// Get the first mipmap.
        /// </summary>
        public GreyScaleImage2D FirstMipmap
        {
            get
            {
                if (Mipmaps == null)
                    throw new InvalidOperationException("Mipmaps have not been created.");

                return Mipmaps[0];
            }
        }

        /// <summary>
        /// Get the last mipmap.
        /// </summary>
        public GreyScaleImage2D LastMipmap
        {
            get
            {
                if (Mipmaps == null)
                    throw new InvalidOperationException("Mipmaps have not been created.");

                int m = Mipmaps.Length - 1;
                return Mipmaps[m];
            }
        }

        /// <summary>
        /// Get the mipmap at index i.
        /// </summary>
        /// <param name="i">The mipmap level.</param>
        /// <returns>The mipmap at index i.</returns>
        protected override IImage2D GetMipmapInterface(int i)
        {
            if (i < 0 || i >= MipmapLevels)
                throw new IndexOutOfRangeException("The mipmap level " + i + "is out of range.");

            return Mipmaps[i];
        }

        /// <summary>
        /// Creates the images mipmaps.
        /// </summary>
        /// <param name="maxLevel">The max level of mipmaps to create. -1 to ignore</param>
        /// <param name="mode">The wrap mode to use.</param>
        /// <param name="method">The interpolation method to use.</param>
        /// <exception cref="ArgumentException">If max levels is not greater than 0.</exception>
        public override void CreateMipmaps(int maxLevel, WRAP_MODE mode = WRAP_MODE.CLAMP, RESCALE method = RESCALE.BICUBIC)
        {
            if (maxLevel <= 0)
                throw new ArgumentException($"Max levels ({maxLevel}) must be greater that 0.");

            GreyScaleImage2D image = this;
            var levels = new List<GreyScaleImage2D>();
            levels.Add(image);

            int min = Math.Min(image.Width, image.Height);

            while (min > 1 && levels.Count < maxLevel)
            {
                image = Rescale(image, image.Width / 2, image.Height / 2, mode, method);
                levels.Add(image);

                min = Math.Min(image.Width, image.Height);
            }

            Mipmaps = levels.ToArray();
        }

    }

}
