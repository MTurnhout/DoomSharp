using DoomGame;
using DoomCore;
using Xunit;

namespace DoomTests
{
    public class SpawnTests
    {
        [Fact]
        public void SpawnMapThing_CreatesMobj_AndStateActionRuns()
        {
            bool actionCalled = false;
            var s0 = new State { NextState = 0, Tics = 1, Sprite = 0, Frame = 0, Action = (m) => actionCalled = true };
            using (TestHelpers.PushStates(new State[] { s0 }))
            {
                TestHelpers.ResetWorld();
                var mt = new MapThing(10, 20, 1);
                var m = SpawnManager.SpawnMapThing(mt);

                Assert.Equal(Fixed.FromInt(10), m.X);
                Assert.Equal(Fixed.FromInt(20), m.Y);

                // State was set during spawn to 0 which has action; action should have been called
                Assert.True(actionCalled);
            }
        }
    }
}
