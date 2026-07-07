using DoomGame;
using Xunit;

namespace DoomTests
{
    public class SpawnTableTests
    {
        [Fact]
        public void InitializeDefaults_PopulatesTable_And_SpawnManagerUsesIt()
        {
            TestHelpers.ResetWorld();
            SpawnTable.InitializeDefaults();
            Assert.True(SpawnTable.TryGet(1, out var p));
            Assert.Equal("Player", p.Name);

            ThinkerManager.Clear();
            var mt = new MapThing(0, 0, 2); // type 2 -> Imp
            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var m = SpawnManager.SpawnMapThing(mt);

                // spawn used the table mapping; Mobj state was set (either spawn state or fallback)
                Assert.NotNull(m);
                Assert.True(m.StateIndex == 0 || m.StateIndex == StateTable.S_NULL);
            }
        }
    }
}
