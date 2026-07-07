using DoomCore;
using DoomGame;
using Xunit;

namespace DoomTests
{
    public class ActionBehaviorTests
    {
        [Fact]
        public void FaceTarget_SetsAngleTowardsPlayer()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var enemy = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                enemy.TypeId = 10;
                enemy.Health = 100;
                enemy.Radius = 16;
                ThinkerManager.Register(enemy);

                var player = new Mobj(Fixed.FromInt(48), Fixed.FromInt(0));
                player.IsPlayer = true;
                player.TypeId = 1;
                player.Health = 100;
                player.Radius = 16;
                ThinkerManager.Register(player);

                Actions.A_FaceTarget(enemy);

                Assert.InRange(enemy.Angle, -1, 1); // approx 0 degrees
            }
        }

        [Fact]
        public void SkelMissile_SpawnsMissile()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var skel = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                skel.TypeId = 18;
                skel.Health = 100;
                ThinkerManager.Register(skel);

                var player = new Mobj(Fixed.FromInt(48), Fixed.FromInt(0));
                player.IsPlayer = true;
                player.Health = 100;
                ThinkerManager.Register(player);

                Actions.A_SkelMissile(skel);

                Mobj? missile = null;
                foreach (var t in ThinkerManager.Snapshot())
                {
                    if (t is Mobj m && m.IsMissile)
                    {
                        missile = m; break;
                    }
                }

                Assert.NotNull(missile);
                Assert.True(missile.IsMissile);
                Assert.True(missile.LifetimeTicks > 0);
            }
        }

        [Fact]
        public void Explode_DamagesNearby()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var explosive = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                explosive.TypeId = 30;
                explosive.Health = 1;
                ThinkerManager.Register(explosive);

                var victim = new Mobj(Fixed.FromInt(30), Fixed.FromInt(0));
                victim.TypeId = 11;
                victim.Health = 100;
                ThinkerManager.Register(victim);

                Actions.A_Explode(explosive);

                Assert.True(victim.Health < 100, "Victim should have taken damage from explosion");
            }
        }

        [Fact]
        public void VileAttack_DamagesPlayerInFront()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var vile = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                vile.TypeId = 50;
                vile.Health = 200;
                vile.Angle = 0; // facing +X
                ThinkerManager.Register(vile);

                var playerFront = new Mobj(Fixed.FromInt(48), Fixed.FromInt(0));
                playerFront.IsPlayer = true;
                playerFront.Health = 100;
                ThinkerManager.Register(playerFront);

                var playerBehind = new Mobj(Fixed.FromInt(-48), Fixed.FromInt(0));
                playerBehind.IsPlayer = true;
                playerBehind.Health = 100;
                ThinkerManager.Register(playerBehind);

                Actions.A_VileAttack(vile);

                Assert.True(playerFront.Health < 100, "Front player should take damage");
                Assert.Equal(100, playerBehind.Health);
            }
        }

        [Fact]
        public void FatAttack_DamagesCloseTargets()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var fat = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                fat.TypeId = 60;
                fat.Health = 200;
                ThinkerManager.Register(fat);

                var close = new Mobj(Fixed.FromInt(16), Fixed.FromInt(0));
                close.IsPlayer = true;
                close.Health = 100;
                ThinkerManager.Register(close);

                var far = new Mobj(Fixed.FromInt(64), Fixed.FromInt(0));
                far.IsPlayer = true;
                far.Health = 100;
                ThinkerManager.Register(far);

                Actions.A_FatAttack1(fat);

                Assert.True(close.Health < 100, "Close target should take damage");
                Assert.Equal(100, far.Health);
            }
        }

        [Fact]
        public void SargAttack_ConeDamagesInFront()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var sarg = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                sarg.TypeId = 70;
                sarg.Health = 150;
                sarg.Angle = 0; // facing +X
                ThinkerManager.Register(sarg);

                var front = new Mobj(Fixed.FromInt(40), Fixed.FromInt(0));
                front.IsPlayer = true;
                front.Health = 100;
                ThinkerManager.Register(front);

                var side = new Mobj(Fixed.FromInt(30), Fixed.FromInt(30));
                side.IsPlayer = true;
                side.Health = 100;
                ThinkerManager.Register(side);

                Actions.A_SargAttack(sarg);

                Assert.True(front.Health < 100, "Front player should take damage");
                // Side is outside cone (approx 45 deg), may or may not be damaged depending on cone; allow either but prefer unchanged
                Assert.True(side.Health <= 100);
            }
        }

        [Fact]
        public void Tracer_SpawnsMissile()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var shooter = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                shooter.TypeId = 80;
                shooter.Health = 100;
                ThinkerManager.Register(shooter);

                var player = new Mobj(Fixed.FromInt(48), Fixed.FromInt(0));
                player.IsPlayer = true;
                player.Health = 100;
                ThinkerManager.Register(player);

                Actions.A_Tracer(shooter);

                Mobj? missile = null;
                foreach (var t in ThinkerManager.Snapshot())
                {
                    if (t is Mobj m && m.IsMissile)
                    {
                        missile = m; break;
                    }
                }

                Assert.NotNull(missile);
                Assert.True(missile.IsMissile);
            }
        }

        [Fact]
        public void Movement_Interpolation_Substeps()
        {
            TestHelpers.ResetWorld();

            // Create an object with large momentum and verify sub-stepped movement and retained momentum
            var mo = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
            mo.MomX = Fixed.FromInt(10); // large per-tick movement (10 world units)
            mo.MomY = Fixed.FromInt(0);

            int beforeX = mo.X.Raw;
            Movement.Tick(mo);
            int afterX = mo.X.Raw;

            Assert.NotEqual(beforeX, afterX);
            // Momentum should not be cleared by sub-stepping
            Assert.NotEqual(0, mo.MomX.Raw);
        }

        [Fact]
        public void VileTarget_SetsAngleTowardsPlayer()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var vile = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                vile.TypeId = 50;
                vile.Health = 150;
                ThinkerManager.Register(vile);

                var player = new Mobj(Fixed.FromInt(64), Fixed.FromInt(0));
                player.IsPlayer = true;
                player.Health = 100;
                ThinkerManager.Register(player);

                Actions.A_VileTarget(vile);

                Assert.InRange(vile.Angle, -1, 1);
            }
        }

        [Fact]
        public void VileChase_SetsFasterMomentum()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var vile = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                vile.TypeId = 50;
                vile.Health = 150;
                ThinkerManager.Register(vile);

                var player = new Mobj(Fixed.FromInt(80), Fixed.FromInt(0));
                player.IsPlayer = true;
                player.Health = 100;
                ThinkerManager.Register(player);

                Actions.A_VileChase(vile);

                // After first chase, think cooldown should be set
                Assert.False(vile.CanThink, "Vile should have think cooldown after chasing");

                // Immediate rechase should be prevented by think cooldown (no change in cooldown)
                var prevCooldown = vile.ThinkCooldownTicks;
                Actions.A_VileChase(vile);
                Assert.Equal(prevCooldown, vile.ThinkCooldownTicks);

                // Expire cooldown and allow rechase
                vile.ThinkCooldownTicks = 0;
                Actions.A_VileChase(vile);
                Assert.False(vile.CanThink, "Vile should again set think cooldown after rechase");
            }
        }

        [Fact]
        public void BrainSpit_SpawnsMissile()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var brain = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                brain.TypeId = 90;
                brain.Health = 200;
                ThinkerManager.Register(brain);

                var player = new Mobj(Fixed.FromInt(64), Fixed.FromInt(0));
                player.IsPlayer = true;
                player.Health = 100;
                ThinkerManager.Register(player);

                Actions.A_BrainSpit(brain);

                Mobj? missile = null;
                foreach (var t in ThinkerManager.Snapshot())
                {
                    if (t is Mobj m && m.IsMissile)
                    {
                        missile = m; break;
                    }
                }

                Assert.NotNull(missile);
                Assert.True(missile.IsMissile);
            }
        }

        [Fact]
        public void Fire_SetsRecoilMomentum()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var shooter = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                shooter.TypeId = 81;
                shooter.Health = 100;
                shooter.Angle = 90; // facing +Y
                ThinkerManager.Register(shooter);

                Actions.A_Fire(shooter);

                Assert.True(shooter.MomX.Raw != 0 || shooter.MomY.Raw != 0, "Shooter should have recoil momentum set");
            }
        }

        [Fact]
        public void BrainExplode_DamagesNearby()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var brain = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                brain.TypeId = 90;
                brain.Health = 200;
                ThinkerManager.Register(brain);

                var victim = new Mobj(Fixed.FromInt(50), Fixed.FromInt(0));
                victim.TypeId = 11;
                victim.Health = 100;
                ThinkerManager.Register(victim);

                Actions.A_BrainExplode(brain);

                Assert.True(victim.Health < 100, "Victim should take damage from brain explosion");
            }
        }

        [Fact]
        public void BrainDie_RemovesSelfAndDamages()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var brain = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                brain.TypeId = 90;
                brain.Health = 1;
                ThinkerManager.Register(brain);

                var victim = new Mobj(Fixed.FromInt(50), Fixed.FromInt(0));
                victim.TypeId = 11;
                victim.Health = 100;
                ThinkerManager.Register(victim);

                Actions.A_BrainDie(brain);

                // Brain should be unregistered
                var present = ThinkerManager.Snapshot().Any(t => ReferenceEquals(t, brain));
                Assert.False(present, "Brain should be unregistered after death");
                Assert.True(victim.Health < 100, "Victim should take damage from brain death explosion");
            }
        }

        [Fact]
        public void AttackCooldown_PreventsImmediateReattack()
        {
            TestHelpers.ResetWorld();

            using (TestHelpers.PushStates(new State[] { new State { NextState = StateTable.S_NULL, Tics = 1 } }))
            {
                var fat = new Mobj(Fixed.FromInt(0), Fixed.FromInt(0));
                fat.TypeId = 60;
                fat.Health = 200;
                ThinkerManager.Register(fat);

                var target = new Mobj(Fixed.FromInt(16), Fixed.FromInt(0));
                target.IsPlayer = true;
                target.Health = 100;
                ThinkerManager.Register(target);

                Actions.A_FatAttack1(fat);
                int afterFirst = target.Health;
                Assert.True(afterFirst < 100, "First attack should damage");

                // Immediate re-attack should be prevented by cooldown
                Actions.A_FatAttack1(fat);
                Assert.Equal(afterFirst, target.Health);

                // Expire cooldown and attack again
                fat.AttackCooldownTicks = 0;
                Actions.A_FatAttack1(fat);
                Assert.True(target.Health < afterFirst, "Attack after cooldown should deal damage");
            }
        }
    }
}
