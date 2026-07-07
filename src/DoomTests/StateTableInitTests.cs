using DoomGame;
using Xunit;

namespace DoomTests
{
    public class StateTableInitTests
    {
        [Fact]
        public void InitializeGenerated_PopulatesStates_AndResolvesActions()
        {
            // Ensure clean state and isolate changes in this test
            using (TestHelpers.PushStates(new State[0]))
            {
                // Call initializer
                StateTable.InitializeGenerated();

                Assert.NotNull(StateTable.States);
                Assert.NotEmpty(StateTable.States);

                // Find a state with a non-null action and verify delegate resolves (no exception)
                bool found = false;
                foreach (var s in StateTable.States)
                {
                    if (s.Action != null)
                    {
                        found = true;
                        s.Action.Invoke(null); // actions are stubs that accept null safely
                        break;
                    }
                }

                Assert.True(found, "No state with an action delegate was found in generated table.");
            }

        }
    }
}
