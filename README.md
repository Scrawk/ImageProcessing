A image processing library for personal use.

Depends on this common code [project](https://github.com/Scrawk/Common)

Based on the material found in the following books.

[Image-Processing-Mathematical-Morphology-Fundamentals](https://www.amazon.com/Image-Processing-Mathematical-Morphology-Fundamentals/dp/1420089439/ref=sr_1_1?crid=E6DGG5JJ6TRQ&keywords=image+processing+and+mathematical+morphology&qid=1648299647&s=books&sprefix=image+processing+and+mathematical+morphology%2Cstripbooks-intl-ship%2C309&sr=1-1)

[Digital-Image-Processing-Algorithmic-Introduction](https://www.amazon.com/Digital-Image-Processing-Algorithmic-Introduction/dp/1447166833/ref=sr_1_16?crid=HCSOG0YBH0VA&keywords=digital+image+processing+a&qid=1651558586&s=books&sprefix=digital+image+processing+a%2Cstripbooks-intl-ship%2C303&sr=1-16) 

[Digital-Image-Processing](https://www.amazon.com.au/Digital-Image-Processing-Rafael-Gonzalez-ebook/dp/B09TB8M315/ref=sr_1_5?crid=3I7WL5T6JOKMF&keywords=digital+image+processing&qid=1652665817&s=books&sprefix=digital+image+processing%2Cstripbooks%2C277&sr=1-5)

The library provides four types of images. Each image type represents a different data type each with its own strengths. Below is a example of the creation of each image type.

```

var color_image = new ColorImage2D(width, height);

var greyscale_image = new GreyScaleImage2D(width, height);

var binary_image  = new BinaryImage2D(width, height);

var vector_image = new VectorImage2D(width, height);

```

Below is a examle of a gaussian blur applied to a color image using a sigma value of 1. 
Other filters are available such as box blur, sharpen and unsharpen.

```

ColorImage2D blurred = ColorImage2D.GaussianBlur(color_image, 1.0f);

```


![lennablur](https://github.com/Scrawk/ImageProcessing/blob/master/Media/lennaBlur.png)
