using System;
using System.Collections.Generic;
using System.Text;

using Common.Core.Numerics;
using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Pixels
{

    public class PixelSegmentation2D<T>
    {

        public PixelSegmentation2D(Image2D<T> image)
        {
            Image = image;
            Sets = new Dictionary<Point2i, PixelSet2D<T>>();
        }

        public Image2D<T> Image { get; private set; }

        public int Count => Sets.Count;

        public Dictionary<Point2i, PixelSet2D<T>> Sets { get; private set; }

        public override string ToString()
        {
            return string.Format("[PixelSegmentation2D: Count={0}]", Count);
        }

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
                    image[p.Index] = color;
            }

            return image;
        }

    }

    public static class SegmentationColors
    {
        public static Dictionary<Point2i, ColorRGB> Generate(int seed, IEnumerable<Point2i> keys)
        {
            var rnd = new Random(seed);
            var colors = new Dictionary<Point2i, ColorRGB>();

            foreach (var p in keys)
            {
                if(!colors.ContainsKey(p))
                    colors.Add(p, rnd.NextColorRGB());
            }

            return colors;
        }
    }
}
