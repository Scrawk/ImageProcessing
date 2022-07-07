using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Collections.Arrays;

using ImageProcessing.Samplers;
using ImageProcessing.IO;

namespace ImageProcessing.Images
{

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
        /// Get a pixel from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="method">The interpolation method.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        /// <returns>The pixel at index x,y.</returns>
        ColorRGBA GetPixelInterpolated(float u, float v, INTERPOLATION method, WRAP_MODE mode = WRAP_MODE.CLAMP);

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
        /// Get a pixels channel value from the image at normalized index u,v.
        /// </summary>
        /// <param name="u">The first normalized (0-1) index.</param>
        /// <param name="v">The second normalized (0-1) index.</param>
        /// <param name="c">The pixels channel index (0-3).</param>
        /// <param name="method">The interpolation method.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds</param>
        /// <returns>The pixels channel at index x,y,c.</returns>
        float GetChannelInterpolated(float u, float v, int c, INTERPOLATION method, WRAP_MODE mode = WRAP_MODE.CLAMP);

        /// <summary>
        /// Set the pixel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="pixel">The pixel.</param>
        /// <param name="wrap">The wrap mode for indices outside image bounds.</param>
        /// <param name="blend">The mode pixels are blended based on there alpha value. 
        /// Only applies to images with a alpha channel.</param>
        void SetPixel(int x, int y, ColorRGBA pixel, WRAP_MODE wrap = WRAP_MODE.CLAMP, BLEND_MODE blend = BLEND_MODE.ALPHA);

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
        void SetPixelMipmap(int x, int y, int m, ColorRGBA pixel, WRAP_MODE wrap = WRAP_MODE.CLAMP, BLEND_MODE blend = BLEND_MODE.ALPHA);

        /// <summary>
        /// Set the pixels channel at index x,y.
        /// </summary>
        /// <param name="x">The first index.</param>
        /// <param name="y">The second index.</param>
        /// <param name="c">The pixels channel index (0-3).</param>
        /// <param name="value">The pixels channel value.</param>
        /// <param name="mode">The wrap mode for indices outside image bounds.</param>
        void SetChannel(int x, int y, int c, float value, WRAP_MODE mode = WRAP_MODE.CLAMP);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixel"></param>
        void Fill(ColorRGBA pixel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        void Fill(ColorRGBA[] source);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="wrap"></param>
        void Fill(ColorRGBA[,] source, int x = 0, int y = 0, WRAP_MODE wrap = WRAP_MODE.CLAMP);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="wrap"></param>
        void Fill(IImage2D source, int x = 0, int y = 0, WRAP_MODE wrap = WRAP_MODE.CLAMP);

        /// <summary>
        /// Fill the images channel from a array.
        /// </summary>
        /// <param name="source">The array to fill from.</param>
        /// <param name="channel">The channel to fill.</param>
        /// <param name="x">The x index to start filling image from.</param>
        /// <param name="y">The y index to start filling image from.</param>
        /// <param name="wrap">The wrap mode for out of bounds indices into the image.</param>
        void FillChannel(float[,] source, int channel, int x = 0, int y = 0, WRAP_MODE wrap = WRAP_MODE.CLAMP);

        /// <summary>
        /// Fill the image with the value from the function.
        /// </summary>
        /// <param name="func">The function that creates the pixels.</param>
        void Fill(Func<int, int, ColorRGBA> func);

    }
}
