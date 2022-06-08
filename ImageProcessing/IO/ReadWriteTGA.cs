using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using Common.Core.Numerics;
using Common.Core.Colors;

using ImageProcessing.Images;
using ImageProcessing.IO;

namespace ImageProcessing.IO
{
    public struct TGAParams
    {
        public PIXEL_FORMAT_IO Format;
        public bool FlipY;
        public bool RLE;
        public int Padding;

        public TGAParams(PIXEL_FORMAT_IO format)
        {
            Format = format;
            FlipY = true;
            RLE = false;
            Padding = 0;
        }

        public static TGAParams Default
        {
            get
            {
                var param = new TGAParams();
                param.Format = PIXEL_FORMAT_IO.BGRA;
                param.FlipY = true;
                param.RLE = false;
                param.Padding = 0;

                return param;
            }

        }

        public override string ToString()
        {
            return string.Format("[TGAParams: Format={0}, FlipY={1},RLE={2}, Padding={3}]",
                Format, FlipY, RLE, Padding);
        }
    }

    /// <summary>
    /// https://github.com/jpcy/xatlas/blob/master/source/thirdparty/stb_image_write.h
    /// http://www.paulbourke.net/dataformats/tga/
    /// </summary>
    public static class ReadWriteTGA
    {
        public static void Write(IImage2D image, string filename, TGAParams param)
        {

            if (!param.RLE)
            {
                using (FileStream file = new FileStream(filename, FileMode.Create))
                {
                    WriteHeader(file, image, param);
                    WritePixels(file, image, param);
                }
            }
            else
            {
                using (FileStream file = new FileStream(filename, FileMode.Create))
                {
                    int width = image.Width;
                    int height = image.Height;
                    int channels = image.Channels;
                    int format_channels = EnumUtil.Channels(param.Format);
                    bool hasAlpha = EnumUtil.HasAlpha(param.Format);    

                    int i, j, k;
                    int jend, jdir;
                    var pixel = new byte[] { 0, 0, 0, 255 };

                    WriteHeader(file, image, param);

                    var raw_param = new RawParams();
                    raw_param.BitDepth = BIT_DEPTH.B8;
                    raw_param.FlipY = false;

                    var bytes = ReadWriteRaw.ToBytes(image, raw_param);

                    if (!param.FlipY)
                    {
                        j = 0;
                        jend = height;
                        jdir = 1;
                    }
                    else
                    {
                        j = height - 1;
                        jend = -1;
                        jdir = -1;
                    }
                    for (; j != jend; j += jdir)
                    {
                        int row = j * width * channels;
                        int len;

                        for (i = 0; i < width; i += len)
                        {
                            int begin = row + i * channels;
                            int diff = 1;
                            len = 1;

                            if (i < width - 1)
                            {
                                ++len;
                                diff = memcmp(bytes, begin, row + (i + 1) * channels, channels);

                                if (diff == 1)
                                {
                                    int prev = begin;
                                    for (k = i + 2; k < width && len < 128; ++k)
                                    {
                                        if (memcmp(bytes, prev, row + k * channels, channels) != 0)
                                        {
                                            prev += channels;
                                            ++len;
                                        }
                                        else
                                        {
                                            --len;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    for (k = i + 2; k < width && len < 128; ++k)
                                    {
                                        if (memcmp(bytes, begin, row + k * channels, channels) == 0)
                                        {
                                            ++len;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                            if (diff == 1)
                            {
                                byte header = (byte)((len - 1) & 255);
                                file.WriteByte(header);

                                for (k = 0; k < len; ++k)
                                {
                                    for (int c = 0; c < 3; ++c)
                                    {
                                        int C = Math.Min(channels - 1, c);
                                        pixel[c] = bytes[(begin + k * channels) + C];
                                    }

                                    if (hasAlpha)
                                        pixel[3] = bytes[(begin + k * channels) + (channels-1)];

                                    WritePixel(file, param, pixel);
                                }
                            }
                            else
                            {
                                byte header = (byte)((len - 129) & 255);
                                file.WriteByte(header);

                                for (int c = 0; c < 3; ++c)
                                {
                                    int C = Math.Min(channels - 1, c);
                                    pixel[c] = bytes[begin + C];
                                }

                                if (hasAlpha)
                                    pixel[3] = bytes[begin + (channels - 1)];

                                WritePixel(file, param, pixel);
                            }
                        }


                    }
                }
            }

        }

        private static int memcmp(byte[] bytes, int ptr1, int ptr2, int count)
        {
            for(int i = 0; i < count; i++)
            {
                int b1 = bytes[ptr1 + i];
                int b2 = bytes[ptr2 + i];

                int n = MathUtil.Clamp(b1 - b2, -1, 1);
                if (n != 0)
                    return n;   
            }

            return 0;
        }

        private static void WriteHeader(FileStream file, IImage2D image, TGAParams param)
        {
            int width = image.Width;
            int height = image.Height;
            int channels = EnumUtil.Channels(param.Format);

            int has_alpha = EnumUtil.HasAlpha(param.Format) ? 1 : 0;
            int colorbytes = has_alpha == 1 ? channels - 1 : channels;
            // 3 color channels (RGB/RGBA) = 2, 1 color channel (Y/YA) = 3
            int format = colorbytes < 2 ? 3 : 2;

            if (param.RLE)
                format += 8;

            var zero2 = new byte[] { 0, 0 };

            file.WriteByte(0);
            file.WriteByte(0);
            file.WriteByte((byte)format);

            file.Write(zero2, 0, 2);
            file.Write(zero2, 0, 2);
            file.WriteByte(0);

            file.Write(zero2, 0, 2);
            file.Write(zero2, 0, 2);
            file.Write(BitConverter.GetBytes(width), 0, 2);
            file.Write(BitConverter.GetBytes(height), 0, 2);

            file.WriteByte((byte)((colorbytes + has_alpha) * 8));
            file.WriteByte((byte)(has_alpha * 8));
        }

        private static void WritePixels(FileStream file, IImage2D image, TGAParams param)
        {
            int width = image.Width;
            int height = image.Height;

            byte[] bytes = new byte[4];

            for (int j = 0; j < height; j++)
            {
                int J = j;
                if (param.FlipY)
                    J = height - 1 - j;

                for (int i = 0; i < width; i++)
                {
                    var pixel = image.GetPixel(i, J);
                    pixel.ToBytes(bytes);

                    WritePixel(file, param, bytes);
                }

                for (int i = 0; i < param.Padding; i++)
                    file.WriteByte(0);
            }
        }

        private static void WritePixel(FileStream file, TGAParams param, byte[] pixel)
        {

            switch (param.Format)
            {
                case PIXEL_FORMAT_IO.R:
                    file.WriteByte(pixel[0]);
                    break;
                case PIXEL_FORMAT_IO.G:
                    file.WriteByte(pixel[1]);
                    break;
                case PIXEL_FORMAT_IO.B:
                    file.WriteByte(pixel[2]);
                    break;
                case PIXEL_FORMAT_IO.A:
                    file.WriteByte(pixel[3]);
                    break;
                case PIXEL_FORMAT_IO.RGB:
                    file.WriteByte(pixel[0]);
                    file.WriteByte(pixel[1]);
                    file.WriteByte(pixel[2]);
                    break;
                case PIXEL_FORMAT_IO.RGBA:
                    file.WriteByte(pixel[0]);
                    file.WriteByte(pixel[1]);
                    file.WriteByte(pixel[2]);
                    file.WriteByte(pixel[3]);
                    break;
                case PIXEL_FORMAT_IO.BGR:
                    file.WriteByte(pixel[2]);
                    file.WriteByte(pixel[1]);
                    file.WriteByte(pixel[0]);
                    break;
                case PIXEL_FORMAT_IO.BGRA:
                    file.WriteByte(pixel[2]);
                    file.WriteByte(pixel[1]);
                    file.WriteByte(pixel[0]);
                    file.WriteByte(pixel[3]);
                    break;
                case PIXEL_FORMAT_IO.RRR:
                    file.WriteByte(pixel[0]);
                    file.WriteByte(pixel[0]);
                    file.WriteByte(pixel[0]);
                    break;
                case PIXEL_FORMAT_IO.RRRA:
                    file.WriteByte(pixel[0]);
                    file.WriteByte(pixel[0]);
                    file.WriteByte(pixel[0]);
                    file.WriteByte(pixel[3]);
                    break;
            }

        }

    }
}
