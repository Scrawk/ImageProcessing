using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{
    public partial class GreyScaleImage2D
    {

        /// <summary>
        /// Returns the normalized square distance between the two images.
        /// </summary>
        /// <param name="image">The other image to compare.</param>
        /// <returns>The normalized square distance.</returns>
        public float SqrDistance(GreyScaleImage2D image)
        {
            float sqdist = 0;

            Iterate((x, y) =>
            {
                float v = this[x, y] - image[x, y];
                sqdist += v * v;
            });

            return sqdist / (Width * Height);
        }

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
            float min, max;
            MinMaxValue(out min, out max);

            Modify((v) =>
            {
                v = MathUtil.Normalize(v, min, max);
                return MathUtil.Clamp01(v);
            });
        }

        /// <summary>
        /// Inverts the values in the image.
        /// </summary>
        public void Invert()
        {
            Modify(v =>
            {
                return 1.0f - v;
            });
        }

        /// <summary>
        /// Offset and scale each value in the image
        /// with the option of taking the abs value first.
        /// </summary>
        /// <param name="offset">The amount to add to the value before appling the function.</param>
        /// <param name="scale">The amount to scale the result of the function.</param>
        /// <param name="abs">True to take the abs of the value before appling the function.</param>
        public void OffsetScale(float offset, float scale, bool abs)
        {
            Modify(v =>
            {
                v = abs ? Math.Abs(v) : v;
                return (v + offset) * scale;
            });
        }

        /// <summary>
        /// Scale then offset each value in the image
        /// with the option of taking the abs value first.
        /// </summary>
        /// <param name="scale">The amount to scale the result of the function.</param>
        /// <param name="offset">The amount to add to the value before appling the function.</param>
        /// <param name="abs">True to take the abs of the value before appling the function.</param>
        public void ScaleOffset(float scale, float offset, bool abs)
        {
            Modify(v =>
            {
                v = abs ? Math.Abs(v) : v;
                return offset + v * scale;
            });
        }

        /// <summary>
        /// Take the sqrt of each value in the image.
        /// </summary>
        public void Sqrt()
        {
            Modify(v =>
            {
                return MathUtil.SafeSqrt(v);
            });
        }

        /// <summary>
        /// Take the log base e of each value in the image.
        /// </summary>
        public void Log()
        {
            Modify(v =>
            {
                return MathUtil.SafeLog(v);
            });
        }

        /// <summary>
        /// Take the log of each value in the image using the provided base value of a.
        /// </summary>
        /// <param name="a">The base to use.</param>
        public void Log(float a)
        {
            Modify(v =>
            {
                return (float)Math.Log(v, a);
            });
        }

        /// <summary>
        /// Take the log base 10 of each value in the image.
        /// </summary>
        public void Log10()
        {
            Modify(v =>
            {
                return MathUtil.SafeLog10(v);
            });
        }

        /// <summary>
        /// Apply th abs fuction to each value in the image.
        /// </summary>
        public void Abs()
        {
            Modify(v =>
            {
                return Math.Abs(v);
            });
        }

        /// <summary>
        /// Add the values in the image.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(float value)
        {
            Modify(v =>
            {
                return v + value;
            });
        }

        /// <summary>
        /// Subtract the values in the image.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="reverse">Reverse the operations order.</param>
        public void Subtract(float value, bool reverse = false)
        {
            Modify(v =>
            {
                if (reverse)
                    return value - v;
                else
                    return v - value;
            });
        }

        /// <summary>
        /// Multiply the values in the image.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Multiply(float value)
        {
            Modify(v =>
            {
                return v * value;
            });
        }

        /// <summary>
        /// Add the values in the image.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="reverse">Reverse the operations order.</param>
        public void Divide(float value, bool reverse = false)
        {
            Modify(v =>
            {
                if (reverse)
                {
                    v = v > 0 ? value / v : 0;
                }
                else
                {
                    v = value > 0 ? v / value : 0;
                }

                return v;
            });
        }

        /// <summary>
        /// Adds the two images together.
        /// The resulting inage will be the same size as the first image.
        /// If the second image is a different size then bilinear filtering will be used sample the image.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="image2">The second image.</param>
        /// <returns>A image the same size as the first.</returns>
        public static GreyScaleImage2D operator +(GreyScaleImage2D image1, GreyScaleImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new GreyScaleImage2D(width, height);

            if (image1.AreSameSize(image2))
            {
                image.Fill((x, y) =>
                {
                    return image1[x, y] + image2[x, y];
                });
            }
            else
            {
                image.Fill((x, y) =>
                {
                    float u = width > 1 ? x / (width - 1.0f) : 0;
                    float v = height > 1 ? y / (height - 1.0f) : 0;

                    return image1[x, y] + image2.GetValue(u, v);
                });
            }

            return image;
        }

        /// <summary>
        /// Subtracts the two images.
        /// The resulting inage will be the same size as the first image.
        /// If the second image is a different size then bilinear filtering will be used sample the image.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="image2">The second image.</param>
        /// <returns>A image the same size as the first.</returns>
        public static GreyScaleImage2D operator -(GreyScaleImage2D image1, GreyScaleImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new GreyScaleImage2D(width, height);

            if (image1.AreSameSize(image2))
            {
                image.Fill((x, y) =>
                {
                    return image1[x, y] - image2[x, y];
                });
            }
            else
            {
                image.Fill((x, y) =>
                {
                    float u = width > 1 ? x / (width - 1.0f) : 0;
                    float v = height > 1 ? y / (height - 1.0f) : 0;

                    return image1[x, y] - image2.GetValue(u, v);
                });
            }

            return image;
        }

        /// <summary>
        /// Multiple the two images.
        /// The resulting inage will be the same size as the first image.
        /// If the second image is a different size then bilinear filtering will be used sample the image.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="image2">The second image.</param>
        /// <returns>A image the same size as the first.</returns>
        public static GreyScaleImage2D operator *(GreyScaleImage2D image1, GreyScaleImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new GreyScaleImage2D(width, height);

            if (image1.AreSameSize(image2))
            {
                image.Fill((x, y) =>
                {
                    return image1[x, y] * image2[x, y];
                });
            }
            else
            {
                image.Fill((x, y) =>
                {
                    float u = width > 1 ? x / (width - 1.0f) : 0;
                    float v = height > 1 ? y / (height - 1.0f) : 0;

                    return image1[x, y] * image2.GetValue(u, v);
                });
            }

            return image;
        }

        /// <summary>
        /// Divide the two images.
        /// The resulting inage will be the same size as the first image.
        /// If the second image is a different size then bilinear filtering will be used sample the image.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="image2">The second image.</param>
        /// <returns>A image the same size as the first.</returns>
        public static GreyScaleImage2D operator /(GreyScaleImage2D image1, GreyScaleImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new GreyScaleImage2D(width, height);

            if (image1.AreSameSize(image2))
            {
                image.Fill((x, y) =>
                {
                    var v1 = image1[x, y];
                    var v2 = image2[x, y];

                    return v2 > 0 ? v1 / v2 : 0;
                });
            }
            else
            {
                image.Fill((x, y) =>
                {
                    float u = width > 1 ? x / (width - 1.0f) : 0;
                    float v = height > 1 ? y / (height - 1.0f) : 0;

                    var v1 = image1[x, y];
                    var v2 = image2.GetValue(u, v);

                    return v2 > 0 ? v1 / v2 : 0;
                });
            }

            return image;
        }

        /// <summary>
        /// Add each value in the image with value.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static GreyScaleImage2D operator +(GreyScaleImage2D image1, float value)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new GreyScaleImage2D(width, height);

            image.Fill((x, y) =>
            {
                return image1[x, y] + value;
            });

            return image;
        }

        /// <summary>
        /// Subtract each value in the image with value.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static GreyScaleImage2D operator -(GreyScaleImage2D image1, float value)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new GreyScaleImage2D(width, height);

            image.Fill((x, y) =>
            {
                return image1[x, y] - value;
            });

            return image;
        }

        /// <summary>
        /// Multiply each value in the image with value.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static GreyScaleImage2D operator *(GreyScaleImage2D image1, float value)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new GreyScaleImage2D(width, height);

            image.Fill((x, y) =>
            {
                return image1[x, y] * value;
            });

            return image;
        }

        /// <summary>
        /// Divide each value in the image with value.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static GreyScaleImage2D operator /(GreyScaleImage2D image1, float value)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new GreyScaleImage2D(width, height);

            image.Fill((x, y) =>
            {
                var v = image1[x, y];
                return value > 0 ? v / value : 0;
            });

            return image;
        }

    }
}
