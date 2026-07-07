using DoomGame;
using DoomCore;
using Xunit;

namespace DoomTests
{
    public class SpawnIntegrationTests
    {
        [Fact]
        public void SpawnMapThing_Integration_SpawnActionInvokedOnce()
        {
            int called = 0;

            var s0 = new State { NextState = 0, Tics = 1, Sprite = 0, Frame = 0, Action = (m) => called++ };
            using (TestHelpers.PushStates(new State[] { s0 }))
            {
                TestHelpers.ResetWorld();

                var mt = new MapThing(10, 20, 1); // type 1 as in SpawnTable.InitializeDefaults
                var m = SpawnManager.SpawnMapThing(mt);

                Assert.Equal(Fixed.FromInt(10), m.X);
                Assert.Equal(Fixed.FromInt(20), m.Y);
                Assert.Equal(1, called);
            }
        }
    }
}
