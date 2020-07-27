using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{

    public partial class BinaryImage2D
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static BinaryImage2D Rotate90(BinaryImage2D a)
        {
            var image = new BinaryImage2D(a.Height, a.Width);
            image.Fill((x, y) => a[y, a.Width - 1 - x]);

            return a;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static BinaryImage2D Rotate180(BinaryImage2D a)
        {
            var image = new BinaryImage2D(a.Height, a.Width);
            image.Fill((x, y) => a[a.Width - 1 - x, a.Height - 1 - y]);

            return image;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static BinaryImage2D Rotate270(BinaryImage2D a)
        {
            var image = new BinaryImage2D(a.Height, a.Width);
            image.Fill((x, y) => a[a.Height - 1 - y, x]);

            return image;
        }

    }

}
