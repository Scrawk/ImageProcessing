using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{
    public partial class Image2D<T>
    {
        public void DrawTriangle(Triangle2f triangle, ColorRGB color, bool filled)
        {
            DrawTriangle(triangle.A, triangle.B, triangle.C, color, filled);
        }

        public void DrawTriangle(Vector2f a, Vector2f b, Vector2f c, ColorRGB color, bool filled)
        {
            if (filled)
            {
                DrawFilledTriangle(a, b, c, color);
            }
            else
            {
                DrawLine(a, b, color);
                DrawLine(b, c, color);
                DrawLine(c, a, color);
            }
        }

        /// <summary>
        /// http://www.sunshine2k.de/coding/java/TriangleRasterization/TriangleRasterization.html
        /// </summary>
        private void DrawFilledTriangle(Vector2f a, Vector2f b, Vector2f c, ColorRGB color)
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

                    var bc = triangle.Barycentric(new Vector2f(x, y));

                    if (bc.x >= 0 && bc.y >= 0 && bc.z >= 0)
                        SetPixel(x, y, color);
                }
            }
        }
    }
}
