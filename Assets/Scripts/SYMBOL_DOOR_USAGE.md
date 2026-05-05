# Symbol Door - Usage Guide

## Architecture

**MVC Pattern:**
- **Model** (`SymbolDoorModel`/`ISymbolDoorModel`) — stores puzzle state, validates answers
- **View** (`SymbolDoorView`) — displays UI, manages visual feedback
- **Controller** (`SymbolDoorController`) — handles user interactions, routes events

**Observer Pattern:**
- `SymbolDoorEventManager` — broadcasts `OnDoorOpened`, `OnDoorFailed`, `OnPuzzleReset`

## Setup in Scene

1. Create empty GameObject "SymbolDoor"
2. Attach `SymbolDoor.cs` component
3. Create Canvas with `SymbolDoorView` prefab
4. Assign View prefab in inspector
5. (Optional) Assign default `PuzzleConfig` asset

## Runtime Usage

### Option 1: Using PuzzleConfig (Recommended)

```csharp
// Create asset: right-click → SymbolDoor/PuzzleConfig
// Add symbol-meaning pairs in editor

SymbolDoor door = SymbolDoor.Instance;
door.ShowPuzzle(); // Uses default config
```

### Option 2: Programmatic

```csharp
var symbols = new List<string> { "☀️", "🌙", "🕐" };
var meanings = new List<string> { "день", "ночь", "время" };
var pairs = new Dictionary<string, string>
{
    { "☀️", "день" },
    { "🌙", "ночь" },
    { "🕐", "время" }
};

SymbolDoor door = SymbolDoor.Instance;
door.ShowPuzzle(symbols, meanings, pairs);
```

### Listening to Events

```csharp
void OnEnable()
{
    SymbolDoorEventManager.OnDoorOpened += HandleSuccess;
    SymbolDoorEventManager.OnDoorFailed += HandleFailure;
}

void OnDisable()
{
    SymbolDoorEventManager.OnDoorOpened -= HandleSuccess;
    SymbolDoorEventManager.OnDoorFailed -= HandleFailure;
}

void HandleSuccess() { /* ... */ }
void HandleFailure() { /* ... */ }
```

## Integration with WaveManager

Between waves, WaveManager shows Symbol Door to gate progression:

```csharp
// Automatically called between waves if showSymbolDoor = true
yield return StartCoroutine(ShowSymbolDoor());
```

## Bug Fixes & Safety

✓ Null checks in all public methods
✓ Input validation for symbol/meaning strings
✓ Coroutine cleanup to prevent leaks
✓ Event listener cleanup in OnDestroy
✓ Interface-based design for testability
✓ Readonly collections to prevent external mutation

## Known Limitations

- Audio clips are optional (no error if null)
- UI prefabs must be assigned manually in editor
- Symbol/meaning shuffling not implemented (order is preserved)
