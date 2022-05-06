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
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="filled"></param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        public void DrawPoint(Point2f center, float size, ColorRGBA color, bool filled, 
            WRAP_MODE wrap = WRAP_MODE.CLAMP, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            float half = size * 0.5f;
            DrawBox(center - half, center + half, color, filled, wrap, blend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <param name="color"></param>
        /// <param name="filled"></param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        public void DrawBox(Box2f box, ColorRGBA color, bool filled, 
            WRAP_MODE wrap = WRAP_MODE.CLAMP, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            DrawBox(box.Min, box.Max, color, filled, wrap, blend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="color"></param>
        /// <param name="filled"></param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        public void DrawBox(Point2f min, Point2f max, ColorRGBA color, bool filled, 
            WRAP_MODE wrap = WRAP_MODE.CLAMP, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            int x1 = (int)Math.Round(min.x);
            int y1 = (int)Math.Round(min.y);

            int x2 = (int)Math.Round(max.x);
            int y2 = (int)Math.Round(max.y);

            if (filled)
                DrawBoxFilled(x1, y1, x2, y2, color, wrap, blend);
            else
                DrawBoxOutline(x1, y1, x2, y2, color, wrap, blend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        /// <param name="filled"></param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        public void DrawBox(int x1, int y1, int x2, int y2, ColorRGBA color, bool filled,
            WRAP_MODE wrap = WRAP_MODE.CLAMP, BLEND_MODE blend = BLEND_MODE.ALPHA)
        {
            if (filled)
                DrawBoxFilled(x1, y1, x2, y2, color, wrap, blend);
            else
                DrawBoxOutline(x1, y1, x2, y2, color, wrap, blend);
        }

        /// <summary>
        /// Draws a rectangle.
        /// x2 has to be greater than x1 and y2 has to be greater than y1.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        private void DrawBoxOutline(int x1, int y1, int x2, int y2, ColorRGBA color, WRAP_MODE wrap, BLEND_MODE blend)
        {
            /*
            var w = Width;
            var h = Height;
            
            // Check boundaries
            if ((x1 < 0 && x2 < 0) || (y1 < 0 && y2 < 0)
             || (x1 >= w && x2 >= w) || (y1 >= h && y2 >= h))
            {
                return;
            }

            // Clamp boundaries
            x1 = MathUtil.Clamp(x1, 0, w - 1);
            y1 = MathUtil.Clamp(y1, 0, h - 1);
            x2 = MathUtil.Clamp(x2, 0, w - 1);
            y2 = MathUtil.Clamp(y2, 0, h - 1);
            */

            for (var x = x1; x <= x2; x++)
            {
                SetPixel(x, y1, color, wrap, blend);
                SetPixel(x, y2, color, wrap, blend);
            }

            for (var y = y1; y <= y2; y++)
            {
                SetPixel(x1, y, color, wrap, blend);
                SetPixel(x2, y, color, wrap, blend);
            }

        }

        /// <summary>
        /// Draws a filled rectangle.
        /// x2 has to be greater than x1 and y2 has to be greater than y1.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        /// <param name="wrap">The wrap mode for out of bounds indices. Defaults to clamp.</param>
        /// <param name="blend">The blend mode for the pixels. Defalus to alpha.</param>
        private void DrawBoxFilled(int x1, int y1, int x2, int y2, ColorRGBA color, WRAP_MODE wrap, BLEND_MODE blend)
        {
            /*
            var w = Width;
            var h = Height;

            // Check boundaries
            if ((x1 < 0 && x2 < 0) || (y1 < 0 && y2 < 0)
             || (x1 >= w && x2 >= w) || (y1 >= h && y2 >= h))
            {
                return;
            }

            // Clamp boundaries
            x1 = MathUtil.Clamp(x1, 0, w - 1);
            y1 = MathUtil.Clamp(y1, 0, h - 1);
            x2 = MathUtil.Clamp(x2, 0, w - 1);
            y2 = MathUtil.Clamp(y2, 0, h - 1);
            */

            for (var y = y1; y <= y2; y++)
            {
                for (var x = x1; x <= x2; x++)
                {
                    SetPixel(x, y, color, wrap, blend);
                }
            }
        }
    }
}
