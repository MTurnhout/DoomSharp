using System;

namespace DoomGame
{
    public static partial class SpawnTable
    {
        // Scan StateTable and heuristically resolve death states for actor definitions
        public static void ResolveDeathStates()
        {
            if (StateTable.States == null || StateTable.States.Length == 0)
                return;

            // Known death action name substrings
            string[] deathIndicators = new[] { "Die", "Death", "PainDie", "BossDeath", "Explode" };

            foreach (var kv in _table)
            {
                var def = kv.Value;
                if (def.DeathState != StateTable.S_NULL)
                    continue; // already set

                // Find first state whose action method name contains a death indicator
                for (int i = 0; i < StateTable.States.Length; i++)
                {
                    var st = StateTable.States[i];
                    if (st.Action == null) continue;
                    var name = st.Action.Method.Name;
                    foreach (var ind in deathIndicators)
                    {
                        if (name.IndexOf(ind, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            def.DeathState = i;
                            goto NextDef;
                        }
                    }
                }

            NextDef: ;
            }
        }
    }
}
