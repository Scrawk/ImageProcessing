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
            image2.Resize(image.Height, image.Width);
            image2.Fill((x, y) => image[y, image2.Width - 1 - x]);
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
            image2.Resize(image.Width, image.Height);
            image2.Fill((x, y) => image[image2.Width - 1 - x, image2.Height - 1 - y]);
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
            image2.Resize(image.Height, image.Width);
            image2.Fill((x, y) => image[image2.Height - 1 - y, x]);
            return image2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="bounds"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IMAGE Crop<IMAGE>(IMAGE image, Box2i bounds, WRAP_MODE mode = WRAP_MODE.CLAMP)
            where IMAGE : Image2D<T>, new()
        {
            var image2 = new IMAGE();
            image2.Resize(bounds.Size);

            for(int y = bounds.Min.y, j = 0; y < bounds.Max.y; y++, j++)
            {
                for (int x = bounds.Min.x, i = 0; x < bounds.Max.x; x++, i++)
                {
                    image2.SetPixel(i, j, image.GetPixel(x, y, mode));
                }
            }

            return image2;
        }

    }

}
