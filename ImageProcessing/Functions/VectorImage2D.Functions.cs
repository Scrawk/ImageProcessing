using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{
    public partial class VectorImage2D
    {

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
                for (int i = 0; i < Channels; i++)
                {
                    if (abs) v[i] = Math.Abs(v[i]);
                    v[i] = offset + v[i] * scale;
                }

                return v;
            });
        }

        /// <summary>
        /// Scale then offset each value in the image
        /// with the option of taking the abs value first.
        /// </summary>
        /// <param name="offset">The amount to add to the value before appling the function.</param>
        /// <param name="scale">The amount to scale the result of the function.</param>
        /// <param name="abs">True to take the abs of the value before appling the function.</param>
        public void ScaleOffset(float offset, float scale, bool abs)
        {
            Modify(v =>
            {
                for (int i = 0; i < Channels; i++)
                {
                    if (abs) v[i] = Math.Abs(v[i]);
                    v[i] = v[i] * scale + offset;
                }

                return v;
            });
        }

        /// <summary>
        /// Take the sqrt of each value in the image.
        /// </summary>
        public void Sqrt(bool includeAlpha = false)
        {
            Modify(v =>
            {
                for (int i = 0; i < Channels; i++)
                    v[i] = MathUtil.SafeLog10(v[i]);

                return v;
            });
        }

        /// <summary>
        /// Take the log base e of each value in the image.
        /// </summary>
        public void Log()
        {
            Modify(v =>
            {
                for (int i = 0; i < Channels; i++)
                    v[i] = MathUtil.SafeLog(v[i]);

                return v;
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
                for (int i = 0; i < Channels; i++)
                    v[i] = (float)Math.Log(v[i], a);

                return v;
            });
        }

        /// <summary>
        /// Take the log base 10 of each value in the image.
        /// </summary>
        public void Log10()
        {
            Modify(v =>
            {
                for (int i = 0; i < Channels; i++)
                    v[i] = MathUtil.SafeLog10(v[i]);

                return v;
            });
        }

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

        /// <summary>
        /// Inverts the vectors in the image.
        /// </summary>
        public void Invert()
        {
            Modify(v =>
            {
                return 1.0f - v;
            });
        }

        /// <summary>
        /// Add the vectors in the image.
        /// </summary>
        /// <param name="vector">The vector.</param>
        public void Add(Vector2f vector)
        {
            Modify(v =>
            {
                return v + vector;
            });
        }

        /// <summary>
        /// Subtract the vectors in the image.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="reverse">Reverse the operations order.</param>
        public void Subtract(Vector2f vector, bool reverse = false)
        {
            Modify(v =>
            {
                if (reverse)
                    return vector - v;
                else
                    return v - vector;
            });
        }

        /// <summary>
        /// Multiply the vectors in the image.
        /// </summary>
        /// <param name="vector">The vector.</param>
        public void Multiply(Vector2f vector)
        {
            Modify(v =>
            {
                return v * vector;
            });
        }

        /// <summary>
        /// Add the vectors in the image.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="reverse">Reverse the operations order.</param>
        public void Divide(Vector2f vector, bool reverse = false)
        {
            Modify(v =>
            {
                if (reverse)
                {
                    v.x = v.x > 0 ? vector.x / v.x : 0;
                    v.y = v.y > 0 ? vector.y / v.y : 0;
                }
                else
                {
                    v.x = vector.x > 0 ? v.x / vector.x : 0;
                    v.y = vector.y > 0 ? v.y / vector.y : 0;
                }

                return v;
            });
        }

        /// <summary>
        /// Adds the two images together.
        /// The resulting image will be the same size as the first image.
        /// If the second image is a different size then bilinear filtering will be used sample the image.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="image2">The second image.</param>
        /// <returns>A image the same size as the first.</returns>
        public static VectorImage2D operator +(VectorImage2D image1, VectorImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new VectorImage2D(width, height);

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
        /// The resulting image will be the same size as the first image.
        /// If the second image is a different size then bilinear filtering will be used sample the image.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="image2">The second image.</param>
        /// <returns>A image the same size as the first.</returns>
        public static VectorImage2D operator -(VectorImage2D image1, VectorImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new VectorImage2D(width, height);

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
        /// Multiply the two images.
        /// The resulting image will be the same size as the first image.
        /// If the second image is a different size then bilinear filtering will be used sample the image.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="image2">The second image.</param>
        /// <returns>A image the same size as the first.</returns>
        public static VectorImage2D operator *(VectorImage2D image1, VectorImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new VectorImage2D(width, height);

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
        /// The resulting image will be the same size as the first image.
        /// If the second image is a different size then bilinear filtering will be used sample the image.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="image2">The second image.</param>
        /// <returns>A image the same size as the first.</returns>
        public static VectorImage2D operator /(VectorImage2D image1, VectorImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new VectorImage2D(width, height);

            if (image1.AreSameSize(image2))
            {
                image.Fill((x, y) =>
                {
                    var v1 = image1[x, y];
                    var v2 = image2[x, y];
                    var v = new Vector2f();

                    v.x = v2.x > 0 ? v1.x / v2.x : 0;
                    v.y = v2.y > 0 ? v1.y / v2.y : 0;
     
                    return v;
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
                    var v3 = new Vector2f();

                    v3.x = v2.x > 0 ? v1.x / v2.x : 0;
                    v3.y = v2.y > 0 ? v1.y / v2.y : 0;
 
                    return v3;
                });
            }

            return image;
        }

        /// <summary>
        /// Add each vector in the image with vector.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="vector">The vector.</param>
        /// <returns></returns>
        public static VectorImage2D operator +(VectorImage2D image1, Vector2f vector)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new VectorImage2D(width, height);

            image.Fill((x, y) =>
            {
                return image1[x, y] + vector;
            });

            return image;
        }

        /// <summary>
        /// Subtract each vector in the image with vector.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="vector">The vector.</param>
        /// <returns></returns>
        public static VectorImage2D operator -(VectorImage2D image1, Vector2f vector)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new VectorImage2D(width, height);

            image.Fill((x, y) =>
            {
                return image1[x, y] - vector;
            });

            return image;
        }

        /// <summary>
        /// Multiply each vector in the image with vector.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="vector">The vector.</param>
        /// <returns></returns>
        public static VectorImage2D operator *(VectorImage2D image1, Vector2f vector)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new VectorImage2D(width, height);

            image.Fill((x, y) =>
            {
                return image1[x, y] * vector;
            });

            return image;
        }

        /// <summary>
        /// Divide each vector in the image with vector.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="vector">The vector.</param>
        /// <returns></returns>
        public static VectorImage2D operator /(VectorImage2D image1, Vector2f vector)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new VectorImage2D(width, height);

            image.Fill((x, y) =>
            {
                var v1 = image1[x, y];
                var v = new Vector2f();

                v.x = vector.x > 0 ? v1.x / vector.x : 0;
                v.y = vector.y > 0 ? v1.y / vector.y : 0;

                return v;
            });

            return image;
        }

    }
}
