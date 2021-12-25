using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Geometry.Shapes;
using Common.Geometry.Polygons;

namespace ImageProcessing.Images
{
    public partial class Image2D<T>
    {
        public void DrawLine(Polyline2f line, ColorRGB color)
        {
            DrawLine(line.Positions, color);
        }

        public void DrawLine(IList<Point2f> line, ColorRGB color)
        {
            int points = line.Count;
            for (int i = 0; i < points - 1; i++)
            {
                int x1 = (int)Math.Round(line[i * 2 + 0].x);
                int y1 = (int)Math.Round(line[i * 2 + 0].y);

                int x2 = (int)Math.Round(line[i * 2 + 1].x);
                int y2 = (int)Math.Round(line[i * 2 + 1].y);

                DrawLine(x1, y1, x2, y2, color);
            }
        }

        public void DrawLine(Segment2f segment, ColorRGB color)
        {
            int x1 = (int)Math.Round(segment.A.x);
            int y1 = (int)Math.Round(segment.A.y);

            int x2 = (int)Math.Round(segment.B.x);
            int y2 = (int)Math.Round(segment.B.y);

            DrawLine(x1, y1, x2, y2, color);
        }

        public void DrawLine(Point2f a, Point2f b, ColorRGB color)
        {
            int x1 = (int)Math.Round(a.x);
            int y1 = (int)Math.Round(a.y);

            int x2 = (int)Math.Round(b.x);
            int y2 = (int)Math.Round(b.y);

            DrawLine(x1, y1, x2, y2, color);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, ColorRGB color)
        {
            DrawLineDDA(x1, y1, x2, y2, color);
        }

        /// <summary>
        /// Draws a colored line by connecting two points using a DDA algorithm (Digital Differential Analyzer).
        /// </summary>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        private void DrawLineDDA(int x1, int y1, int x2, int y2, ColorRGB color)
        {
            if (x1 == x2 && y1 == y2) return;
            int w = Width;
            int h = Height;

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Determine slope (absolute value)
            int len = dy >= 0 ? dy : -dy;
            int lenx = dx >= 0 ? dx : -dx;
            if (lenx > len)
                len = lenx;

            // Prevent division by zero
            if (len != 0)
            {
                // Init steps and start
                float incx = dx / (float)len;
                float incy = dy / (float)len;
                float x = x1;
                float y = y1;

                // Walk the line!
                for (int i = 0; i < len; i++)
                {
                    if (y < h && y >= 0 && x < w && x >= 0)
                        SetPixel((int)x, (int)y, color);

                    x += incx;
                    y += incy;
                }
            }

        }

        /// <summary>
        /// Draws a colored line by connecting two points using the Bresenham algorithm.
        /// </summary>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        private void DrawLineBresenham(int x1, int y1, int x2, int y2, ColorRGB color)
        {
            if (x1 == x2 && y1 == y2) return;
            int w = Width;
            int h = Height;

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Determine sign for direction x
            int incx = 0;
            if (dx < 0)
            {
                dx = -dx;
                incx = -1;
            }
            else if (dx > 0)
                incx = 1;

            // Determine sign for direction y
            int incy = 0;
            if (dy < 0)
            {
                dy = -dy;
                incy = -1;
            }
            else if (dy > 0)
                incy = 1;

            // Which gradient is larger
            int pdx, pdy, odx, ody, es, el;
            if (dx > dy)
            {
                pdx = incx;
                pdy = 0;
                odx = incx;
                ody = incy;
                es = dy;
                el = dx;
            }
            else
            {
                pdx = 0;
                pdy = incy;
                odx = incx;
                ody = incy;
                es = dx;
                el = dy;
            }

            // Init start
            int x = x1;
            int y = y1;
            int error = el >> 1;
            if (y < h && y >= 0 && x < w && x >= 0)
                SetPixel(x, y, color);

            // Walk the line!
            for (int i = 0; i < el; i++)
            {
                // Update error term
                error -= es;

                // Decide which coord to use
                if (error < 0)
                {
                    error += el;
                    x += odx;
                    y += ody;
                }
                else
                {
                    x += pdx;
                    y += pdy;
                }

                // Set pixel
                if (y < h && y >= 0 && x < w && x >= 0)
                    SetPixel(x, y, color);
            }

        }

        private bool Clip(Box2i extents, ref int xi0, ref int yi0, ref int xi1, ref int yi1)
        {
            double x0 = xi0;
            double y0 = yi0;
            double x1 = xi1;
            double y1 = yi1;

            var isValid = Clip(extents, ref x0, ref y0, ref x1, ref y1);

            // Update the clipped line
            xi0 = (int)x0;
            yi0 = (int)y0;
            xi1 = (int)x1;
            yi1 = (int)y1;

            return isValid;
        }

        /// <summary>
        /// Bitfields used to partition the space into 9 regions
        /// </summary>
        private const byte INSIDE = 0; // 0000
        private const byte LEFT = 1;   // 0001
        private const byte RIGHT = 2;  // 0010
        private const byte BOTTOM = 4; // 0100
        private const byte TOP = 8;    // 1000

        /// <summary>
        /// Cohen–Sutherland clipping algorithm clips a line from
        /// P0 = (x0, y0) to P1 = (x1, y1) against a rectangle with 
        /// diagonal from (xmin, ymin) to (xmax, ymax).
        /// </summary>
        /// <remarks>See http://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm for details</remarks>
        /// <returns>a list of two points in the resulting clipped line, or zero</returns>
        private bool Clip(Box2i extents, ref double x0, ref double y0, ref double x1, ref double y1)
        {
            // compute outcodes for P0, P1, and whatever point lies outside the clip rectangle
            byte outcode0 = ComputeOutCode(extents, x0, y0);
            byte outcode1 = ComputeOutCode(extents, x1, y1);

            // No clipping if both points lie inside viewport
            if (outcode0 == INSIDE && outcode1 == INSIDE)
                return true;

            bool isValid = false;

            while (true)
            {
                // Bitwise OR is 0. Trivially accept and get out of loop
                if ((outcode0 | outcode1) == 0)
                {
                    isValid = true;
                    break;
                }
                // Bitwise AND is not 0. Trivially reject and get out of loop
                else if ((outcode0 & outcode1) != 0)
                {
                    break;
                }
                else
                {
                    // failed both tests, so calculate the line segment to clip
                    // from an outside point to an intersection with clip edge
                    double x, y;

                    // At least one endpoint is outside the clip rectangle; pick it.
                    byte outcodeOut = (outcode0 != 0) ? outcode0 : outcode1;

                    // Now find the intersection point;
                    // use formulas y = y0 + slope * (x - x0), x = x0 + (1 / slope) * (y - y0)
                    if ((outcodeOut & TOP) != 0)
                    {   // point is above the clip rectangle
                        x = x0 + (x1 - x0) * (extents.Min.y - y0) / (y1 - y0);
                        y = extents.Min.y;
                    }
                    else if ((outcodeOut & BOTTOM) != 0)
                    { // point is below the clip rectangle
                        x = x0 + (x1 - x0) * (extents.Max.y - y0) / (y1 - y0);
                        y = extents.Max.y;
                    }
                    else if ((outcodeOut & RIGHT) != 0)
                    {  // point is to the right of clip rectangle
                        y = y0 + (y1 - y0) * (extents.Max.x - x0) / (x1 - x0);
                        x = extents.Max.x;
                    }
                    else if ((outcodeOut & LEFT) != 0)
                    {   // point is to the left of clip rectangle
                        y = y0 + (y1 - y0) * (extents.Min.x - x0) / (x1 - x0);
                        x = extents.Min.x;
                    }
                    else
                    {
                        x = double.NaN;
                        y = double.NaN;
                    }

                    // Now we move outside point to intersection point to clip
                    // and get ready for next pass.
                    if (outcodeOut == outcode0)
                    {
                        x0 = x;
                        y0 = y;
                        outcode0 = ComputeOutCode(extents, x0, y0);
                    }
                    else
                    {
                        x1 = x;
                        y1 = y;
                        outcode1 = ComputeOutCode(extents, x1, y1);
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Compute the bit code for a point (x, y) using the clip rectangle
        /// bounded diagonally by (xmin, ymin), and (xmax, ymax)
        /// ASSUME THAT xmax , xmin , ymax and ymin are global constants.
        /// </summary>
        /// <param name="extents">The extents.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private byte ComputeOutCode(Box2i extents, double x, double y)
        {
            // initialized as being inside of clip window
            byte code = INSIDE;

            if (x < extents.Min.x)           // to the left of clip window
                code |= LEFT;
            else if (x > extents.Max.x)     // to the right of clip window
                code |= RIGHT;
            if (y > extents.Max.y)         // below the clip window
                code |= BOTTOM;
            else if (y < extents.Min.y)       // above the clip window
                code |= TOP;

            return code;
        }
    }
}
