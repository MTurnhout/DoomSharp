using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;

namespace DoomIO
{
    public class WadLump
    {
        public int FilePos { get; init; }
        public int Size { get; init; }
        public string Name { get; init; } = string.Empty;
    }

    public class WadReader
    {
        public string Identification { get; private set; } = string.Empty; // "IWAD" or "PWAD"
        public int NumLumps { get; private set; }
        public int InfoTableOffset { get; private set; }
        public List<WadLump> Lumps { get; } = new List<WadLump>();

        private readonly string _path;

        public WadReader(string path)
        {
            _path = path;
        }

        public void Load()
        {
            using var fs = File.OpenRead(_path);
            using var br = new BinaryReader(fs);

            // Read header: 4 char id, int numlumps, int infotableofs (little-endian)
            var idBytes = br.ReadBytes(4);
            Identification = System.Text.Encoding.ASCII.GetString(idBytes);
            var numlumps = br.ReadInt32();
            var infotableofs = br.ReadInt32();

            // Original DOOM stores ints in little-endian on x86; BinaryReader reads little-endian by default.
            NumLumps = numlumps;
            InfoTableOffset = infotableofs;

            // Seek to directory
            fs.Seek(InfoTableOffset, SeekOrigin.Begin);

            for (int i = 0; i < NumLumps; i++)
            {
                int filepos = br.ReadInt32();
                int size = br.ReadInt32();
                var nameBytes = br.ReadBytes(8);
                string name = System.Text.Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');

                Lumps.Add(new WadLump { FilePos = filepos, Size = size, Name = name });
            }
        }

        public bool IsIWad => Identification == "IWAD";
        public bool IsPWad => Identification == "PWAD";

        public byte[] ReadLumpData(WadLump lump)
        {
            using var fs = File.OpenRead(_path);
            fs.Seek(lump.FilePos, SeekOrigin.Begin);
            var data = new byte[lump.Size];
            fs.ReadExactly(data, 0, lump.Size);
            return data;
        }
    }
}
