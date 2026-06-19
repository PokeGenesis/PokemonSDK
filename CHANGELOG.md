# Changelog

All notable changes to PokemonSDK are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). Versioning follows [SemVer](https://semver.org/) (see D-18).

---

## [0.2.0] - 2026-06-19

### Added

**Phase 13: EXP + Level-up + Evolution (BTLUI-02)**

- `IExpFormula` interface with `CalcExpGain` and `ExpThreshold` methods
- `Gen1ExpFormula`: Generation I EXP gain formula + all 6 growth rate thresholds (MediumFast, MediumSlow, Fast, Slow, Erratic, Fluctuating)
- `Gen5ExpFormula`: Generation V EXP gain formula (cube-root scaling) + all 6 growth rate thresholds
- `GrowthRate` enum extended with `Erratic` and `Fluctuating` values
- `BattleEngine.AwardExp()`: multi-level-up loop with level cap enforcement (`BattleConfig.GetLevelCap()`)
- `BattleEngine.GetMoveLearnQueue()`: returns moves learned on level-up per `PendingLearnedMoves`
- `ExpBar` UI widget: animated EXP gain bar (MonoGame)
- `LevelUpOverlay` UI overlay: level-up stats panel
- `MoveLearnOverlay` UI overlay: move-forget decision screen (cursor navigation, B-cancel)
- `EvolutionOverlay` UI overlay: flash animation + cancel mechanic (B-button during flash)
- `BattleScene` phase 4 (ShowEXP) and phase 5 (ShowLevelUp/ShowMoveLearn/ShowEvolution) state machine
- `PendingEvolution` and `PendingLearnedMoves` value types on `PlayerState`
- `IDifficultyMode` default implementations for `ExpMultiplier` and `CatchRateMultiplier`

### Fixed

- `BattleEngine.AwardExp`: level cap was enforced on entry but not inside the multi-level-up while loop (Finding 9)
- `Gen1ExpFormula.ExpThreshold`: guard `level < 1` now returns 0 to prevent negative EXP thresholds (Finding 5)
- `Gen5ExpFormula.ExpThreshold`: same guard as Gen1 (Finding 5)
- `Gen5ExpFormula.CalcExpGain`: negative inputs now throw `ArgumentOutOfRangeException` (Finding 13)
- `Gen1ExpFormula.ErraticThreshold`: parentheses added around `(1911 - 10*n) / 3` to apply integer floor before multiply — matches Bulbapedia spec
- `Gen1ExpFormula.FluctuatingThreshold`: `(n+1)/3` kept as integer division per spec (float conversion was a regression)
- `Gen5ExpFormula`: same Erratic/Fluctuating parentheses fixes as Gen1
- `MoveLearnOverlay.Reset()`: added to prevent auto-skip of move-learn on the second battle (Finding 3)
- `EvolutionOverlay.Reset()`: added to prevent state leak across battles (Finding 8)
- `BattleScene.LoadBattle`: now resets `_moveLearnOverlay` and `_evolutionOverlay` on battle start

### Changed

- `SDK.Bundle` removed from `PokemonSDK.slnx` build — it is a publish-only meta-package that requires NuGet packages to exist before restore; pack it separately with `dotnet pack src/SDK.Bundle/SDK.Bundle.csproj`

---

## [0.1.0] - 2026-01-01

Initial release. Includes Phases 1–9:

- SDK.Core: `BattleState` (immutable record), `GrowthRate`, `BattleMove`, `PlayerState`, `BattleConfig`
- SDK.Data: EF Core 10 + SQLite, 9-generation schema, translations table (D-07), migrations
- SDK.Battle: `BattleEngine` 1v1, `Gen1ExpFormula`, `Gen5ExpFormula`, `IDamageFormula` ×2, `IDifficultyMode` ×2, sleep/freeze fix (D-11)
- SDK.Scripting: MoonSharp `Preset_SoftSandbox`, `GameState`, `SaveSystem` JSON
- SDK.MonoGame: MonoGame DesktopGL runtime, `BattleScene`, `WorldScene`, HD pipeline 480×270→1920×1080 (D-14), xBR shader (D-15)
- SDK.Plugins: Nuzlocke, Randomizer, Turbo
- SDK.Tools: `SpriteValidator`, asset pipeline, hot reload Lua
- SDK.Cli: `pokeforge` CLI scaffold
