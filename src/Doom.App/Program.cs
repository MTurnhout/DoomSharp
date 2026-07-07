using System.IO;
using DoomGame;

// App bootstrap: initialize generated data (state table, spawn table etc.)
StateTable.InitializeGenerated();
SpawnTable.InitializeGenerated();
// Resolve death states heuristically so ActorDefinition.DeathState is populated when possible
SpawnTable.ResolveDeathStates();

// If original DOOM sources are present, use info.c to set authoritative death state indices
var origInfo = Path.Combine("original", "linuxdoom-1.10", "info.c");
if (File.Exists(origInfo))
{
    try
    {
        SpawnTable.ResolveDeathStatesFromInfoC(origInfo);
    }
    catch (Exception ex)
    {
        // ignore parsing failures - fall back to heuristic
    }
}

// Doom.App initialized (state/spawn counts logged during debug runs)

// CLI: support regeneration of generated spawn table.
if (args.Length >= 2 && args[0] == "--regen-spawn")
{
    var outPath = args[1];
    // Ensure generated initialization has run and runtime resolver attempted
    StateTable.InitializeGenerated();
    SpawnTable.InitializeGenerated();
    if (File.Exists(origInfo))
    {
        try { SpawnTable.ResolveDeathStatesFromInfoC(origInfo); } catch { }
    }
    SpawnTable.ExportGeneratedSource(outPath);
    System.Console.WriteLine($"Wrote regenerated spawn table to: {outPath}");
    return;
}
