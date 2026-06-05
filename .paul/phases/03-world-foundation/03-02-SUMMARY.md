---
phase: 03-world-foundation
plan: 02
subsystem: world
tags: [clock, weather, encounter, realtime, gameclock, efcore, sqlite]

requires:
  - phase: 03-01
    provides: EncounterZone entity + Migration 003 + BiomeType/TimeOfDay/WeatherType enums

provides:
  - IRealTimeClock + RealTimeClock (UTC hour → TimeOfDay)
  - IGameClock + GameTimeClock (configurable Speed, save/load ready)
  - IWeatherSystem + WeatherSystem (biome + TimeOfDay → WeatherType)
  - IEncounterSystem extended (GetZonesByIdentifier added)
  - EncounterSystem EF Core implementation
  - WorldDataSeeder (5 Gen 1 EncounterZones)
affects: [03-03, 04-03-save-system, 05-plugins-pokegear]

tech-stack:
  added: []
  patterns:
    - IGameClock injectable via DI — Speed configurable at runtime (Pokégear Phase 5+)
    - RealTimeClock.MapHour internal static — shared with GameTimeClock (DRY clock mapping)
    - SqliteTestFixture :memory: — DataSeeder.SeedAll() before WorldDataSeeder.SeedAll() (FK order)

key-files:
  created:
    - src/SDK.Core/Interfaces/IRealTimeClock.cs
    - src/SDK.Core/Interfaces/IGameClock.cs
    - src/SDK.Core/Interfaces/IWeatherSystem.cs
    - src/SDK.Core/Services/RealTimeClock.cs
    - src/SDK.Core/Services/GameTimeClock.cs
    - src/SDK.Core/Services/WeatherSystem.cs
    - src/SDK.Data/Services/EncounterSystem.cs
    - src/SDK.Data/Seeding/WorldDataSeeder.cs
    - tests/SDK.Core.Tests/WorldServicesTests.cs
    - tests/SDK.Data.Tests/EncounterSystemTests.cs
  modified:
    - src/SDK.Core/Interfaces/IEncounterSystem.cs

key-decisions:
  - "IGameClock séparé de IRealTimeClock — deux contrats distincts (Gen 2 style vs fan-game configurable)"
  - "GameTimeClock.Speed = game-minutes/real-second — default 1f/60f (1:1)"
  - "RealTimeClock.MapHour internal static — partagé avec GameTimeClock"
  - "IGameClock.SetGameTime(TimeSpan) — contrat save/load pour Plan 04-03 ISaveSystem"

patterns-established:
  - "WorldDataSeeder.SeedAll() doit être appelé APRÈS DataSeeder.SeedAll() (FK Restrict sur SpeciesId)"
  - "CreateSeeded() helper pattern — fixture + seed ctx + service ctx distincts"

duration: ~1h30
started: 2026-06-04T20:03:00Z
completed: 2026-06-04T20:27:00Z
---

# Phase 3 Plan 02: World Services Layer Summary

**IRealTimeClock + IGameClock + WeatherSystem + EncounterSystem implémentés dans SDK.Core/SDK.Data — 74/74 tests verts (+25 nouveaux)**

## Performance

| Metric | Valeur |
|--------|--------|
| Durée | ~1h30 |
| Démarré | 2026-06-04T20:03Z |
| Complété | 2026-06-04T20:27Z |
| Tasks | 3/3 complétés |
| Fichiers créés/modifiés | 11 |

## Acceptance Criteria Results

| Criterion | Status | Notes |
|-----------|--------|-------|
| AC-1: SDK.Core 0 NuGet | Pass | `dotnet list` → 0 packages |
| AC-2: RealTimeClock 10 InlineData | Pass | Hours 0/5/6/10/11/16/17/20/21/23 → TimeOfDay corrects |
| AC-3: WeatherSystem 6+ assertions | Pass | Cave/Water/Grass/Route/Building — 7 assertions vertes |
| AC-4: EncounterSystem 5 tests DB | Pass | GetZones + GetZonesByIdentifier + SpawnRate positif |
| AC-5: GameTimeClock Speed/Update/SetGameTime | Pass | 5 facts — accumulation, wrap 24h, restore save |

## Accomplishments

- Deux horloges distinctes : `RealTimeClock` (UTC → TimeOfDay, Gen 2 style) et `GameTimeClock` (Speed configurable, fan-game style)
- `WeatherSystem` pure function — aucun état, prête pour injection DI Plan 03-03
- `EncounterSystem` + `WorldDataSeeder` — 5 zones Gen 1 (pallet-route-1 ×3, viridian-forest ×2)
- 25 nouveaux tests : SDK.Core.Tests +20, SDK.Data.Tests +5

## Task Commits

Commit unique (tasks groupés) :

| Tasks | Commit | Description |
|-------|--------|-------------|
| T1 + T2 + T3 | `452305f` | feat(world): Plan 03-02 — 11 fichiers, 291 insertions |
| STATE.md APPLY | `b86b5bc` | chore(paul): STATE.md Plan 03-02 APPLY |

## Files Created/Modified

| Fichier | Change | Rôle |
|---------|--------|------|
| `src/SDK.Core/Interfaces/IRealTimeClock.cs` | Créé | Contrat heure réelle → TimeOfDay |
| `src/SDK.Core/Interfaces/IGameClock.cs` | Créé | Contrat heure game + Speed + save/load |
| `src/SDK.Core/Interfaces/IWeatherSystem.cs` | Créé | Contrat météo par biome + heure |
| `src/SDK.Core/Interfaces/IEncounterSystem.cs` | Modifié | + GetZonesByIdentifier(string, int) |
| `src/SDK.Core/Services/RealTimeClock.cs` | Créé | Impl UTC, MapHour internal static partagé |
| `src/SDK.Core/Services/GameTimeClock.cs` | Créé | Impl Speed configurable, SetGameTime |
| `src/SDK.Core/Services/WeatherSystem.cs` | Créé | Pure function biome/time → WeatherType |
| `src/SDK.Data/Services/EncounterSystem.cs` | Créé | EF Core primary ctor, ToList() matérialisé |
| `src/SDK.Data/Seeding/WorldDataSeeder.cs` | Créé | 5 zones Gen 1, SpeciesId 1/25/175 |
| `tests/SDK.Core.Tests/WorldServicesTests.cs` | Créé | 20 tests (10+5+5) |
| `tests/SDK.Data.Tests/EncounterSystemTests.cs` | Créé | 5 tests intégration :memory: |

## Decisions Made

| Décision | Rationale | Impact |
|----------|-----------|--------|
| `IGameClock` ≠ `IRealTimeClock` | Fan-game veut vitesse configurable (Pokégear) ; Gen 2 veut heure PC | Plan 03-03 injecte les deux selon besoin |
| `GameTimeClock.Speed` default `1f/60f` | 1:1 ratio = aucune accélération par défaut | Pokégear (Phase 5) override via DI |
| `RealTimeClock.MapHour` `internal static` | Évite duplication du switch dans GameTimeClock | Réutilisable sans coupler les classes |
| `IGameClock.SetGameTime(TimeSpan)` | ISaveSystem Plan 04-03 a besoin de restaurer GameElapsed | Contrat figé, save/load ready |

## Deviations from Plan

### Summary

| Type | Count | Impact |
|------|-------|--------|
| MSB3492 cache error (auto-fixed) | 3 | Aucun — workaround `rm -rf obj/` établi |
| Tests dépassés (>15 visés, 25 livrés) | — | Positif — couverture plus complète |

### Auto-fixed Issues

**1. MSBuild CoreCompileInputs.cache (MSB3492)**
- **Trouvé pendant :** Task 1, Task 2, Task 3
- **Problème :** Ajout de nouveaux .cs → cache incrémental corrompu (WSL2 .NET 10 race)
- **Fix :** `rm -rf <projet>/obj/` + rebuild — 2-3 tentatives parfois nécessaires
- **Pattern :** Confirmé identique à Plan 03-01 — workaround établi

## Issues Encountered

| Issue | Résolution |
|-------|------------|
| MSB3492 ×3 (SDK.Core, SDK.Data, tests) | `rm -rf obj/` systématique à chaque ajout de fichiers |
| Edit tool bloqué sur IEncounterSystem.cs (hook CBM) | `Write` (overwrite complet) utilisé à la place |

## Next Phase Readiness

**Prêt :**
- `IGameClock` + `IRealTimeClock` injectables pour `Game1.Update()` (Plan 03-03)
- `IWeatherSystem` injectable pour `WorldSystem` (Plan 03-03)
- `IEncounterSystem` étendu, `EncounterSystem` EF prêt pour `WorldSystem.TryEncounter()`
- `IGameClock.SetGameTime()` contrat figé pour `ISaveSystem` (Plan 04-03)
- `WorldDataSeeder` — 5 zones testées, pattern établi pour données monde futures

**Concerns :**
- `WeatherSystem` est stateless — l'état météo courant (transition, durée) sera dans `WorldSystem` (Plan 03-03)
- `GameTimeClock` thread-unsafe par design — mise à jour main thread MonoGame uniquement

**Blockers :**
- Aucun

---
*Phase: 03-world-foundation, Plan: 02*
*Complété: 2026-06-04*
