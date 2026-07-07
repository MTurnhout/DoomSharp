using DoomCore;
using DoomGame;
using Xunit;

namespace DoomTests
{
    public class DeathStateSequenceTests
    {
        [Fact]
        public void TakeDamage_TransitionsToDeathState_Then_RemovesAfterTics()
        {
            TestHelpers.ResetWorld();

            // Define a simple spawn state (index 0) and a death state (index 1) that lasts 2 ticks
            bool deathActionCalled = false;
            var s0 = new State { NextState = 0, Tics = 1 };
            var sDeath = new State { NextState = StateTable.S_NULL, Tics = 2, Action = (m) => deathActionCalled = true };

            using (TestHelpers.PushStates(new State[] { s0, sDeath }))
            {
                // Register an actor definition using typeId 99 with deathState = 1
                var def = new ActorDefinition(99, "TestEnemy", spawnState: 0, deathState: 1, health: 10, radius: 8, height: 16);
                SpawnTable.Register(def);

                // Create the mobj and register it as thinker
                var m = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                m.TypeId = 99;
                m.Health = 5; // below death threshold after damage
                m.Radius = 8;
                ThinkerManager.Register(m);

                // Apply fatal damage
                m.TakeDamage(10);

                // After TakeDamage, it should have transitioned to death state (1)
                Assert.Equal(1, m.StateIndex);
                Assert.True(deathActionCalled, "Death action was not invoked on entering death state.");

                // Tick until the death state's tics expire and the mobj is removed
                // sDeath.Tics == 2, but SetState invoked the action and set Tics; ticking m.Tick should decrement lifetime if used,
                // however State-based removal occurs when state next becomes S_NULL inside SetState.
                // To emulate, call Tick and then ensure removal eventually.

                for (int i = 0; i < 5; i++)
                {
                    ThinkerManager.TickAll();
                }

                // Mobj should be removed (StateIndex == S_NULL)
                Assert.Equal(StateTable.S_NULL, m.StateIndex);
            }
        }
    }
}
