Contributing to DoomSharp

Quickstart for contributors

- Build: dotnet build src/DoomSharp.sln -c Release
- Tests: dotnet test src/DoomTests/DoomTests.csproj -c Release

Spawn-table regeneration

- original/ (DOOM C sources) is intentionally gitignored. To regenerate locally:
  1. dotnet build src/Doom.App/Doom.App.csproj -c Release
  2. dotnet run --project src/Doom.App --configuration Release -- --regen-spawn src/DoomGame/GeneratedSpawnTable.regenerated.cs
  3. Review and replace src/DoomGame/GeneratedSpawnTable.cs, then commit.

CI behavior

- CI runs build and tests on push/PR. It includes a regen-check job that regenerates the spawn table and fails if it differs from the committed file. The job is skipped if original/ is not available in the runner.

Notes

- Use TestHelpers.PushStates and ThinkerManager.Clear when writing tests to avoid global-state leakage.
- Prefer making small, focused PRs.
