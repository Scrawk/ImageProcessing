using System;
using System.Collections.Generic;
using System.Text;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Extensions;
using ImageProcessing.Images;

namespace ImageProcessing.Pixels
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PixelSegmentation2D<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public PixelSegmentation2D(Image2D<T> image)
        {
            Image = image;
            Sets = new Dictionary<Point2i, PixelSet2D<T>>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Image2D<T> Image { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Count => Sets.Count;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Point2i, PixelSet2D<T>> Sets { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[PixelSegmentation2D: Count={0}]", Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="pixel"></param>
        public void AddPixel(Point2i root, PixelIndex2D<T> pixel)
        {
            PixelSet2D<T> set;
            if (!Sets.TryGetValue(root, out set))
            {
                set = new PixelSet2D<T>();
                Sets.Add(root, set);
            }

            set.Pixels.Add(pixel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public ColorImage2D ColorizeSegmentation(int seed)
        {
            var colors = SegmentationColors.Generate(seed, Sets.Keys);

            var image = new ColorImage2D(Image.Width, Image.Height);

            foreach (var kvp in Sets)
            {
                var idx = kvp.Key;
                var set = kvp.Value;
                var color = colors[idx];

                foreach (var p in set.Pixels)
                    image[p.x, p.y] = color;
            }

            return image;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public static class SegmentationColors
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static Dictionary<Point2i, ColorRGBA> Generate(int seed, IEnumerable<Point2i> keys)
        {
            var rnd = new Random(seed);
            var colors = new Dictionary<Point2i, ColorRGBA>();

            foreach (var p in keys)
            {
                if(!colors.ContainsKey(p))
                    colors.Add(p, rnd.NextColorRGB().rgb1);
            }

            return colors;
        }
    }
}
