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
    public partial class BinaryImage2D : IImage2D<int>
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
        public int this[int x, int y]
        {
            get => Data[x + y * Width] ? 1 : 0;
            set => Data[x + y * Width] = value > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int this[Vector2i i]
        {
            get => Data[i.x + i.y * Width] ? 1 : 0;
            set => Data[i.x + i.y * Width] = value > 0;
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
        public IEnumerator<int> GetEnumerator()
        {
            foreach (bool t in Data)
                yield return t ? 1 : 0;
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
        public int GetClamped(int x, int y)
        {
            x = MathUtil.Clamp(x, 0, Width - 1);
            y = MathUtil.Clamp(y, 0, Height - 1);
            return this[x, y];
        }

        /// <summary>
        /// Get the element at wrapped index x,y.
        /// </summary>
        public int GetWrapped(int x, int y)
        {
            x = MathUtil.Wrap(x, Width);
            y = MathUtil.Wrap(y, Height);
            return this[x, y];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public bool GetBool(int x, int y)
        {
            return Data[x + y * Width];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void SetBool(int x, int y, bool value)
        {
            Data[x + y * Width] = value;
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
        /// <param name="points"></param>
        public void Clear(IList<Vector2i> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                this[p.x, p.y] = 0;
            }
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
                        this[x, y] = 1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void Fill(IList<Vector2i> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                this[p.x, p.y] = 1;
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
        public List<Vector2i> ToPoints2()
        {
            var points = new List<Vector2i>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (this[x, y] == 1)
                        points.Add(new Vector2i(x, y));

                }
            }

            return points;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Vector3i> ToPoints3()
        {
            var points = new List<Vector3i>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (this[x, y] == 1)
                        points.Add(new Vector3i(x, y));

                }
            }

            return points;
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
                    array[x, y] = this[x, y];
                }
            }

            return array;
        }
    }

}
