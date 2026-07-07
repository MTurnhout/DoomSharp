using System;
using System.Linq;
using System.Reflection;
using DoomCore;

namespace DoomGame
{
    public static class Actions
    {
        // Example action: set a property or call sound. These are stubs for porting.
        public static void Action_None(Mobj m) { /* no-op */ }

        public static StateAction MakeFlagSetter(Action setter)
        {
            return (m) => { setter(); };
        }

        // Resolve action names from original C (e.g., "A_Light0") to delegates in this class.
        public static StateAction? GetByName(string? name)
        {
            if (string.IsNullOrEmpty(name) || name == "NULL") return null;
            // Look for a static method with this name
            var mi = typeof(Actions).GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (mi == null) return null;
            return (StateAction)Delegate.CreateDelegate(typeof(StateAction), mi);
        }

        // Helpers
        private static Mobj? FindNearestPlayer(Mobj seeker)
        {
            var thinkers = ThinkerManager.Snapshot();
            Mobj? best = null;
            double bestDist = double.MaxValue;
            foreach (var t in thinkers)
            {
                if (t is Mobj mm && mm.IsPlayer)
                {
                    double dx = (mm.X.Raw - seeker.X.Raw) / (double)Fixed.FRACUNIT;
                    double dy = (mm.Y.Raw - seeker.Y.Raw) / (double)Fixed.FRACUNIT;
                    double d2 = dx * dx + dy * dy;
                    if (d2 < bestDist) { bestDist = d2; best = mm; }
                }
            }
            return best;
        }

        private static int AngleTo(Mobj from, Mobj to)
        {
            double dx = (to.X.Raw - from.X.Raw) / (double)Fixed.FRACUNIT;
            double dy = (to.Y.Raw - from.Y.Raw) / (double)Fixed.FRACUNIT;
            double rad = Math.Atan2(dy, dx);
            double deg = rad * 180.0 / Math.PI;
            if (deg < 0) deg += 360.0;
            return (int)Math.Round(deg) % 360;
        }

        // Some commonly referenced stubs (add more as needed)
        public static void A_Light0(Mobj m) { }
        public static void A_WeaponReady(Mobj m) { }
        public static void A_Lower(Mobj m) { }
        public static void A_Raise(Mobj m) { }
        public static void A_Punch(Mobj m) { }
        public static void A_ReFire(Mobj m) { }
        public static void A_FirePistol(Mobj m) { }
        public static void A_Light1(Mobj m) { }
        public static void A_FireShotgun(Mobj m) { }
        public static void A_Light2(Mobj m) { }
        public static void A_FireShotgun2(Mobj m) { }
        public static void A_CheckReload(Mobj m) { }
        public static void A_OpenShotgun2(Mobj m) { }
        public static void A_LoadShotgun2(Mobj m) { }
        public static void A_CloseShotgun2(Mobj m) { }
        public static void A_FireCGun(Mobj m) { }
        public static void A_GunFlash(Mobj m) { }

        public static void A_FireMissile(Mobj m)
        {
            if (m == null) return;
            var missile = new Mobj(m.X, m.Y);
            missile.IsMissile = true;
            missile.Radius = 4;
            missile.Health = 1;
            missile.LifetimeTicks = 120;

            double rad = (m.Angle * Math.PI / 180.0);
            double speed = 2.5;
            long momxRaw = (long)(Math.Cos(rad) * speed * Fixed.FRACUNIT);
            long momyRaw = (long)(Math.Sin(rad) * speed * Fixed.FRACUNIT);
            missile.MomX = new Fixed((int)momxRaw);
            missile.MomY = new Fixed((int)momyRaw);
            ThinkerManager.Register(missile);
        }

        public static void A_Saw(Mobj m) { }
        public static void A_FirePlasma(Mobj m) { }
        public static void A_BFGsound(Mobj m) { }
        public static void A_FireBFG(Mobj m) { }
        public static void A_BFGSpray(Mobj m) { }

        // Explode: apply area damage to nearby mobjs and remove self
        public static void A_Explode(Mobj m)
        {
            if (m == null) return;
            const int radiusWorld = 64; // world units
            long rRaw = radiusWorld * Fixed.FRACUNIT;
            long r2 = rRaw * rRaw;
            foreach (var t in ThinkerManager.Snapshot().OfType<Mobj>())
            {
                if (t == m) continue;
                long dx = t.X.Raw - m.X.Raw;
                long dy = t.Y.Raw - m.Y.Raw;
                long dist2 = dx * dx + dy * dy;
                if (dist2 <= r2)
                {
                    // Apply fixed damage scaled by proximity (simple)
                    t.TakeDamage(50);
                }
            }
            // Remove explosive actor
            ThinkerManager.Unregister(m);
        }

        public static void A_Pain(Mobj m)
        {
            if (m == null) return;
            m.MomX = new Fixed(-m.MomX.Raw);
            m.MomY = new Fixed(-m.MomY.Raw);
        }
        public static void A_PlayerScream(Mobj m) { }
        public static void A_Fall(Mobj m) { }
        public static void A_XScream(Mobj m) { }

        // Look: try to find a player and face them
        public static void A_Look(Mobj m)
        {
            if (m == null) return;
            if (!m.CanThink) return;
            var player = FindNearestPlayer(m);
            if (player != null)
            {
                m.Angle = AngleTo(m, player);
                m.SetThinkCooldown(2);
            }
        }

        // Chase: set momentum towards nearest player and face them
        public static void A_Chase(Mobj m)
        {
            if (m == null) return;
            if (!m.CanThink) return;
            var player = FindNearestPlayer(m);
            if (player == null) return;
            m.Angle = AngleTo(m, player);
            double rad = (m.Angle * Math.PI / 180.0);
            double speed = 1.5; // units per tick
            long momxRaw = (long)(Math.Cos(rad) * speed * Fixed.FRACUNIT);
            long momyRaw = (long)(Math.Sin(rad) * speed * Fixed.FRACUNIT);
            m.MomX = new Fixed((int)momxRaw);
            m.MomY = new Fixed((int)momyRaw);
            // small think cooldown to avoid thrashing
            m.SetThinkCooldown(3);
        }

        public static void A_FaceTarget(Mobj m)
        {
            if (m == null) return;
            var player = FindNearestPlayer(m);
            if (player != null)
                m.Angle = AngleTo(m, player);
        }

        public static void A_PosAttack(Mobj m)
        {
            if (m == null) return;
            m.MomX = Fixed.FromInt(2);
        }

        public static void A_Scream(Mobj m) { }
        public static void A_SPosAttack(Mobj m) { }
        public static void A_VileChase(Mobj m)
        {
            if (m == null) return;
            if (!m.CanThink) return;
            var target = FindNearestPlayer(m);
            if (target == null) return;
            m.Angle = AngleTo(m, target);
            double rad = (m.Angle * Math.PI / 180.0);
            double speed = 2.0; // faster than normal chase
            m.MomX = new Fixed((int)(Math.Cos(rad) * speed * Fixed.FRACUNIT));
            m.MomY = new Fixed((int)(Math.Sin(rad) * speed * Fixed.FRACUNIT));
            m.SetThinkCooldown(3);
        }
        public static void A_VileStart(Mobj m)
        {
            // Start: could play sound or set up internal state. Keep minimal for now.
            if (m == null) return;
            // small idle jitter
            m.MomX = Fixed.FromInt(0);
            m.MomY = Fixed.FromInt(0);
        }
        public static void A_VileTarget(Mobj m)
        {
            if (m == null) return;
            var player = FindNearestPlayer(m);
            if (player != null)
            {
                m.Angle = AngleTo(m, player);
            }
        }
        public static void A_VileAttack(Mobj m)
        {
            if (m == null) return;
            if (!m.CanAttack) return;
            const int range = 64; // world units in front
            long rRaw = range * Fixed.FRACUNIT;
            long r2 = rRaw * rRaw;

            foreach (var t in ThinkerManager.Snapshot().OfType<Mobj>())
            {
                if (t == m) continue;
                long dx = t.X.Raw - m.X.Raw;
                long dy = t.Y.Raw - m.Y.Raw;
                long dist2 = dx * dx + dy * dy;
                if (dist2 > r2) continue;

                // Compute angle to target in degrees [0,360)
                double angTo = Math.Atan2((double)dy, (double)dx) * 180.0 / Math.PI;
                if (angTo < 0) angTo += 360.0;
                double diff = Math.Abs(angTo - m.Angle);
                if (diff > 180.0) diff = 360.0 - diff;

                // Simple cone check: 45 degrees either side
                if (diff <= 45.0)
                {
                    // Apply damage (simplified)
                    t.TakeDamage(40);
                }
            }
            m.SetAttackCooldown(30);
        }
        public static void A_StartFire(Mobj m) { }
        public static void A_Fire(Mobj m)
        {
            if (m == null) return;
            // small recoil backwards when firing
            double rad = (m.Angle * Math.PI / 180.0);
            double recoil = 0.5;
            long momxRaw = (long)(-Math.Cos(rad) * recoil * Fixed.FRACUNIT);
            long momyRaw = (long)(-Math.Sin(rad) * recoil * Fixed.FRACUNIT);
            m.MomX = new Fixed((int)momxRaw);
            m.MomY = new Fixed((int)momyRaw);
        }
        public static void A_FireCrackle(Mobj m) { }
        public static void A_Tracer(Mobj m)
        {
            if (m == null) return;
            var player = FindNearestPlayer(m);
            if (player == null) return;
            var missile = new Mobj(m.X, m.Y) { IsMissile = true, Radius = 3, Health = 1, LifetimeTicks = 120 };
            int ang = AngleTo(m, player);
            double rad = ang * Math.PI / 180.0;
            double speed = 3.0;
            missile.MomX = new Fixed((int)(Math.Cos(rad) * speed * Fixed.FRACUNIT));
            missile.MomY = new Fixed((int)(Math.Sin(rad) * speed * Fixed.FRACUNIT));
            ThinkerManager.Register(missile);
        }
        public static void A_SkelWhoosh(Mobj m) { }
        public static void A_SkelFist(Mobj m) { }

        // Skel missile: fire towards nearest player if present
        public static void A_SkelMissile(Mobj m)
        {
            if (m == null) return;
            var player = FindNearestPlayer(m);
            if (player == null) return;
            var missile = new Mobj(m.X, m.Y) { IsMissile = true, Radius = 4, Health = 1, LifetimeTicks = 120 };
            int ang = AngleTo(m, player);
            double rad = ang * Math.PI / 180.0;
            double speed = 2.2;
            missile.MomX = new Fixed((int)(Math.Cos(rad) * speed * Fixed.FRACUNIT));
            missile.MomY = new Fixed((int)(Math.Sin(rad) * speed * Fixed.FRACUNIT));
            ThinkerManager.Register(missile);
        }

        public static void A_FatRaise(Mobj m) { }
        public static void A_FatAttack1(Mobj m)
        {
            if (m == null) return;
                    // Attack only if cooldown expired
                    if (!m.CanAttack) return;
                    const int range = 24;
                    long rRaw = range * Fixed.FRACUNIT;
                    long r2 = rRaw * rRaw;
                    foreach (var t in ThinkerManager.Snapshot().OfType<Mobj>())
                    {
                        if (t == m) continue;
                        long dx = t.X.Raw - m.X.Raw;
                        long dy = t.Y.Raw - m.Y.Raw;
                        long dist2 = dx * dx + dy * dy;
                        if (dist2 <= r2)
                        {
                            t.TakeDamage(30);
                        }
                    }
                    // Set cooldown (ticks)
                    m.SetAttackCooldown(20);
                }
        public static void A_FatAttack2(Mobj m) { }
        public static void A_FatAttack3(Mobj m) { }
        public static void A_BossDeath(Mobj m)
        {
            if (m == null) return;
            // Boss death: large-radius damage and remove self
            const int radiusWorld = 128;
            long rRaw = radiusWorld * Fixed.FRACUNIT;
            long r2 = rRaw * rRaw;
            foreach (var t in ThinkerManager.Snapshot().OfType<Mobj>())
            {
                if (t == m) continue;
                long dx = t.X.Raw - m.X.Raw;
                long dy = t.Y.Raw - m.Y.Raw;
                long dist2 = dx * dx + dy * dy;
                if (dist2 <= r2)
                {
                    // scale damage by proximity (simple linear)
                    double dist = Math.Sqrt((double)dist2) / (double)Fixed.FRACUNIT;
                    int dmg = Math.Max(10, (int)(150 * (1 - (dist / radiusWorld))));
                    t.TakeDamage(dmg);
                }
            }
            ThinkerManager.Unregister(m);
        }
        public static void A_CPosAttack(Mobj m) { }
        public static void A_CPosRefire(Mobj m) { }
        public static void A_TroopAttack(Mobj m) { }
        public static void A_SargAttack(Mobj m)
        {
            if (m == null) return;
            if (!m.CanAttack) return;
            // Sarg attack: moderate-range melee with cone in front
            const int range = 40;
            long rRaw = range * Fixed.FRACUNIT;
            long r2 = rRaw * rRaw;
            foreach (var t in ThinkerManager.Snapshot().OfType<Mobj>())
            {
                if (t == m) continue;
                long dx = t.X.Raw - m.X.Raw;
                long dy = t.Y.Raw - m.Y.Raw;
                long dist2 = dx * dx + dy * dy;
                if (dist2 > r2) continue;
                double angTo = Math.Atan2((double)dy, (double)dx) * 180.0 / Math.PI;
                if (angTo < 0) angTo += 360.0;
                double diff = Math.Abs(angTo - m.Angle);
                if (diff > 180.0) diff = 360.0 - diff;
                if (diff <= 60.0)
                {
                    t.TakeDamage(25);
                }
            }
            m.SetAttackCooldown(15);
        }
        public static void A_HeadAttack(Mobj m) { }
        public static void A_BruisAttack(Mobj m) { }

        // Skull attack: immediate small-damage area around shooter (melee)
        public static void A_SkullAttack(Mobj m)
        {
            if (m == null) return;
            if (!m.CanAttack) return;
            const int range = 16;
            long rRaw = range * Fixed.FRACUNIT;
            long r2 = rRaw * rRaw;
            foreach (var t in ThinkerManager.Snapshot().OfType<Mobj>())
            {
                if (t == m) continue;
                long dx = t.X.Raw - m.X.Raw;
                long dy = t.Y.Raw - m.Y.Raw;
                long dist2 = dx * dx + dy * dy;
                if (dist2 <= r2)
                {
                    t.TakeDamage(20);
                }
            }
            m.SetAttackCooldown(10);
        }

        public static void A_Metal(Mobj m) { }
        public static void A_SpidRefire(Mobj m) { }
        public static void A_BabyMetal(Mobj m) { }
        public static void A_BspiAttack(Mobj m) { }
        public static void A_Hoof(Mobj m) { }
        public static void A_CyberAttack(Mobj m) { }
        public static void A_PainAttack(Mobj m) { }
        public static void A_PainDie(Mobj m) { }
        public static void A_KeenDie(Mobj m) { }
        public static void A_BrainPain(Mobj m) { }
        public static void A_BrainScream(Mobj m) { }
        public static void A_BrainDie(Mobj m)
        {
            if (m == null) return;
            // Play death animation/effects (stub)
            A_BrainExplode(m);
            ThinkerManager.Unregister(m);
        }
        public static void A_BrainAwake(Mobj m) { }
        public static void A_BrainSpit(Mobj m)
        {
            if (m == null) return;
            var player = FindNearestPlayer(m);
            if (player == null) return;
            var missile = new Mobj(m.X, m.Y) { IsMissile = true, Radius = 4, Health = 1, LifetimeTicks = 200 };
            int ang = AngleTo(m, player);
            double rad = ang * Math.PI / 180.0;
            double speed = 2.8;
            missile.MomX = new Fixed((int)(Math.Cos(rad) * speed * Fixed.FRACUNIT));
            missile.MomY = new Fixed((int)(Math.Sin(rad) * speed * Fixed.FRACUNIT));
            ThinkerManager.Register(missile);
        }
        public static void A_SpawnSound(Mobj m) { }
        public static void A_SpawnFly(Mobj m) { }
        public static void A_BrainExplode(Mobj m)
        {
            if (m == null) return;
            const int radiusWorld = 96;
            long rRaw = radiusWorld * Fixed.FRACUNIT;
            long r2 = rRaw * rRaw;
            foreach (var t in ThinkerManager.Snapshot().OfType<Mobj>())
            {
                if (t == m) continue;
                long dx = t.X.Raw - m.X.Raw;
                long dy = t.Y.Raw - m.Y.Raw;
                long dist2 = dx * dx + dy * dy;
                if (dist2 <= r2)
                {
                    double dist = Math.Sqrt((double)dist2) / (double)Fixed.FRACUNIT;
                    int dmg = Math.Max(5, (int)(100 * (1 - (dist / radiusWorld))));
                    t.TakeDamage(dmg);
                }
            }
        }
    }
}
