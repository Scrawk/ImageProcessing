using System;
using System.Collections.Generic;

using Common.Core.Numerics;

namespace ImageProcessing.Pixels
{
    /// <summary>
    /// Struct to hold a pixels value and its index in a image.
    /// </summary>
    /// <typeparam name="T">The pixels type</typeparam>
    public struct PixelIndex2D<T>
    {
        /// <summary>
        /// The pixels index in a image.
        /// </summary>
        public int x, y;

        /// <summary>
        /// The pixels value.
        /// </summary>
        public T Value;

        /// <summary>
        /// Tag for algorithms on the image can mark or 
        /// id the image if needed. May change at any point.
        /// </summary>
        public int Tag;

        /// <summary>
        /// Create a new PixelIndex2D.
        /// </summary>
        /// <param name="x">The pixels x coordinate.</param>
        /// <param name="y">The pixels y coordinate.</param>
        /// <param name="value">The pixels value</param>
        public PixelIndex2D(int x, int y, T value)
        {
            this.x = x;
            this.y = y;
            Value = value;
            Tag = 0;
        }

        /// <summary>
        /// Create a new PixelIndex2D.
        /// </summary>
        /// <param name="x">The pixels x coordinate.</param>
        /// <param name="y">The pixels y coordinate.</param>
        /// <param name="value">The pixels value</param>
        /// <param name="tag"></param>
        public PixelIndex2D(int x, int y, T value, int tag)
        {
            this.x = x;
            this.y = y;
            Value = value;
            Tag = tag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[PixelIndex2D: x={0}, y={1}, Value={2}]", x, y, Value);
        }
    }
}
