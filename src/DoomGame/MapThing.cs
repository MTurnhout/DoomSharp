namespace DoomGame
{
    // Minimal representation of a map thing from WAD MAPINFO lumps
    public struct MapThing
    {
        public int X; // map units
        public int Y;
        public int Type; // actor type id
        public int Angle;
        public int Flags;

        public MapThing(int x, int y, int type)
        {
            X = x; Y = y; Type = type; Angle = 0; Flags = 0;
        }
    }
}
