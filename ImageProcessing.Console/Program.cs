using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Time;
using Common.Core.Shapes;
using Common.Core.Colors;
using Common.GraphTheory.GridGraphs;

using ImageProcessing.Images;
using ImageProcessing.Synthesis;

using CONSOLE = System.Console;

namespace ImageProcessing.Console
{
    class Program
    {
        static void WriteLine(object obj)
        {
            if (obj == null)
                System.Console.WriteLine("Null");
            else
                System.Console.WriteLine(obj.ToString());
        }

        private static ColorRGB ToColorRGB(Color pixel)
        {
            float r = pixel.R / 255.0f;
            float g = pixel.G / 255.0f;
            float b = pixel.B / 255.0f;

            return new ColorRGB(r, g, b);
        }

        private static GreyScaleImage2D SquareDifference(ColorImage2D image1, ColorImage2D image2)
        {
            var image = new GreyScaleImage2D(image1.Width, image1.Height);
            image.Fill((x, y) =>
            {
                var sd = ColorRGB.SqrDistance(image1[x, y], image2[x, y]);
                return Math.Max(1, sd * 255);
            });

            return image;
        }

        private static ColorImage2D ToImage(Bitmap bitmap)
        {
            var image = new ColorImage2D(bitmap.Width, bitmap.Height);
            image.Fill((x, y) =>
            {
                return ToColorRGB(bitmap.GetPixel(x, y));
            });

            return image;
        }

        static void Main(string[] args)
        {

            var bitmap1 = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/Test1.png"));
            var bitmap2 = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/Test2.png"));

            var image1 = ToImage(bitmap1);
            var image2 = ToImage(bitmap2);

            var sd = SquareDifference(image1, image2);

            var graph = new GridFlowGraph(sd.ToArray());

            var source = new Box2i(0, 0, graph.Width - 1, graph.Height - 1);
            var center = source.Center.Point2i;
            var sink = new Box2i(center - new Point2i(16), center + new Point2i(16));

            foreach( var p in source.EnumeratePerimeter())
            {
                graph.SetLabel(p, GridFlowGraph.SOURCE);
            }

            foreach (var p in sink.EnumerateBounds())
            {
                graph.SetLabel(p, GridFlowGraph.SINK);
            }

            graph.Calculate();

            

            var cut = new ColorImage2D(graph.Width, graph.Height);
            cut.Fill((x, y) =>
            {
                /*
                if (graph.GetLabel(x, y) == GridFlowGraph.SOURCE)
                    return ColorRGB.Red;
                if (graph.GetLabel(x, y) == GridFlowGraph.SINK)
                    return ColorRGB.Green;
                else
                    return ColorRGB.Black;
                */

                if (graph.GetLabel(x, y) == GridFlowGraph.SOURCE)
                    return image1.GetPixel(x, y);
                if (graph.GetLabel(x, y) == GridFlowGraph.SINK)
                    return image2.GetPixel(x, y);
                else
                    return ColorRGB.Black;
            });

            cut.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/Cut.raw");

            WriteLine("Done");

            //var set = new WangTileSet(3, 2, 128);
            //set.Test();
            //WriteLine("Done");

            /*
            var timer = new Timer();    
            timer.Start();

            var bitmap = new Bitmap(Image.FromFile("C:/Users/Justin/OneDrive/Desktop/TexturesCom_Gravel0160_2_S.jpg"));

            var image = new ColorImage2D(bitmap.Width, bitmap.Height);
            image.Fill((x, y) =>
            {
                return ToColorRGB(bitmap.GetPixel(x, y));
            });

            int overlap = 8;
            int exemplarSize = 128 + overlap;
            int imageSize = 512;

            var exemplars = new ExemplarSet(exemplarSize);
            exemplars.CreateExemplarFromRandom(image, 0, 32);

            var systhesis = new ImageSynthesis(exemplars, imageSize, overlap);

            systhesis.CreateSeamlessImage();

            systhesis.Image.SaveAsRaw("C:/Users/Justin/OneDrive/Desktop/Image1.raw");

            timer.Stop();

            WriteLine("Done in " + timer.ElapsedMilliseconds);
            */
        }

    }
}
