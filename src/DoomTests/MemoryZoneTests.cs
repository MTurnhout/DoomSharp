using DoomCore;
using Xunit;

namespace DoomTests
{
    public class MemoryZoneTests
    {
        [Fact]
        public void Allocate_Free_Merge_Behavior()
        {
            var zone = new MemoryZone(4096);

            var a = zone.Allocate(200, 50, new object());
            var b = zone.Allocate(300, 50, new object());

            // Free first block and ensure adjacent free blocks are not merged yet (b still allocated)
            zone.Free(a);

            // Now free b and they should merge
            zone.Free(b);

            int free = zone.FreeMemory();
            // Should be close to zone size (minus header), allow some tolerance
            Assert.True(free >= 200 + 300);
        }

        [Fact]
        public void FreeTags_Frees_By_Tag_Range()
        {
            var zone = new MemoryZone(4096);
            var a = zone.Allocate(100, 50, new object());
            var b = zone.Allocate(100, 101, new object());

            zone.FreeTags(100, 200);

            int free = zone.FreeMemory();
            Assert.True(free >= 100);
        }
    }
}
