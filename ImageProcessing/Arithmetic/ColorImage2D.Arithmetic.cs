using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Images
{
    public partial class ColorImage2D
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        public void Min(float min)
        {
            ParallelModify((v) =>
            {
                return ColorRGB.Min(v, min);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="max"></param>
        public void Max(float max)
        {
            ParallelModify((v) =>
            {
                return ColorRGB.Max(v, max);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Clamp(float min, float max)
        {
            ParallelModify((v) =>
            {
                return ColorRGB.Clamp(v, min, max);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Multiply(ColorRGB value)
        {
            ParallelModify((v) =>
            {
                return v * value;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void Multiply(ColorImage2D image)
        {
            if (Size != image.Size)
                throw new ArgumentException("Images must be the same size.");

            ParallelFill((x, y) =>
            {
                return this[x, y] * image[x, y];
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void Multiply(GreyScaleImage2D image)
        {
            if (Size != image.Size)
                throw new ArgumentException("Images must be the same size.");

            ParallelFill((x, y) =>
            {
                return this[x, y] * image[x, y];
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void Multiply(BinaryImage2D image)
        {
            if (Size != image.Size)
                throw new ArgumentException("Images must be the same size.");

            ParallelFill((x, y) =>
            {
                return this[x, y] * image.GetValue(x, y);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Add(ColorRGB value)
        {
            ParallelModify((v) =>
            {
                return v + value;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void Add(ColorImage2D image)
        {
            if (Size != image.Size)
                throw new ArgumentException("Images must be the same size.");

            ParallelFill((x, y) =>
            {
                return this[x, y] + image[x, y];
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void Add(GreyScaleImage2D image)
        {
            if (Size != image.Size)
                throw new ArgumentException("Images must be the same size.");

            ParallelFill((x, y) =>
            {
                return this[x, y] + image[x, y];
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Subtract(ColorRGB value)
        {
            ParallelModify((v) =>
            {
                return v - value;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void Subtract(ColorImage2D image)
        {
            if (Size != image.Size)
                throw new ArgumentException("Images must be the same size.");

            ParallelFill((x, y) =>
            {
                return this[x, y] - image[x, y];
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void Subtract(GreyScaleImage2D image)
        {
            if (Size != image.Size)
                throw new ArgumentException("Images must be the same size.");

            ParallelFill((x, y) =>
            {
                return this[x, y] - image[x, y];
            });
        }

    }
}
