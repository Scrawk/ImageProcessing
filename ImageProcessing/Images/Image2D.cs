using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;
using Common.Core.Threading;
using Common.Core.Extensions;

using ImageProcessing.Pixels;

namespace ImageProcessing.Images
{
    /// <summary>
    /// Base class for 2D images.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    public abstract partial class Image2D<T> : IImage2D
    {

        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// The size of the arrays 1st dimention.
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        /// The size of the arrays 2st dimention.
        /// </summary>
        public abstract int Height { get; }

        /// <summary>
        /// The number of channels in the images pixel.
        /// </summary>
        public abstract int Channels { get; }

        /// <summary>
        /// The number of mipmap levels in image.
        /// CreateMipmaps must be called for the image to have mipmaps.
        /// </summary>
        public abstract int MipmapLevels { get; }

        /// <summary>
        /// Does the image have mipmaps.
        /// </summary>
        public bool HasMipmaps => MipmapLevels > 0;

        /// <summary>
        /// The size of the image as a vector.
        /// </summary>
        public Point2i Size => new Point2i(Width, Height);

        /// <summary>
        /// The size of the image as a box.
        /// </summary>
        public Box2i Bounds => new Box2i(new Point2i(0, 0), Size-1);

        /// <summary>
        /// Tag for algorithms on the image can mark or 
        /// id the image if needed. May change at any point.
        /// </summary>
        public int Tag { get; set; }

        /// <summary>
        /// The images optional name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Access a element at index x,y.
        /// </summary>
        public abstract T this[int x, int y]
        {
            get;
            set;
        }

        /// <summary>
        /// The images description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Image2D: Name={0}, Width={1}, Height={2}, Channels={3}, Mipmaps={4}]", 
                Name, Width, Height, Channels, MipmapLevels);
        }

        /// <summary>
        /// Clear the image of all data.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Clear the image of all mipmaps.
        /// </summary>
        public abstract void ClearMipmaps();

        /// <summary>
        /// Helper function to create generic image.
        /// </summary>
        /// <typeparam name="IMAGE">THe images generic type.</typeparam>
        /// <param name="width">The images width.</param>
        /// <param name="height">The images height.</param>
        /// <returns>The image.</returns>
        protected static IMAGE NewImage<IMAGE>(int width, int height)
            where IMAGE : IImage2D, new()
        {
            var image = new IMAGE();
            image.Resize(width, height);
            return image;
        }

        /// <summary>
        /// Create a hashcode by hasing all pixels in the image together.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public int HashCode()
        {
            unchecked
            {
                int hash = (int)MathUtil.HASH_PRIME_1;

                Iterate((x, y) =>
                {
                    hash = (hash * MathUtil.HASH_PRIME_2) ^ this[x,y].GetHashCode();
                });

                return hash;
            }
        }

        /// <summary>
        /// Get a pixel from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public abstract ColorRGBA GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a pixel from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public abstract ColorRGBA GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a pixel from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="m">The mipmap index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public ColorRGBA GetPixelMipmap(int x, int y, int m, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return GetMipmapInterface(m).GetPixel(x, y, mode);
        }

        /// <summary>
        /// Get a pixel from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="m">The mipmap index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public ColorRGBA GetPixelMipmap(float u, float v, int m, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            return GetMipmapInterface(m).GetPixel(u, v, mode);
        }

        /// <summary>
        /// Get a pixel from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="m">The mipmap normalized (0-1) index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public ColorRGBA GetPixelMipmap(float u, float v, float m, WRAP_MODE mode = WRAP_MODE.CLAMP)
        {
            int levels = MipmapLevels - 1;
 
            float m0 = MathUtil.Clamp(m * levels, 0.0f, levels);
            float m1 = MathUtil.Clamp((m * levels) + 1, 0.0f, levels);
            float a = m1 - m0;

            var p0 = GetMipmapInterface((int)m0).GetPixel(u, v, mode);
            var p1 = GetMipmapInterface((int)m1).GetPixel(u, v, mode);

            return ColorRGBA.Lerp(p0, p1, a);
        }

        /// <summary>
        /// Get a pixels channel value from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="c">The pixels channel index (0-3).</param>
        /// <param name="mode">The wrap mode for indices outside image bounds</param>
        /// <returns>The pixels channel at index x,y,c.</returns>
        public abstract float GetChannel(int x, int y, int c, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a pixels channel value from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="c">The pixels channel index (0-3).</param>
        /// <param name="mode">The wrap mode for indices outside image bounds</param>
        /// <returns>The pixels channel at index x,y,c.</returns>
        public abstract float GetChannel(float u, float v, int c, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a value from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds</param>
        /// <returns>The pixels channel at index x,y,c.</returns>
        public abstract T GetValue(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a value from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds</param>
        /// <returns>The pixels channel at index x,y,c.</returns>
        public abstract T GetValue(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="pixel">The pixel.</param>
        /// <param name="wrap">The wrap mode for indices outside image bounds.</param>
        /// <param name="blend">The mode pixels are blended based on there alpha value. 
        /// Only applies to images with a alpha channel.</param>
        public abstract void SetPixel(int x, int y, ColorRGBA pixel, WRAP_MODE wrap = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA);

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="m">The mipmap index.</param>
        /// <param name="pixel">The pixel.</param>
        /// <param name="wrap">The wrap mode for indices outside image bounds.</param>
        /// <param name="blend">The mode pixels are blended based on there alpha value. 
        /// Only applies to images with a alpha channel.</param>
        public void SetPixelMipmap(int x, int y, int m, ColorRGBA pixel, WRAP_MODE wrap = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            GetMipmapInterface(m).SetPixel(x, y, pixel, wrap, blend);
        }

        /// <summary>
        /// Set the pixels channel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="c">The pixels channel index (0-2).</param>
        /// <param name="value">The pixels channel value.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        public abstract void SetChannel(int x, int y, int c, float value, WRAP_MODE mode = WRAP_MODE.NONE);

        /// <summary>
        /// Set the value at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="value">The pixels channel value.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        public abstract void SetValue(int x, int y, T value, WRAP_MODE mode = WRAP_MODE.NONE);

        /// <summary>
        /// Is this array the same size as the other array.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool AreSameSize(IImage2D image)
        {
            if (Width != image.Width) return false;
            if (Height != image.Height) return false;
            return true;
        }

        /// <summary>
        /// Are the images the same size.
        /// </summary>
        /// <param name="image"></param>
        /// <returns>True if the images are not the same size.</returns>
        public bool AreNotSameSize(IImage2D image)
        {
            return !AreSameSize(image);
        }

        /// <summary>
        /// Are the x and y index in the bounds of the array.
        /// </summary>
        public bool InBounds(int x, int y)
        {
            if (x < 0 || x >= Width) return false;
            if (y < 0 || y >= Height) return false;
            return true;
        }

        /// <summary>
        /// Are the x and y index not in the bounds of the array.
        /// </summary>
        public bool NotInBounds(int x, int y)
        {
            return !InBounds(x, y);
        }

        /// <summary>
        /// Resize the array. Will clear any existing data.
        /// </summary>
        public abstract void Resize(int width, int height);

        /// <summary>
        /// Get the indices values depending on the wrapping mode.
        /// </summary>
        /// <param name="x">The index on the x axis.</param>
        /// <param name="y">The index on the y axis.</param>
        /// <param name="mode">The wrapping mode.</param>
        public void Indices(ref int x, ref int y, WRAP_MODE mode)
        {
            switch (mode)
            {
                case WRAP_MODE.CLAMP:
                    x = MathUtil.Clamp(x, 0, Width - 1);
                    y = MathUtil.Clamp(y, 0, Height - 1);
                    break;

                case WRAP_MODE.WRAP:
                    x = MathUtil.Wrap(x, Width);
                    y = MathUtil.Wrap(y, Height);
                    break;

                case WRAP_MODE.MIRROR:
                    x = MathUtil.Mirror(x, Width);
                    y = MathUtil.Mirror(y, Height);
                    break;

                case WRAP_MODE.NONE:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Recommended blocks for parallel processing.
        /// </summary>
        /// <param name="divisions">Number of divisions on each axis to make.</param>
        /// <returns></returns>
        public int BlockSize(int divisions = 4)
        {
            return ThreadingBlock2D.BlockSize(Width, Height, divisions);
        }

        /// <summary>
        /// Iterate over the array with the action.
        /// </summary>
        public void Iterate(Action<int, int> func)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    func(x, y);
                }
            }
        }

        /// <summary>
        /// Iterate over the array with the action in parallel.
        /// </summary>
        public void ParallelIterate(Action<int, int> func)
        {
            ParallelIterate(BlockSize(), func);
        }

        /// <summary>
        /// Iterate over the array with the action in parallel.
        /// </summary>
        public void ParallelIterate(int blockSize, Action<int, int> func)
        {
            var blocks = ThreadingBlock2D.CreateBlocks(Width, Height, blockSize);
            Parallel.ForEach(blocks, (block) =>
            {
                for (int y = block.Start.y; y <= block.End.y; y++)
                {
                    for (int x = block.Start.x; x <= block.End.x; x++)
                    {
                        func(x, y);
                    }
                }
            });
        }

        /// <summary>
        /// Fill the array with the value.
        /// </summary>
        public void Fill(T value)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    this[x, y] = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="wrap">The wrap mode for indices outside image bounds.</param>
        public void Fill(T[,] source, int x = 0, int y = 0, WRAP_MODE wrap = WRAP_MODE.CLAMP)
        {
            for (int j = 0; j < source.GetLength(0); j++)
            {
                for (int i = 0; i < source.GetLength(1); i++)
                {
                    SetValue(x + i, y + j,  source[i, j], wrap);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public void Fill(T[] source)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    this[x, y] = source[x + y * Width];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="image_wrap">The wrap mode for indices outside image bounds.</param>
        /// <param name="source_wrap">The wrap mode for indices outside source bounds.</param>
        public void Fill(Image2D<T> source, int x = 0, int y = 0, WRAP_MODE image_wrap = WRAP_MODE.CLAMP, WRAP_MODE source_wrap = WRAP_MODE.CLAMP)
        {
            for (int j = 0; j < source.Height; j++)
            {
                for (int i = 0; i < source.Width; i++)
                {
                    SetPixel(x + i, y + j, source.GetPixel(i, j, source_wrap), image_wrap);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="bounds"></param>
        /// <param name="image_wrap">The wrap mode for indices outside image bounds.</param>
        /// <param name="source_wrap">The wrap mode for indices outside source bounds.</param>
        public void Fill(Image2D<T> source, Box2i bounds, WRAP_MODE image_wrap = WRAP_MODE.CLAMP, WRAP_MODE source_wrap = WRAP_MODE.CLAMP)
        {
            for (int y = bounds.Min.y, yy = 0; y < bounds.Max.y; yy++, y++)
            {
                for (int x = bounds.Min.x, xx = 0; x < bounds.Max.x; xx++, x++)
                {
                    SetPixel(x, y, source.GetPixel(xx, yy, source_wrap), image_wrap);
                }
            }
        }

        /// <summary>
        /// Fill the array with the value from the function.
        /// </summary>
        public void Fill(Func<int, int, T> func)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    this[x, y] = func(x, y);
                }
            }
        }

        /// <summary>
        /// Fill the image with the values at the provided indices.
        /// </summary>
        /// <param name="indices">The indices and value to fill.</param>
        /// <param name="wrap">The wrap mode for indices outside image bounds.</param>
        public void Fill(IList<PixelIndex2D<T>> indices, WRAP_MODE wrap = WRAP_MODE.CLAMP)
        {
            for (int i = 0; i < indices.Count; i++)
            {
                var p = indices[i];
                SetValue(p.x, p.y, p.Value, wrap);
            }
        }

        /// <summary>
        /// Fill the image with the value at the provided indices.
        /// </summary>
        /// <param name="indices">The indices to fill.</param>
        /// <param name="value">The value to fill.</param>
        /// <param name="wrap">The wrap mode for indices outside image bounds.</param>
        public void Fill(IList<Point2i> indices, T value, WRAP_MODE wrap = WRAP_MODE.CLAMP)
        {
            for (int i = 0; i < indices.Count; i++)
            {
                var j = indices[i];
                SetValue(j.x, j.y, value, wrap);
            }
        }

        /// <summary>
        /// Fill the image with the value at the provided indices.
        /// </summary>
        /// <param name="bounds">The area to fill.</param>
        /// <param name="value">The value to fill.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="wrap">The wrap mode for indices outside image bounds.</param>
        public void Fill(Box2i bounds, T value, int x = 0, int y = 0, WRAP_MODE wrap = WRAP_MODE.CLAMP)
        {
            for (int j = bounds.Min.y; j < bounds.Max.y; j++)
            {
                for (int i = bounds.Min.x; i < bounds.Max.x; i++)
                {
                    SetValue(i + x, j + y, value, wrap);
                }
            }
        }

        /// <summary>
        /// Fill the array with the value from the function in parallel.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="wrap">The wrap mode for indices outside image bounds.</param>
        public void ParallelFill(Func<int, int, T> func, WRAP_MODE wrap = WRAP_MODE.CLAMP)
        {
            ParallelFill(BlockSize(), func);
        }

        /// <summary>
        /// Fill the array with the value from the function in parallel.
        /// </summary>
        /// <param name="blockSize"></param>
        /// <param name="func"></param>
        /// <param name="wrap">The wrap mode for indices outside image bounds.</param>
        public void ParallelFill(int blockSize, Func<int, int, T> func, WRAP_MODE wrap = WRAP_MODE.CLAMP)
        {
            var blocks = ThreadingBlock2D.CreateBlocks(Width, Height, blockSize);
            Parallel.ForEach(blocks, (block) =>
            {
                for (int y = block.Start.y; y <= block.End.y; y++)
                {
                    for (int x = block.Start.x; x <= block.End.x; x++)
                    {
                        SetValue(x, y, func(x,y), wrap);
                    }
                }
            });
        }

        /// <summary>
        /// Modify the array with the function.
        /// </summary>
        public void Modify(Func<T, T> func)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    this[x, y] = func(this[x, y]);
                }
            }
        }

        /// <summary>
        /// Modify the array with the function in parallel.
        /// </summary>
        public void ParallelModify(Func<T, T> func)
        {
            ParallelModify(BlockSize(), func);
        }

        /// <summary>
        /// Modify the array with the function in parallel.
        /// </summary>
        public void ParallelModify(int blockSize, Func<T, T> func)
        {
            var blocks = ThreadingBlock2D.CreateBlocks(Width, Height, blockSize);
            Parallel.ForEach(blocks, (block) =>
            {
                for (int y = block.Start.y; y <= block.End.y; y++)
                {
                    for (int x = block.Start.x; x <= block.End.x; x++)
                    {
                        this[x, y] = func(this[x, y]);
                    }
                }
            });
        }

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
        public void ToPixelIndexList(List<PixelIndex2D<T>> list, Func<T, bool> predicate)
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
        /// Convert to a 2D array.
        /// </summary>
        /// <returns>A 2D arry fill with the images values.</returns>
        public T[,] ToArray()
        {
            var array = new T[Height, Width];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    array[x, y] = this[x, y];
                }
            }

            return array;
        }

        /// <summary>
        /// Do these two images contain the the same contents.
        /// </summary>
        /// <param name="image">The other image.</param>
        /// <returns>Do these two images contain the the same contents.</returns>
        public bool AreEqual(Image2D<T> image)
        {
            if (image == this) return true;
            if (image.Size != Size) return false;
            
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var p1 = this[x, y];
                    var p2 = image[x, y];

                    if (p1.Equals(p2))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Convert to a greyscale image.
        /// </summary>
        /// <returns>The greayscale image.</returns>
        public GreyScaleImage2D ToGreyScaleImage()
        {
            var copy = new GreyScaleImage2D(Width, Height);
            copy.Iterate((x, y) =>
            {
                var pixel = GetPixel(x, y);
                copy.SetPixel(x, y, pixel);
            });

            return copy;
        }

        /// <summary>
        /// Convert to a binary image.
        /// </summary>
        /// <param name="threshold">The threshold that determines 
        /// if the images values are tru or false.</param>
        /// <returns>The binary image.</returns>
        public BinaryImage2D ToBinaryImage(float threshold = 0.5f)
        {
            var copy = new BinaryImage2D(Width, Height);
            copy.Threshold = threshold;

            copy.Iterate((x, y) =>
            {
                var pixel = GetPixel(x, y);
                copy.SetPixel(x, y, pixel);
            });

            return copy;
        }

        /// <summary>
        /// Convert to a color image.
        /// </summary>
        /// <returns>The color image.</returns>
        public ColorImage2D ToColorImage()
        {
            var copy = new ColorImage2D(Width, Height);
            copy.Iterate((x, y) =>
            {
                var pixel = GetPixel(x, y);
                copy.SetPixel(x, y, pixel);
            });

            return copy;
        }

        /// <summary>
        /// Get the mipmaps width at level m.
        /// </summary>
        /// <param name="m">The mipmap level.</param>
        /// <returns>The mipmaps width.</returns>
        public int MipmapWidth(int m)
        {
            return GetMipmapInterface(m).Width;
        }

        /// <summary>
        /// Get the mipmaps height at level m.
        /// </summary>
        /// <param name="m">The mipmap level.</param>
        /// <returns>The mipmaps height.</returns>
        public int MipmapHeight(int m)
        {
            return GetMipmapInterface(m).Height;
        }

        /// <summary>
        /// Get the mipmaps size at level m.
        /// </summary>
        /// <param name="m">The mipmap level.</param>
        /// <returns>The mipmaps size.</returns>
        public Point2i MipmapSize(int m)
        {
            return new Point2i(MipmapWidth(m), MipmapHeight(m));
        }

        /// <summary>
        /// Get the mipmap at index i.
        /// </summary>
        /// <param name="i">The mipmap level.</param>
        /// <returns>The mipmap at index i.</returns>
        protected abstract IImage2D GetMipmapInterface(int i);

        /// <summary>
        /// Calculate how many mipmap levels a image of thie size would have.
        /// </summary>
        /// <param name="width">The images width.</param>
        /// <param name="height">The images height.</param>
        /// <returns>How many mipmap levels a image of thie size would have</returns>
        public static int CalculateMipmapLevels(int width, int height)
        {
            int min = Math.Min(width, height);

            int levels = 1;
            while (min > 1)
            {
                min /= 2;
                levels++;
            }

            return levels;
        }

        /// <summary>
        /// Creates the images mipmaps.
        /// </summary>
        /// <param name="mode">The wrap mode to use.</param>
        /// <param name="method">The interpolation method to use.</param>
        /// <exception cref="ArgumentException">If max levels is not greater than 0.</exception>
        public void CreateMipmaps(WRAP_MODE mode = WRAP_MODE.CLAMP, RESCALE method = RESCALE.BICUBIC)
        {
            int maxLevel = CalculateMipmapLevels(Width, Height);
            CreateMipmaps(maxLevel, mode, method);
        }

        /// <summary>
        /// Creates the images mipmaps.
        /// </summary>
        /// <param name="maxLevel">The max level of mipmaps to create.</param>
        /// <param name="mode">The wrap mode to use.</param>
        /// <param name="method">The interpolation method to use.</param>
        public abstract void CreateMipmaps(int maxLevel, WRAP_MODE mode = WRAP_MODE.CLAMP, RESCALE method = RESCALE.BICUBIC);


    }
}
