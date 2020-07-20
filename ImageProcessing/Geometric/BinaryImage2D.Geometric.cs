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

            for (int y = 0; y < a.Height; y++)
            {
                for (int x = 0; x < a.Width; x++)
                {
                    image[y, a.Width - 1 - x] = a[x, y];
                }
            }

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

            for (int y = 0; y < a.Height; y++)
            {
                for (int x = 0; x < a.Width; x++)
                {
                    image[a.Width - 1 - x, a.Height - 1 - y] = a[x, y];
                }
            }

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

            for (int y = 0; y < a.Height; y++)
            {
                for (int x = 0; x < a.Width; x++)
                {
                    image[a.Height - 1 - y, x] = a[x, y];
                }
            }

            return image;
        }

    }

}
