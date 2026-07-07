using DoomCore;
using DoomGame;
using Xunit;

namespace DoomTests
{
    public class MovementTests
    {
        [Fact]
        public void Mobj_Movement_Tick_UpdatesPositionAndKeepsMomentum()
        {
            var m = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
            m.MomX = Fixed.FromInt(3);
            m.MomY = Fixed.FromInt(1);

            // register and tick
            ThinkerManager.Clear();
            ThinkerManager.Register(m);
            ThinkerManager.TickAll();

            // Position should have advanced by at least one substep
            Assert.NotEqual(Fixed.FromInt(0), m.X);
            Assert.NotEqual(Fixed.FromInt(0), m.Y);
            // Momentum should be preserved for smoother continuous motion
            Assert.Equal(Fixed.FromInt(3), m.MomX);
            Assert.Equal(Fixed.FromInt(1), m.MomY);
        }

        [Fact]
        public void TryMove_RespectsWorldMapBlocking()
        {
            TestHelpers.ResetWorld();

            var m = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
            m.MomX = Fixed.FromInt(5);
            m.MomY = Fixed.FromInt(0);

            // Install blocker at the destination
            WorldMap.SetBlocker((x, y, r) => x.Raw > 0);

            ThinkerManager.Clear();
            ThinkerManager.Register(m);
            ThinkerManager.TickAll();

            // Movement should be blocked and position unchanged
            Assert.Equal(Fixed.FromInt(0), m.X);

            WorldMap.ResetBlocker();
        }
    }
}
