using System;
using System.IO;
using DoomGame;
using DoomIO;
using Xunit;

namespace DoomTests
{
    public class GameLogicTests
    {
        [Fact]
        public void GameState_Tick_IncrementsGametic()
        {
            var gs = new GameState();
            Assert.Equal(0, gs.Gametic);
            gs.Tick();
            Assert.Equal(1, gs.Gametic);
            gs.Paused = true;
            gs.Tick();
            Assert.Equal(1, gs.Gametic);
        }

        [Fact]
        public void MapLoader_DetectsLumps_FromSyntheticWad()
        {
            var tmp = Path.GetTempFileName();
            try
            {
                using (var fs = File.OpenWrite(tmp))
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write(System.Text.Encoding.ASCII.GetBytes("PWAD"));
                    bw.Write(1);
                    int lumpDataPos = 12;
                    int lumpSize = 4;
                    int infotableofs = lumpDataPos + lumpSize;
                    bw.Write(infotableofs);
                    bw.Write(System.Text.Encoding.ASCII.GetBytes("DATA"));
                    bw.Write(lumpDataPos);
                    bw.Write(lumpSize);
                    var name = new byte[8];
                    var nameBytes = System.Text.Encoding.ASCII.GetBytes("MAPHEAD");
                    Array.Copy(nameBytes, name, nameBytes.Length);
                    bw.Write(name);
                }

                var wr = new WadReader(tmp);
                wr.Load();
                var loader = new MapLoader(wr);
                Assert.True(loader.HasMapData());
            }
            finally
            {
                File.Delete(tmp);
            }
        }
    }
}
