using System;
using System.Collections.Generic;
using System.Linq;

using Common.Core.Numerics;
using Common.Core.Colors;

namespace ImageProcessing.Images
{
    public enum BIT_DEPTH
    {
        B8 = 8, 
        B16 = 16, 
        B32 = 32
    };

    public partial class Image2D<T>
    {

        /// <summary>
        /// Save the image as raw bytes.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        /// <param name="bitDepth">The bitdepth of the file.</param>
        /// <param name="bigEndian">The endianness if 16 bits.</param>
        public void SaveAsRaw(string filename, bool includeAlpha = false, BIT_DEPTH bitDepth = BIT_DEPTH.B8, bool bigEndian = false)
        {
            if (!filename.EndsWith(".raw"))
                filename += ".raw";

            var bytes = ToBytes(bitDepth, includeAlpha, bigEndian);
            System.IO.File.WriteAllBytes(filename, bytes);
        }

        /// <summary>
        /// Save the images mipmaps as raw bytes.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        /// <param name="bitDepth">The bitdepth of the file.</param>
        /// <param name="bigEndian">The endianness if 16 bits.</param>
        public void SaveMipmapsAsRaw(string filename, bool includeAlpha = false, BIT_DEPTH bitDepth = BIT_DEPTH.B8, bool bigEndian = false)
        {
            if(!HasMipmaps)
            {
                SaveAsRaw(filename, includeAlpha, bitDepth, bigEndian);
            }
            else
            {
                if (filename.EndsWith(".raw"))
                    filename = filename.Substring(0, filename.Length - 4);

                for (int i = 0; i < MipmapLevels; i++)
                {
                    string name = filename + i + ".raw";
                    var bytes = GetMipmapInterface(i).ToBytes(bitDepth, includeAlpha, bigEndian);
                    System.IO.File.WriteAllBytes(name, bytes);
                }

            }
        }

        /// <summary>
        /// Load from a byte array.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <param name="includeAlpha">Is the alpha channel included.</param>
        /// <param name="bitDepth">The bytes bit depth.</param>
        /// <param name="bigEndian">The endianness if 16 bits.</param>
        public void LoadFromRaw(string filename, bool includeAlpha = false, BIT_DEPTH bitDepth = BIT_DEPTH.B8, bool bigEndian = false)
        {
            if (!filename.EndsWith(".raw"))
                filename += ".raw";

            var bytes = System.IO.File.ReadAllBytes(filename);

            FromBytes(bytes, bitDepth, includeAlpha, bigEndian);
        }

        /// <summary>
        /// Get the images data as bytes.
        /// </summary>
        /// <param name="bitDepth">The bitdepth of the bytes.</param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        /// <param name="bigEndian">The endianness if 16 bits.</param>
        /// <returns></returns>
        public byte[] ToBytes(BIT_DEPTH bitDepth, bool includeAlpha = false, bool bigEndian = false)
        {
            int numBytes = (int)bitDepth / 8;
            var bytes = new byte[Width * Height * Channels * numBytes];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    ColorRGBA pixel = GetPixel(x, y);

                    int channels = Channels;
                    if(includeAlpha)
                        channels = Math.Min(channels, 3);

                    for (int c = 0; c < channels; c++)
                    {
                        int i = (x + y * Width) * Channels + c;
                        Write(pixel[c], i, bytes, (int)bitDepth, bigEndian);
                    }
                }
            }

            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        /// <param name="bitDepth"></param>
        /// <param name="bigEndian"></param>
        public void FromBytes(byte[] bytes, BIT_DEPTH bitDepth, bool includeAlpha, bool bigEndian = false)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int channels = Channels;
                    if (includeAlpha)
                        channels = Math.Min(channels, 3);

                    ColorRGBA pixel = new ColorRGBA(0,0,0,1);
                    for (int c = 0; c < channels; c++)
                    {
                        int i = (x + y * Width) * Channels + c;
                        pixel[c] = Read(i, bytes, (int)bitDepth, bigEndian);
                    }

                    if (Channels == 1)
                        pixel = pixel.rrra;

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
