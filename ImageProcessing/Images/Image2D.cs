using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;
using Common.Core.Threading;

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
        /// The size of the image as a vector.
        /// </summary>
        public Point2i Size => new Point2i(Width, Height);

        /// <summary>
        /// The size of the image as a box.
        /// </summary>
        public Box2i Bounds => new Box2i(new Point2i(0, 0), Size);

        /// <summary>
        /// Access a element at index x,y.
        /// </summary>
        public abstract T this[int x, int y]
        {
            get;
            set;
        }

        /// <summary>
        /// Access a element at index x,y.
        /// </summary>
        public abstract T this[Point2i i]
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
            return string.Format("[Image2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// Get a pixel from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public abstract ColorRGB GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a pixel from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first index.</param>
        /// <param name="v">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        public abstract ColorRGB GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="pixel">The pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        public abstract void SetPixel(int x, int y, ColorRGB pixel, WRAP_MODE mode = WRAP_MODE.NONE);

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="pixel">The pixel.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        public void SetPixel(int x, int y, ColorRGBA pixel, WRAP_MODE mode = WRAP_MODE.NONE)
        {
            var col = ColorRGB.Lerp(GetPixel(x, y, mode), pixel.rgb, pixel.a);
            SetPixel(x, y, col, mode);
        }

        /// <summary>
        /// Is this array the same size as the other array.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsSameSize(IImage2D image)
        {
            if (Width != image.Width) return false;
            if (Height != image.Height) return false;
            return true;
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
        /// Are the x and y index in the bounds of the array.
        /// </summary>
        public bool InBounds(Point2i i)
        {
            return InBounds(i.x, i.y);
        }

        /// <summary>
        /// Are the x and y index not in the bounds of the array.
        /// </summary>
        public bool NotInBounds(Point2i i)
        {
            return !InBounds(i.x, i.y);
        }

        /// <summary>
        /// Sets all elements in the array to default value.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Resize the array. Will clear any existing data.
        /// </summary>
        public abstract void Resize(int width, int height);

        /// <summary>
        /// Resize the array. Will clear any existing data.
        /// </summary>
        public abstract void Resize(Point2i size);

        /// <summary>
        /// Get the element at clamped index x,y.
        /// </summary>
        public T GetClamped(int x, int y)
        {
            x = MathUtil.Clamp(x, 0, Width - 1);
            y = MathUtil.Clamp(y, 0, Height - 1);
            return this[x, y];
        }

        /// <summary>
        /// Get the element at wrapped index x,y.
        /// </summary>
        public T GetWrapped(int x, int y)
        {
            x = MathUtil.Wrap(x, Width);
            y = MathUtil.Wrap(y, Height);
            return this[x, y];
        }

        /// <summary>
        /// Get the element at mirrored index x,y.
        /// </summary>
        public T GetMirrored(int x, int y)
        {
            x = MathUtil.Mirror(x, Width);
            y = MathUtil.Mirror(y, Height);
            return this[x, y];
        }

        /// <summary>
        /// Set the element at clamped index x.
        /// </summary>
        public void SetClamped(int x, int y, T value)
        {
            x = MathUtil.Clamp(x, 0, Width - 1);
            y = MathUtil.Clamp(y, 0, Height - 1);
            this[x, y] = value;
        }

        /// <summary>
        /// Set the element at wrapped index x.
        /// </summary>
        public void SetWrapped(int x, int y, T value)
        {
            x = MathUtil.Wrap(x, Width);
            y = MathUtil.Wrap(y, Height);
            this[x, y] = value;
        }

        /// <summary>
        /// Set the element at mirred index x.
        /// </summary>
        public void SetMirrored(int x, int y, T value)
        {
            x = MathUtil.Mirror(x, Width);
            y = MathUtil.Mirror(y, Height);
            this[x, y] = value;
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
                for (int y = block.Min.y; y < block.Max.y; y++)
                {
                    for (int x = block.Min.x; x < block.Max.x; x++)
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
        public void Fill(T[,] source, int x = 0, int y = 0)
        {
            for (int j = 0; j < source.GetLength(0); j++)
            {
                for (int i = 0; i < source.GetLength(1); i++)
                {
                    this[x + i, y + j] = source[i, j];
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
        public void Fill(Image2D<T> source, int x = 0, int y = 0)
        {
            for (int j = 0; j < source.Height; j++)
            {
                for (int i = 0; i < source.Width; i++)
                {
                    this[x + i, y + j] = source[i, j];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="bounds"></param>
        public void Fill(Image2D<T> source, Box2i bounds)
        {
            for (int y = bounds.Min.y; y < bounds.Max.y; y++)
            {
                for (int x = bounds.Min.x; x < bounds.Max.x; x++)
                {
                    this[x, y] = source[x, y];
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
        /// Fill the array with the value from the function in parallel.
        /// </summary>
        public void ParallelFill(Func<int, int, T> func)
        {
            ParallelFill(BlockSize(), func);
        }

        /// <summary>
        /// Fill the array with the value from the function in parallel.
        /// </summary>
        public void ParallelFill(int blockSize, Func<int, int, T> func)
        {
            var blocks = ThreadingBlock2D.CreateBlocks(Width, Height, blockSize);
            Parallel.ForEach(blocks, (block) =>
            {
                for (int y = block.Min.y; y < block.Max.y; y++)
                {
                    for (int x = block.Min.x; x < block.Max.x; x++)
                    {
                        this[x, y] = func(x, y);
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
                for (int y = block.Min.y; y < block.Max.y; y++)
                {
                    for (int x = block.Min.x; x < block.Max.x; x++)
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


    }
}
