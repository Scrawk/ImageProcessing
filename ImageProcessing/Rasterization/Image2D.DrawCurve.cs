using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;
using Common.Geometry.Bezier;

namespace ImageProcessing.Images
{
    public partial class Image2D<T>
    {
        private const float StepFactor = 2;

        public void DrawCurve(Bezier2f curve, ColorRGBA color, GreyScaleImage2D mask = null, WRAP_MODE mode = WRAP_MODE.NONE)
        {
            // Determine distances between controls points (bounding rect) to find the optimal stepsize
            var box = Box2f.CalculateBounds(curve.Control);
            int minX = (int)Math.Round(box.Min.x);
            int minY = (int)Math.Round(box.Min.y);
            int maxX = (int)Math.Round(box.Max.x);
            int maxY = (int)Math.Round(box.Max.y);

            // Get slope
            var lenx = maxX - minX;
            var len = maxY - minY;
            if (lenx > len)
            {
                len = lenx;
            }

            // Prevent division by zero
            if (len != 0)
            {
                int w = Width;
                int h = Height;

                int d = curve.Degree;
                int x1 = (int)Math.Round(curve.Control[0].x);
                int y1 = (int)Math.Round(curve.Control[0].y);
                int x2 = (int)Math.Round(curve.Control[d].x);
                int y2 = (int)Math.Round(curve.Control[d].y);

                // Init vars
                var step = StepFactor / len;
                int tx1 = x1;
                int ty1 = y1;
                int tx2, ty2;

                // Interpolate
                for (var t = step; t <= 1; t += step)
                {
                    var p = curve.Point(t);

                    tx2 = (int)Math.Round(p.x);
                    ty2 = (int)Math.Round(p.y);

                    // Draw line
                    DrawLine(tx1, ty1, tx2, ty2, color, mask, mode);
                    tx1 = tx2;
                    ty1 = ty2;
                }

                // Prevent rounding gap
                DrawLine(tx1, ty1, x2, y2, color, mask, mode);
            }
        }
    }
}
