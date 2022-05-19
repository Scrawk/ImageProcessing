using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{

    public enum GREYSCALE_OVERLAY_MODE
    {
        ADD, SUBTRACT, MULTIPLY, DIVIDE,
        SCREEN, OVERLAY,
        SOFT_LIGHT,
        DIFFERENCE, DARKEN, LIGHTEN
    }

    public partial class GreyScaleImage2D
    {
        /// <summary>
        /// Overlay two images.
        /// https://en.wikipedia.org/wiki/Blend_modes
        /// </summary>
        /// <param name="mode">The overlay mode</param>
        /// <param name="image">The other image.</param>
        public void Overlay(GREYSCALE_OVERLAY_MODE mode, GreyScaleImage2D image)
        {
            switch (mode)
            {
                case GREYSCALE_OVERLAY_MODE.ADD:
                    Add(image);
                    break;

                case GREYSCALE_OVERLAY_MODE.SUBTRACT:
                    Subtract(image);
                    break;

                case GREYSCALE_OVERLAY_MODE.MULTIPLY:
                    Multiply(image);
                    break;

                case GREYSCALE_OVERLAY_MODE.DIVIDE:
                    Divide(image);
                    break;

                case GREYSCALE_OVERLAY_MODE.SCREEN:
                    Screen(image);
                    break;

                case GREYSCALE_OVERLAY_MODE.OVERLAY:
                    Overlay(image);
                    break;

                case GREYSCALE_OVERLAY_MODE.SOFT_LIGHT:
                    SoftLight(image);
                    break;

                case GREYSCALE_OVERLAY_MODE.DIFFERENCE:
                    Difference(image);
                    break;

                case GREYSCALE_OVERLAY_MODE.DARKEN:
                    Min(image);
                    break;

                case GREYSCALE_OVERLAY_MODE.LIGHTEN:
                    Max(image);
                    break;
            }
        }

        /// <summary>
        /// Add the two images together.
        /// </summary>
        /// <param name="image"></param>
        public void Add(GreyScaleImage2D image)
        {
            bool sameSize = Size == image.Size;

            FillFromFunction((x, y) =>
            {
                float p1 = this[x, y];
                float p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetValue(u, v);
                }

                return p1 + p2;
            });
        }

        /// <summary>
        /// Subtract the two images.
        /// </summary>
        /// <param name="image"></param>
        public void Subtract(GreyScaleImage2D image)
        {
            bool sameSize = Size == image.Size;

            FillFromFunction((x, y) =>
            {
                float p1 = this[x, y];
                float p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetValue(u, v);
                }

                return p1 - p2;
            });
        }

        /// <summary>
        /// Multiply the two images.
        /// </summary>
        /// <param name="image"></param>
        public void Multiply(GreyScaleImage2D image)
        {
            bool sameSize = Size == image.Size;

            FillFromFunction((x, y) =>
            {
                float p1 = this[x, y];
                float p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetValue(u, v);
                }

                return p1 * p2;
            });
        }

        /// <summary>
        /// Divide the two images.
        /// </summary>
        /// <param name="image"></param>
        public void Divide(GreyScaleImage2D image)
        {
            bool sameSize = Size == image.Size;

            FillFromFunction((x, y) =>
            {
                float p1 = this[x, y];
                float p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetValue(u, v);
                }

                p1 = p2 > 0 ? p1 / p2 : 0;

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
        public void Screen(GreyScaleImage2D image)
        {
            bool sameSize = Size == image.Size;

            FillFromFunction((x, y) =>
            {
                float p1 = this[x, y];
                float p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetValue(u, v);
                }

                var p = 1.0f - (1.0f - p1) * (1.0f - p2);

                return p;
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
        public void Overlay(GreyScaleImage2D image)
        {
            bool sameSize = Size == image.Size;

            FillFromFunction((x, y) =>
            {
                float p1 = this[x, y];
                float p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetValue(u, v);
                }

                float p = 0;

                if (p1 < 0.5f)
                    p = 2 * p1 * p2;
                else
                    p = 1.0f - 2.0f * (1.0f - p1) * (1.0f - p2);

                return p;
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
        public void Difference(GreyScaleImage2D image)
        {
            bool sameSize = Size == image.Size;

            FillFromFunction((x, y) =>
            {
                float p1 = this[x, y];
                float p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetValue(u, v);
                }

                float p = 0;

                if (p1 > p2)
                    p = p1 - p2;
                else
                    p = p2 - p1;

                return p;
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
        public void SoftLight(GreyScaleImage2D image)
        {
            bool sameSize = Size == image.Size;

            FillFromFunction((x, y) =>
            {
                float p1 = this[x, y];
                float p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetValue(u, v);
                }

                float p = 0;

                if (p2 < 0.5f)
                    p = 2 * p1 * p2 + (p1 * p1) * (1.0f - 2.0f * p2);
                else
                    p = 1.0f - 2.0f * (1.0f - p1) * (1.0f - p2);

                return p;
            });
        }

        /// <summary>
        /// Darken Only creates a pixel that retains the smallest components of the foreground and background pixels. 
        /// If the foreground pixel has the components and the background has, the resultant pixel is
        /// </summary>
        /// <param name="image"></param>
        public void Min(GreyScaleImage2D image)
        {
            bool sameSize = Size == image.Size;

            FillFromFunction((x, y) =>
            {
                float p1 = this[x, y];
                float p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetValue(u, v);
                }

                return Math.Min(p1, p2);
            });
        }

        /// <summary>
        /// Lighten Only has the opposite action of Darken Only. 
        /// It selects the maximum of each component from the foreground and background pixels. 
        /// The mathematical expression for Lighten Only is
        /// </summary>
        /// <param name="image"></param>
        public void Max(GreyScaleImage2D image)
        {
            bool sameSize = Size == image.Size;

            FillFromFunction((x, y) =>
            {
                float p1 = this[x, y];
                float p2;

                if (sameSize)
                    p2 = image[x, y];
                else
                {
                    float u = x / (Width - 1.0f);
                    float v = y / (Height - 1.0f);
                    p2 = image.GetValue(u, v);
                }

                return Math.Max(p1, p2);
            });
        }

    }
}
