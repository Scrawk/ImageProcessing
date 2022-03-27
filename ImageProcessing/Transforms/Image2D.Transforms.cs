using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{

    public partial class Image2D<T>
    {
        /// <summary>
        /// Flip the image on the x axis.
        /// </summary>
        /// <typeparam name="IMAGE"></typeparam>
        /// <param name="image">The image ti flip.</param>
        /// <returns>The flipped image.</returns>
        public static IMAGE FlipHorizontal<IMAGE>(IMAGE image)
            where IMAGE : Image2D<T>, new()
        {
            var image2 = new IMAGE();
            image2.Resize(image.Height, image.Width);
            image2.Fill((x, y) => image[image.Width - x - 1, y]);
            return image2;
        }

        /// Flip the image on the y axis.
        /// </summary>
        /// <typeparam name="IMAGE"></typeparam>
        /// <param name="image">The image ti flip.</param>
        /// <returns>The flipped image.</returns>
        public static IMAGE FlipVertical<IMAGE>(IMAGE image)
            where IMAGE : Image2D<T>, new()
        {
            var image2 = new IMAGE();
            image2.Resize(image.Height, image.Width);
            image2.Fill((x, y) => image[x, image.Height - y - 1]);
            return image2;
        }

        /// <summary>
        /// Returen a copy of the image rotated 90 degrees.
        /// </summary>
        /// <param name="image">The image to rotate.</param>
        /// <returns>The rotated image.</returns>
        public static IMAGE Rotate90<IMAGE>(IMAGE image)
            where IMAGE : Image2D<T>, new()
        {
            var image2 = new IMAGE();
            image2.Resize(image.Height, image.Width);
            image2.Fill((x, y) => image[y, image2.Width - 1 - x]);
            return image2;
        }

        /// <summary>
        /// Returen a copy of the image rotated 180 degrees.
        /// </summary>
        /// <param name="image">The image to rotate.</param>
        /// <returns>The rotated image.</returns>
        public static IMAGE Rotate180<IMAGE>(IMAGE image)
            where IMAGE : Image2D<T>, new()
        {
            var image2 = new IMAGE();
            image2.Resize(image.Width, image.Height);
            image2.Fill((x, y) => image[image2.Width - 1 - x, image2.Height - 1 - y]);
            return image2;
        }

        /// <summary>
        /// Returen a copy of the image rotated 270 degrees.
        /// </summary>
        /// <param name="image">The image to rotate.</param>
        /// <returns>The rotated image.</returns>
        public static IMAGE Rotate270<IMAGE>(IMAGE image)
            where IMAGE : Image2D<T>, new()
        {
            var image2 = new IMAGE();
            image2.Resize(image.Height, image.Width);
            image2.Fill((x, y) => image[image2.Height - 1 - y, x]);
            return image2;
        }

        /// <summary>
        /// Return a copy of the image cropped to the bounds.
        /// </summary>
        /// <param name="image">The image to crop.</param>
        /// <param name="bounds">The bounds to crop.</param>
        /// <param name="mode">The wrap mode to use for pixels outside the bounds.</param>
        /// <returns>The cropped image.</returns>
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

        /// <summary>
        /// Cut a image into smaller images.
        /// </summary>
        /// <typeparam name="IMAGE"></typeparam>
        /// <param name="image">The image to cut.</param>
        /// <param name="numX">The number of images to cut on the x axis.</param>
        /// <param name="numY">The number of images to cut on the y axis.</param>
        /// <param name="mode">The wrap mode</param>
        /// <returns>A list of the new images.</returns>
        public static List<IMAGE> Crop<IMAGE>(IMAGE image, int numX, int numY, WRAP_MODE mode = WRAP_MODE.CLAMP)
            where IMAGE : Image2D<T>, new()
        {
            int width = image.Width / numX;
            int height = image.Height / numY;

            var images = new List<IMAGE>();

            for(int m = 0; m < numX; m++)
            {
                for (int n = 0; n < numY; n++)
                {
                    var image2 = new IMAGE();
                    image2.Resize(width, height);

                    var min = new Point2i(m * width, n * height);
                    var max = new Point2i(min.x + width, min.y + height);
                    var bounds = new Box2i(min, max);

                    for (int y = bounds.Min.y, j = 0; y < bounds.Max.y; y++, j++)
                    {
                        for (int x = bounds.Min.x, i = 0; x < bounds.Max.x; x++, i++)
                        {
                            image2.SetPixel(i, j, image.GetPixel(x, y, mode));
                        }
                    }

                    images.Add(image2);

                }
            }

            return images;
        }

    }

}
