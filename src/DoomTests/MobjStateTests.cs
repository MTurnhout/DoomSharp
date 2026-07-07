using DoomGame;
using DoomCore;
using Xunit;

namespace DoomTests
{
    public class MobjStateTests
    {
        [Fact]
        public void SetState_AdvancesThroughZeroTics_AndCallsAction()
        {
            bool actionCalled = false;

            var s0 = new State { NextState = 1, Tics = 0, Sprite = 0, Frame = 0 };
            var s1 = new State { NextState = StateTable.S_NULL, Tics = 3, Sprite = 1, Frame = 2, Action = (m) => actionCalled = true };

            using (TestHelpers.PushStates(new State[] { s0, s1 }))
            {
                TestHelpers.ResetWorld();

                var m = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));

                bool present = m.SetState(0);

                Assert.True(present);
                Assert.Equal(1, m.StateIndex);
                Assert.Equal(3, m.Tics);
                Assert.True(actionCalled);
            }
        }

        [Fact]
        public void SetState_SetsNull_RemovesMobj()
        {
            using (TestHelpers.PushStates(new State[0]))
            {
                var m = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                bool present = m.SetState(StateTable.S_NULL);
                Assert.False(present);
                Assert.Equal(StateTable.S_NULL, m.StateIndex);
            }
        }
    }
}
