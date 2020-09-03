using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{

    public partial class Image2D<T>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static IMAGE Rotate90<IMAGE>(IMAGE image)
            where IMAGE : Image2D<T>, new()
        {
            var image2 = new IMAGE();
            image2.Resize(image.Size);
            image2.Fill((x, y) => image[y, image.Width - 1 - x]);
            return image2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static IMAGE Rotate180<IMAGE>(IMAGE image)
            where IMAGE : Image2D<T>, new()
        {
            var image2 = new IMAGE();
            image2.Resize(image.Size);
            image2.Fill((x, y) => image[image.Width - 1 - x, image.Height - 1 - y]);
            return image2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static IMAGE Rotate270<IMAGE>(IMAGE image)
            where IMAGE : Image2D<T>, new()
        {
            var image2 = new IMAGE();
            image2.Resize(image.Size);
            image2.Fill((x, y) => image[image.Height - 1 - y, x]);
            return image2;
        }

    }

}
