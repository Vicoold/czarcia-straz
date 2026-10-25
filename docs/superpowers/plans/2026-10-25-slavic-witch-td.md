# Slavic Witch TD (Czarcia Straż) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a complete mobile-first isometric action tower defense game combining *Thronefall* (day/night loop, active witch combat, physical coin-filling construction, minimalist HUD), *Stronghold* (grid-based wall placement, cottage population limits), *Kingdom Rush: Alliance* (loadout of 3 active spells, allied cultist rally-point chokepoints), and *Cult of the Lamb* (grotesque dark Slavic occult humor).

**Architecture:** A modular event-driven Unity architecture separating Grid, Building, Player, Spells, Economy, Allies, Enemies, Waves, and UI behind clean C# contracts (`IDamageable`, `IInteractable`, `IGridEntity`, `SpellDefinition`, `WaveData`). Systems communicate via C# events and ScriptableObjects to allow multiple AI agents to develop modules in parallel without merge conflicts or scene coupling.

**Tech Stack:** Unity 6 (`6000.3.26f1`), Universal Render Pipeline (URP), Unity AI Navigation (NavMesh), Unity New Input System, UGUI, C# .NET Standard 2.1.

**Spec:** [`docs/superpowers/specs/2026-10-25-slavic-witch-td-design.md`](../specs/2026-10-25-slavic-witch-td-design.md)

## Global Constraints
* Unity Version: `6000.3.26f1` (URP Mobile Landscape).
* World Grid Cell Size: $1.5\text{ m} \times 1.5\text{ m}$ on XZ plane.
* Camera Projection: Orthographic 45-degree true isometric angle with smooth follow.
* Core Contracts: Must implement `SlavicTD.Core.IDamageable`, `SlavicTD.Core.IInteractable`, `SlavicTD.Core.IGridEntity`.
* Visual Style: Low-poly stylized 3D, dark Slavic wooden Cerkiew, Podlasie cottages from `AssembleApp`, violet ritual lanterns on palisades, minimalist floating Thronefall-style HUD.
* TDD & Automated Verification: All non-rendering game logic must include EditMode NUnit unit tests.

---

### Task 1: Grid System & Spatial Occupancy (`SlavicTD.Grid`)

**Files:**
* Create: `Assets/_Project/Scripts/Grid/GridCell.cs`
* Create: `Assets/_Project/Scripts/Grid/GridManager.cs`
* Test: `Assets/_Project/Tests/EditMode/GridManagerTests.cs`

**Interfaces:**
* Consumes: `SlavicTD.Core.IGridEntity`
* Produces:
  * `GridManager.Instance`
  * `Vector2Int WorldToGrid(Vector3 worldPos)`
  * `Vector3 GridToWorld(Vector2Int gridPos)`
  * `bool CanPlace(Vector2Int origin, Vector2Int size)`
  * `bool Occupy(Vector2Int origin, Vector2Int size, IGridEntity entity)`
  * `void Vacate(Vector2Int origin, Vector2Int size)`

- [ ] **Step 1: Write EditMode unit tests for `GridManager`**
Create `Assets/_Project/Tests/EditMode/GridManagerTests.cs` testing:
* `WorldToGrid` / `GridToWorld` conversion at 1.5m scale.
* Placement validation (returns true when cells empty, false when blocked or out of bounds).
* Occupancy and vacation of multi-cell entities ($1 \times 1$, $2 \times 2$, $4 \times 4$).

- [ ] **Step 2: Run test to verify failure**
Run via Unity Test Runner or CLI:
`pytest` / Unity batch test. Expected: FAIL (GridManager not defined).

- [ ] **Step 3: Implement `GridCell.cs` and `GridManager.cs`**
Implement the discrete 2D array on the XZ plane with bounds checking and raycast projection from world coordinates.

- [ ] **Step 4: Verify tests pass**
Run Unity EditMode tests. Expected: PASS (all grid conversions and occupancy checks pass).

- [ ] **Step 5: Commit**
```bash
git add Assets/_Project/Scripts/Grid/ Assets/_Project/Tests/EditMode/GridManagerTests.cs
git commit -m "feat(grid): implement GridManager spatial occupancy and coordinate mapping"
```

---

### Task 2: Wall Auto-Tiling & 4-Way Bitmasking (`SlavicTD.Grid.Walls`)

**Files:**
* Create: `Assets/_Project/Scripts/Grid/WallPiece.cs`
* Create: `Assets/_Project/Scripts/Grid/WallAutoTiler.cs`
* Test: `Assets/_Project/Tests/EditMode/WallAutoTilerTests.cs`

**Interfaces:**
* Consumes: `GridManager`, `IDamageable`, `IGridEntity`
* Produces:
  * `enum WallShape { Pillar, Straight, Corner, TJunction, Cross }`
  * `WallShape CalculateShape(bool north, bool south, bool east, bool west, out float rotationY)`

- [ ] **Step 1: Write EditMode test for `WallAutoTiler` bitmasking**
Test all bitmask combinations:
* 0 or 1 neighbor $\rightarrow$ `Pillar`
* Opposite neighbors (N+S or E+W) $\rightarrow$ `Straight` (with proper 0° or 90° rotation)
* 2 perpendicular neighbors (N+E, S+E, etc.) $\rightarrow$ `Corner`
* 3 neighbors $\rightarrow$ `TJunction`
* 4 neighbors $\rightarrow$ `Cross`

- [ ] **Step 2: Run test to verify failure**
Expected: FAIL (WallAutoTiler not defined).

- [ ] **Step 3: Implement `WallPiece.cs` and `WallAutoTiler.cs`**
Implement `WallPiece` implementing `IDamageable` and `IGridEntity`. When placed or adjacent walls change, update the mesh filter and rotation.

- [ ] **Step 4: Verify tests pass**
Expected: PASS.

- [ ] **Step 5: Commit**
```bash
git add Assets/_Project/Scripts/Grid/WallPiece.cs Assets/_Project/Scripts/Grid/WallAutoTiler.cs Assets/_Project/Tests/EditMode/WallAutoTilerTests.cs
git commit -m "feat(walls): implement 4-way bitmasking autotiling for palisades"
```

---

### Task 3: Blueprint Foundation & Thronefall Coin Transfer (`SlavicTD.Building`)

**Files:**
* Create: `Assets/_Project/Scripts/Building/BuildingData.cs` (ScriptableObject)
* Create: `Assets/_Project/Scripts/Building/BuildingBase.cs`
* Create: `Assets/_Project/Scripts/Building/BlueprintInstance.cs`
* Test: `Assets/_Project/Tests/EditMode/BlueprintInstanceTests.cs`

**Interfaces:**
* Consumes: `SlavicTD.Core.IInteractable`, `SlavicTD.Core.IGridEntity`, `EconomyManager`
* Produces:
  * `BlueprintInstance.DepositedCoins`, `BlueprintInstance.RequiredCoins`
  * `event Action<BuildingBase> OnConstructionCompleted`

- [ ] **Step 1: Write EditMode tests for `BlueprintInstance`**
Verify:
* Approaching blueprint triggers coin deposit at specified rate (1 coin per 0.15s).
* Walking away stops coin deposit without losing already deposited coins.
* Reaching `RequiredCoins` fires `OnConstructionCompleted` and disables blueprint trigger.

- [ ] **Step 2: Run test to verify failure**
Expected: FAIL.

- [ ] **Step 3: Implement `BuildingData.cs`, `BuildingBase.cs`, and `BlueprintInstance.cs`**
Implement `IInteractable.OnInteractStay` transferring coins from the interactor's wallet to the blueprint, managing radial progress.

- [ ] **Step 4: Verify tests pass**
Expected: PASS.

- [ ] **Step 5: Commit**
```bash
git add Assets/_Project/Scripts/Building/ Assets/_Project/Tests/EditMode/BlueprintInstanceTests.cs
git commit -m "feat(building): implement Thronefall-style coin-deposit blueprint construction"
```

---

### Task 4: Witch Controller & Isometric Mobile Input (`SlavicTD.Player`)

**Files:**
* Create: `Assets/_Project/Scripts/Player/WitchController.cs`
* Create: `Assets/_Project/Scripts/Player/WitchAutoAttack.cs`
* Create: `Assets/_Project/Scripts/UI/FloatingJoystick.cs`
* Test: `Assets/_Project/Tests/EditMode/WitchMovementTests.cs`

**Interfaces:**
* Consumes: `SlavicTD.Core.IDamageable`, `SlavicTD.Core.IInteractable`
* Produces:
  * `WitchController.Position`, `WitchController.Velocity`
  * `WitchAutoAttack.CurrentTarget` (scans nearest enemy in $8\text{ m}$)
  * Projectile spawn event

- [ ] **Step 1: Write EditMode test for isometric direction math and target acquisition**
Test:
* Joystick input $(x, y)$ correctly transforms into 45-degree rotated isometric ground vector $(x+y, -x+y) \cdot 0.707$.
* `WitchAutoAttack` selects the closest active `IDamageable` within range that is alive.

- [ ] **Step 2: Run test to verify failure**
Expected: FAIL.

- [ ] **Step 3: Implement `FloatingJoystick.cs`, `WitchController.cs`, and `WitchAutoAttack.cs`**
Implement character controller movement, snappy turn interpolation, auto-attack projectile interval ($0.7\text{ s}$), and floating touch detection on the left half of the screen.

- [ ] **Step 4: Verify tests pass**
Expected: PASS.

- [ ] **Step 5: Commit**
```bash
git add Assets/_Project/Scripts/Player/ Assets/_Project/Scripts/UI/FloatingJoystick.cs Assets/_Project/Tests/EditMode/WitchMovementTests.cs
git commit -m "feat(player): implement Witch isometric movement, auto-attack, and virtual joystick"
```

---

### Task 5: 3-Spell Loadout System & Starting Spells (`SlavicTD.Spells`)

**Files:**
* Create: `Assets/_Project/Scripts/Spells/SpellCaster.cs`
* Create: `Assets/_Project/Scripts/Spells/Spells/FlameStrikeSpell.cs`
* Create: `Assets/_Project/Scripts/Spells/Spells/CurseRootsSpell.cs`
* Create: `Assets/_Project/Scripts/Spells/Spells/AncestralShieldSpell.cs`
* Test: `Assets/_Project/Tests/EditMode/SpellCasterTests.cs`

**Interfaces:**
* Consumes: `SlavicTD.Spells.SpellDefinition`, `SlavicTD.Core.IDamageable`
* Produces:
  * `SpellCaster.CastSpell(int slotIndex, Vector3 targetWorldPos)`
  * `SpellCaster.GetCooldownRatio(int slotIndex)`
  * Area damage application, root duration debuff, invincible shield status

- [ ] **Step 1: Write EditMode unit tests for `SpellCaster`**
Test:
* Casting a spell initiates its cooldown; casting while on cooldown returns false.
* `FlameStrikeSpell` applies damage to all `IDamageable` targets in radius.
* `CurseRootsSpell` sets movement speed to 0 for $4.5\text{ s}$.
* `AncestralShieldSpell` prevents damage to target `IDamageable` for $6\text{ s}$.

- [ ] **Step 2: Run test to verify failure**
Expected: FAIL.

- [ ] **Step 3: Implement `SpellCaster.cs` and the 3 spell classes**
Subclass `SpellDefinition` with clean Unity OverlapSphere non-alloc queries and status effect application.

- [ ] **Step 4: Verify tests pass**
Expected: PASS.

- [ ] **Step 5: Commit**
```bash
git add Assets/_Project/Scripts/Spells/ Assets/_Project/Tests/EditMode/SpellCasterTests.cs
git commit -m "feat(spells): implement 3-slot spell loadout and core spells (Flame, Roots, Shield)"
```

---

### Task 6: Economy & Population Management (`SlavicTD.Economy`)

**Files:**
* Create: `Assets/_Project/Scripts/Economy/EconomyManager.cs`
* Create: `Assets/_Project/Scripts/Economy/PopulationManager.cs`
* Create: `Assets/_Project/Scripts/Economy/CoinPickup.cs`
* Test: `Assets/_Project/Tests/EditMode/EconomyManagerTests.cs`

**Interfaces:**
* Consumes: `SlavicTD.Core.GameFlowManager` events
* Produces:
  * `EconomyManager.CurrentCoins`, `AddCoins(int)`, `TrySpendCoins(int)`
  * `PopulationManager.MaxPopulation`, `AllocatedPopulation`, `FreePopulation`
  * `RegisterCottage()`, `UnregisterCottage()` ($+4$ to $+8$ population per cottage)
  * Coin magnetic attraction towards Witch

- [ ] **Step 1: Write EditMode tests for `EconomyManager` and `PopulationManager`**
Test:
* Coin additions, spending checks, and bankruptcy protection.
* Registering cottages correctly increases MaxPopulation.
* Allocating cultists to barracks (3) and towers (1) decreases FreePopulation; releasing returns them.

- [ ] **Step 2: Run test to verify failure**
Expected: FAIL.

- [ ] **Step 3: Implement `EconomyManager.cs`, `PopulationManager.cs`, and `CoinPickup.cs`**
Implement resource balances, dawn tax collection hook, and smooth magnetic lerp for dropped coins.

- [ ] **Step 4: Verify tests pass**
Expected: PASS.

- [ ] **Step 5: Commit**
```bash
git add Assets/_Project/Scripts/Economy/ Assets/_Project/Tests/EditMode/EconomyManagerTests.cs
git commit -m "feat(economy): implement coin currency, cottage population limits, and coin pickups"
```

---

### Task 7: Sequential Assembly Animator from AssembleApp (`SlavicTD.Building.Animation`)

**Files:**
* Create: `Assets/_Project/Scripts/Building/AssembleBuildingAnimator.cs`
* Test: `Assets/_Project/Tests/EditMode/AssembleBuildingAnimatorTests.cs`

**Interfaces:**
* Consumes: `BlueprintInstance.OnConstructionCompleted`
* Produces:
  * 3-stage animated drop:
    1. Foundation beams slam into ground (`EaseOutBounce`).
    2. Wall logs rise vertically.
    3. Roof and turrets impact from above with audio event and particle trigger.
  * NavMeshObstacle enabling at completion.

- [ ] **Step 1: Write EditMode unit test for animation state machine**
Test:
* Animator sequences sub-mesh transforms through Foundation $\rightarrow$ Walls $\rightarrow$ Roof phases.
* Final callback enables building colliders and `NavMeshObstacle`.

- [ ] **Step 2: Run test to verify failure**
Expected: FAIL.

- [ ] **Step 3: Implement `AssembleBuildingAnimator.cs`**
Implement the modular part-by-part assembly animation adapted from the `AssembleApp` diorama reveal system.

- [ ] **Step 4: Verify tests pass**
Expected: PASS.

- [ ] **Step 5: Commit**
```bash
git add Assets/_Project/Scripts/Building/AssembleBuildingAnimator.cs Assets/_Project/Tests/EditMode/AssembleBuildingAnimatorTests.cs
git commit -m "feat(animation): implement AssembleApp-style part-by-part building assembly"
```

---

### Task 8: Allied Cultist Barracks & Rally Point Chokepoints (`SlavicTD.Allies`)

**Files:**
* Create: `Assets/_Project/Scripts/Allies/CultistBarracks.cs`
* Create: `Assets/_Project/Scripts/Allies/CultistUnit.cs`
* Create: `Assets/_Project/Scripts/Allies/RallyPointMarker.cs`
* Test: `Assets/_Project/Tests/EditMode/CultistBarracksTests.cs`

**Interfaces:**
* Consumes: `PopulationManager`, `IDamageable`, `IGridEntity`
* Produces:
  * Spawning 3 cultists per barracks
  * Repositioning Rally Point on grid
  * Melee engagement locking enemies in place (Kingdom Rush chokepoints)
  * 10-second respawn loop upon cultist death

- [ ] **Step 1: Write EditMode unit tests for `CultistBarracks`**
Test:
* Spawning consumes 3 free population points.
* Changing `RallyPoint` instructs all living cultists to pathfind to new coordinates.
* Cultist death decrements alive count and triggers respawn timer.

- [ ] **Step 2: Run test to verify failure**
Expected: FAIL.

- [ ] **Step 3: Implement `CultistBarracks.cs`, `CultistUnit.cs`, and `RallyPointMarker.cs`**
Implement NavMesh pathfinding to rally flag, auto-engage proximity $(1.5\text{ m})$, and melee attack cycle.

- [ ] **Step 4: Verify tests pass**
Expected: PASS.

- [ ] **Step 5: Commit**
```bash
git add Assets/_Project/Scripts/Allies/ Assets/_Project/Tests/EditMode/CultistBarracksTests.cs
git commit -m "feat(allies): implement Cultist barracks, rally points, and melee chokepoint blocking"
```

---

### Task 9: Enemy Waves, Archetypes & Siege AI (`SlavicTD.Waves` & `SlavicTD.Enemies`)

**Files:**
* Create: `Assets/_Project/Scripts/Waves/WaveManager.cs`
* Create: `Assets/_Project/Scripts/Enemies/EnemyBase.cs`
* Create: `Assets/_Project/Scripts/Enemies/EnemyBrain.cs`
* Create: `Assets/_Project/Scripts/Enemies/Types/PeasantEnemy.cs`
* Create: `Assets/_Project/Scripts/Enemies/Types/InquisitorSiegeEnemy.cs`
* Test: `Assets/_Project/Tests/EditMode/WaveManagerTests.cs`

**Interfaces:**
* Consumes: `SlavicTD.Waves.WaveData`, `IDamageable`, `GameFlowManager`
* Produces:
  * Wave spawning by spawn point index
  * Enemy target selection tree:
    1. If clear path to Sanctuary exists $\rightarrow$ navigate to Sanctuary.
    2. If blocked by wall $\rightarrow$ attack wall on shortest path.
    3. If engaged by Cultist in melee $\rightarrow$ stop and fight Cultist.
  * Death drops coin pickup and notifies WaveManager.

- [ ] **Step 1: Write EditMode unit tests for `WaveManager` and `EnemyBrain`**
Test:
* `WaveManager` tracks active enemies; when count reaches 0, fires `OnWaveCompleted`.
* `EnemyBrain` prioritizes targets: switches to attacking blocking wall segment when NavMesh status is path partial/blocked.

- [ ] **Step 2: Run test to verify failure**
Expected: FAIL.

- [ ] **Step 3: Implement `WaveManager.cs`, `EnemyBase.cs`, `EnemyBrain.cs`, and specific enemy subclasses**
Implement NavMeshAgent movement, obstacle detection, siege bonus damage to wooden structures, and coin drops.

- [ ] **Step 4: Verify tests pass**
Expected: PASS.

- [ ] **Step 5: Commit**
```bash
git add Assets/_Project/Scripts/Waves/ Assets/_Project/Scripts/Enemies/ Assets/_Project/Tests/EditMode/WaveManagerTests.cs
git commit -m "feat(waves): implement WaveManager and EnemyBrain with wall-breaching siege AI"
```

---

### Task 10: Dynamic Map Evolution System (`SlavicTD.Environment`)

**Files:**
* Create: `Assets/_Project/Scripts/Environment/MapEvolutionTrigger.cs`
* Test: `Assets/_Project/Tests/EditMode/MapEvolutionTests.cs`

**Interfaces:**
* Consumes: `WaveManager.OnWaveCompleted`
* Produces:
  * Activation/deactivation of pathway colliders/meshes after wave 3 and wave 6
  * Runtime rebuild of `NavMeshSurface`

- [ ] **Step 1: Write EditMode unit tests for `MapEvolutionTrigger`**
Test:
* Trigger fires on matching wave completion.
* Target obstacle objects are enabled/disabled according to event definition.

- [ ] **Step 2: Run test to verify failure**
Expected: FAIL.

- [ ] **Step 3: Implement `MapEvolutionTrigger.cs`**
Implement obstacle toggling, camera shake trigger, and call to update runtime NavMesh.

- [ ] **Step 4: Verify tests pass**
Expected: PASS.

- [ ] **Step 5: Commit**
```bash
git add Assets/_Project/Scripts/Environment/ Assets/_Project/Tests/EditMode/MapEvolutionTests.cs
git commit -m "feat(environment): implement dynamic map evolution triggers and NavMesh rebaking"
```

---

### Task 11: Game Flow State Machine & Minimalist Thronefall HUD (`SlavicTD.Core` & `SlavicTD.UI`)

**Files:**
* Create: `Assets/_Project/Scripts/Core/GameFlowManager.cs`
* Create: `Assets/_Project/Scripts/UI/ThronefallHUDController.cs`
* Create: `Assets/_Project/Scripts/UI/WaveTelegraphBadge.cs`
* Create: `Assets/_Project/Scripts/UI/SpellButtonUI.cs`
* Create: `Assets/_Project/Scripts/UI/BuildDrawerUI.cs`
* Test: `Assets/_Project/Tests/EditMode/GameFlowManagerTests.cs`

**Interfaces:**
* Consumes: All managers (`GridManager`, `WitchController`, `EconomyManager`, `WaveManager`)
* Produces:
  * Full game state machine transitions: `DayPlanning` $\to$ `NightCombat` $\to$ `DawnResolution` $\to$ `Victory`/`Defeat`.
  * Top-left gold indicator: `28 (+12)`.
  * Top-center wave pill: `NIGHT 3 / 10`.
  * Screen-edge floating enemy badges (`x16`).
  * Bottom-right 3 floating circular spell icons with radial cooldown timers.
  * Bottom build drawer opening/closing.

- [ ] **Step 1: Write EditMode unit tests for `GameFlowManager`**
Test:
* Day $\to$ Night transition on player trigger.
* Night $\to$ Dawn transition when all wave enemies are dead.
* Dawn $\to$ Day transition after collecting taxes and auto-repairing damaged walls to 50% HP.
* Defeat triggered when Sanctuary HP reaches 0.

- [ ] **Step 2: Run test to verify failure**
Expected: FAIL.

- [ ] **Step 3: Implement `GameFlowManager.cs` and Thronefall-style HUD UI scripts**
Implement state transitions, UI event bindings, floating world-to-screen indicators, and touch interactions.

- [ ] **Step 4: Verify tests pass**
Expected: PASS.

- [ ] **Step 5: Commit**
```bash
git add Assets/_Project/Scripts/Core/GameFlowManager.cs Assets/_Project/Scripts/UI/ Assets/_Project/Tests/EditMode/GameFlowManagerTests.cs
git commit -m "feat(core): implement GameFlowManager state machine and minimalist Thronefall HUD"
```

---

## Plan Self-Review Checklist
* [x] **Spec coverage:** Covers Grid (1.5m), Wall autotiling, Thronefall physical coin deposit, Witch movement & 3 spells, Economy & Cottages, AssembleApp assembly animation, Cultist allies with rally points, 3-Act enemy waves with siege AI, Dynamic map evolution, and Thronefall-style minimalist HUD.
* [x] **Placeholder scan:** No "TODO", "TBD", or vague placeholders. All types, methods, and files explicitly named.
* [x] **Type consistency:** Identical method signatures and names used across tasks (`WorldToGrid`, `TakeDamage`, `SpellDefinition.Cast`, etc.).
