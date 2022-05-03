using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Extensions;

namespace ImageProcessing.Images
{

    public partial class Image2D<T>
    {
        /// <summary>
        /// Create a image filled with randow values.
        /// </summary>
        /// <typeparam name="IMAGE">The images generic type</typeparam>
        /// <param name="width">The images width.</param>
        /// <param name="height">The images height.</param>
        /// <param name="seed">The random generators seed.</param>
        /// <param name="min">The min value generated.</param>
        /// <param name=max">The max value generated.</param>
        /// <returns>THe new image.</returns>
        public static IMAGE Random<IMAGE>(int width, int height, int seed, float min = 0, float max = 1)
            where IMAGE : IImage2D, new()
        {
            var rnd = new Random(seed);
            return Random<IMAGE>(width, height, rnd, min, max);
        }

        /// <summary>
        /// Create a image filled with randow values.
        /// </summary>
        /// <typeparam name="IMAGE">The images generic type</typeparam>
        /// <param name="width">The images width.</param>
        /// <param name="height">The images height.</param>
        /// <param name="rnd">The random generator.</param>
        /// <param name="min">The min value generated.</param>
        /// <param name=max">The max value generated.</param>
        /// <returns>The new image.</returns>
        public static IMAGE Random<IMAGE>(int width, int height, Random rnd, float min = 0, float max = 1)
            where IMAGE : IImage2D, new()
        {
            var image = NewImage<IMAGE>(width, height);
   
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (image.Channels == 1)
                    {
                        var value = rnd.NextFloat(min, max);
                        image.SetChannel(x, y, 0, value);
                    }
                    else
                    {
                        var r = rnd.NextFloat(min, max);
                        var g = rnd.NextFloat(min, max);
                        var b = rnd.NextFloat(min, max);
                        image.SetPixel(x, y, new ColorRGB(r,g,b));
                    }
                }
            }

            return image;
        }

        /// <summary>
        /// Create a image filled with a single value.
        /// </summary>
        /// <typeparam name="IMAGE">The images generic type</typeparam>
        /// <param name="width">The images width.</param>
        /// <param name="height">The images height.</param>
        /// <param name="value">The constant value.</param>
        /// <returns>The new image.</returns>
        public static IMAGE Const<IMAGE>(int width, int height, float value)
            where IMAGE : IImage2D, new()
        {
            var image = NewImage<IMAGE>(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    image.SetPixel(x, y, new ColorRGB(value));
                }
            }

            return image;
        }

        /// <summary>
        /// Create a image filled with a single value.
        /// </summary>
        /// <typeparam name="IMAGE">The images generic type</typeparam>
        /// <param name="width">The images width.</param>
        /// <param name="height">The images height.</param>
        /// <param name="value">The constant value.</param>
        /// <returns>The new image.</returns>
        public static IMAGE Const<IMAGE>(int width, int height, ColorRGB value)
            where IMAGE : IImage2D, new()
        {
            var image = NewImage<IMAGE>(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    image.SetPixel(x, y, value);
                }
            }

            return image;
        }
    }
}
