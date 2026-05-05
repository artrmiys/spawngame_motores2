# motores2 Development Notes

Last updated: 2026-05-03

## Current Goal

`motores2` is the active Unity project for a portrait mobile survival game called **Swarm Survivor**.

The current target is:

- Unity `2022.3.5f1`.
- Android/iOS-style vertical mobile layout.
- Portrait orientation only.
- Simple two-level wave survival loop.
- Fast manual testing through generated scenes.

`motores2_tp1` is an `Assets`-only copy/export. Keep it synced when coursework submission needs only the asset folder, but open and work in the full Unity project at `motores2`.

## How To Open And Generate The Game

Open this folder in Unity:

```powershell
E:\..davinci_learn\My Game\motores2
```

After Unity finishes importing, run:

```text
Tools -> Motores2 - Setup All Scenes
```

That editor command generates:

- `Assets/Scenes/SplashScreen.unity`
- `Assets/Scenes/MainMenu.unity`
- `Assets/Scenes/Level1.unity`
- `Assets/Scenes/Level2.unity`
- `Assets/Generated/ProjectilePrefab.prefab`
- `Assets/Generated/EnemyPrefab.prefab`
- `Assets/Generated/HealPowerUpPrefab.prefab`
- `Assets/Generated/SpeedPowerUpPrefab.prefab`

It also writes the generated scenes into Build Settings in this order:

```text
SplashScreen -> MainMenu -> Level1 -> Level2
```

## Current Game Flow

```text
SplashScreen
  -> MainMenu
  -> Level1 - Garden Rush
  -> Level2 - Night Swarm
  -> final win screen
```

`Level1` is easier:

- 3 waves.
- 4 base enemies.
- No obstacles.
- Green garden-like palette.

`Level2` is harder:

- 5 waves.
- 6 base enemies.
- Obstacles in the arena.
- Darker night palette.

## Mobile / Portrait Work Done

Project settings changed in `ProjectSettings/ProjectSettings.asset`:

- `productName`: `Swarm Survivor`
- `defaultScreenOrientation`: portrait
- portrait auto-rotation only
- default/editor window size: `1080x1920`
- Android default window size: `1080x1920`
- Android/iPhone bundle id: `com.menshchikov.swarmsurvivor`
- Android target architectures: ARMv7 + ARM64

UI generation changed in `Assets/Editor/Motores2SceneSetup.cs`:

- Canvas uses `Scale With Screen Size`.
- Reference resolution is `1080x1920`.
- UI is placed under a `SafeArea` object.
- `SafeAreaFitter` applies `Screen.safeArea` at runtime.
- Joystick is anchored bottom-left for one-thumb mobile movement.

Gameplay arena changed:

- Portrait arena is approximately `12 x 24`.
- Camera orthographic size is tuned for vertical gameplay.
- Level 2 adds physical obstacles.

## Main Scripts And Responsibilities

`Assets/Editor/Motores2SceneSetup.cs`

- Main scene generator.
- Use `Tools -> Motores2 - Setup All Scenes`.
- Creates scenes, prefabs, UI, build settings, and serialized references.
- Avoid manual reference assignment after generation.

`Assets/Scripts/GameManager.cs`

- Handles win/lose/restart/menu.
- Supports `nextSceneName`.
- If `nextSceneName` is set, win loads the next level after a short delay.
- If empty, win shows the final win screen.

`Assets/Scripts/WaveManager.cs`

- Spawns enemy waves.
- Per-level values are assigned by the editor setup:
  - `levelWaveCount`
  - `levelTitle`
  - `baseEnemies`
  - `spawnRadius`

`Assets/Scripts/RemoteConfigManager.cs`

- Now local-only tuning.
- No dependency on Unity Services or Remote Config packages.
- Provides `PowerupEnabled`, `EnemySpeed`, `PlayerSpeed`, and `SpawnInterval`.

`Assets/Scripts/UI/SafeAreaFitter.cs`

- Runtime UI safe-area fitter for phones with notches/cutouts.
- Updates when `Screen.safeArea` or screen size changes.

`Assets/Scripts/Player/PlayerController.cs`

- Uses virtual joystick when present.
- Falls back to keyboard axes for editor testing.
- Uses `Rigidbody2D.velocity`, not `linearVelocity`, for Unity 2022 compatibility.

`Assets/Scripts/Player/AutoAttack.cs`

- Finds nearest enemy in range.
- Shoots projectiles automatically.
- Has a null guard if projectile prefab is missing.

`Assets/Scripts/Player/PlayerHealth.cs` and `Assets/Scripts/Enemy/EnemyHealth.cs`

- UnityEvents are initialized safely in code.
- Player health exposes `MaxHP` for UI.

`Assets/Scripts/UI/UIManager.cs`

- Shows HP, wave text, message text, win screen, lose screen.
- Auto-subscribes to player health.

## Safe Mode Issue And Fix

Unity opened in Safe Mode because `RemoteConfigManager.cs` referenced packages that were not resolving:

```text
Unity.Services.RemoteConfig
ConfigResponse
RemoteConfigService
```

There were also old package/cache errors around `com.unity.collab-proxy`.

Fixes applied:

- Removed all `Unity.Services.*` usage from `RemoteConfigManager.cs`.
- Removed these direct dependencies from `Packages/manifest.json`:
  - `com.unity.services.authentication`
  - `com.unity.services.remoteconfig`
  - `com.unity.collab-proxy`
- Removed matching entries from `Packages/packages-lock.json`.

After the fix, these builds passed:

```powershell
dotnet build motores2\Assembly-CSharp.csproj -v:minimal
dotnet build motores2\Assembly-CSharp-Editor.csproj -v:minimal
```

Both completed with zero errors.

If Unity still shows Safe Mode after these changes:

1. Close Unity completely.
2. Reopen `motores2`.
3. Let Package Manager resolve the updated manifest.
4. Exit Safe Mode if prompted.
5. Run `Tools -> Motores2 - Setup All Scenes`.

## Verification Done

Static checks:

```powershell
rg -n "Unity\.Services|RemoteConfigService|ConfigResponse|AuthenticationService|UnityServices" motores2\Assets motores2\Packages motores2_tp1\Assets -g "*.cs" -g "*.json" -g "!**/Library/**"
rg -n "linearVelocity" motores2 motores2_tp1 -g "*.cs" -g "!**/Library/**"
rg -n 'LoadScene\("Level"' motores2 motores2_tp1 -g "*.cs" -g "!**/Library/**"
```

Results:

- No `Unity.Services` code references remain.
- No `linearVelocity` remains.
- No old `LoadScene("Level")` remains.

Compilation checks:

```powershell
dotnet build motores2\Assembly-CSharp.csproj -v:minimal
dotnet build motores2\Assembly-CSharp-Editor.csproj -v:minimal
```

Results:

- Runtime assembly: OK.
- Editor assembly: OK.

Unity batchmode note:

- A direct `Unity.exe -batchmode` attempt exited with code `127` and did not produce the requested custom log in this shell session.
- The useful confirmation came from the Unity-generated `.csproj` builds plus the Editor log diagnosis.

## Research Notes Used

Official Unity/platform notes used for direction:

- Unity Android Player Settings: portrait orientation, resolution/presentation, target architectures.
  - https://docs.unity.cn/2022.3/Documentation/Manual/class-PlayerSettingsAndroid.html
- Unity UI / Canvas Scaler guidance.
  - https://learn.unity.com/tutorial/working-with-ui-in-unity
- Unity Device Simulator: safe area, touch input, default orientation simulation.
  - https://docs.unity3d.com/cn/2022.3/Manual/device-simulator-introduction.html
- Google Play 64-bit requirement.
  - https://developer.android.com/google/play/requirements/64-bit

Practical interpretation for this project:

- Lock to portrait because the assignment/game is now mobile-first and vertical.
- Use `1080x1920` as reference layout.
- Keep UI under a safe-area container.
- Test in Unity Device Simulator when available.
- Keep Android ARM64 enabled for modern mobile builds.

## Suggested Next Improvements

Highest-value next tasks:

1. Add visible enemy variety.
   - Fast weak enemy.
   - Slow tank enemy.
   - Ranged enemy or splitter enemy.

2. Add player progression between waves.
   - Pick one upgrade after each wave: fire rate, projectile damage, movement speed, or max HP.

3. Add mobile polish.
   - Bigger touch targets.
   - Short hit flash on enemies.
   - Floating damage text.
   - Screen shake on player damage.

4. Add better level identity.
   - Level 1: open garden.
   - Level 2: tighter maze/night arena.
   - Different enemy colors per level.

5. Add pause/settings.
   - Pause button top-right.
   - Restart.
   - Main menu.
   - Music/SFX toggles if audio is added.

6. Add object pooling.
   - Projectiles and enemies currently instantiate/destroy.
   - Pooling is better for mobile performance and also useful academically as a design pattern.

7. Add real Android build pass.
   - Switch platform to Android in Unity.
   - Open Device Simulator.
   - Run `Tools -> Motores2 - Setup All Scenes`.
   - Build APK/AAB.
   - Test portrait on a phone or emulator.

## Important Do Not Regress

- Do not reintroduce `Unity.Services.RemoteConfig` unless the package setup is intentionally restored and verified.
- Do not use `Rigidbody2D.linearVelocity` in Unity 2022. Use `Rigidbody2D.velocity`.
- Do not go back to a single scene named `Level`; current flow expects `Level1` and `Level2`.
- Do not manually wire generated scene references unless the setup script failed. The setup script should own generated references.
- Keep `motores2_tp1` synced with script changes if it is used for submission.
