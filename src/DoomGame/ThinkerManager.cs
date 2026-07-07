using System.Collections.Generic;

namespace DoomGame
{
    public interface IThinker
    {
        void Tick();
    }

    public static class ThinkerManager
    {
        private static readonly List<IThinker> _thinkers = new List<IThinker>();

        public static void Register(IThinker t) => _thinkers.Add(t);

        public static void Unregister(IThinker t) => _thinkers.Remove(t);

        public static IReadOnlyList<IThinker> Snapshot() => _thinkers.ToArray();

        public static void TickAll()
        {
            // Iterate over a snapshot to allow thinkers to register/unregister during ticks
            foreach (var t in _thinkers.ToArray())
                t.Tick();
        }

        public static void Clear() => _thinkers.Clear();
    }
}
