﻿using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{
    public partial class ColorImage2D
    {

        /// <summary>
        /// Offset and scale each value in the image
        /// with the option of taking the abs value first.
        /// </summary>
        /// <param name="offset">The amount to add to the value before appling the function.</param>
        /// <param name="scale">The amount to scale the result of the function.</param>
        /// <param name="abs">True to take the abs of the value before appling the function.</param>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        public void OffsetScale(float offset, float scale, bool abs, bool includeAlpha = false)
        {
            Modify(v =>
            {
                int channels = includeAlpha ? 4 : 3;
                for (int i = 0; i < channels; i++)
                {
                    if (abs) v[i] = Math.Abs(v[i]);
                    v[i] = (v[i] + offset) * scale;
                }

                return v;
            });
        }

        /// <summary>
        /// Scale then offset each value in the image
        /// with the option of taking the abs value first.
        /// </summary>
        /// <param name="scale">The amount to scale the result of the function.</param>
        /// <param name="offset">The amount to add to the value before appling the function.</param>
        /// <param name="abs">True to take the abs of the value before appling the function.</param>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        public void ScaleOffset(float scale, float offset, bool abs, bool includeAlpha = false)
        {
            Modify(v =>
            {
                int channels = includeAlpha ? 4 : 3;
                for (int i = 0; i < channels; i++)
                {
                    if(abs) v[i] = Math.Abs(v[i]);
                    v[i] = v[i] * scale + offset;
                }

                return v;
            });
        }

        /// <summary>
        /// Take the sqrt of each value in the image.
        /// </summary>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        public void Sqrt(bool includeAlpha = false)
        {
            Modify(v =>
            {
                int channels = includeAlpha ? 4 : 3;
                for (int i = 0; i < channels; i++)
                    v[i] = MathUtil.SafeLog10(v[i]);

                return v;
            });
        }

        /// <summary>
        /// Take the log base e of each value in the image.
        /// </summary>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        public void Log(bool includeAlpha = false)
        {
            Modify(v =>
            {
                int channels = includeAlpha ? 4 : 3;
                for (int i = 0; i < channels; i++)
                    v[i] = MathUtil.SafeLog(v[i]);

                return v;
            });
        }

        /// <summary>
        /// Take the log of each value in the image using the provided base value of a.
        /// </summary>
        /// <param name="a">The base to use.</param>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        public void Log(float a, bool includeAlpha = false)
        {
            Modify(v =>
            {
                int channels = includeAlpha ? 4 : 3;
                for (int i = 0; i < channels; i++)
                    v[i] = MathUtil.SafeLog(v[i]);

                return v;
            });
        }

        /// <summary>
        /// Take the log base 10 of each value in the image.
        /// </summary>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        public void Log10(bool includeAlpha = false)
        {
            Modify(v =>
            {
                int channels = includeAlpha ? 4 : 3;
                for(int i = 0; i < channels; i++)
                    v[i] = MathUtil.SafeLog10(v[i]);

                return v;
            });
        }

        /// <summary>
        /// Set the channel at index c of each pixel to the value.
        /// </summary>
        /// <param name="c">The channels index (0-3).</param>
        /// <param name="value">The value.</param>
        public void SetChannel(int c, float value)
        {
            Modify((v) =>
            {
                v[c] = value;
                return v;
            });
        }

        /// <summary>
        /// Returns the normalized square distance between the two images.
        /// </summary>
        /// <param name="image">The other image to compare.</param>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        /// <returns>The normalized square distance.</returns>
        public float SqrDistance(ColorImage2D image, bool includeAlpha = false)
        {
            float sqdist = 0;

            Iterate((x,y) =>
            {
                if(includeAlpha)
                    sqdist += ColorRGBA.SqrDistance(this[x, y], image[x, y]);
                else
                    sqdist += ColorRGB.SqrDistance(this[x, y].rgb, image[x, y].rgb);
            });

            return sqdist / (Width * Height);
        }

        /// <summary>
        /// Normalize each pixels rgba values in the image to be between (inclusive) 0 and 1.
        /// </summary>
        /// <param name="includeAlpha">Should the alpha channel be normalized.</param>
        public void NormalizeRGBA(bool includeAlpha = false)
        {
            ColorRGBA min, max;
            MinMaxRGBA(out min, out max);

            Modify((c) =>
            {
                //Normalize the rgb channels.
                for(int i = 0; i < 3; i++)
                    c[i] = MathUtil.Normalize(c[i], min[i], max[i]);

                //Normalize the alpha channel if required.
                if(includeAlpha)
                    c.a = MathUtil.Normalize(c.a, min.a, max.a);

                return ColorRGBA.Clamp(c, 0, 1);
            });
        }

        /// <summary>
        /// Normalize each pixels rgba values by intensity in the image to be between (inclusive) 0 and 1.
        /// </summary>
        /// <param name="includeAlpha">Should the alpha channel be normalized.</param>
        public void NormalizeIntensity(bool includeAlpha = false)
        {
            float min, max;
            MinMaxIntensity(out min, out max);

            Modify((c) =>
            {
                //Normalize the rgb channels.
                for (int i = 0; i < 3; i++)
                    c[i] = MathUtil.Normalize(c[i], min, max);

                //Normalize the alpha channel if required.
                if (includeAlpha)
                    c.a = MathUtil.Normalize(c.a, min, max);

                return ColorRGBA.Clamp(c, 0, 1);
            });
        }

        /// <summary>
        /// Make each value in image the smaller of the two values.
        /// </summary>
        /// <param name="value">The another value.</param>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        public void Min(float value, bool includeAlpha = false)
        {
            Modify((c) =>
            {
                if(includeAlpha)
                    return ColorRGBA.Min(c, value);
                else
                    return ColorRGB.Min(c.rgb, value).RGBA(c.a);
            });
        }

        /// <summary>
        /// Make each value in image the larger of the two values.
        /// </summary>
        /// <param name="value">The another value.</param>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        public void Max(float value, bool includeAlpha = false)
        {
            Modify((c) =>
            {
                if (includeAlpha)
                    return ColorRGBA.Max(c, value);
                else
                    return ColorRGB.Max(c.rgb, value).RGBA(c.a);
            });
        }

        /// <summary>
        /// Clamp each pixel to be no more or less that the min and max.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        public void Clamp(float min, float max, bool includeAlpha = false)
        {
            Modify((c) =>
            {
                if (includeAlpha)
                    return ColorRGBA.Clamp(c, min, max);
                else
                    return ColorRGB.Clamp(c.rgb, min, max).RGBA(c.a);
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
                return new ColorRGBA(hsv.h, hsv.s, hsv.v, 1);
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
                return ColorHSV.ToRGB(c.r, c.g, c.b).rgb1;
            });
        }

        /// <summary>
        /// Applies the gamma function to the pixels in the image.
        /// </summary>
        /// <param name="lambda">The power to raise each channel to.</param>
        /// <param name="a">The constant the result is multiplied by. Defaults to 1.</param>
        public void Gamma(float lambda, float a = 1)
        {
            Modify(c =>
            {
                c.Gamma(lambda, a);
                return c;
            });
        }

        /// <summary>
        /// Inverts the pixels in the image.
        /// </summary>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        public void Invert(bool includeAlpha = false)
        {
            Modify(c =>
            {
                if (includeAlpha)
                    return 1.0f - c;
                else
                    return (1.0f - c.rgb).RGBA(c.a);
            });
        }

        /// <summary>
        /// Add the pixels in the image.
        /// </summary>
        /// <param name="pixel">The pixel.</param>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        public void Add(ColorRGBA pixel, bool includeAlpha = false)
        {
            Modify(c =>
            {
                if(includeAlpha)
                    return c + pixel;
                else
                    return (c.rgb + pixel.rgb).RGBA(c.a);
            });
        }

        /// <summary>
        /// Subtract the pixels in the image.
        /// </summary>
        /// <param name="pixel">The pixel.</param>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        /// <param name="reverse">Reverse the operations order.</param>
        public void Subtract(ColorRGBA pixel, bool includeAlpha = false, bool reverse = false)
        {
            Modify(c =>
            {
                if (includeAlpha)
                {
                    if (reverse)
                        return pixel - c;
                    else
                        return c - pixel;
                }
                else
                {
                    if (reverse)
                        return (pixel.rgb - c.rgb).RGBA(c.a);
                    else
                        return (c.rgb - pixel.rgb).RGBA(c.a);
                }

            });
        }

        /// <summary>
        /// Multiply the pixels in the image.
        /// </summary>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        /// <param name="pixel">The pixel.</param>
        public void Multiply(ColorRGBA pixel, bool includeAlpha = false)
        {
            Modify(c =>
            {
                if (includeAlpha)
                    return c * pixel;
                else
                    return (c.rgb * pixel.rgb).RGBA(c.a);
            });
        }

        /// <summary>
        /// Add the pixels in the image.
        /// </summary>
        /// <param name="includeAlpha">Should the alpha channel the included.</param>
        /// <param name="pixel">The pixel.</param>
        /// <param name="reverse">Reverse the operations order.</param>
        public void Divide(ColorRGBA pixel, bool includeAlpha = false, bool reverse = false)
        {
            Modify(c =>
            {
                if (includeAlpha)
                {
                    if (reverse)
                    {
                        c.r = c.r > 0 ? pixel.r / c.r : 0;
                        c.g = c.g > 0 ? pixel.g / c.g : 0;
                        c.b = c.b > 0 ? pixel.b / c.b : 0;
                        c.a = c.a > 0 ? pixel.a / c.a : 0;
                    }
                    else
                    {
                        c.r = pixel.r > 0 ? c.r / pixel.r : 0;
                        c.g = pixel.g > 0 ? c.g / pixel.g : 0;
                        c.b = pixel.b > 0 ? c.b / pixel.b : 0;
                        c.a = pixel.a > 0 ? c.a / pixel.a : 0;
                    }
                }
                else
                {
                    if (reverse)
                    {
                        c.r = c.r > 0 ? pixel.r / c.r : 0;
                        c.g = c.g > 0 ? pixel.g / c.g : 0;
                        c.b = c.b > 0 ? pixel.b / c.b : 0;
                    }
                    else
                    {
                        c.r = pixel.r > 0 ? c.r / pixel.r : 0;
                        c.g = pixel.g > 0 ? c.g / pixel.g : 0;
                        c.b = pixel.b > 0 ? c.b / pixel.b : 0;
                    }
                }

                return c;
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
        public static ColorImage2D operator +(ColorImage2D image1, ColorImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new ColorImage2D(width, height);

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

                    return image1[x, y] + image2.GetPixel(u, v);
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
        public static ColorImage2D operator -(ColorImage2D image1, ColorImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new ColorImage2D(width, height);

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

                    return image1[x, y] - image2.GetPixel(u, v);
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
        public static ColorImage2D operator *(ColorImage2D image1, ColorImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new ColorImage2D(width, height);

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

                    return image1[x, y] * image2.GetPixel(u, v);
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
        public static ColorImage2D operator /(ColorImage2D image1, ColorImage2D image2)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new ColorImage2D(width, height);

            if (image1.AreSameSize(image2))
            {
                image.Fill((x, y) =>
                {
                    var c1 = image1[x, y];
                    var c2 = image2[x, y];
                    var c = new ColorRGBA();

                    c.r = c2.r > 0 ? c1.r / c2.r : 0;
                    c.g = c2.g > 0 ? c1.g / c2.g : 0;
                    c.b = c2.b > 0 ? c1.b / c2.b : 0;
                    c.a = c2.a > 0 ? c1.a / c2.a : 0;

                    return c;
                });
            }
            else
            {
                image.Fill((x, y) =>
                {
                    float u = width > 1 ? x / (width - 1.0f) : 0;
                    float v = height > 1 ? y / (height - 1.0f) : 0;

                    var c1 = image1[x, y];
                    var c2 = image2.GetPixel(u, v);
                    var c = new ColorRGBA();

                    c.r = c2.r > 0 ? c1.r / c2.r : 0;
                    c.g = c2.g > 0 ? c1.g / c2.g : 0;
                    c.b = c2.b > 0 ? c1.b / c2.b : 0;
                    c.a = c2.a > 0 ? c1.a / c2.a : 0;

                    return c;
                });
            }

            return image;
        }

        /// <summary>
        /// Add each pixel in the image with pixel.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="pixel">The pixel.</param>
        /// <returns></returns>
        public static ColorImage2D operator +(ColorImage2D image1, ColorRGBA pixel)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new ColorImage2D(width, height);

            image.Fill((x, y) =>
            {
                return image1[x, y] + pixel;
            });

            return image;
        }

        /// <summary>
        /// Subtract each pixel in the image with pixel.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="pixel">The pixel.</param>
        /// <returns></returns>
        public static ColorImage2D operator -(ColorImage2D image1, ColorRGBA pixel)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new ColorImage2D(width, height);

            image.Fill((x, y) =>
            {
                return image1[x, y] - pixel;
            });

            return image;
        }

        /// <summary>
        /// Multiply each pixel in the image with pixel.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="pixel">The pixel.</param>
        /// <returns></returns>
        public static ColorImage2D operator *(ColorImage2D image1, ColorRGBA pixel)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new ColorImage2D(width, height);

            image.Fill((x, y) =>
            {
                return image1[x, y] * pixel;
            });

            return image;
        }

        /// <summary>
        /// Divide each pixel in the image with pixel.
        /// </summary>
        /// <param name="image1">The first image.</param>
        /// <param name="pixel">The pixel.</param>
        /// <returns></returns>
        public static ColorImage2D operator /(ColorImage2D image1, ColorRGBA pixel)
        {
            int width = image1.Width;
            int height = image1.Height;
            var image = new ColorImage2D(width, height);

            image.Fill((x, y) =>
            {
                var c1 = image1[x, y];
                var c = new ColorRGBA();

                c.r = pixel.r > 0 ? c1.r / pixel.r : 0;
                c.g = pixel.g > 0 ? c1.g / pixel.g : 0;
                c.b = pixel.b > 0 ? c1.b / pixel.b : 0;
                c.a = pixel.a > 0 ? c1.a / pixel.a : 0;

                return c;
            });

            return image;
        }

    }
}
