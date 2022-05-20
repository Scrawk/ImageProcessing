using System;
using System.Collections.Generic;
using System.Numerics;

using Common.Core.Numerics;
using ImageProcessing.Spectral;

namespace ImageProcessing.Images
{
    public partial class VectorImage2D
    {
        /// <summary>
        /// Calculate the 2D inverse DFT on a vector image.
        /// Here we are using a vector to store the complex numbers
        /// where the x component is the real part and the y component is the imaginary part.
        /// </summary>
        /// <param name="image">The image to transform.</param>
        /// <returns>THe real part of the transform in a greyscale image.</returns>
        public static GreyScaleImage2D InverseDFT(VectorImage2D image)
        {
            var dft = new DFT2D();

            var real = image.ToFloatArray(0);
            var imag = image.ToFloatArray(1);

            dft.Inverse(real, imag);

            var dft_image = new GreyScaleImage2D(image.Width, image.Height);

            dft_image.FillChannel(real, 0);

            return dft_image;
        }

        /// <summary>
        /// Calculate the 2D DCT on a vector image.
        /// Each channel is transformed independantly.
        /// </summary>
        /// <returns>A Vector image containing the spectrum.</returns>
        public static VectorImage2D ForwardDCT(VectorImage2D image)
        {
            var dct = new DCT2D();
            var dct_image = new VectorImage2D(image.Width, image.Height);

            for (int i = 0; i < image.Channels; i++)
            {
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
        public static VectorImage2D InverseDCT(VectorImage2D image)
        {
            var dct = new DCT2D();
            var dct_image = new VectorImage2D(image.Width, image.Height);

            for (int i = 0; i < image.Channels; i++)
            {
                var data = image.ToFloatArray(i);
                dct.Inverse(data);
                dct_image.FillChannel(data, i);
            }

            return dct_image;
        }

    }
}
