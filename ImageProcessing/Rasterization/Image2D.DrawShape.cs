using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{
    public partial class Image2D<T>
    {
        /// <summary>
        /// Draw a shape into image by setting any pixels 
        /// in the image to the color if they are contained in the shape.
        /// </summary>
        /// <param name="shape">The shape interface.</param>
        /// <param name="color">The color to use.</param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        public void DrawShape(IShape2f shape, ColorRGBA color, WRAP_MODE wrap = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            var bounds = shape.Bounds;

            Point2i min, max;
            min.x = (int)(bounds.Min.x - 1);
            min.y = (int)(bounds.Min.y - 1);
            max.x = (int)(bounds.Max.x + 1);
            max.y = (int)(bounds.Max.y + 1);

            for(int y = min.y; y <= max.y; y++)
            {
                for(int x = min.x; x <= max.x; x++)
                {
                    if(shape.Contains(new Point2f(x,y), true))
                    {
                        SetPixel(x, y, color, wrap, blend);
                    }
                }
            }
        }

        /// <summary>
        /// Draw a shape into image by setting any pixels 
        /// in the image to the color if they are contained in the shape.
        /// </summary>
        /// <param name="shape">The shape interface.</param>
        /// <param name="color">The color to use.</param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        public void DrawShape(IShape2d shape, ColorRGBA color, WRAP_MODE wrap = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            var bounds = shape.Bounds;

            Point2i min, max;
            min.x = (int)(bounds.Min.x - 1);
            min.y = (int)(bounds.Min.y - 1);
            max.x = (int)(bounds.Max.x + 1);
            max.y = (int)(bounds.Max.y + 1);

            for (int y = min.y; y <= max.y; y++)
            {
                for (int x = min.x; x <= max.x; x++)
                {
                    if (shape.Contains(new Point2d(x, y), true))
                    {
                        SetPixel(x, y, color, wrap, blend);
                    }
                }
            }
        }

    }
}
