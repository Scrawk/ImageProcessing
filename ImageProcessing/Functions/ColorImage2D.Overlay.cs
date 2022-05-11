using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{

    public enum COLOR_OVERLAY_MODE
    {
        ALPHA_BLEND,
        ADD, SUBTRACT, MULTIPLY, DIVIDE,
        SCREEN, OVERLAY,
        SOFT_LIGHT,
        DIFFERENCE, DARKEN, LIGHTEN
    }

    public partial class ColorImage2D
    {
        /// <summary>
        /// Overlay two images.
        /// https://en.wikipedia.org/wiki/Blend_modes
        /// </summary>
        /// <param name="mode">The overlay mode</param>
        /// <param name="image">The other image.</param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        public void Overlay(COLOR_OVERLAY_MODE mode, ColorImage2D image, bool includeAlpha = false)
        {
            switch (mode)
            {
                case COLOR_OVERLAY_MODE.ALPHA_BLEND:
                    AlphaBlend(image);
                    break;

                case COLOR_OVERLAY_MODE.ADD:
                    Add(image, includeAlpha);
                    break;

                case COLOR_OVERLAY_MODE.SUBTRACT:
                    Subtract(image, includeAlpha);
                    break;

                case COLOR_OVERLAY_MODE.MULTIPLY:
                    Multiply(image, includeAlpha);
                    break;

                case COLOR_OVERLAY_MODE.DIVIDE:
                    Divide(image, includeAlpha);
                    break;

                case COLOR_OVERLAY_MODE.SCREEN:
                    Screen(image, includeAlpha);
                    break;

                case COLOR_OVERLAY_MODE.OVERLAY:
                    Overlay(image, includeAlpha);
                    break;

                case COLOR_OVERLAY_MODE.SOFT_LIGHT:
                    SoftLight(image, includeAlpha);
                    break;

                case COLOR_OVERLAY_MODE.DIFFERENCE:
                    Difference(image, includeAlpha);
                    break;

                case COLOR_OVERLAY_MODE.DARKEN:
                    Min(image, includeAlpha);
                    break;

                case COLOR_OVERLAY_MODE.LIGHTEN:
                    Max(image, includeAlpha);
                    break;
            }
        }

        /// <summary>
        /// Alpha blend the two images.
        /// </summary>
        /// <param name="image"></param>
        public void AlphaBlend(ColorImage2D image)
        {
            bool sameSize = Size == image.Size;

            Fill((x, y) =>
            {
                ColorRGBA p1 = this[x,y];
                ColorRGBA p2;

                if(sameSize)
                    p2 = image[x,y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetPixel(u,v);
                }

                return ColorRGBA.AlphaBlend(p1, p2);
            });
        }

        /// <summary>
        /// Add the two images together.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        public void Add(ColorImage2D image, bool includeAlpha = false)
        {
            bool sameSize = Size == image.Size;

            Fill((x, y) =>
            {
                ColorRGBA p1 = this[x, y];
                ColorRGBA p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetPixel(u, v);
                }

                if (includeAlpha)
                    return p1 + p2;
                else
                    return (p1.rgb + p2.rgb).RGBA(p1.a);
            });
        }

        /// <summary>
        /// Subtract the two images.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        public void Subtract(ColorImage2D image, bool includeAlpha = false)
        {
            bool sameSize = Size == image.Size;

            Fill((x, y) =>
            {
                ColorRGBA p1 = this[x, y];
                ColorRGBA p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetPixel(u, v);
                }

                if (includeAlpha)
                    return p1 - p2;
                else
                    return (p1.rgb - p2.rgb).RGBA(p1.a);
            });
        }

        /// <summary>
        /// Multiply the two images.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        public void Multiply(ColorImage2D image, bool includeAlpha = false)
        {
            bool sameSize = Size == image.Size;

            Fill((x, y) =>
            {
                ColorRGBA p1 = this[x, y];
                ColorRGBA p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetPixel(u, v);
                }

                if (includeAlpha)
                    return p1 * p2;
                else
                    return (p1.rgb * p2.rgb).RGBA(p1.a);
            });
        }

        /// <summary>
        /// Divide the two images.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        public void Divide(ColorImage2D image, bool includeAlpha = false)
        {
            bool sameSize = Size == image.Size;

            Fill((x, y) =>
            {
                ColorRGBA p1 = this[x, y];
                ColorRGBA p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetPixel(u, v);
                }

                if (includeAlpha)
                {
                    p1.r = p2.r > 0 ? p1.r / p2.r : 0;
                    p1.g = p2.g > 0 ? p1.g / p2.g : 0;
                    p1.b = p2.b > 0 ? p1.b / p2.b : 0;
                    p1.a = p2.a > 0 ? p1.a / p2.a : 0;
                }
                else
                {
                    p1.r = p2.r > 0 ? p1.r / p2.r : 0;
                    p1.g = p2.g > 0 ? p1.g / p2.g : 0;
                    p1.b = p2.b > 0 ? p1.b / p2.b : 0;
                }

                return p1;
            });
        }

        /// <summary>
        /// With Screen blend mode, the values of the pixels in the two layers are inverted, multiplied, 
        /// and then inverted again. The result is the opposite of Multiply: wherever either layer was 
        /// darker than white, the composite is brighter.
        /// This mode is commutative: exchanging two layers does not change the result. 
        /// If one layer contains a homogeneous gray, Screen blend mode is equivalent to using 
        /// this gray value as opacity when doing "normal mode" blend with white top layer.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        public void Screen(ColorImage2D image, bool includeAlpha = false)
        {
            bool sameSize = Size == image.Size;

            Fill((x, y) =>
            {
                ColorRGBA p1 = this[x, y];
                ColorRGBA p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetPixel(u, v);
                }

                var p = 1.0f - (1.0f - p1) * (1.0f - p2);

                if (includeAlpha)
                    return p;
                else
                    return new ColorRGBA(p.r, p.g, p.b, p1.a);
            });
        }

        /// <summary>
        /// Overlay combines Multiply and Screen blend modes.
        /// Where the base layer is light, the top layer becomes lighter; where the base layer is dark, 
        /// the top becomes darker; where the base layer is mid grey, the top is unaffected. 
        /// An overlay with the same picture looks like an S-curve.
        /// Depending on the value a of the base layer, one gets a linear interpolation between 
        /// black (a=0), the top layer (a=0.5), and white(a= 1).
        /// </summary>
        /// <param name="image"></param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        public void Overlay(ColorImage2D image, bool includeAlpha = false)
        {
            bool sameSize = Size == image.Size;

            Fill((x, y) =>
            {
                ColorRGBA p1 = this[x, y];
                ColorRGBA p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetPixel(u, v);
                }

                ColorRGBA p = new ColorRGBA();

                for(int i = 0; i< Channels; i++)
                {
                    float a = p1[i];
                    float b = p2[i];

                    if (p1[i] < 0.5f)
                        p[i] = 2 * a * b;
                    else
                        p[i] = 1.0f - 2.0f * (1.0f - a) * (1.0f - b);
                }

                if (includeAlpha)
                    return p;
                else
                    return new ColorRGBA(p.r, p.g, p.b, p1.a);
            });
        }

        /// <summary>
        /// Difference subtracts the bottom layer from the top layer or the other way around, 
        /// to always get a non-negative value.Blending with black produces no change, 
        /// as values for all colors are 0. (The RGB value for black is (0,0,0).) 
        /// Blending with white inverts the picture.
        /// One of the main utilities for this is during the editing process, 
        /// when it can be used to verify alignment of pictures with similar content.
        /// Exclusion is a very similar blend mode with lower contrast.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        public void Difference(ColorImage2D image, bool includeAlpha = false)
        {
            bool sameSize = Size == image.Size;

            Fill((x, y) =>
            {
                ColorRGBA p1 = this[x, y];
                ColorRGBA p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetPixel(u, v);
                }

                ColorRGBA p = new ColorRGBA();

                for (int i = 0; i < Channels; i++)
                {
                    float a = p1[i];
                    float b = p2[i];

                    if (a > b)
                        p[i] = a - b;
                    else
                        p[i] = b - a;
                }

                if (includeAlpha)
                    return p;
                else
                    return new ColorRGBA(p.r, p.g, p.b, p1.a);
            });
        }

        /// <summary>
        /// Soft light is most closely related to Overlay and is only similar to Hard Light by name. 
        /// Applying pure black or white does not result in pure black or white.
        /// There are a variety of different methods of applying a soft light blend.
        /// All the flavors produce the same result when the top layer is pure black; 
        /// same for when the top layer is pure neutral gray.The Photoshop and illusions.
        /// hu flavors also produce the same result when the top layer is pure white 
        /// (the differences between these two are in how one interpolates between these 3 results).
        /// These three results coincide with gamma correction of the bottom layer with γ=2 (for top black),
        /// unchanged bottom layer(or, what is the same, γ= 1; for top neutral gray), and γ = 0.5(for top white).
        /// </summary>
        /// <param name="image"></param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        public void SoftLight(ColorImage2D image, bool includeAlpha = false)
        {
            bool sameSize = Size == image.Size;

            Fill((x, y) =>
            {
                ColorRGBA p1 = this[x, y];
                ColorRGBA p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetPixel(u, v);
                }

                ColorRGBA p = new ColorRGBA();

                for (int i = 0; i < Channels; i++)
                {
                    float a = p1[i];
                    float b = p2[i];

                    if (b < 0.5f)
                        p[i] = 2 * a * b + (a*a)* (1.0f - 2.0f * b);
                    else
                        p[i] = 1.0f - 2.0f * (1.0f - a) * (1.0f - b);
                }

                if (includeAlpha)
                    return p;
                else
                    return new ColorRGBA(p.r, p.g, p.b, p1.a);
            });
        }

        /// <summary>
        /// Darken Only creates a pixel that retains the smallest components of the foreground and background pixels. 
        /// If the foreground pixel has the components and the background has, the resultant pixel is
        /// </summary>
        /// <param name="image"></param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        public void Min(ColorImage2D image, bool includeAlpha = false)
        {
            bool sameSize = Size == image.Size;

            Fill((x, y) =>
            {
                ColorRGBA p1 = this[x, y];
                ColorRGBA p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetPixel(u, v);
                }

                ColorRGBA p = new ColorRGBA();

                for (int i = 0; i < Channels; i++)
                    p[i] = Math.Min(p1[i], p2[i]);

                if (includeAlpha)
                    return p;
                else
                    return new ColorRGBA(p.r, p.g, p.b, p1.a);
            });
        }

        /// <summary>
        /// Lighten Only has the opposite action of Darken Only. 
        /// It selects the maximum of each component from the foreground and background pixels. 
        /// The mathematical expression for Lighten Only is
        /// </summary>
        /// <param name="image"></param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        public void Max(ColorImage2D image, bool includeAlpha = false)
        {
            bool sameSize = Size == image.Size;

            Fill((x, y) =>
            {
                ColorRGBA p1 = this[x, y];
                ColorRGBA p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetPixel(u, v);
                }

                ColorRGBA p = new ColorRGBA();

                for (int i = 0; i < Channels; i++)
                    p[i] = Math.Max(p1[i], p2[i]);

                if (includeAlpha)
                    return p;
                else
                    return new ColorRGBA(p.r, p.g, p.b, p1.a);
            });
        }

    }
}
