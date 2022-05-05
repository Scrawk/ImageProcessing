using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{

    public partial class BinaryImage2D
    {
        /// <summary>
        /// Invert each bit in the image.
        /// </summary>
        public void Invert()
        {
            Modify((v) => !v);
        }

        /// <summary>
        /// Or the bits in this image with another.
        /// </summary>
        /// <param name="image">Another image of the same size.</param>
        public void Or(BinaryImage2D image)
        {
            if (AreNotSameSize(image))
                throw new ArgumentException("The images must be the same size to perform the Or operation.");

            Fill((x, y) => this[x, y] | image[x, y]);
        }

        /// <summary>
        /// XOr the bits in this image with another.
        /// </summary>
        /// <param name="image">Another image of the same size.</param>
        public void Xor(BinaryImage2D image)
        {
            if (AreNotSameSize(image))
                throw new ArgumentException("The images must be the same size to perform the Xor operation.");

            Fill((x, y) => this[x, y] ^ image[x, y]);
        }

        /// <summary>
        /// And the bits in this image with another.
        /// </summary>
        /// <param name="image">Another image of the same size.</param>
        public void And(BinaryImage2D image)
        {
            if (AreNotSameSize(image))
                throw new ArgumentException("The images must be the same size to perform the And operation.");

            Fill((x, y) => this[x, y] & image[x, y]);
        }

    }

}
