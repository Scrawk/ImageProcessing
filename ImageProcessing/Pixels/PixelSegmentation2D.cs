using System;
using System.Collections.Generic;
using System.Text;

using Common.Core.Numerics;
using Common.Core.Colors;
using ImageProcessing.Images;

namespace ImageProcessing.Pixels
{

    public abstract class PixelSegmentation2D
    {
        public static Dictionary<Vector2i, ColorRGB> SegmentationColors(int seed, IEnumerable<Vector2i> keys)
        {
            var rnd = new Random(seed);
            var colors = new Dictionary<Vector2i, ColorRGB>();

            foreach (var p in keys)
            {
                colors.TryAdd(p, rnd.NextColorRGB());
            }

            return colors;
        }
    }

    public class PixelSegmentation2D<T> : PixelSegmentation2D
    {

        public PixelSegmentation2D(Image2D<T> image)
        {
            Image = image;
            Sets = new Dictionary<Vector2i, PixelSet2D<T>>();
        }

        public Image2D<T> Image { get; private set; }

        public Dictionary<Vector2i, PixelSet2D<T>> Sets { get; private set; }

        public void AddPixel(Vector2i root, PixelIndex2D<T> pixel)
        {
            PixelSet2D<T> set;
            if (!Sets.TryGetValue(root, out set))
            {
                set = new PixelSet2D<T>(root);
                Sets.Add(root, set);
            }

            set.Pixels.Add(pixel);
        }

        public ColorImage2D ColorizeSegmentation(int seed)
        {
            var colors = SegmentationColors(seed, Sets.Keys);

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
}
