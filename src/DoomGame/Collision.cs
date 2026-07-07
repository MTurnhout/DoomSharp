using System;
using DoomCore;

namespace DoomGame
{
    public static class Collision
    {
        // Check missile hit against other mobjs; if hit apply damage and remove missile
        public static void CheckHit(Mobj missile)
        {
            if (missile == null || !missile.IsMissile) return;

            var thinkers = ThinkerManager.Snapshot();
            foreach (var t in thinkers)
            {
                if (t is not Mobj target) continue;
                if (ReferenceEquals(target, missile)) continue;
                if (target.Health <= 0) continue; // ignore dead targets

                // Both must have radius
                long dx = (long)target.X.Raw - missile.X.Raw;
                long dy = (long)target.Y.Raw - missile.Y.Raw;

                // Use precise distance-squared check to detect collision in fixed-point raw units
                long dist2 = dx * dx + dy * dy;
                long threshRaw = (long)(missile.Radius + target.Radius) * Fixed.FRACUNIT; // threshold in raw fixed units
                long thresh2 = threshRaw * threshRaw;
                if (dist2 <= thresh2)
                {
                    int damage = 20;
                    target.TakeDamage(damage);
                    missile.SetState(StateTable.S_NULL);
                    return;
                }
            }
        }
    }
}
