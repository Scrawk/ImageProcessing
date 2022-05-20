using System;
using System.Collections.Generic;
using System.Numerics;

using Common.Core.Numerics;

using ImageProcessing.Spectral;

namespace ImageProcessing.Images
{
    public partial class GreyScaleImage2D
    {
        /// <summary>
        /// Calculate the 2D DFT on a greyscale image.
        /// Here we are using a vector to store the complex numbers
        /// where the x component is the real part and the y component is the imaginary part.
        /// </summary>
        /// <returns>A vector image containing the spectrum as complex numbers.</returns>
        public static VectorImage2D ForwardDFT(GreyScaleImage2D image)
        {
            var dft = new DFT2D();

            var real = image.ToFloatArray(0);
            var imag = new float[image.Width, image.Height];

            dft.Forward(real, imag);

            var dft_image = new VectorImage2D(image.Width, image.Height);

            dft_image.FillChannel(real, 0);
            dft_image.FillChannel(imag, 1);

            return dft_image;
        }


        /// <summary>
        /// Calculate the 2D DCT on a greyscale image.
        /// Here we are using a greyscale image to store the real numbers.
        /// </summary>
        /// <returns>A greyscale image containing the spectrum.</returns>
        public static GreyScaleImage2D ForwardDCT(GreyScaleImage2D image)
        {
            var dct = new DCT2D();

            var real = image.ToFloatArray(0);

            dct.Forward(real);

            var dct_image = new GreyScaleImage2D(image.Width, image.Height);

            dct_image.FillChannel(real, 0);

            return dct_image;
        }

        /// <summary>
        /// Calculate the 2D inverse DCT on a greyscale image.
        /// </summary>
        /// <param name="image">The image to transform.</param>
        /// <returns>The transformed greyscale image.</returns>
        public static GreyScaleImage2D InverseDCT(GreyScaleImage2D image)
        {
            var dct = new DCT2D();

            var real = image.ToFloatArray(0);

            dct.Inverse(real);

            var dft_image = new GreyScaleImage2D(image.Width, image.Height);

            dft_image.FillChannel(real, 0);

            return dft_image;
        }

    }
}
