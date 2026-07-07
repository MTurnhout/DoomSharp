using System;

namespace DoomRenderer
{
    // Very small software rasterizer helpers for testing the asset pipeline.
    public static class SoftwareRasterizer
    {
        // Fill a rectangle with a color
        public static void FillRect(Framebuffer fb, int x, int y, int w, int h, uint color)
        {
            if (w <= 0 || h <= 0) return;
            int x0 = Math.Max(0, x);
            int y0 = Math.Max(0, y);
            int x1 = Math.Min(fb.Width, x + w);
            int y1 = Math.Min(fb.Height, y + h);

            for (int yy = y0; yy < y1; yy++)
            for (int xx = x0; xx < x1; xx++)
                fb.SetPixel(xx, yy, color);
        }

        // Copy a small texture (w*h ARGB array) to framebuffer at x,y
        public static void Blit(Framebuffer fb, int x, int y, int w, int h, uint[] tex)
        {
            for (int yy = 0; yy < h; yy++)
            for (int xx = 0; xx < w; xx++)
            {
                var c = tex[yy * w + xx];
                fb.SetPixel(x + xx, y + yy, c);
            }
        }
    }
}
