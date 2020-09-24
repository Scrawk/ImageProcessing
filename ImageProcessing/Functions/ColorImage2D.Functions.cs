using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{
    public partial class ColorImage2D
    {
        /// <summary>
        /// Make each value in image the smaller of the two values.
        /// </summary>
        /// <param name="value">The another value.</param>
        public void Min(float value)
        {
            Modify((v) =>
            {
                return ColorRGB.Min(v, value);
            });
        }

        /// <summary>
        /// Make each value in image the larger of the two values.
        /// </summary>
        /// <param name="value">The another value.</param>
        public void Max(float value)
        {
            Modify((v) =>
            {
                return ColorRGB.Max(v, value);
            });
        }

        /// <summary>
        /// Clamp each pixel to be no more or less that the min and max.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public void Clamp(float min, float max)
        {
            Modify((v) =>
            {
                return ColorRGB.Clamp(v, min, max);
            });
        }


        /// <summary>
        /// Presuming the image color space is rgb 
        /// convert all pixels to hsv color space.
        /// </summary>
        public void MakeHSV()
        {
            Modify(c =>
            {
                var hsv = c.hsv;
                return new ColorRGB(hsv.h, hsv.s, hsv.v);
            });
        }

        /// <summary>
        /// Presuming the image color space is hsv 
        /// convert all pixels to rgb color space.
        /// </summary>
        public void MakeRGB()
        {
            Modify(c =>
            {
                return ColorHSV.ToRGB(c.r, c.g, c.b);
            });
        }
    }
}
