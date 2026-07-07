using DoomCore;
using System;

namespace DoomGame
{
    // Minimal world map blocker abstraction for movement/collision checks.
    // By default nothing is blocked. Tests and map loader can install a blocker.
    public static class WorldMap
    {
        private static Func<Fixed, Fixed, int, bool>? _isBlocked;

        // Default: no blocking
        static WorldMap() => _isBlocked = (x, y, radius) => false;

        public static bool IsBlocked(Fixed x, Fixed y, int radius) => _isBlocked?.Invoke(x, y, radius) ?? false;

        // For tests or map loader: set a custom blocker predicate
        public static void SetBlocker(Func<Fixed, Fixed, int, bool> blocker) => _isBlocked = blocker;

        public static void ResetBlocker() => _isBlocked = (x, y, r) => false;
    }
}
