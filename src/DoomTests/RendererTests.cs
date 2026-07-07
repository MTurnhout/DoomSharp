using DoomRenderer;
using Xunit;

namespace DoomTests
{
    public class RendererTests
    {
        [Fact]
        public void FillRect_WritesPixels()
        {
            var fb = new Framebuffer(64, 64);
            fb.Clear(0xFF000000); // black
            SoftwareRasterizer.FillRect(fb, 10, 12, 8, 6, 0xFFFF0000); // red

            // sample some pixels inside and outside
            Assert.Equal(0xFFFF0000u, fb.GetPixel(10, 12));
            Assert.Equal(0xFFFF0000u, fb.GetPixel(17, 17));
            Assert.Equal(0xFF000000u, fb.GetPixel(9, 12));
        }

        [Fact]
        public void Blit_CopiesTexture()
        {
            var fb = new Framebuffer(32, 32);
            fb.Clear(0xFF000000);
            var tex = new uint[4 * 4];
            for (int i = 0; i < tex.Length; i++) tex[i] = 0xFF00FF00; // green

            SoftwareRasterizer.Blit(fb, 5, 5, 4, 4, tex);
            Assert.Equal(0xFF00FF00u, fb.GetPixel(5,5));
            Assert.Equal(0xFF00FF00u, fb.GetPixel(8,8));
            Assert.Equal(0xFF000000u, fb.GetPixel(4,4));
        }
    }
}
