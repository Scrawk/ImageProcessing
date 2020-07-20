using System;
using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BinaryImage2D : IImage2D<bool>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BinaryImage2D(int width, int height)
        {
            Width = width;
            Height = height;
            Data = new BitArray(width * height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="value"></param>
        public BinaryImage2D(int width, int height, int value)
        {
            Width = width;
            Height = height;
            Data = new BitArray(width * height);
            Data.SetAll(value != 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="data"></param>
        public BinaryImage2D(int width, int height, BitArray data)
        {
            Width = width;
            Height = height;
            Data = new BitArray(data);
        }

        /// <summary>
        /// 
        /// </summary>
        public BitArray Data { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Count => Width * Height;

        /// <summary>
        /// 
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool this[int x, int y]
        {
            get => Data[x + y * Width];
            set => Data[x + y * Width] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool this[Vector2i i]
        {
            get => Data[i.x + i.y * Width];
            set => Data[i.x + i.y * Width] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[BinaryImage2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// Enumerate all elements in the array.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<bool> GetEnumerator()
        {
            foreach (bool b in Data)
                yield return b;
        }

        /// <summary>
        /// Enumerate all elements in the array.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Get the element at clamped index x,y.
        /// </summary>
        public bool GetClamped(int x, int y)
        {
            x = MathUtil.Clamp(x, 0, Width - 1);
            y = MathUtil.Clamp(y, 0, Height - 1);
            return this[x, y];
        }

        /// <summary>
        /// Get the element at wrapped index x,y.
        /// </summary>
        public bool GetWrapped(int x, int y)
        {
            x = MathUtil.Wrap(x, Width);
            y = MathUtil.Wrap(y, Height);
            return this[x, y];
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            Data.SetAll(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Fill()
        {
            Data.SetAll(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        public void Fill(Func<int, int, bool> func)
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
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="center"></param>
        public void Fill(IShape2f shape, bool center = true)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var p = new Vector2f(x, y);
                    if (center) p += 0.5f;

                    if (shape.Contains(p))
                        this[x, y] = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void Fill(IList<PixelIndex2D<bool>> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                this[p.x, p.y] = p.value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BinaryImage2D Copy()
        {
            return new BinaryImage2D(Width, Height, Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<PixelIndex2D<bool>> ToPixelIndexList()
        {
            var pixel = new List<PixelIndex2D<bool>>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (this[x, y])
                        pixel.Add(new PixelIndex2D<bool>(x, y, true));
                }
            }

            return pixel;
        }

        /// <summary>
        /// 
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
