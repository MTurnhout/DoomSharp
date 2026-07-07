namespace DoomRenderer
{
    public class Framebuffer
    {
        public int Width { get; }
        public int Height { get; }
        // store as 32-bit ARGB
        private readonly uint[] _pixels;

        public Framebuffer(int width, int height)
        {
            Width = width;
            Height = height;
            _pixels = new uint[width * height];
        }

        public void SetPixel(int x, int y, uint argb)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return;
            _pixels[y * Width + x] = argb;
        }

        public uint GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return 0;
            return _pixels[y * Width + x];
        }

        public void Clear(uint argb = 0xFF000000)
        {
            for (int i = 0; i < _pixels.Length; i++) _pixels[i] = argb;
        }
    }
}
