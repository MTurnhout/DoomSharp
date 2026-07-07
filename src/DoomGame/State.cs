using System;

namespace DoomGame
{
    public delegate void StateAction(Mobj m);

    public class State
    {
        public int NextState { get; set; }
        public int Tics { get; set; }
        public int Sprite { get; set; }
        public int Frame { get; set; }
        public StateAction? Action { get; set; }
    }

    public static partial class StateTable
    {
        // Mutable for tests; real port will load from data
        public static State[] States { get; set; } = Array.Empty<State>();

        public const int S_NULL = -1;
    }
}
