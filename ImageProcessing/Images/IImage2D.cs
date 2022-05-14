﻿using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Collections.Arrays;

using ImageProcessing.Samplers;
using ImageProcessing.Pixels;

namespace ImageProcessing.Images
{

    /// <summary>
    /// Wrap mode options for when sampling a image.
    /// </summary>
    public enum WRAP_MODE { CLAMP, WRAP, MIRROR, NONE };

    /// <summary>
    /// The mode pixels are blended based on there alpha value.
    /// Only applies to images with a alpha channel.
    /// </summary>
    public enum BLEND_MODE { ALPHA, NONE };

    /// <summary>
    /// General interface for a 2 dimensional image.
    /// </summary>
    public interface  IImage2D : IImageSampler2D
    {
        /// <summary>
        /// The images size on the x axis.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The images size on the y axis.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The size of the image.
        /// </summary>
        Point2i Size { get; }   

        /// <summary>
        /// The number of channels in the images pixel.
        /// </summary>
        int Channels { get; }

        /// <summary>
        /// The number of mipmap levels in image.
        /// CreateMipmaps must be called for the image to have mipmaps.
        /// </summary>
        int MipmapLevels { get; }

        /// <summary>
        /// Get the mipmaps width at level m.
        /// </summary>
        /// <param name="m">The mipmap level.</param>
        /// <returns>The mipmaps width.</returns>
        int MipmapWidth(int m);

        /// <summary>
        /// Get the mipmaps height at level m.
        /// </summary>
        /// <param name="m">The mipmap level.</param>
        /// <returns>The mipmaps height.</returns>
        int MipmapHeight(int m);

        /// <summary>
        /// Resize the array. Will clear any existing data.
        /// </summary>
        void Resize(int width, int height);

        /// <summary>
        /// Get a pixel from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        new ColorRGBA GetPixel(int x, int y, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a pixel from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="m">The mipmap index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        ColorRGBA GetPixelMipmap(int x, int y, int m, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a pixel from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="m">The mipmap index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        ColorRGBA GetPixelMipmap(float u, float v, int m, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a pixel from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="m">The mipmap normalized (0-1) index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        ColorRGBA GetPixelMipmap(float u, float v, float m, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a pixel from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        new ColorRGBA GetPixel(float u, float v, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a pixels channel value from the image at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="c">The pixels channel index (0-3).</param>
        /// <param name="mode">The wrap mode for indices outside image bounds</param>
        /// <returns>The pixels channel at index x,y,c.</returns>
        float GetChannel(int x, int y, int c, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Get a pixels channel value from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="c">The pixels channel index (0-3).</param>
        /// <param name="mode">The wrap mode for indices outside image bounds</param>
        /// <returns>The pixels channel at index x,y,c.</returns>
        float GetChannel(float u, float v, int c, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="pixel">The pixel.</param>
        /// <param name="wrap">The wrap mode for indices outside image bounds.</param>
        /// <param name="blend">The mode pixels are blended based on there alpha value. 
        /// Only applies to images with a alpha channel.</param>
        void SetPixel(int x, int y, ColorRGBA pixel, WRAP_MODE wrap = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA);

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="m">The mipmap index.</param>
        /// <param name="pixel">The pixel.</param>
        /// <param name="wrap">The wrap mode for indices outside image bounds.</param>
        /// <param name="blend">The mode pixels are blended based on there alpha value. 
        /// Only applies to images with a alpha channel.</param>
        void SetPixelMipmap(int x, int y, int m, ColorRGBA pixel, WRAP_MODE wrap = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA);

        /// <summary>
        /// Set the pixels channel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="c">The pixels channel index (0-3).</param>
        /// <param name="value">The pixels channel value.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        void SetChannel(int x, int y, int c, float value, WRAP_MODE mode = WRAP_MODE.NONE);

        /// <summary>
        /// Get the images data as bytes.
        /// </summary>
        /// <param name="bitDepth">The bitdepth of the bytes.</param>
        /// <param name="includeAlpha">Should the alpha channel be included.</param>
        /// <param name="bigEndian">The endianness if 16 bits.</param>
        /// <returns></returns>
        byte[] ToBytes(BIT_DEPTH bitDepth, bool includeAlpha, bool bigEndian = false);

        /// <summary>
        /// Add a property to the image. Could be any object.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The properties value.</param>
        void AddProperty(string name, object value);

        /// <summary>
        /// Get a property by its name.
        /// </summary>
        /// <param name="name">The properties name.</param>
        /// <returns>The property if found or null if not.</returns>
        object GetProperty(string name);

        /// <summary>
        /// Remove a property.
        /// </summary>
        /// <param name="name">The properties name.</param>
        /// <returns>True if tthe property was removed.</returns>
        bool RemoveProperty(string name);

        /// <summary>
        /// Clear all properties.
        /// </summary>
        void ClearProperties();

    }
}
