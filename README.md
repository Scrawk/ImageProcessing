A image processing library for personal use.

# Project Layout

The solution consits of three projects as follows.

The ImageProcessing.Console project is used to test code during development and is optional.

The ImageProcessing.Test project contains the unit tests. The unit tests are added as needed and coverage is still low but will increase as the project develops further.

The ImageProcessing project contains the code to build the required binarys and they can be found in the release or debug folders. In these folders you will find the following dll's which you will need to include in any project you wish to use them.

ImageProcessing.dll.
Common.Core.dll.
Common.Collections.dll.
Common.Geometry.dll.
Common.GraphTheory.dll.

The common code [project](https://github.com/Scrawk/Common) is a dependancy of this project and is the source of the common dll's.

For each dll a xml and pbd file can be found which contains code comments and debugging symbals. Inculding theses files is optional but recommended.

# Build Events

Build events are used to move the dll's to other projects I am currenty working on and this may generate a error when you build the project. The build events can the found under the projects properties->build events->on post build. you will need to delete or edit the events to stop any errors.

# Source Material

Based on the material found in the following books.

[Image-Processing-Mathematical-Morphology-Fundamentals](https://www.amazon.com/Image-Processing-Mathematical-Morphology-Fundamentals/dp/1420089439/ref=sr_1_1?crid=E6DGG5JJ6TRQ&keywords=image+processing+and+mathematical+morphology&qid=1648299647&s=books&sprefix=image+processing+and+mathematical+morphology%2Cstripbooks-intl-ship%2C309&sr=1-1)

[Digital-Image-Processing-Algorithmic-Introduction](https://www.amazon.com/Digital-Image-Processing-Algorithmic-Introduction/dp/1447166833/ref=sr_1_16?crid=HCSOG0YBH0VA&keywords=digital+image+processing+a&qid=1651558586&s=books&sprefix=digital+image+processing+a%2Cstripbooks-intl-ship%2C303&sr=1-16) 

[Digital-Image-Processing](https://www.amazon.com.au/Digital-Image-Processing-Rafael-Gonzalez-ebook/dp/B09TB8M315/ref=sr_1_5?crid=3I7WL5T6JOKMF&keywords=digital+image+processing&qid=1652665817&s=books&sprefix=digital+image+processing%2Cstripbooks%2C277&sr=1-5)

# Usage

The library provides four types of images. Each image type represents a different data type each with its own strengths. Below is a example of the creation of each image type.

```

var color_image = new ColorImage2D(width, height);

var greyscale_image = new GreyScaleImage2D(width, height);

var binary_image  = new BinaryImage2D(width, height);

var vector_image = new VectorImage2D(width, height);

```

The images pixels can be accessed using a varity of functions. If normalized uv coordinates are used then linear interpolation will be used when getting the pixels.
A optional wrap mode can be provided (default is clamp) which will tell the image how to handle out of bounds indices. If NONE is used then the image will throw a exception in the case of out of bounds indices.

GetPixel will always return a ColorRGBA even if the data type of the image is not a color type. 
The function GetChannel will return the value for that channel as a float. Each value in a pixel is represented as a float.
The function GetValue will return the value for the provided indices as the images data type.

```

//The wrap mode options.
public enum WRAP_MODE { CLAMP, WRAP, MIRROR, NONE };

float u, v;
int c, m;
var wrap = WRAP_MODE.WRAP;

//Get a pixel from the image.
//GetPixel will always return a ColorRGBA even if the data type of the image is not a color type.
ColorRGBA pixel = greyscale_image.GetPixel(u, v, wrap);

//Get a pixels channel value from the image.
//GetChannel will always return a float even if the data type of the image is not a float type.
float channel = greyscale_image.GetChannel(u, v, c, wrap);

//Get a value from the image.
//GetValue will always return the images data type. 
//ie ColorRGBA for color images, float for greyscale images and bool for binary images.
float value = greyscale_image.GetValue(u, v, wrap);

```

By default GetPixels will use bilinear interpolation if normalized uv's are provided. 
The interpolation method can be changed by using GetPixelsInterpolated.

```

//The interpolation mode options.
public enum INTERPOLATION
{
BILINEAR,
BICUBIC,
BSPLINE,
LANZCOS,
POINT
}

float u, v;
var interp = INTERPOLATION.BICUBIC;

//Get a pixel from the image using interpolation
ColorRGBA pixel = color_image.GetPixelInterpolated(u, v, interp);

```

A image can be rescaled to new smaller or larger sizes and the interpolation method can be provided.
Below is a example of a image being rescaled to 4 times its orginal size and using bicubic interpolation.

```
//The rescale mode options.
public enum RESCALE
{
BILINEAR,
BICUBIC,
BSPLINE,
LANZCOS,
POINT
}

image = ColorImage2D.Rescale(image, image.Width * 4, image.Height * 4, RESCALE.BICUBIC);

```

![lennaresize](https://github.com/Scrawk/ImageProcessing/blob/master/Media/lennaResized.png)

A images mipmaps can be created as follows and the GetPixelMipmap function can be used to get a pixel from the mipmap.
If the mipmap level m is provided as a normalized float then bilinear interpolation will be used to interpolate between the mipmap levels.

```
float u, v;
int m;
var wrap = WRAP_MODE.WRAP;

color_image.CreateMipmaps();

//Get a pixel from the images mipmap at level m.
ColorRGBA pixel = color_image.GetPixelMipmap(u, v, m, wrap);

```

![lennamipmaps](https://github.com/Scrawk/ImageProcessing/blob/master/Media/lennaMipmaps.png)

Below is a example of a gaussian blur applied to a color image using a sigma value of 1. 
Other filters are available such as box blur, sharpen and unsharpen.

```

ColorImage2D blurred = ColorImage2D.GaussianBlur(color_image, 1.0f);

```

![lennablur](https://github.com/Scrawk/ImageProcessing/blob/master/Media/lennaBlur.png)

A images histogram can be created as follows. A histogram can be used to find statistical information from the image and to apply other algorithms like equalization, histogram matching and thresholding. 

The histograms line or bar graph can also be saved as a image for debugging or display purposes.
Below is a example of a histograms bar graph from the greyscale image.

```
// create a histogram with 256 bins
var histogram = new Histogram(greyscale_image, 256);

//create the graph with the bars being white in a black background
var graph = histogram.CreateHistogramBarGraph(ColorRGBA.White, ColorRGBA.Black);

```

![lennabarhisto](https://github.com/Scrawk/ImageProcessing/blob/master/Media/lennaBarHisto.png)

A Image can be converted to a binary image through thresholding. Binary images consist of a single bit per pixel and can have a number morphological algorithms applied to them. Below is a example of a image having Otus thresholding applied. You can see there is still some noise left which can be removed with other methods.

![](https://github.com/Scrawk/ImageProcessing/blob/master/Media/CoinsThreshold.png)

Below is a example of open and close morphological filters being applied to a binary image. You can see this results in most of the noise being removed. Other morphological filters provided are erode, dilate and hit-miss filters.

![](https://github.com/Scrawk/ImageProcessing/blob/master/Media/CoinsOpenClose.png)

Other filters can be used to find the borders of existing structures.

![](https://github.com/Scrawk/ImageProcessing/blob/master/Media/CoinsBorder.png)

Below is a example of a morphological filter called thinning that can find the skeleton of existing structure.

![](https://github.com/Scrawk/ImageProcessing/blob/master/Media/CoinsThinning.png)

Below is a example of a distance transform where a pixels value represents its distance from the nearest empty pixel.

![](https://github.com/Scrawk/ImageProcessing/blob/master/Media/CoinsDist.png)

Spectral methods can also be applied to a image. Below is a example of a discrete fourier transform being applied to a greyscale image. 

![](https://github.com/Scrawk/ImageProcessing/blob/master/Media/CoinsDFT.png)

Below is a example of a discrete cosine transform being applied to a greyscale image. 

![](https://github.com/Scrawk/ImageProcessing/blob/master/Media/CoinsDCT.png)





