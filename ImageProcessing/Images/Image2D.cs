﻿using System;
using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Geometry.Shapes;
using Common.Collections.Arrays;

namespace ImageProcessing.Images
{
    /// <summary>
    /// Base class for 2D images.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    public abstract class Image2D<T> : Array2<T>, IImage2D<T>
    {

        /// <summary>
        /// Create a new array.
        /// </summary>
        /// <param name="width">The size of the arrays 1st dimention.</param>
        /// <param name="height">The size of the arrays 2st dimention.</param>
        public Image2D(int width, int height)
            : base(width, height)
        {

        }

        /// <summary>
        /// Create a new array.
        /// </summary>
        /// <param name="size">The size of the array.</param>
        public Image2D(Vector2i size)
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
        /// Create a new array.
        /// </summary>
        /// <param name="data">The data form the array. Will be deep copied.</param>
        public Image2D(T[,] data)
            : base(data)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Image2D: Width={0}, Height={1}]", Width, Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract float GetValue(int x, int y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract ColorRGB GetPixelRGB(int x, int y);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<PixelIndex2D<T>> ToPixelIndexList(Func<T, bool> predicate)
        {
            var pixel = new List<PixelIndex2D<T>>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var v = this[x, y];
                    if (predicate(v))
                        pixel.Add(new PixelIndex2D<T>(x, y, v));
                }
            }

            return pixel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="value"></param>
        /// <param name="center"></param>
        public void Fill(IShape2f shape, T value, bool center = true)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var p = new Vector2f(x, y);
                    if (center) p += 0.5f;

                    if (shape.Contains(p))
                        this[x, y] = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void Fill(IList<PixelIndex2D<T>> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                this[p.x, p.y] = p.value;
            }
        }


    }
}