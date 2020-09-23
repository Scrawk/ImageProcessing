using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Images
{
    public partial class Image2D<T>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitDepth"></param>
        /// <param name="bigEndian"></param>
        /// <returns></returns>
        public byte[] ToBytes(int bitDepth, bool bigEndian = false)
        {
            var bytes = new byte[Width * Height * Channels];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    ColorRGB pixel = GetPixel(x, y);

                    for (int c = 0; c < Channels; c++)
                    {
                        int i = (x + y * Width) * Channels + c;
                        Write(pixel[c], i, bytes, bitDepth, bigEndian);
                    }
                }
            }

            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="bitDepth"></param>
        /// <param name="bigEndian"></param>
        public void FromBytes(byte[] bytes, int bitDepth, bool bigEndian = false)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    ColorRGB pixel = new ColorRGB();
                    for (int c = 0; c < Channels; c++)
                    {
                        int i = (x + y * Width) * Channels + c;
                        pixel[c] = Read(i, bytes, bitDepth, bigEndian);
                    }

                    if (Channels == 1)
                        pixel = pixel.rrr;

                    SetPixel(x, y, pixel);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="bytes"></param>
        /// <param name="bitDepth"></param>
        /// <param name="bigEndian"></param>
        /// <returns></returns>
        private static float Read(int i, byte[] bytes, int bitDepth, bool bigEndian)
        {
            switch (bitDepth)
            {
                case 8:
                    return bytes[i] / (float)byte.MaxValue;

                case 16:
                    return ReadShort(i, bytes, bigEndian) / (float)ushort.MaxValue;

                case 32:
                    return ReadFloat(i, bytes);

                default:
                    throw new ArgumentException("Unhandled bit depth " + bitDepth);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="i"></param>
        /// <param name="bytes"></param>
        /// <param name="bitDepth"></param>
        /// <param name="bigEndian"></param>
        private static void Write(float c, int i, byte[] bytes, int bitDepth, bool bigEndian)
        {
            switch (bitDepth)
            {
                case 8:
                    bytes[i] = (byte)MathUtil.Clamp(c * 255, 0, 255);
                    break;

                case 16:
                    WriteShort(c, i, bytes, bigEndian);
                    break;

                case 32:
                    WriteFloat(c, i, bytes);
                    break;

                default:
                    throw new ArgumentException("Unhandled bit depth " + bitDepth);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="bytes"></param>
        /// <param name="bigEndian"></param>
        /// <returns></returns>
        private static ushort ReadShort(int i, byte[] bytes, bool bigEndian)
        {
            int index = i * 2;

            if (bigEndian)
                return (ushort)(bytes[index] * 256 + bytes[index + 1]);
            else
                return (ushort)(bytes[index] + bytes[index + 1] * 256);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static float ReadFloat(int i, byte[] bytes)
        {
            int index = i * 4;
            return BitConverter.ToSingle(bytes, index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="i"></param>
        /// <param name="bytes"></param>
        /// <param name="bigEndian"></param>
        private static void WriteShort(float c, int i, byte[] bytes, bool bigEndian)
        {
            int index = i * 2;

            ushort v = (ushort)MathUtil.Clamp(c * ushort.MaxValue, 0, ushort.MaxValue);
            byte[] b = BitConverter.GetBytes(v);

            if (bigEndian)
            {
                bytes[index + 0] = b[1];
                bytes[index + 1] = b[0];
            }
            else
            {
                bytes[index + 0] = b[0];
                bytes[index + 1] = b[1];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="i"></param>
        /// <param name="bytes"></param>
        private static void WriteFloat(float c, int i, byte[] bytes)
        {
            int index = i * 4;

            byte[] b = BitConverter.GetBytes(c);
            bytes[index + 0] = b[0];
            bytes[index + 1] = b[1];
            bytes[index + 2] = b[2];
            bytes[index + 3] = b[3];
        }
    }
}
