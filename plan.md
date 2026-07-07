DoomSharp Port Plan

Status (2026-07-07):
- Scaffolded .NET 8 solution and test projects.
- Fixed-point and core primitives implemented.
- Generated state & spawn tables wired and tested.
- Many A_* actions implemented and covered by tests (missiles, explode, vile, fat, sarg, tracer, fire, skel missile).
- Actions.cs had stray control chars — file cleaned and rebuilt.
- Integration smoke test (Doom.App startup) passed.

Next immediate steps:
1. Implement remaining monster actions (brain/boss/fat refinements) and add unit tests for each behavior. (In progress)
2. Continue expanding AI: pathing/chase refinements, attack timing, and death flows.
3. Add CI to run dotnet build/test and a regen script to produce GeneratedSpawnTable.cs from info.c.

Short-term milestone (next 2 days):
- Implement Brain actions (A_BrainSpit, A_BrainDie, A_BrainExplode) and tests. (done)
- Implement any missing boss attack/death behaviors and add focused tests. (pending)
- Stabilize state table mapping: ensure GeneratedStateTable comments match info.c tokens and add a CI check. (pending)

Progress log:
- Implemented Vile actions and tests.
- Implemented BrainSpit, BrainExplode, BrainDie and added tests; all tests pass locally.

Long-term:
- Port renderer/audio platform bindings, add WAD asset loading, and implement deterministic gameplay tests.

Notes:
- Keep runtime ResolveDeathStates fallback but prefer embedding numeric indices during generation.
- Use TestHelpers.PushStates for per-test isolation to avoid global state leakage.

Signed-off-by: Copilot
