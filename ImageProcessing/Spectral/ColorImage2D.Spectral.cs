using System;
using System.Collections.Generic;
using System.Numerics;

using Common.Core.Numerics;
using ImageProcessing.Spectral;

namespace ImageProcessing.Images
{
    public partial class ColorImage2D
    {
        /// <summary>
        /// Calculate the 2D DCT on a vector image.
        /// Each channel is transformed independantly.
        /// </summary>
        /// <returns>A Vector image containing the spectrum.</returns>
        public static ColorImage2D ForwardDCT(ColorImage2D image, bool includeAlpha = false)
        {
            var dct = new DCT2D();
            var dct_image = new ColorImage2D(image.Width, image.Height);

            for (int i = 0; i < image.Channels; i++)
            {
                if (image.Channels == 3 && !includeAlpha)
                    break;

                var data = image.ToFloatArray(i);
                dct.Forward(data);
                dct_image.FillChannel(data, i);
            }

            return dct_image;
        }

        /// <summary>
        /// Calculate the 2D DCT on a vector image.
        /// Each channel is transformed independantly.
        /// </summary>
        /// <returns>A Vector image containing the spectrum.</returns>
        public static ColorImage2D InverseDCT(ColorImage2D image, bool includeAlpha = false)
        {
            var dct = new DCT2D();
            var dct_image = new ColorImage2D(image.Width, image.Height);

            for (int i = 0; i < image.Channels; i++)
            {
                if (image.Channels == 3 && !includeAlpha)
                    break;

                var data = image.ToFloatArray(i);
                dct.Inverse(data);
                dct_image.FillChannel(data, i);
            }

            return dct_image;
        }
    }
}
