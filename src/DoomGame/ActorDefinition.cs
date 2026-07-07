namespace DoomGame
{
    public class ActorDefinition
    {
        public int TypeId { get; init; }
        public string Name { get; init; } = string.Empty;
        public int SpawnState { get; init; }
        public int DeathState { get; set; } = StateTable.S_NULL;
        public int Health { get; init; }
        public int Radius { get; init; }
        public int Height { get; init; }

        public ActorDefinition(int typeId, string name, int spawnState, int deathState = StateTable.S_NULL, int health = 100, int radius = 20, int height = 56)
        {
            TypeId = typeId;
            Name = name;
            SpawnState = spawnState;
            DeathState = deathState;
            Health = health;
            Radius = radius;
            Height = height;
        }
    }
}
