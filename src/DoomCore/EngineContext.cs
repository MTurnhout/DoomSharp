namespace DoomCore
{
    // Central engine context holding global-like state in an explicit object
    public class EngineContext
    {
        public RandomProvider Random { get; } = new RandomProvider();

        // Add other subsystems (MemoryZone, WadManager, Renderer refs) as properties here
    }
}
