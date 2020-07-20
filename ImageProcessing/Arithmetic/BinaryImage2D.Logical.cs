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
            Data.Not();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void Or(BinaryImage2D b)
        {
            Data.Or(b.Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void Xor(BinaryImage2D b)
        {
            Data.Xor(b.Data);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void And(BinaryImage2D b)
        {
            Data.And(b.Data);
        }

    }

}
