using System;

using Common.Core.Numerics;
using Common.Core.Time;
using Common.Geometry.Shapes;

using ImageProcessing.Images;
using ImageProcessing.Morphology;

using CONSOLE = System.Console;

namespace ImageProcessing.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            var bounds = new Box2f((0, 0), (1024, 1024));

            var circle = new BinaryImage2D(1024, 1024);
            circle.Fill(new Circle2f((512, 512), 48));

            var box1 = new BinaryImage2D(1024, 1024);
            box1.Fill(new Box2f((512 - 256, 512 - 64), (512 + 256, 512 + 64)));

            var box2 = new BinaryImage2D(1024, 1024);
            box2.Fill(new Box2f((512 - 64, 512 - 256), (512 + 64, 512 + 256)));

            box2.Or(box1);
            box2.Xor(circle);

            var image = box2.Copy();

            var timer = new Timer();
            timer.Start();

            image = BinaryImage2D.Thinning(image);

            var e = StructureElement2D.BoxElement(3);
            image = BinaryImage2D.Dilate(image, e);

            timer.Stop();

            CONSOLE.WriteLine("Time = " + timer.ElapsedMilliseconds);

        }
    }
}
