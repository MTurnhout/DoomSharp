using System.Collections.Generic;

namespace DoomGame
{
    public static partial class SpawnTable
    {
        private static readonly System.Collections.Generic.Dictionary<int, ActorDefinition> _table = new System.Collections.Generic.Dictionary<int, ActorDefinition>();

        public static void InitializeDefaults()
        {
            // Example mappings. Real port will populate from original DOOM actor enums/data.
            _table.Clear();
            _table[1] = new ActorDefinition(1, "Player", spawnState: 0, deathState: StateTable.S_NULL, health: 100, radius: 16, height: 56);
            _table[2] = new ActorDefinition(2, "Imp", spawnState: 0, deathState: StateTable.S_NULL, health: 60, radius: 16, height: 56);
            _table[3] = new ActorDefinition(3, "Demon", spawnState: 0, deathState: StateTable.S_NULL, health: 300, radius: 32, height: 56);
        }

        // Register or override an actor definition at runtime (used by tests or generators)
        public static void Register(ActorDefinition def)
        {
            _table[def.TypeId] = def;
        }

        public static bool TryGet(int typeId, out ActorDefinition def) => _table.TryGetValue(typeId, out def!);

        // Export the generated spawn table as C# source that calls Register(...) so the generator can bake in death indices.
        public static void ExportGeneratedSource(string path)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("// Auto-generated spawn table (regenerated).\nusing System;\nnamespace DoomGame\n{");
            sb.AppendLine("    public static partial class SpawnTable\n    {");
            sb.AppendLine("        public static void InitializeGenerated()\n        {");
            foreach (var kv in _table)
            {
                var def = kv.Value;
                // Emit a Register call with numeric values. Use literal consts for state indices.
                sb.AppendLine($"            Register(new ActorDefinition({def.TypeId}, \"{def.Name}\", spawnState: {def.SpawnState}, deathState: {def.DeathState}, health: {def.Health}, radius: {def.Radius}, height: {def.Height}));");
            }
            sb.AppendLine("        }\n    }\n}");
            System.IO.File.WriteAllText(path, sb.ToString());
        }
    }
}
