using DoomCore;

namespace DoomGame
{
    public static class SpawnManager
    {
        // Spawn a Mobj from a MapThing. Uses map coordinates (integers) -> Fixed.
        public static Mobj SpawnMapThing(MapThing mt)
        {
            // Convert map coordinates to fixed (assume map units directly map)
            var x = Fixed.FromInt(mt.X);
            var y = Fixed.FromInt(mt.Y);
            var m = new Mobj(x, y);

            // In full port: determine mobj type, properties, health, flags, state, etc.
            int chosenState = -1;
            if (SpawnTable.TryGet(mt.Type, out var def))
            {
                // If the spawn state's index is valid for current StateTable, use it; otherwise fall back to 0 if available.
                if (StateTable.States != null && def.SpawnState >= 0 && def.SpawnState < StateTable.States.Length)
                {
                    chosenState = def.SpawnState;
                }
                else if (StateTable.States != null && StateTable.States.Length > 0)
                {
                    chosenState = 0;
                }
                else
                {
                    chosenState = StateTable.S_NULL;
                }
            }
            else if (StateTable.States != null && StateTable.States.Length > 0)
            {
                // fallback
                chosenState = 0;
            }
            else
            {
                chosenState = StateTable.S_NULL;
            }

            // Apply actor definition properties
            if (def != null)
            {
                m.TypeId = def.TypeId;
                m.Health = def.Health;
                m.Radius = def.Radius;
                m.Height = def.Height;
            }

            // Set determined state (may be 0). Call SetState directly — don't treat 0 as special sentinel here.
            m.SetState(chosenState);

            // Register thinker so it ticks
            ThinkerManager.Register(m);

            // Do not re-invoke the state's action here; SetState already calls it per P_SetMobjState semantics.
            return m;
        }
    }
}
