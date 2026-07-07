using System;
using System.Collections.Generic;
using System.IO;

namespace DoomGame
{
    public static partial class SpawnTable
    {
        // Populate ActorDefinition.DeathState from a canonical info.c file by mapping named states to indices
        public static void ResolveDeathStatesFromInfoC(string infoCPath)
        {
            if (!File.Exists(infoCPath)) return;

            // Build mapping from state name (S_NAME) -> index using GeneratedStateTable.cs ordering
            var stateMap = new Dictionary<string, int>(StringComparer.Ordinal);
            var genPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "src", "DoomGame", "GeneratedStateTable.cs");
            // Try direct repo path if file exists, otherwise try to discover in assembly location
            string? genFull = genPath;
            if (!File.Exists(genFull))
            {
                // fallback: look relative to current working directory
                genFull = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "src", "DoomGame", "GeneratedStateTable.cs");
                if (!File.Exists(genFull))
                    genFull = null;
            }

            if (string.IsNullOrEmpty(genFull) || !File.Exists(genFull))
            {
                // Can't find generated state table - abort
                return;
            }

            var genLines = File.ReadAllLines(genFull!);
            int idx = 0;
            foreach (var line in genLines)
            {
                var s = line.TrimEnd();
                int commentPos = s.IndexOf("// S_");
                if (commentPos >= 0)
                {
                    var comment = s.Substring(commentPos + 3).Trim(); // e.g. S_NAME
                    var name = comment.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    if (!stateMap.ContainsKey(name))
                    {
                        stateMap[name] = idx;
                    }
                    idx++;
                }
                else
                {
                    // Only count lines that start with "new State" as state entries
                    if (s.StartsWith("new State"))
                    {
                        // No comment; still add placeholder name like _UNKNOWN_x
                        var placeholder = "_UNKNOWN_" + idx;
                        stateMap[placeholder] = idx;
                        idx++;
                    }
                }
            }

            // Now parse info.c mobjinfo array to extract deathstate symbols per mobj type index
            var infoLines = File.ReadAllLines(infoCPath);
            int lineCount = infoLines.Length;
            bool inMobj = false;
            int mobjIndex = 0;
            for (int i = 0; i < lineCount; i++)
            {
                var line = infoLines[i].Trim();
                if (!inMobj)
                {
                    if (line.StartsWith("mobjinfo_t mobjinfo"))
                    {
                        // find opening brace
                        while (i < lineCount && !infoLines[i].Contains("={")) i++;
                        inMobj = true;
                        i++; // move to first entry
                    }
                }
                else
                {
                    // We're inside array; each mobj entry is a block of lines ending with '},'
                    if (infoLines[i].TrimStart().StartsWith("{"))
                    {
                        // Scan this block to find the deathstate line
                        int j = i;
                                                string? deathStateName = null;
                        while (j < lineCount && !infoLines[j].TrimEnd().EndsWith("},") && !infoLines[j].TrimEnd().EndsWith("}\n") && !infoLines[j].TrimEnd().EndsWith("}\r\n"))
                        {
                            var l = infoLines[j].Trim();
                            if (l.StartsWith("S_"))
                            {
                                // This could be many S_ lines (seestate, etc.). We need the deathstate which appears after missilestate
                                // We'll instead scan for the literal comment "// deathstate" on previous lines
                            }
                            if (l.Contains("// deathstate"))
                            {
                                // line expected like: S_VILE_DIE1,    // deathstate
                                var parts = l.Split(new[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                // find token starting with S_
                                foreach (var p in parts)
                                {
                                    if (p.StartsWith("S_"))
                                    {
                                        deathStateName = p.Trim().TrimEnd(',');
                                        break;
                                    }
                                }
                            }
                            j++;
                        }

                        // Move i to end of block
                        i = j;

                        // Map death state
                        if (deathStateName != null && deathStateName != "S_NULL")
                        {
                            // find numeric index
                            if (stateMap.TryGetValue(deathStateName, out var stIndex))
                            {
                                if (TryGet(mobjIndex, out var def))
                                {
                                    def.DeathState = stIndex;
                                }
                            }
                        }

                        mobjIndex++;
                    }

                    if (infoLines[i].Trim().StartsWith("};"))
                    {
                        // end of mobjinfo array
                        break;
                    }
                }
            }
        }
    }
}
