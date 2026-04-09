# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 6 (6000.4.0f1) arcade Pong game built on the **PaddleGameSO** pattern from the Unity Game Systems Cookbook. Two-player local gameplay with keyboard controls (Player 1: W/S, Player 2: Arrow keys). Entry point scene: `Assets/Scenes/Bootloader_Scene.unity`.

## Architecture

### Event-Driven ScriptableObject Pattern

All systems communicate through **ScriptableObject event channels** (pub/sub), never via direct references. This is the core architectural principle.

- **EventChannelSO** assets live in `Assets/PaddleBall/Data/EventChannels/` (grouped by Gameplay, SceneManagement, UI)
- Base class: `GenericEventChannelSO<T>` exposes `UnityAction<T> OnEventRaised` and `RaiseEvent(T)`
- `VoidEventChannelSO` handles parameterless events (game started, game ended, etc.)
- Game-specific channels: `PlayerIDEventChannelSO`, `PlayerScoreEventChannelSO`, `ScoreListEventChannelSO`
- Subscription pattern: subscribe in `OnEnable()`, unsubscribe in `OnDisable()`

### Game Loop Flow

```
GameManager.StartGame() → raises "GameStarted"
  → Ball serves → Bouncer detects collision → raises "BallCollided"
  → Ball bounces → ScoreGoal detects ball → raises "GoalHit"
  → GameManager.OnGoalHit() → raises "PointScored"
  → ScoreManager updates score → raises "ScoreManagerUpdated"
  → ScoreObjectiveSO checks target → if reached, raises "TargetScoreReached"
  → GameManager → raises "GameEnded" → WinScreen shows winner
```

### Key ScriptableObjects

- **GameDataSO** — All gameplay parameters (speeds, physics, player references, level layout). Single source of truth for game configuration
- **InputReaderSO** — Translates Unity InputSystem events into UnityActions consumed by game logic
- **LevelLayoutSO** — Transform positions for paddles, ball, goals, walls. Supports JSON import/export
- **PlayerIDSO** — Lightweight identity marker (empty SO used as reference key, instances: Player1_SO, Player2_SO)
- **ScoreObjectiveSO** — Objective that completes when a player reaches target score

### Key Classes

- **GameManager** — Orchestrates game flow (start, goal, win, reset). Listens/broadcasts via event channels
- **GameSetup** — Instantiates prefabs (paddles, ball, walls, goals) from LevelLayoutSO data
- **ScoreManager** — Maintains `List<PlayerScore>`, updates UI, notifies objective system
- **Score** — Plain C# class (not MonoBehaviour) holding score value with `IncrementScore()`/`ResetScore()`
- **NullRefChecker** — Reflection-based validation of `[SerializeField]` fields; `[Optional]` attribute skips check

## Code Layout

- `Assets/Core/` — Reusable framework (event channels, objectives, UI manager, commands, scene management, save/load). Assembly: `GameSystems.Core` (has its own `.asmdef`)
- `Assets/PaddleBall/` — Game-specific code (no `.asmdef`, compiles into default Assembly-CSharp)
- `Assets/Patterns/` — Example pattern implementations (not used in gameplay)

## Conventions

- **Namespaces:** `GameSystemsCookbook` (Core), `GameSystemsCookbook.Demos.PaddleBall` (game-specific)
- **SO suffix:** All ScriptableObject classes end with `SO` (e.g., `GameDataSO`, `VoidEventChannelSO`)
- **Private fields:** `m_` prefix with camelCase (e.g., `m_PaddleSpeed`), exposed via PascalCase read-only properties
- **Inspector organization:** `[Header("Section")]`, `[Tooltip("...")]` on all serialized fields
- **Initialization:** `Initialize()` methods called externally, not in Awake/Start. Validation via `NullRefChecker.Validate(this)` in Awake or OnEnable
- **All SOs inherit from `DescriptionSO`** which adds a `[TextArea] m_Description` field

## Testing

No test infrastructure exists yet. To add EditMode tests:
1. Create a test assembly with `.asmdef` referencing `GameSystems.Core` and the game assembly
2. Note: PaddleBall has no `.asmdef` — tests in a separate assembly cannot reference Assembly-CSharp by default. An `.asmdef` for PaddleBall may be needed
3. Testable without MonoBehaviour: `Score` (plain C# class), `ScoreObjectiveSO` logic, event channel raise/subscribe
4. Run tests: Unity **Window > General > Test Runner > EditMode > Run All**

## MCP Integration

The project uses MCP for Unity to connect Claude Code to the Unity editor. Server: `mcpforunityserver` via `uvx`.
