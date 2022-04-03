using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{
    public partial class Image2D<T>
    {
        public void DrawTriangle(Triangle2f triangle, ColorRGBA color, bool filled, WRAP_MODE mode = WRAP_MODE.NONE)
        {
            DrawTriangle(triangle.A, triangle.B, triangle.C, color, filled);
        }

        public void DrawTriangle(Point2f a, Point2f b, Point2f c, ColorRGBA color, bool filled, WRAP_MODE mode = WRAP_MODE.NONE)
        {
            if (filled)
            {
                DrawFilledTriangle(a, b, c, color, mode);
            }
            else
            {
                DrawLine(a, b, color, mode);
                DrawLine(b, c, color, mode);
                DrawLine(c, a, color, mode);
            }
        }

        /// <summary>
        /// http://www.sunshine2k.de/coding/java/TriangleRasterization/TriangleRasterization.html
        /// </summary>
        private void DrawFilledTriangle(Point2f a, Point2f b, Point2f c, ColorRGBA color, WRAP_MODE mode)
        {
            int width1 = Width - 1;
            int height1 = Height - 1;

            int maxX = (int)Math.Ceiling(Math.Max(a.x, Math.Max(b.x, c.x)));
            int minX = (int)Math.Floor(Math.Min(a.x, Math.Min(b.x, c.x)));
            int maxY = (int)Math.Ceiling(Math.Max(a.y, Math.Max(b.y, c.y)));
            int minY = (int)Math.Floor(Math.Min(a.y, Math.Min(b.y, c.y)));

            var triangle = new Triangle2f(a, b, c);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (x < 0 || x > width1) continue;
                    if (y < 0 || y > height1) continue;

                    var bc = triangle.Barycentric(new Point2f(x, y));

                    if (bc.x >= 0 && bc.y >= 0 && bc.z >= 0)
                        SetPixel(x, y, color, mode);
                }
            }
        }
    }
}
