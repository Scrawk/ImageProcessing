using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Core.Shapes;

namespace ImageProcessing.Images
{
    public partial class Image2D<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="color"></param>
        /// <param name="filled"></param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        public void DrawCircle(Circle2f circle, ColorRGBA color, bool filled, 
            WRAP_MODE wrap = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            var c = circle.Center;
            DrawEllipse(c.x, c.y, circle.Radius, circle.Radius, color, filled, wrap, blend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        /// <param name="filled"></param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        public void DrawCircle(Point2f center, float radius, ColorRGBA color, bool filled, 
            WRAP_MODE wrap = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            DrawEllipse(center.x, center.y, radius, radius, color, filled, wrap, blend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        /// <param name="filled"></param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        public void DrawCircle(float x, float y, float radius, ColorRGBA color, bool filled, 
            WRAP_MODE wrap = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            DrawEllipse(x, y, radius, radius, color, filled, wrap, blend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radiusX"></param>
        /// <param name="radiusY"></param>
        /// <param name="color"></param>
        /// <param name="filled"></param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        public void DrawEllipse(Point2f center, float radiusX, float radiusY, ColorRGBA color, bool filled, 
            WRAP_MODE wrap = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            DrawEllipse(center.x, center.y, radiusX, radiusY, color, filled, wrap, blend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radiusX"></param>
        /// <param name="radiusY"></param>
        /// <param name="color"></param>
        /// <param name="filled"></param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        public void DrawEllipse(float x, float y, float radiusX, float radiusY, ColorRGBA color, bool filled, 
            WRAP_MODE wrap = WRAP_MODE.NONE, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            int xc = (int)Math.Round(x);
            int yc = (int)Math.Round(y);
            int rx = (int)Math.Round(radiusX);
            int ry = (int)Math.Round(radiusY);

            if (filled)
                DrawEllipseFilled(xc, yc, rx, ry, color, wrap, blend);
            else
                DrawEllipseOutline(xc, yc, rx, ry, color, wrap, blend);
        }

        /// <summary>
        /// 
        /// TODO - Draws as filled?
        /// 
        /// A Fast Bresenham Type Algorithm For Drawing Ellipses http://homepage.smc.edu/kennedy_john/belipse.pdf 
        /// Uses a different parameter representation than DrawEllipse().
        /// </summary>
        /// <param name="xc">The x-coordinate of the ellipses center.</param>
        /// <param name="yc">The y-coordinate of the ellipses center.</param>
        /// <param name="xr">The radius of the ellipse in x-direction.</param>
        /// <param name="yr">The radius of the ellipse in y-direction.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        private void DrawEllipseOutline(int xc, int yc, int xr, int yr, ColorRGBA color, WRAP_MODE wrap, BLEND_MODE blend)
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

                SetPixel(rx, uy, color, wrap, blend);      // Quadrant I (Actually an octant)
                SetPixel(lx, uy, color, wrap, blend);      // Quadrant II
                SetPixel(lx, ly, color, wrap, blend);      // Quadrant III
                SetPixel(rx, ly, color, wrap, blend);      // Quadrant IV

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

                SetPixel(rx, uy, color, wrap, blend);      // Quadrant I (Actually an octant)
                SetPixel(lx, uy, color, wrap, blend);      // Quadrant II
                SetPixel(lx, ly, color, wrap, blend);      // Quadrant III
                SetPixel(rx, ly, color, wrap, blend);      // Quadrant IV

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
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        private void DrawEllipseFilled(int xc, int yc, int xr, int yr, ColorRGBA color, WRAP_MODE wrap, BLEND_MODE blend)
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
  
                    SetPixel(i, uy, color, wrap, blend);      // Quadrant II to I (Actually two octants)
                    SetPixel(i, ly, color, wrap, blend);      // Quadrant IV
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
                    SetPixel(i, uy, color, wrap, blend); // Quadrant II to I (Actually two octants)
                    SetPixel(i, ly, color, wrap, blend);  // Quadrant III to IV
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
