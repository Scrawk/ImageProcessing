using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{
    public partial class VectorImage2D
    {

        /// <summary>
        /// Make each value in image the smaller of the two values.
        /// </summary>
        /// <param name="value">The another value.</param>
        public void Min(float value)
        {
            Modify((v) =>
            {
                return Vector2f.Min(v, value);
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
                return Vector2f.Max(v, value);
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
                return Vector2f.Clamp(v, min, max);
            });
        }

        /// <summary>
        /// Normalize each vector in the image.
        /// </summary>
        public void Normalize()
        {
            Modify((v) =>
            {
                return v.Normalized;
            });
        }

    }
}
