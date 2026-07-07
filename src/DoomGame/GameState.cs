using DoomCore;

namespace DoomGame
{
    public class GameState
    {
        public int Gametic { get; private set; }
        public bool Paused { get; set; }

        public GameState()
        {
            Gametic = 0;
        }

        public void Tick()
        {
            if (!Paused)
                Gametic++;
        }
    }
}
