using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{
    public partial class Image2D<T>
    {
        public void DrawCircle(Circle2f circle, ColorRGBA color, bool filled)
        {
            DrawEllipse(circle.Center, circle.Radius, circle.Radius, color, filled);
        }

        public void DrawCircle(Point2f center, float radius, ColorRGBA color, bool filled)
        {
            DrawEllipse(center, radius, radius, color, filled);
        }

        public void DrawEllipse(Point2f center, float radiusX, float radiusY, ColorRGBA color, bool filled)
        {
            int xc = (int)Math.Round(center.x);
            int yc = (int)Math.Round(center.y);
            int rx = (int)Math.Round(radiusX);
            int ry = (int)Math.Round(radiusY);

            if (filled)
                DrawEllipseFilled(xc, yc, rx, ry, color);
            else
                DrawEllipseOutline(xc, yc, rx, ry, color);
        }

        /// <summary>
        /// A Fast Bresenham Type Algorithm For Drawing Ellipses http://homepage.smc.edu/kennedy_john/belipse.pdf 
        /// Uses a different parameter representation than DrawEllipse().
        /// </summary>
        /// <param name="xc">The x-coordinate of the ellipses center.</param>
        /// <param name="yc">The y-coordinate of the ellipses center.</param>
        /// <param name="xr">The radius of the ellipse in x-direction.</param>
        /// <param name="yr">The radius of the ellipse in y-direction.</param>
        /// <param name="color">The color for the line.</param>
        private void DrawEllipseOutline(int xc, int yc, int xr, int yr, ColorRGBA color)
        {
            var w = Width;
            var h = Height;

            // Avoid endless loop
            if (xr < 1 || yr < 1) return;

            // Init vars
            //int uh, lh;
            int uy, ly, lx, rx;
            int x = xr;
            int y = 0;
            int xrSqTwo = (xr * xr) << 1;
            int yrSqTwo = (yr * yr) << 1;
            int xChg = yr * yr * (1 - (xr << 1));
            int yChg = xr * xr;
            int err = 0;
            int xStopping = yrSqTwo * xr;
            int yStopping = 0;

            // Draw first set of points counter clockwise where tangent line slope > -1.
            while (xStopping >= yStopping)
            {
                // Draw 4 quadrant points at once
                uy = yc + y;                  // Upper half
                ly = yc - y;                  // Lower half
                if (uy < 0) uy = 0;          // Clip
                if (uy >= h) uy = h - 1;      // ...
                if (ly < 0) ly = 0;
                if (ly >= h) ly = h - 1;
                //uh = uy * w;                  // Upper half
                //lh = ly * w;                  // Lower half

                rx = xc + x;
                lx = xc - x;
                if (rx < 0) rx = 0;          // Clip
                if (rx >= w) rx = w - 1;      // ...
                if (lx < 0) lx = 0;
                if (lx >= w) lx = w - 1;

                SetPixel(rx, uy, color);      // Quadrant I (Actually an octant)
                SetPixel(lx, uy, color);      // Quadrant II
                SetPixel(lx, ly, color);      // Quadrant III
                SetPixel(rx, ly, color);      // Quadrant IV

                y++;
                yStopping += xrSqTwo;
                err += yChg;
                yChg += xrSqTwo;
                if ((xChg + (err << 1)) > 0)
                {
                    x--;
                    xStopping -= yrSqTwo;
                    err += xChg;
                    xChg += yrSqTwo;
                }
            }

            // ReInit vars
            x = 0;
            y = yr;
            uy = yc + y;                  // Upper half
            ly = yc - y;                  // Lower half
            if (uy < 0) uy = 0;          // Clip
            if (uy >= h) uy = h - 1;      // ...
            if (ly < 0) ly = 0;
            if (ly >= h) ly = h - 1;
            //uh = uy * w;                  // Upper half
            //lh = ly * w;                  // Lower half
            xChg = yr * yr;
            yChg = xr * xr * (1 - (yr << 1));
            err = 0;
            xStopping = 0;
            yStopping = xrSqTwo * yr;

            // Draw second set of points clockwise where tangent line slope < -1.
            while (xStopping <= yStopping)
            {
                // Draw 4 quadrant points at once
                rx = xc + x;
                lx = xc - x;
                if (rx < 0) rx = 0;          // Clip
                if (rx >= w) rx = w - 1;      // ...
                if (lx < 0) lx = 0;
                if (lx >= w) lx = w - 1;

                SetPixel(rx, uy, color);      // Quadrant I (Actually an octant)
                SetPixel(lx, uy, color);      // Quadrant II
                SetPixel(lx, ly, color);      // Quadrant III
                SetPixel(rx, ly, color);      // Quadrant IV

                x++;
                xStopping += yrSqTwo;
                err += xChg;
                xChg += yrSqTwo;
                if ((yChg + (err << 1)) > 0)
                {
                    y--;
                    uy = yc + y;                  // Upper half
                    ly = yc - y;                  // Lower half
                    if (uy < 0) uy = 0;          // Clip
                    if (uy >= h) uy = h - 1;      // ...
                    if (ly < 0) ly = 0;
                    if (ly >= h) ly = h - 1;
                    //uh = uy * w;                  // Upper half
                    //lh = ly * w;                  // Lower half
                    yStopping -= xrSqTwo;
                    err += yChg;
                    yChg += xrSqTwo;
                }
            }
        }

        /// <summary>
        /// A Fast Bresenham Type Algorithm For Drawing filled ellipses http://homepage.smc.edu/kennedy_john/belipse.pdf  
        /// Uses a different parameter representation than DrawEllipse().
        /// </summary>
        /// <param name="xc">The x-coordinate of the ellipses center.</param>
        /// <param name="yc">The y-coordinate of the ellipses center.</param>
        /// <param name="xr">The radius of the ellipse in x-direction.</param>
        /// <param name="yr">The radius of the ellipse in y-direction.</param>
        /// <param name="color">The color for the line.</param>
        private void DrawEllipseFilled(int xc, int yc, int xr, int yr, ColorRGBA color)
        {
            int w = Width;
            int h = Height;

            // Avoid endless loop
            if (xr < 1 || yr < 1)
            {
                return;
            }

            // Init vars
            //int uh, lh;
            int uy, ly, lx, rx;
            int x = xr;
            int y = 0;
            int xrSqTwo = (xr * xr) << 1;
            int yrSqTwo = (yr * yr) << 1;
            int xChg = yr * yr * (1 - (xr << 1));
            int yChg = xr * xr;
            int err = 0;
            int xStopping = yrSqTwo * xr;
            int yStopping = 0;

            // Draw first set of points counter clockwise where tangent line slope > -1.
            while (xStopping >= yStopping)
            {
                // Draw 4 quadrant points at once
                // Upper half
                uy = yc + y;
                // Lower half
                ly = yc - y;

                // Clip
                if (uy < 0) uy = 0;
                if (uy >= h) uy = h - 1;
                if (ly < 0) ly = 0;
                if (ly >= h) ly = h - 1;

                rx = xc + x;
                lx = xc - x;

                // Clip
                if (rx < 0) rx = 0;
                if (rx >= w) rx = w - 1;
                if (lx < 0) lx = 0;
                if (lx >= w) lx = w - 1;

                // Draw line
                for (int i = lx; i <= rx; i++)
                {
  
                    SetPixel(i, uy, color);      // Quadrant II to I (Actually two octants)
                    SetPixel(i, ly, color);      // Quadrant IV
                }

                y++;
                yStopping += xrSqTwo;
                err += yChg;
                yChg += xrSqTwo;
                if ((xChg + (err << 1)) > 0)
                {
                    x--;
                    xStopping -= yrSqTwo;
                    err += xChg;
                    xChg += yrSqTwo;
                }
            }

            // ReInit vars
            x = 0;
            y = yr;

            // Upper half
            uy = yc + y;
            // Lower half
            ly = yc - y;

            // Clip
            if (uy < 0) uy = 0;
            if (uy >= h) uy = h - 1;
            if (ly < 0) ly = 0;
            if (ly >= h) ly = h - 1;

            xChg = yr * yr;
            yChg = xr * xr * (1 - (yr << 1));
            err = 0;
            xStopping = 0;
            yStopping = xrSqTwo * yr;

            // Draw second set of points clockwise where tangent line slope < -1.
            while (xStopping <= yStopping)
            {
                // Draw 4 quadrant points at once
                rx = xc + x;
                lx = xc - x;

                // Clip
                if (rx < 0) rx = 0;
                if (rx >= w) rx = w - 1;
                if (lx < 0) lx = 0;
                if (lx >= w) lx = w - 1;

                // Draw line
                for (int i = lx; i <= rx; i++)
                {
                    SetPixel(i, uy, color); // Quadrant II to I (Actually two octants)
                    SetPixel(i, ly, color);  // Quadrant III to IV
                }

                x++;
                xStopping += yrSqTwo;
                err += xChg;
                xChg += yrSqTwo;
                if ((yChg + (err << 1)) > 0)
                {
                    y--;
                    uy = yc + y; // Upper half
                    ly = yc - y; // Lower half
                    if (uy < 0) uy = 0; // Clip
                    if (uy >= h) uy = h - 1; // ...
                    if (ly < 0) ly = 0;
                    if (ly >= h) ly = h - 1;

                    yStopping -= xrSqTwo;
                    err += yChg;
                    yChg += xrSqTwo;
                }
            }

        }
    }
}
