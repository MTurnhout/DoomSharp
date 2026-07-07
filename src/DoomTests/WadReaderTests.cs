using System;
using System.IO;
using DoomIO;
using Xunit;

namespace DoomTests
{
    public class WadReaderTests
    {
        [Fact]
        public void ParsesSyntheticWad()
        {
            var tmp = Path.GetTempFileName();
            try
            {
                using (var fs = File.OpenWrite(tmp))
                using (var bw = new BinaryWriter(fs))
                {
                    // Header: id(4), numlumps(int), infotableofs(int)
                    bw.Write(System.Text.Encoding.ASCII.GetBytes("PWAD"));
                    bw.Write(1); // numlumps

                    // We'll place lump data immediately after header (header is 12 bytes)
                    int lumpDataPos = 12;
                    int lumpSize = 4;
                    int infotableofs = lumpDataPos + lumpSize; // directory after data

                    bw.Write(infotableofs);

                    // Lump data at offset 12
                    bw.Write(System.Text.Encoding.ASCII.GetBytes("DATA"));

                    // Directory (filelump_t): filepos, size, name[8]
                    bw.Write(lumpDataPos);
                    bw.Write(lumpSize);
                    var name = new byte[8];
                    var nameBytes = System.Text.Encoding.ASCII.GetBytes("TESTLUMP");
                    Array.Copy(nameBytes, name, nameBytes.Length);
                    bw.Write(name);
                }

                var reader = new WadReader(tmp);
                reader.Load();

                Assert.Equal(1, reader.NumLumps);
                Assert.Equal("TESTLUMP", reader.Lumps[0].Name);
                var data = reader.ReadLumpData(reader.Lumps[0]);
                Assert.Equal(System.Text.Encoding.ASCII.GetBytes("DATA"), data);
            }
            finally
            {
                File.Delete(tmp);
            }
        }
    }
}
