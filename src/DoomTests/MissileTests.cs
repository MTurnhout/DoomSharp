using DoomCore;
using DoomGame;
using Xunit;

namespace DoomTests
{
    public class MissileTests
    {
        [Fact]
        public void Missile_HitsTarget_AppliesDamage_And_Expires()
        {
            TestHelpers.ResetWorld();

            // Setup states minimal
            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                // Create shooter at (0,0) facing right (0 degrees)
                var shooter = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                shooter.Angle = 0;
                shooter.TypeId = 1;
                shooter.Health = 100;
                shooter.Radius = 16;
                ThinkerManager.Register(shooter);

                // Create target at (48,0) - within collision range if missile speed moves it
                var target = new Mobj(Fixed.FromInt(48), Fixed.FromInt(0));
                target.TypeId = 2;
                target.Health = 100;
                target.Radius = 16;
                ThinkerManager.Register(target);

                // Fire missile
                Actions.A_FireMissile(shooter);

                // Find missile in thinkers
                Mobj? missile = null;
                foreach (var t in ThinkerManager.Snapshot())
                {
                    if (t is Mobj m && m.IsMissile)
                    {
                        missile = m;
                        break;
                    }
                }

                Assert.NotNull(missile);

                // Quick sanity: missile has non-zero momentum and moves
                var initialX = missile.X.Raw;
                Assert.NotEqual(0, missile.MomX.Raw);
                missile.Tick();
                Assert.NotEqual(initialX, missile.X.Raw);

                // Tick missile directly up to 120 ticks to allow it to reach
                bool hit = false;
                for (int i = 0; i < 120; i++)
                {
                    missile.Tick();
                    if (target.Health < 100)
                    {
                        hit = true;
                        break;
                    }
                }

                long finalDx = Math.Abs((long)target.X.Raw - missile.X.Raw);
                long thresh = (missile.Radius + target.Radius) * (long)Fixed.FRACUNIT;
                Assert.True(hit, $"Missile did not hit the target within expected ticks; finalDx={finalDx} thresh={thresh}");

                // Missile should be removed from thinkers
                bool missilePresent = false;
                foreach (var t in ThinkerManager.Snapshot())
                {
                    if (t is Mobj m && m.IsMissile)
                        missilePresent = true;
                }
                Assert.False(missilePresent, "Missile still present after hit");
            }
        }
    }
}
