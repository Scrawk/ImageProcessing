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
    public partial class ColorImage2D : Image2D<ColorRGBA>
    {

        /// <summary>
        /// Create a default of image.
        /// </summary>
        public ColorImage2D()
            : this(1, 1, ColorRGBA.Black)
        {

        }

        /// <summary>
        /// Create a image of a given size.
        /// </summary>
        /// <param name="size">The size of the image. x is the width and y is the height.</param>
        public ColorImage2D(Point2i size)
            : this(size.x, size.y, ColorRGBA.Black)
        {
 
        }

        /// <summary>
        /// Create a image of a given width and height.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public ColorImage2D(int width, int height)
             : this(width, height, ColorRGBA.Black)
        {

        }

        /// <summary>
        /// Create a image of a given width and height and filled with a value.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="value">The value to fill the image with.</param>
        public ColorImage2D(int width, int height, ColorRGBA value)
        {
            Data = new ColorRGBA[width, height];
            Fill(value);
        }

        /// <summary>
        /// Create a image from the given data.
        /// </summary>
        /// <param name="data">The images data.</param>
        public ColorImage2D(ColorRGBA[,] data)
        {
            Data = data.Copy();
        }

        /// <summary>
        /// The images pixels.
        /// </summary>
        private ColorRGBA[,] Data;

        /// <summary>
        /// The images mipmaps.
        /// CreateMipmaps must be called for the image to have mipmaps.
        /// </summary>
        private ColorImage2D[] Mipmaps { get; set; }

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
        public override int Channels => 4;

        /// <summary>
        /// The number of mipmap levels in image.
        /// CreateMipmaps must be called for the image to have mipmaps.
        /// </summary>
        public override int MipmapLevels => (Mipmaps != null) ? Mipmaps.Length : 0;

        /// <summary>
        /// Access a element at index x,y.
        /// </summary>
        public override ColorRGBA this[int x, int y]
        {
            get { return Data[x, y]; }
            set { Data[x, y] = value; }
        }

        /// <summary>
        /// Access a element at index x,y and mipmap m.
        /// </summary>
        public ColorRGBA this[int x, int y, int m]
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
            return string.Format("[ColorImage2D: Name={0}, Width={1}, Height={2}, Channels={3}, Mipmaps={4}]",
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
            Data = new ColorRGBA[width, height];
            Data.Fill(ColorRGBA.Black);
        }

        /// <summary>
        /// Get a channels value from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="c">The channel index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The value at index x,y.</returns>
        public override float GetChannel(int x, int y, int c, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return GetPixel(x, y, mode)[c];
        }

        /// <summary>
        /// Get a channels value from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="c">The channel index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The value at index x,y.</returns>
        public override float GetChannel(float u, float v, int c, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            float x = u * (Width - 1);
            float y = v * (Height - 1);

            int xi = (int)x;
            int yi = (int)y;
            int xi1 = xi + 1;
            int yi1 = yi + 1;

            Indices(ref xi, ref yi, mode);
            Indices(ref xi1, ref yi1, mode);

            float v00 = this[xi, yi][c];
            float v10 = this[xi1, yi][c];
            float v01 = this[xi, yi1][c];
            float v11 = this[xi1, yi1][c];

            return MathUtil.BLerp(v00, v10, v01, v11, x - xi, y - yi);
        }

        /// <summary>
        /// Get a value from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public override ColorRGBA GetValue(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return GetPixel(x, y, mode);
        }

        /// <summary>
        /// Get a value from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public override ColorRGBA GetValue(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return GetPixel(u, v, mode);
        }

        /// <summary>
        /// Set the channel value at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="c">The channel index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <param name="value">The value.</param>
        public override void SetChannel(int x, int y, int c, float value, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            var pixel = GetPixel(x, y, mode);
            pixel[c] = value;
            SetPixel(x, y, pixel, mode);
        }

        /// <summary>
        /// Set the value at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <param name="value">The value.</param>
        public override void SetValue(int x, int y, ColorRGBA value, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            SetPixel(x, y, value, mode);
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
            Indices(ref x, ref y, mode);
            return this[x, y];
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
            float x = u * (Width - 1);
            float y = v * (Height - 1);

            int xi = (int)x;
            int yi = (int)y;
            int xi1 = xi + 1;
            int yi1 = yi + 1;

            Indices(ref xi, ref yi, mode);
            Indices(ref xi1, ref yi1, mode);

            ColorRGBA v00 = this[xi, yi];
            ColorRGBA v10 = this[xi1, yi];
            ColorRGBA v01 = this[xi, yi1];
            ColorRGBA v11 = this[xi1, yi1];

            return ColorRGBA.BLerp(v00, v10, v01, v11, x - xi, y - yi);
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
                    this[x, y] = ColorRGBA.AlphaBlend(pixel, this[x, y]);
                    break;

                case BLEND_MODE.NONE:
                    this[x, y] = pixel;
                    break;

                default:
                    this[x, y] = pixel;
                    break;
            }
        }

        /// <summary>
        /// Get the values of a row in the image. 
        /// </summary>
        /// <param name="row">The array to hold the values.</param>
        /// <param name="y">The column index.</param>
        /// <exception cref="ArgumentException">Thrown if the row length does not match the images width.</exception>
        public void GetRow(ColorRGBA[] row, int y)
        {
            if (row.Length != Width)
                throw new ArgumentException("The row length must be equal to the images width.");

            for (int x = 0; x < Width; x++)
            {
                row[x] = this[x, y];
            }
        }

        /// <summary>
        /// Get the values of a row in the image. 
        /// </summary>
        /// <param name="row">The array to hold the values.</param>
        /// <param name="y">The column index.</param>
        /// <param name="c">The channels index.</param>
        /// <exception cref="ArgumentException">Thrown if the row length does not match the images width.</exception>
        public void GetRow(float[] row, int y, int c)
        {
            if (row.Length != Width)
                throw new ArgumentException("The row length must be equal to the images width.");

            if (c < 0 || c >= Channels)
                throw new ArgumentException($"The channel index must be > 0 and < {Channels}.");

            for (int x = 0; x < Width; x++)
            {
                row[x] = this[x, y][c];
            }
        }

        /// <summary>
        /// Set the values of a row in the image. 
        /// </summary>
        /// <param name="row">The array to holding the values.</param>
        /// <param name="y">The column index.</param>
        /// <exception cref="ArgumentException">Thrown if the row length does not match the images width.</exception>
        public void SetRow(ColorRGBA[] row, int y)
        {
            if (row.Length != Width)
                throw new ArgumentException("The row length must be equal to the images width.");

            for (int x = 0; x < Width; x++)
            {
                this[x, y] = row[x];
            }
        }

        /// <summary>
        /// Set the values of a row in the image. 
        /// </summary>
        /// <param name="row">The array to holding the values.</param>
        /// <param name="y">The column index.</param>
        /// <param name="c">The channels index.</param>
        /// <exception cref="ArgumentException">Thrown if the row length does not match the images width.</exception>
        public void SetRow(float[] row, int y, int c)
        {
            if (row.Length != Width)
                throw new ArgumentException("The row length must be equal to the images width.");

            if (c < 0 || c >= Channels)
                throw new ArgumentException($"The channel index must be > 0 and < {Channels}.");

            for (int x = 0; x < Width; x++)
            {
                var v = this[x, y];
                v[c] = row[x];
                this[x, y] = v;
            }
        }

        /// <summary>
        /// Get the values of a column in the image. 
        /// </summary>
        /// <param name="column">The array to hold the values.</param>
        /// <param name="x">The column index.</param>
        /// <exception cref="ArgumentException">Thrown if the column length does not match the images height.</exception>
        public void GetColumn(ColorRGBA[] column, int x)
        {
            if (column.Length != Height)
                throw new ArgumentException("The column length must be equal to the images height.");

            for (int y = 0; y < Height; y++)
            {
                column[y] = this[x, y];
            }
        }

        /// <summary>
        /// Get the values of a column in the image. 
        /// </summary>
        /// <param name="column">The array to hold the values.</param>
        /// <param name="x">The column index.</param>
        /// <param name="c">The channels index.</param>
        /// <exception cref="ArgumentException">Thrown if the column length does not match the images height.</exception>
        public void GetColumn(float[] column, int x, int c)
        {
            if (column.Length != Height)
                throw new ArgumentException("The column length must be equal to the images height.");

            if (c < 0 || c >= Channels)
                throw new ArgumentException($"The channel index must be > 0 and < {Channels}.");

            for (int y = 0; y < Height; y++)
            {
                column[y] = this[x, y][c];
            }
        }

        /// <summary>
        /// Set the values of a column in the image. 
        /// </summary>
        /// <param name="column">The array to holding the values.</param>
        /// <param name="x">The column index.</param>
        /// <exception cref="ArgumentException">Thrown if the column length does not match the images height.</exception>
        public void SetColumn(ColorRGBA[] column, int x)
        {
            if (column.Length != Height)
                throw new ArgumentException("The column length must be equal to the images height.");

            for (int y = 0; y < Height; y++)
            {
                this[x, y] = column[y];
            }
        }

        /// <summary>
        /// Set the values of a column in the image. 
        /// </summary>
        /// <param name="column">The array to holding the values.</param>
        /// <param name="x">The column index.</param>
        /// <param name="c">The channels index.</param>
        /// <exception cref="ArgumentException">Thrown if the column length does not match the images height.</exception>
        public void SetColumn(float[] column, int x, int c)
        {
            if (column.Length != Height)
                throw new ArgumentException("The column length must be equal to the images height.");

            if (c < 0 || c >= Channels)
                throw new ArgumentException($"The channel index must be > 0 and < {Channels}.");

            for (int y = 0; y < Height; y++)
            {
                var v = this[x, y];
                v[c] = column[y];
                this[x, y] = v;
            }
        }

        /// <summary>
        /// Return a copy of the image.
        /// </summary>
        /// <returns></returns>
        public ColorImage2D Copy()
        {
            var copy = new ColorImage2D(Data);
            base.Copy(copy);

            if (HasMipmaps)
            {
                copy.Mipmaps = new ColorImage2D[Mipmaps.Length];
                for (int i = 0; i < Mipmaps.Length; i++)
                    copy.Mipmaps[i] = Mipmaps[i].Copy();
            }

            return copy;
        }

        /// <summary>
        /// Convert to a greyscale image.
        /// </summary>
        /// <param name="c">The channel to use.</param>
        /// <returns>The greayscale image.</returns>
        public GreyScaleImage2D ToGreyScaleImage(int c)
        {
            var copy = new GreyScaleImage2D(Width, Height);
            copy.Iterate((x, y) =>
            {
                copy[x, y] = this[x, y][c];
            });

            return copy;
        }

        /// <summary>
        /// Get the mipmap at index i.
        /// </summary>
        /// <param name="i">The mipmap level.</param>
        /// <returns>The mipmap at index i.</returns>
        /// <exception cref="IndexOutOfRangeException">If the index is out of bounds or if there are no mipmaps.</exception>
        public ColorImage2D GetMipmap(int i)
        {
            if (i < 0 || i >= MipmapLevels)
                throw new IndexOutOfRangeException("The mipmap level " + i + "is out of range.");

            return Mipmaps[i];
        }

        /// <summary>
        /// Get the first mipmap.
        /// </summary>
        public ColorImage2D FirstMipmap
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
        public ColorImage2D LastMipmap
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
        /// <param name="wrap">The wrap mode to use.</param>
        /// <param name="method">The interpolation method to use.</param>
        /// <exception cref="ArgumentException">If max levels is not greater than 0.</exception>
        public override void CreateMipmaps(int maxLevel, WRAP_MODE wrap = WRAP_MODE.CLAMP, RESCALE method = RESCALE.BICUBIC)
        {
            if(maxLevel <= 0)
                throw new ArgumentException($"Max levels ({maxLevel}) must be greater that 0.");

            ColorImage2D image = this;
            var levels = new List<ColorImage2D>();
            levels.Add(image);

            int min = Math.Min(image.Width, image.Height);
 
            while (min > 1 && levels.Count < maxLevel)
            {
                image = Rescale(image, image.Width / 2, image.Height / 2, method, wrap);
                levels.Add(image);

                min = Math.Min(image.Width, image.Height);
            }

            Mipmaps = levels.ToArray();
        }

    }

}
