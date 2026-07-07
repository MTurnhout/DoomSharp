DoomSharp

Purpose

This repository ports the DOOM 1.10 game logic to idiomatic .NET 8 C# (DoomSharp).

Regenerating the spawn table

- The original DOOM C sources (original/) are intentionally gitignored and not committed.
- To regenerate the spawn table locally:
  1. Build Doom.App: dotnet build src/Doom.App/Doom.App.csproj -c Release
  2. Run regen: dotnet run --project src/Doom.App --configuration Release -- --regen-spawn src/DoomGame/GeneratedSpawnTable.regenerated.cs
  3. Verify and replace src/DoomGame/GeneratedSpawnTable.cs, then commit.

CI behavior

- CI includes a regen-check job that regenerates the spawn table (when originals are present) and fails the run if the regenerated file differs from the committed GeneratedSpawnTable.cs. This prevents drift.

Notes

- Keep original/ out of git (licensing / size). To run full regen in CI, add original/info.c to the runner or use a private artifact.
- If you update GeneratedSpawnTable.cs locally, run regen and commit the updated file to keep CI happy.