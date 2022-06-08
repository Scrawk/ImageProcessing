using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;

using ImageProcessing.Images;

namespace ImageProcessing.IO
{
    public struct RawParams
    {
        public BIT_DEPTH BitDepth;
        public bool IncludeAlpha;
        public bool BigEndian;
        public bool FlipY;


        public RawParams(BIT_DEPTH bitDepth)
        {
            BitDepth = bitDepth;
            IncludeAlpha = false;
            BigEndian = false;
            FlipY = false;
        }

        public static RawParams Default
        {
            get
            {
                var param = new RawParams();
                param.BitDepth = BIT_DEPTH.B8;
                param.FlipY = true;
                param.IncludeAlpha = false;

                return param;
            }

        }

        public override string ToString()
        {
            return string.Format("[RawParams: BitDepth={0}, FlipY={1}, IncludeAlpha={2}]",
                BitDepth, FlipY, IncludeAlpha);
        }
    }

    internal static class ReadWriteRaw
    {
        /// <summary>
        /// Save the image as raw bytes.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="filename">The filename.</param>
        /// <param name="param"></param>
        public static void Write(IImage2D image, string filename, RawParams param)
        {
            if (!filename.EndsWith(".raw"))
                filename += ".raw";

            var bytes = ReadWriteRaw.ToBytes(image, param);
            System.IO.File.WriteAllBytes(filename, bytes);
        }

        /// <summary>
        /// Load from a byte array.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="filename">The file name.</param>
        /// <param name="param"></param>
        public static void Read(IImage2D image, string filename, RawParams param)
        {
            if (!filename.EndsWith(".raw"))
                filename += ".raw";

            var bytes = System.IO.File.ReadAllBytes(filename);

            ReadWriteRaw.FromBytes(image, bytes, param);
        }

        /// <summary>
        /// Get the images data as bytes.
        /// </summary>
        /// <param name="image">The image to process.</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static byte[] ToBytes(IImage2D image, RawParams param)
        {
            int Width = image.Width;
            int Height = image.Height;  
            int Channels = image.Channels;  

            int numBytes = (int)param.BitDepth / 8;
            var bytes = new byte[Width * Height * Channels * numBytes];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    ColorRGBA pixel = image.GetPixel(x, y);

                    int channels = Channels;
                    if (!param.IncludeAlpha)
                        channels = Math.Min(channels, 3);

                    for (int c = 0; c < channels; c++)
                    {
                        int i = (x + y * Width) * Channels + c;
                        Write(pixel[c], i, bytes, (int)param.BitDepth, param.BigEndian);
                    }
                }
            }

            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="bytes"></param>
        /// <param name="param"></param>
        public static void FromBytes(IImage2D image, byte[] bytes, RawParams param)
        {
            int Width = image.Width;
            int Height = image.Height;
            int Channels = image.Channels;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int channels = Channels;
                    if (!param.IncludeAlpha)
                        channels = Math.Min(channels, 3);

                    ColorRGBA pixel = new ColorRGBA(0, 0, 0, 1);
                    for (int c = 0; c < channels; c++)
                    {
                        int i = (x + y * Width) * Channels + c;
                        pixel[c] = Read(i, bytes, (int)param.BitDepth, param.BigEndian);
                    }

                    if (Channels == 1)
                        pixel = pixel.rrra;

                    image.SetPixel(x, y, pixel);
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
        internal static float Read(int i, byte[] bytes, int bitDepth, bool bigEndian)
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
        internal static void Write(float c, int i, byte[] bytes, int bitDepth, bool bigEndian)
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
        internal static ushort ReadShort(int i, byte[] bytes, bool bigEndian)
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
        internal static float ReadFloat(int i, byte[] bytes)
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
        internal static void WriteShort(float c, int i, byte[] bytes, bool bigEndian)
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
        internal static void WriteFloat(float c, int i, byte[] bytes)
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
