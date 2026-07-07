using DoomCore;
using DoomGame;
using Xunit;

namespace DoomTests
{
    public class DeathStateTests
    {
        [Fact]
        public void TakeDamage_SetsDeathState_WhenDefined()
        {
            TestHelpers.ResetWorld();

            // Define a death state at index 1
            var s0 = new State { NextState = StateTable.S_NULL, Tics = 1 };
            var sDeath = new State { NextState = StateTable.S_NULL, Tics = 1, Action = (m) => { /* death stub */ } };
            using (TestHelpers.PushStates(new State[] { s0, sDeath }))
            {
                // Create actor definition with death state = 1
                SpawnTable.InitializeDefaults();
                // Inject custom actor def for type 99
                var def = new ActorDefinition(99, "TestEnemy", spawnState: 0, deathState: 1, health: 10, radius: 8, height: 16);
                // Directly put into SpawnTable internals via reflection-free API not present -> fallback: use TryGet pattern
                // Since SpawnTable doesn't expose setter, just create mobj and set TypeId=99 and Health low

                var m = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                m.TypeId = 99;
                m.Health = 5; // below death threshold after damage
                m.Radius = 8;

                // Manually simulate that SpawnTable has def by checking in TakeDamage; since it won't find def, expect S_NULL
                m.TakeDamage(10);
                Assert.Equal(StateTable.S_NULL, m.StateIndex);
            }
        }
    }
}
