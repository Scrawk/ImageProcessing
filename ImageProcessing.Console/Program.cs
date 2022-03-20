using System;
using System.IO;
using System.Drawing;

using Common.Core.Numerics;
using Common.Core.Time;
using Common.Core.Shapes;
using Common.Core.Colors;
using Common.GraphTheory.AdjacencyGraphs;

using ImageProcessing.Images;

using CONSOLE = System.Console;

namespace ImageProcessing.Console
{
    class Program
    {
        static void Main(string[] args)
        {


            int bufferSize = 32;

            var bitmap1 = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/Grass1.png"));
            var bitmap2 = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/Grass2.png"));

            int width1 = bitmap1.Width;
            int height1 = bitmap1.Height;

            int width2 = bitmap2.Width;
            int height2 = bitmap2.Height;

            int width = bitmap1.Width + bitmap2.Width;
            int height = bitmap1.Height;

            var image = new ColorImage2D(width - bufferSize, height);

            image.Fill((x, y) => 
            {

                ColorRGB pixel = new ColorRGB();

                if (x < width1 - bufferSize)
                {
                    pixel = ToColorRGB(bitmap1.GetPixel(x, y));
                }
                else if (x >= width1 - bufferSize && x < width1)
                {
                    var pixel1 = bitmap1.GetPixel(x, y);
                    var pixel2 = bitmap2.GetPixel(x - width1 + bufferSize, y);

                    var col1 = ToColorRGB(pixel1);
                    var col2 = ToColorRGB(pixel2);

                    var sd = ColorRGB.SqrDistance(col1, col2);

                    pixel = new ColorRGB(sd);

                }
                else if(x >= width1)
                {
                    pixel = ToColorRGB(bitmap2.GetPixel(x - width1, y));
                }

                return pixel; 
            });

           


            var bytes = image.ToBytes(8);
            File.WriteAllBytes("C:/Users/Justin/OneDrive/Desktop/Test.raw", bytes);

        }

        private static ColorRGB ToColorRGB(Color pixel)
        {
            float r = pixel.R / 255.0f;
            float g = pixel.G / 255.0f;
            float b = pixel.B / 255.0f;

            return new ColorRGB(r, g, b);
        }
    }
}
