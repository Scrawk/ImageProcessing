using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{
    public partial class GreyScaleImage2D
    {

        /// <summary>
        /// Make each value in image the smaller of the two values.
        /// </summary>
        /// <param name="value">The another value.</param>
        public void Min(float value)
        {
            Modify((v) =>
            {
                return Math.Min(v, value);
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
                return Math.Max(v, value);
            });
        }

        /// <summary>
        /// Clamp each value to be no more or less that the min and max.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public void Clamp(float min, float max)
        {
            Modify((v) =>
            {
                return MathUtil.Clamp(v, min, max);
            });
        }

        /// <summary>
        /// Normalize each value in the image to be between (inclusive) 0 and 1.
        /// </summary>
        public void Normalize()
        {
            var minMax = MinMax();

            Modify((v) =>
            {
                v = MathUtil.Normalize(v, minMax.min, minMax.max);
                return MathUtil.Clamp01(v);
            });
        }

    }
}
