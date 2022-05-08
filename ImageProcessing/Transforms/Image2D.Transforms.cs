using System.Collections;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{

    public partial class Image2D<T>
    {
        /// <summary>
        /// Offsets the pixels in the image.
        /// </summary>
        /// <typeparam name="IMAGE"></typeparam>
        /// <param name="image">The image to offset.</param>
        /// <param name="offsetX">The offset on the x axis.</param>
        /// <param name="offsetY">The offset on the y axis.</param>
        /// <returns>The offset image.</returns>
        public static IMAGE Offset<IMAGE>(IMAGE image, int offsetX, int offsetY)
            where IMAGE : IImage2D, new()
        {
            var image2 = NewImage<IMAGE>(image.Height, image.Width);
   
            for(int y = 0; y < image.Height; y++)
            {
                for(int x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(x - offsetX, y - offsetY, WRAP_MODE.WRAP);
                    image2.SetPixel(x, y, pixel);
                }
            }

            return image2;
        }

        /// <summary>
        /// Flip the image on the x axis.
        /// </summary>
        /// <typeparam name="IMAGE"></typeparam>
        /// <param name="image">The image to flip.</param>
        /// <returns>The flipped image.</returns>
        public static IMAGE FlipHorizontal<IMAGE>(IMAGE image)
            where IMAGE : IImage2D, new()
        {
            var image2 = NewImage<IMAGE>(image.Height, image.Width);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(image.Width - x - 1, y);
                    image2.SetPixel(x, y, pixel);
                }
            }

            return image2;
        }

        /// <summary>
        /// Flip the image on the y axis.
        /// </summary>
        /// <typeparam name="IMAGE"></typeparam>
        /// <param name="image">The image ti flip.</param>
        /// <returns>The flipped image.</returns>
        public static IMAGE FlipVertical<IMAGE>(IMAGE image)
            where IMAGE : IImage2D, new()
        {
            var image2 = NewImage<IMAGE>(image.Height, image.Width);
        
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(x, image.Height - y - 1);
                    image2.SetPixel(x, y, pixel);
                }
            }

            return image2;
        }

        /// <summary>
        /// Returen a copy of the image rotated 90 degrees.
        /// </summary>
        /// <param name="image">The image to rotate.</param>
        /// <returns>The rotated image.</returns>
        public static IMAGE Rotate90<IMAGE>(IMAGE image)
            where IMAGE : IImage2D, new()
        {
            var image2 = NewImage<IMAGE>(image.Height, image.Width);
     
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(y, image2.Width - 1 - x);
                    image2.SetPixel(x, y, pixel);
                }
            }

            return image2;
        }

        /// <summary>
        /// Returen a copy of the image rotated 180 degrees.
        /// </summary>
        /// <param name="image">The image to rotate.</param>
        /// <returns>The rotated image.</returns>
        public static IMAGE Rotate180<IMAGE>(IMAGE image)
            where IMAGE : IImage2D, new()
        {
            var image2 = NewImage<IMAGE>(image.Width, image.Height);
   
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(image2.Width - 1 - x, image2.Height - 1 - y);
                    image2.SetPixel(x, y, pixel);
                }
            }

            return image2;
        }

        /// <summary>
        /// Returen a copy of the image rotated 270 degrees.
        /// </summary>
        /// <param name="image">The image to rotate.</param>
        /// <returns>The rotated image.</returns>
        public static IMAGE Rotate270<IMAGE>(IMAGE image)
            where IMAGE : IImage2D, new()
        {
            var image2 = NewImage<IMAGE>(image.Height, image.Width);
 
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(image2.Height - 1 - y, x);
                    image2.SetPixel(x, y, pixel);
                }
            }

            return image2;
        }

        /// <summary>
        /// Return a copy of the image cropped to the bounds.
        /// </summary>
        /// <param name="image">The image to crop.</param>
        /// <param name="bounds">The bounds to crop.</param>
        /// <param name="overlap">The amount to overlap with the previous images.</param>
        /// <param name="mode">The wrap mode to use for pixels outside the bounds.</param>
        /// <returns>The cropped image.</returns>
        public static IMAGE Crop<IMAGE>(IMAGE image, Box2i bounds, int overlap, WRAP_MODE mode = WRAP_MODE.CLAMP)
            where IMAGE : IImage2D, new()
        {
            var image2 = NewImage<IMAGE>(bounds.Width + overlap * 2, bounds.Height + overlap * 2);
      
            for(int y = bounds.Min.y - overlap, j = 0; y < bounds.Max.y + overlap; y++, j++)
            {
                for (int x = bounds.Min.x - overlap, i = 0; x < bounds.Max.x + overlap; x++, i++)
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
        /// <param name="overlap">The amount to overlap with the previous images.</param>
        /// <param name="mode">The wrap mode</param>
        /// <returns>A list of the new images.</returns>
        public static List<IMAGE> Crop<IMAGE>(IMAGE image, int numX, int numY, int overlap, WRAP_MODE mode = WRAP_MODE.CLAMP)
            where IMAGE : IImage2D, new()
        {
          
            int width = image.Width / numX + overlap * 2;
            int height = image.Height / numY + overlap * 2;

            var images = new List<IMAGE>();

            for(int m = 0; m < numX; m++)
            {
                for (int n = 0; n < numY; n++)
                {
                    var image2 = NewImage<IMAGE>(width, height);

                    var min = new Point2i(m * width, n * height);
                    var max = new Point2i(min.x + width, min.y + height);
                    var bounds = new Box2i(min, max);

                    for (int y = bounds.Min.y - overlap, j = 0; y < bounds.Max.y + overlap; y++, j++)
                    {
                        for (int x = bounds.Min.x - overlap, i = 0; x < bounds.Max.x + overlap; x++, i++)
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
