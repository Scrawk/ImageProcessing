using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BinaryImage2D
    {
        /// <summary>
        /// 
        /// </summary>
        public void Invert()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    this[x, y] = !this[x, y];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void Or(BinaryImage2D b)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    this[x, y] |= b[x, y];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void Xor(BinaryImage2D b)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    this[x, y] ^= b[x, y];
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void And(BinaryImage2D b)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    this[x, y] &= b[x, y];
                }
            }
        }

    }

}
