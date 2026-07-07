using System.Linq;
using DoomIO;

namespace DoomGame
{
    public class MapLoader
    {
        private readonly WadReader _wad;

        public MapLoader(WadReader wad)
        {
            _wad = wad;
        }

        // Minimal map presence check: ensure WAD has at least one lump named TEST or any non-empty lump list
        public bool HasMapData()
        {
            return _wad.Lumps != null && _wad.Lumps.Any();
        }
    }
}
