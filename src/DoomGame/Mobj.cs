using DoomCore;

namespace DoomGame
{
    public class Mobj : IThinker
    {
        public Fixed X { get; private set; }
        public Fixed Y { get; private set; }
        public Fixed MomX { get; set; }
        public Fixed MomY { get; set; }

        public bool IsPlayer { get; set; }

        // New: Type and health wiring for actor definitions
        public int TypeId { get; set; }
        public int Health { get; set; }
        public int Angle { get; set; } // degrees

        public Mobj(Fixed x, Fixed y)
        {
            X = x; Y = y; MomX = new Fixed(0); MomY = new Fixed(0);
            TypeId = 0;
            Health = 0;
            Angle = 0;
            Radius = 0;
            Height = 0;
        }

        public void SetPosition(Fixed x, Fixed y)
        {
            X = x; Y = y;
        }

        // Very small stub for TryMove: in the full port this checks collisions with map geometry.
        public virtual bool TryMove(Fixed newX, Fixed newY)
        {
            // Consult world map blocker for terrain collisions.
            if (WorldMap.IsBlocked(newX, newY, Radius))
                return false;
            return true;
        }

        // Damage and health
        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                // Dead: try to set to death state if defined, otherwise remove
                if (SpawnTable.TryGet(TypeId, out var def) && def.DeathState != StateTable.S_NULL)
                {
                    SetState(def.DeathState);
                }
                else
                {
                    SetState(StateTable.S_NULL);
                }
            }
            else
            {
                // Pain reaction: call pain action if present
                // Find current state and invoke its action if it exists
                if (StateTable.States != null && StateIndex >= 0 && StateIndex < StateTable.States.Length)
                {
                    var st = StateTable.States[StateIndex];
                    if (st.Action != null)
                        st.Action.Invoke(this);
                }
            }
        }

        public int StateIndex { get; private set; } = StateTable.S_NULL;
        public int Tics { get; private set; }
        public int Sprite { get; private set; }
        public int Frame { get; private set; }

        // Physical properties
        public int Radius { get; set; }
        public int Height { get; set; }
        public bool IsMissile { get; set; } = false;

        public int LifetimeTicks { get; set; } = 0;

        public int AttackCooldownTicks { get; set; } = 0;
        public int ThinkCooldownTicks { get; set; } = 0;

        public void SetAttackCooldown(int ticks) => AttackCooldownTicks = ticks;
        public void SetThinkCooldown(int ticks) => ThinkCooldownTicks = ticks;

        public bool CanAttack => AttackCooldownTicks == 0;
        public bool CanThink => ThinkCooldownTicks == 0;

        public void Tick()
        {
            // Movement and per-tick lifecycle management
            Movement.Tick(this);

            // Decrement attack cooldown per tick
            if (AttackCooldownTicks > 0) AttackCooldownTicks--;
            if (ThinkCooldownTicks > 0) ThinkCooldownTicks--;
            if (LifetimeTicks > 0)
            {
                LifetimeTicks--;
                if (LifetimeTicks <= 0)
                {
                    // expire
                    SetState(StateTable.S_NULL);
                    return;
                }
            }

            // Handle state tics: decrement and advance when zero
            if (StateIndex != StateTable.S_NULL && Tics > 0)
            {
                Tics--;
                if (Tics == 0)
                {
                    // Advance to next state
                    if (StateTable.States != null && StateIndex >= 0 && StateIndex < StateTable.States.Length)
                    {
                        var current = StateTable.States[StateIndex];
                        SetState(current.NextState);
                    }
                    else
                    {
                        SetState(StateTable.S_NULL);
                    }
                }
            }
        }

        // Port of P_SetMobjState: set state, run action functions, advance through zero-tic states
        public bool SetState(int stateIndex)
        {
            int state = stateIndex;
            while (true)
            {
                if (state == StateTable.S_NULL)
                {
                    // In original, this removes the mobj. For skeleton, mark as absent and unregister thinker.
                    StateIndex = StateTable.S_NULL;
                    ThinkerManager.Unregister(this);
                    return false;
                }

                if (StateTable.States == null || state < 0 || state >= StateTable.States.Length)
                {
                    // invalid state index: treat as removal
                    StateIndex = StateTable.S_NULL;
                    ThinkerManager.Unregister(this);
                    return false;
                }

                var st = StateTable.States[state];

                StateIndex = state;
                Tics = st.Tics;
                Sprite = st.Sprite;
                Frame = st.Frame;

                // Call action function if present
                if (st.Action != null)
                {
                    st.Action.Invoke(this);
                }

                state = st.NextState;

                // If tics != 0, stop here
                if (Tics != 0)
                    break;
                // else loop to next state immediately
            }

            return true;
        }
    }
}
