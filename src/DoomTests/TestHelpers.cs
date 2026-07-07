using System;
using DoomGame;

namespace DoomTests
{
    public static class TestHelpers
    {
        public static void ResetWorld()
        {
            // Reset global tables to known defaults (do NOT clear StateTable.States; tests set it as needed)
            SpawnTable.InitializeDefaults();
            ThinkerManager.Clear();
        }

        // Push provided State[] as the current StateTable.States and return an IDisposable
        // that restores the previous states on dispose. Use in tests with `using` to ensure isolation.
        public static IDisposable PushStates(State[] states)
        {
            var previous = StateTable.States;
            StateTable.States = states;
            return new DisposableAction(() => StateTable.States = previous);
        }

        private sealed class DisposableAction : IDisposable
        {
            private readonly Action _onDispose;
            public DisposableAction(Action onDispose) => _onDispose = onDispose ?? throw new ArgumentNullException(nameof(onDispose));
            public void Dispose() => _onDispose();
        }
    }
}
