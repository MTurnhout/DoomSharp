param(
    [string]$Out = "D:\\Dev\\DoomSharp\\src\\DoomGame\\GeneratedSpawnTable.regenerated.cs"
)

Write-Output "Running spawn-table regeneration..."
cd D:\Dev\DoomSharp\src
# Build Doom.App first to ensure latest code
dotnet build Doom.App\Doom.App.csproj -c Release --nologo
# Run Doom.App with regen arg
dotnet run --project Doom.App --configuration Release -- --regen-spawn $Out
Write-Output "Done. Output: $Out"
