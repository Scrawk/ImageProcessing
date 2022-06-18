using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using Common.Core.Numerics;
using Common.Core.Colors;

using ImageProcessing.Images;

namespace ImageProcessing.IO
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct TGAParams
    {
        public PIXEL_FORMAT_IO Format;
        public bool FlipY;
        public bool RLE;

        public TGAParams(PIXEL_FORMAT_IO format)
        {
            Format = format;
            FlipY = true;
            RLE = false;
        }

        public static TGAParams Default
        {
            get
            {
                var param = new TGAParams();
                param.Format = PIXEL_FORMAT_IO.BGR;
                param.FlipY = true;
                param.RLE = false;
 
                return param;
            }

        }

        public override string ToString()
        {
            return string.Format("[TGAParams: Format={0}, FlipY={1}, RLE={2}]",
                Format, FlipY, RLE);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    internal struct TGAHeader
    {
        [FieldOffset(0)] public byte Offset;
        [FieldOffset(1)] public byte Indexed;
        [FieldOffset(2)] public byte ImageType;
        [FieldOffset(3)] public byte PaletteStart0;
        [FieldOffset(4)] public byte PaletteStart1;
        [FieldOffset(5)] public byte PaletteLen0;
        [FieldOffset(6)] public byte PaletteLen1;
        [FieldOffset(7)] public byte PaletteBits;
        [FieldOffset(8)] public byte XOrigin0;
        [FieldOffset(9)] public byte XOrigin1;
        [FieldOffset(10)] public byte YOrigin0;
        [FieldOffset(11)] public byte YOrigin1;
        [FieldOffset(12)] public byte Width0;
        [FieldOffset(13)] public byte Width1;
        [FieldOffset(14)] public byte Height0;
        [FieldOffset(15)] public byte Height1;
        [FieldOffset(16)] public byte BitsPerPixel;
        [FieldOffset(17)] public byte Inverted;

        [FieldOffset(3)]
        public short PaletteStart;

        [FieldOffset(5)]
        public short PaletteLen;

        [FieldOffset(8)]
        public short XOrigin;

        [FieldOffset(10)]
        public short YOrigin;

        [FieldOffset(12)]
        public short Width;

        [FieldOffset(14)]
        public short Height;

        public override string ToString()
        {
            return string.Format("[TGAHeader: Widtht={0}, Height={1}, ImageType={2}, Indexed={3}, BitsPerPixel={4}, Inverted={5}]",
                Width, Height, ImageType, Indexed, BitsPerPixel, Inverted);
        }
    }

    /// <summary>
    /// stb_image_write - v1.16 - public domain.
    /// writes out PNG/BMP/TGA/JPEG/HDR images to C stdio - Sean Barrett 2010-2015.
    /// no warranty implied; use at your own risk.
    /// https://github.com/nothings/stb
    /// https://github.com/nothings/stb/blob/master/stb_image.h
    /// https://github.com/nothings/stb/blob/master/stb_image_write.h
    /// http://www.paulbourke.net/dataformats/tga/
    /// </summary>
    public static class ReadWriteTGA
    {

        private const int HEADER_BYTES = 18;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="filename"></param>
        public static void Read(IImage2D image, string filename)
        {
            if (!filename.EndsWith(".tga"))
                filename += ".tga";

            using (FileStream file = new FileStream(filename, FileMode.Open))
            {
                var header = ReadHeader(file);
                var pixels = ReadPixels(file, header);

                if (pixels != null)
                {
                    image.Resize(header.Width, header.Height);
                    image.Fill(pixels);
                }

                file.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="IMAGE"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static IMAGE Read<IMAGE>(string filename)
            where IMAGE : IImage2D, new()
        {
            IMAGE image = new IMAGE();

            if (!filename.EndsWith(".tga"))
                filename += ".tga";

            using (FileStream file = new FileStream(filename, FileMode.Open))
            {
                var header = ReadHeader(file);
                var pixels = ReadPixels(file, header);

                if(pixels != null)
                {
                    image.Resize(header.Width, header.Height);  
                    image.Fill(pixels); 
                }

                file.Close();
            }

            return image;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="filename"></param>
        /// <param name="param"></param>
        public static void Write(IImage2D image, string filename, TGAParams param)
        {
            if (image == null)
                return;

            if (!filename.EndsWith(".tga"))
                filename += ".tga";

            using (FileStream file = new FileStream(filename, FileMode.Create))
            {
                WriteHeader(file, image, param);

                if(param.RLE)
                    WritePixelsRLE(file, image, param);
                else
                    WritePixels(file, image, param);

                file.Close();
            }

        }

        private static TGAHeader ReadHeader(FileStream file)
        {
            var header = new TGAHeader();
            var header_bytes = new Byte[HEADER_BYTES];

            int read_bytes = file.Read(header_bytes, 0, header_bytes.Length);
            if (read_bytes != HEADER_BYTES)
                throw new InvalidOperationException("Failed to read header bytes.");

            header.Offset = header_bytes[0];
            header.Indexed = header_bytes[1];
            header.ImageType = header_bytes[2];
            header.PaletteStart0 = header_bytes[3];
            header.PaletteStart1 = header_bytes[4];
            header.PaletteLen0 = header_bytes[5];
            header.PaletteLen1 = header_bytes[6];
            header.PaletteBits = header_bytes[7];
            header.XOrigin0 = header_bytes[8];
            header.XOrigin1 = header_bytes[9];
            header.YOrigin0 = header_bytes[10];
            header.YOrigin1 = header_bytes[11];
            header.Width0 = header_bytes[12];
            header.Width1 = header_bytes[13];
            header.Height0 = header_bytes[14];
            header.Height1 = header_bytes[15];
            header.BitsPerPixel = header_bytes[16];

            //Photoshop seems to presume image is always inverted
            //header.inverted = header_bytes[17];
            header.Inverted = 1;

            return header;
        }

        private static void WriteHeader(FileStream file, IImage2D image, TGAParams param)
        {
            short x_origin = 0;
            short y_origin = 0; 
            short width_short = (short)image.Width;
            short height_short = (short)image.Height;
            int channels = EnumUtil.Channels(param.Format);

            //int has_alpha = EnumUtil.HasAlpha(param.Format) ? 1 : 0;
            int color_bytes = Math.Min(3, channels);
            int format = color_bytes < 2 ? 3 : 2;

            if (param.RLE)
                format += 8;

            file.WriteByte(0); //0 bytes
            file.WriteByte(0); //1 bytes
            file.WriteByte((byte)format); //2 bytes

            file.WriteByte(0); //3 bytes
            file.WriteByte(0); //4 bytes
            file.WriteByte(0); //5 bytes
            file.WriteByte(0); //6 bytes
            file.WriteByte(0); //7 bytes

            var x_origin_bytes = BitConverter.GetBytes(x_origin);
            var y_origin_bytes = BitConverter.GetBytes(y_origin);

            file.WriteByte(x_origin_bytes[0]); //8 bytes
            file.WriteByte(x_origin_bytes[1]); //19 bytes
            file.WriteByte(y_origin_bytes[0]); //10 bytes
            file.WriteByte(y_origin_bytes[1]); //11 bytes

            var width_bytes = BitConverter.GetBytes(width_short);
            var heigth_bytes = BitConverter.GetBytes(height_short);

            file.WriteByte(width_bytes[0]); //12 bytes
            file.WriteByte(width_bytes[1]); //13 bytes
            file.WriteByte(heigth_bytes[0]); //14 bytes
            file.WriteByte(heigth_bytes[1]); //15 bytes

            file.WriteByte((byte)(color_bytes * 8)); //16 bytes
            file.WriteByte(0); //17  bytes
        }

        private static ColorRGBA[] ReadPixels(FileStream file, TGAHeader header)
        {
            byte[] short_bytes = new byte[2];

            int bits_per_pixel = header.BitsPerPixel;
            int image_type = header.ImageType;
            bool is_RLE = false;
            bool is_indexed = header.Indexed != 0;
            bool is_grey = !is_indexed && header.ImageType == 3;
            bool rgb16 = (bits_per_pixel == 15 || bits_per_pixel == 16) && !is_grey;
            int comp = GetComp(bits_per_pixel, is_grey);
            int width = header.Width;
            int height = header.Height;

            if (image_type >= 8)
            {
                image_type -= 8;
                is_RLE = true;
            }

            if (is_indexed)
                throw new InvalidOperationException("Indexed TGA not supported.");

            if (comp == 0)  
                throw new InvalidOperationException("Can't find TGA pixelformat");

            if (width <= 0 || height <= 0)
                throw new InvalidOperationException("Invald size");

            file.Seek(HEADER_BYTES, SeekOrigin.Begin);

            int RLE_count = 0;
            int RLE_repeating = 0;
            int read_next_pixel = 1;
            float inverse_byte_max = 1.0f / byte.MaxValue;

            ColorRGBA pixel = new ColorRGBA(0,0,0,255);
            var pixels = new ColorRGBA[width * height];

            for (int i = 0; i < width * height; ++i)
            {
                if (is_RLE)
                {
                    if (RLE_count == 0)
                    {
                        int RLE_cmd = file.ReadByte();
                        RLE_count = 1 + (RLE_cmd & 127);
                        RLE_repeating = RLE_cmd >> 7;
                        read_next_pixel = 1;
                    }
                    else if (RLE_repeating == 0)
                    {
                        read_next_pixel = 1;
                    }
                }
                else
                {
                    read_next_pixel = 1;
                }

                if (read_next_pixel != 0)
                {
                    if(is_grey)
                    {
                        float r = file.ReadByte();
                        pixel = new ColorRGBA(r, r, r, 255);
                    }
                    else if (rgb16)
                    {
                        short_bytes[0] = (byte)file.ReadByte();
                        short_bytes[1] = (byte)file.ReadByte();
                        ushort px = BitConverter.ToUInt16(short_bytes, 0);
                        int fiveBitMask = 31;

                        // we have 3 channels with 5 bits each
                        float r = (px >> 10) & fiveBitMask;
                        float g = (px >> 5) & fiveBitMask;
                        float b = px & fiveBitMask;

                        r = (r * 255) / fiveBitMask;
                        g = (g * 255) / fiveBitMask;
                        b = (b * 255) / fiveBitMask;

                        pixel = new ColorRGBA(b, g, r, 255);
                    }
                    else
                    {
                        for (int j = 0; j < comp; ++j)
                            pixel[j] = (byte)file.ReadByte();

                        pixel.a = 255;
                    }

                    read_next_pixel = 0;

                    if (comp == 3 || comp == 4)
                        pixel = pixel.Permutate(2, 1, 0, 3);

                    pixel *= inverse_byte_max;
                }

                pixels[i] = pixel;

                --RLE_count;
            }
           
            if (header.Inverted != 0)
            {
                for (int j = 0; j * 2 < height; ++j)
                {
                    int index1 = j * width;
                    int index2 = (height - 1 - j) * width;

                    for (int i = width; i > 0; --i)
                    {
                        var temp = pixels[index1];
                        pixels[index1] = pixels[index2];
                        pixels[index2] = temp;
                        ++index1;
                        ++index2;
                    }
                }
            }

            return pixels;   
        }

        private static int GetComp(int bits_per_pixel, bool is_grey)
        {
            const int grey = 1;
            const int grey_alpha = 2;
            const int rgb = 3;

            if (bits_per_pixel == 8)
            {
                return grey;
            }
            else if (bits_per_pixel == 16 && is_grey)
            {
                return grey_alpha;
            }
            else if (bits_per_pixel == 15 || bits_per_pixel == 16)
            {
                return rgb;
            }
            else if (bits_per_pixel == 24 || bits_per_pixel == 32)
            {
                return bits_per_pixel / 8;
            }

            return 0;
        }

        private static void WritePixels(FileStream file, IImage2D image, TGAParams param)
        {
            int width = image.Width;
            int height = image.Height;

            byte[] bytes = new byte[4];

            file.Seek(HEADER_BYTES, SeekOrigin.Begin);

            for (int j = 0; j < height; j++)
            {
                int J = j;
                if (param.FlipY)
                    J = height - 1 - j;

                for (int i = 0; i < width; i++)
                {
                    var pixel = image.GetPixel(i, J);
                    pixel.ToBytes(bytes);

                    WritePixel(file, param.Format, bytes);
                }
            }
        }

        private static void WritePixelsRLE(FileStream file, IImage2D image, TGAParams param)
        {
            int width = image.Width;
            int height = image.Height;
            int channels = image.Channels;
            //int format_channels = EnumUtil.Channels(param.Format);
            bool hasAlpha = EnumUtil.HasAlpha(param.Format);

            int i, j, k;
            int jend, jdir;
            var pixel = new byte[] { 0, 0, 0, 255 };

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
                                pixel[3] = bytes[(begin + k * channels) + (channels - 1)];

                            WritePixel(file, param.Format, pixel);
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

                        WritePixel(file, param.Format, pixel);
                    }
                }
            }
        }

        private static int memcmp(byte[] bytes, int ptr1, int ptr2, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int b1 = bytes[ptr1 + i];
                int b2 = bytes[ptr2 + i];

                int n = MathUtil.Clamp(b1 - b2, -1, 1);
                if (n != 0)
                    return n;
            }

            return 0;
        }

        private static void WritePixel(FileStream file, PIXEL_FORMAT_IO format, byte[] pixel)
        {

            switch (format)
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
                    //file.WriteByte(pixel[3]);
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
                    //file.WriteByte(pixel[3]);
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
                    //file.WriteByte(pixel[3]);
                    break;
            }

        }

    }
}
