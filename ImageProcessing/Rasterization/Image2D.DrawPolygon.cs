using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Geometry.Polygons;

namespace ImageProcessing.Images
{
    public partial class Image2D<T>
    {
        public void DrawPolygon(Polygon2f polygon, ColorRGBA color, bool filled, WRAP_MODE mode = WRAP_MODE.NONE)
        {
            DrawPolygon(polygon.Positions, color, filled);
        }

        public void DrawPolygon(IList<Point2f> polygon, ColorRGBA color, bool filled, WRAP_MODE mode = WRAP_MODE.NONE)
        {
            int count = polygon.Count;

            if (filled)
            {
                var points = new int[(count + 1) * 2];
                for (int i = 0; i <= count; i++)
                {
                    var p = polygon.GetCircular(i);
                    points[i * 2 + 0] = (int)Math.Round(p.x);
                    points[i * 2 + 1] = (int)Math.Round(p.y);
                }

                DrawFilledPolygon(points, color, mode);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    var a = polygon.GetCircular(i);
                    var b = polygon.GetCircular(i + 1);
                    DrawLine(a, b, color, mode);
                }
            }
        }

        /// <summary>
        /// Draws a filled polygon. 
        /// Add the first point also at the end of the array if the line should be closed.
        /// </summary>
        private void DrawFilledPolygon(int[] points, ColorRGBA color, WRAP_MODE mode)
        {
            int w = Width;
            int h = Height;

            int pn = points.Length;
            int pnh = points.Length >> 1;
            int[] intersectionsX = new int[pnh];

            // Find y min and max (slightly faster than scanning from 0 to height)
            int yMin = h;
            int yMax = 0;
            for (int i = 1; i < pn; i += 2)
            {
                int py = points[i];
                if (py < yMin) yMin = py;
                if (py > yMax) yMax = py;
            }
            if (yMin < 0) yMin = 0;
            if (yMax >= h) yMax = h - 1;

            // Scan line from min to max
            for (int y = yMin; y <= yMax; y++)
            {
                // Initial point x, y
                float vxi = points[0];
                float vyi = points[1];

                // Find all intersections
                // Based on http://alienryderflex.com/polygon_fill/
                int intersectionCount = 0;
                for (int i = 2; i < pn; i += 2)
                {
                    // Next point x, y
                    float vxj = points[i];
                    float vyj = points[i + 1];

                    // Is the scanline between the two points
                    if (vyi < y && vyj >= y
                     || vyj < y && vyi >= y)
                    {
                        // Compute the intersection of the scanline with the edge (line between two points)
                        intersectionsX[intersectionCount++] = (int)(vxi + (y - vyi) / (vyj - vyi) * (vxj - vxi));
                    }
                    vxi = vxj;
                    vyi = vyj;
                }

                // Sort the intersections from left to right using Insertion sort 
                // It's faster than Array.Sort for this small data set
                int t, j;
                for (int i = 1; i < intersectionCount; i++)
                {
                    t = intersectionsX[i];
                    j = i;
                    while (j > 0 && intersectionsX[j - 1] > t)
                    {
                        intersectionsX[j] = intersectionsX[j - 1];
                        j = j - 1;
                    }
                    intersectionsX[j] = t;
                }

                // Fill the pixels between the intersections
                for (int i = 0; i < intersectionCount - 1; i += 2)
                {
                    int x0 = intersectionsX[i];
                    int x1 = intersectionsX[i + 1];

                    // Check boundary
                    if (x1 > 0 && x0 < w)
                    {
                        if (x0 < 0) x0 = 0;
                        if (x1 >= w) x1 = w - 1;

                        // Fill the pixels
                        for (int x = x0; x <= x1; x++)
                            SetPixel(x, y, color, mode);
                    }
                }
            }
        }
    }
}
