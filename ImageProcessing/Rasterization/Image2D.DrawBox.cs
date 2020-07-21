using System;
using System.Collections.Generic;

using Common.Core.Numerics;
using Common.Core.Colors;
using Common.Geometry.Shapes;

namespace ImageProcessing.Images
{
    public partial class Image2D<T>
    {

        public void DrawPoint(Vector2f center, float size, ColorRGB color)
        {
            float half = size * 0.5f;
            DrawBox(center - half, center + half, color, true);
        }

        public void DrawBox(Box2f box, ColorRGB color, bool filled)
        {
            DrawBox(box.Min, box.Max, color, filled);
        }

        public void DrawBox(Vector2f min, Vector2f max, ColorRGB color, bool filled)
        {
            int x1 = (int)Math.Round(min.x);
            int y1 = (int)Math.Round(min.y);

            int x2 = (int)Math.Round(max.x);
            int y2 = (int)Math.Round(max.y);

            if (filled)
                DrawBoxFilled(x1, y1, x2, y2, color);
            else
                DrawBoxOutline(x1, y1, x2, y2, color);
        }

        /// <summary>
        /// Draws a rectangle.
        /// x2 has to be greater than x1 and y2 has to be greater than y1.
        /// </summary>
        private void DrawBoxOutline(int x1, int y1, int x2, int y2, ColorRGB color)
        {
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

            for (var x = x1; x <= x2; x++)
            {
                SetPixel(x, y1, color);
                SetPixel(x, y2, color);
            }

            for (var y = y1; y <= y2; y++)
            {
                SetPixel(x1, y, color);
                SetPixel(x2, y, color);
            }

        }

        /// <summary>
        /// Draws a filled rectangle.
        /// x2 has to be greater than x1 and y2 has to be greater than y1.
        /// </summary>
        private void DrawBoxFilled(int x1, int y1, int x2, int y2, ColorRGB color)
        {
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

            for (var y = y1; y <= y2; y++)
            {
                for (var x = x1; x <= x2; x++)
                {
                    SetPixel(x, y, color);
                }
            }
        }
    }
}
