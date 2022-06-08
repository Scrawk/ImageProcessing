using System;
using System.Collections.Generic;

namespace ImageProcessing.Images
{
    /// <summary>
    /// Wrap mode options for when sampling a image.
    /// </summary>
    public enum WRAP_MODE 
    { 
        CLAMP, 
        WRAP, 
        MIRROR, 
        NONE 
    };

    /// <summary>
    /// The mode pixels are blended based on there alpha value.
    /// Only applies to images with a alpha channel.
    /// </summary>
    public enum BLEND_MODE 
    { 
        ALPHA, 
        NONE 
    };

    /// <summary>
    /// The bit depth the values in a pixel.
    /// </summary>
    public enum BIT_DEPTH
    {
        B8 = 8,
        B16 = 16,
        B32 = 32
    };

    /// <summary>
    /// The format of a pixel is how the channels are layed ouy.
    /// This format is restricted to what form should be used 
    /// when saving/loading the pixels data from a file.
    /// </summary>
    public enum PIXEL_FORMAT_IO
    {
        R,
        G,
        B,
        A,
        RGB,
        RGBA,
        BGR,
        BGRA,
        RRR,
        RRRA
    }

    public static class EnumUtil
    {
        public static bool HasAlpha(PIXEL_FORMAT_IO format)
        {
            switch (format)
            {
                case PIXEL_FORMAT_IO.RGBA:
                case PIXEL_FORMAT_IO.BGRA:
                case PIXEL_FORMAT_IO.RRRA:
                    return true;   
                    
                    default:
                    return false;
            }
        }

        public static int Channels(PIXEL_FORMAT_IO format)
        {
            switch (format)
            {
                case PIXEL_FORMAT_IO.R:
                case PIXEL_FORMAT_IO.G:
                case PIXEL_FORMAT_IO.B:
                case PIXEL_FORMAT_IO.A:
                    return 1;

                case PIXEL_FORMAT_IO.RRR:
                case PIXEL_FORMAT_IO.BGR:
                case PIXEL_FORMAT_IO.RGB:
                    return 3;

                case PIXEL_FORMAT_IO.RRRA:
                case PIXEL_FORMAT_IO.BGRA:
                case PIXEL_FORMAT_IO.RGBA:
                    return 4;
                
                default:
                    return 0;
            }
        }
    }
}
