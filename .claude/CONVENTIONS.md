# Conventions — PokemonSDK

## Nommage C#

```csharp
// Tables SQLite : snake_case pluriel
// pokemon_species, pokemon_forms, translations, type_effectiveness

// Classes/Records/Interfaces : PascalCase
public class PokemonSpecies { }
public record BattleState { }
public interface IBattleEngine { }
public interface IDamageFormula { }

// Events : verbe au passé
public record PokemonFaintedEvent(BattlePokemon Pokemon, BattleSlot Slot);
public record BadgeEarnedEvent(int GymLeaderId, string BadgeName);
public record MoveUsedEvent(Move Move, Pokemon User, Pokemon Target);

// Enums
public enum DifficultyMode  { Story, Hard }
public enum WeatherType     { None, Rain, Sun, Sandstorm, Hail, Snow }
public enum TimeOfDay       { Morning, Day, Evening, Night }
public enum MoveCategory    { Physical, Special, Status }  // Gen 4+ uniquement
public enum NonVolatileStatus { None, Burn, Paralysis, Poison, BadPoison, Sleep, Freeze }

// Fichiers : un fichier = une classe/record/interface
// src/SDK.Battle/Engine/BattleState.cs → public record BattleState
// Tests miroir : tests/SDK.Battle.Tests/Engine/BattleStateTests.cs
```

---

## BattleState — Immuabilité stricte (D-05)

```csharp
// CORRECT — record immuable avec init-only properties
public record BattleState
{
    public BattlePokemon Active   { get; init; }
    public BattlePokemon Opponent { get; init; }
    public WeatherType   Weather  { get; init; }
    public int           Turn     { get; init; }
    public bool          IsOver   { get; init; }
}

// CORRECT — créer un nouvel état avec `with`
var nextState = currentState with
{
    Turn    = currentState.Turn + 1,
    Weather = WeatherType.Rain
};

// INTERDIT — jamais de mutation directe
currentState.Turn++;            // ← compile error avec record init-only
currentState.Weather = Rain;    // ← idem

// INTERDIT — jamais de setters publics
public WeatherType Weather { get; set; }  // ← jamais set; sur BattleState
```

---

## MonoGame — Performance (critique)

```csharp
// ═══ RÈGLE 1 : ZÉRO allocation dans Update() ou Draw() ═══

// INTERDIT
protected override void Draw(GameTime gt)
{
    var sprites = new List<Sprite>();        // ← allocation par frame = GC hell
    var pos     = new Vector2(x, y);        // ← utiliser ref struct ou cache
}

// CORRECT — pré-allouer en LoadContent()
private readonly List<Sprite>        _visibleSprites = new(256);
private readonly ObjectPool<Particle> _particlePool;
private Vector2 _cachedPos;

// ═══ RÈGLE 2 : SpriteBatch — un seul Begin/End par frame ═══

// CORRECT
_spriteBatch.Begin(
    blendState:   BlendState.AlphaBlend,
    samplerState: SamplerState.PointClamp,   // pixel-perfect en passe interne
    effect:       null);
DrawWorld();
DrawEntities();
DrawUI();
_spriteBatch.End();

// INTERDIT — plusieurs Begin/End sans raison
_spriteBatch.Begin(); DrawWorld();  _spriteBatch.End();
_spriteBatch.Begin(); DrawUI();     _spriteBatch.End();  // ← 2 flush GPU inutiles

// ═══ RÈGLE 3 : ZÉRO requête DB dans Draw() ═══

// INTERDIT dans Draw() ou Update()
var pokemon = _db.PokemonSpecies.Where(...).ToList();  // ← DB IO dans le game loop

// CORRECT — charger en LoadContent() ou OnZoneChange()
protected override void LoadContent()
{
    _zoneEncounters = _encounterRepo.GetByZone(_currentZone);
}
```

---

## Lua Scripting — Sandbox (D-04)

```lua
-- API exposée via whitelist (dialog, battle, flags, items, player)
-- Tout le reste est bloqué par Preset_SoftSandbox

-- ✅ Script NPC type
function on_interact(player, npc)
    dialog.show("Prêt pour le combat, dresseur ?")
    if battle.start({ trainer_id = npc.trainer_id }) then
        if npc.is_gym_leader then
            flags.set("gym_" .. npc.id .. "_defeated", true)
            player.award_badge(npc.badge_id)
        end
    end
end

-- ✅ Script zone trigger
function on_zone_enter(player, zone)
    if flags.get("legendary_unlocked") then
        dialog.show("Un Pokémon légendaire rôde ici...")
    end
end

-- ❌ Ces appels DOIVENT lever ScriptRuntimeException en SoftSandbox
os.exit()                  -- interdit
io.open("data.txt", "r")   -- interdit
require("os")              -- interdit
loadfile("evil.lua")       -- interdit

-- Convention fichiers scripts
-- data/scripts/npcs/gym_leader_1.lua
-- data/scripts/zones/route_01.lua
-- data/scripts/events/legendary_spawn.lua
```

---

## Tests — Conventions

```csharp
// ═══ Convention nommage : NomClasse_Scenario_ResultatAttendu ═══
[Fact]
public void DamageCalculator_Gen4SpecialMove_DifferentFromGen1Formula() { }

[Fact]
public void BattleEngine_PoisonStatus_DealsDamageEachTurn() { }

[Fact]
public void LuaEngine_OsExitCall_ThrowsScriptRuntimeException() { }

// ═══ FluentAssertions — obligatoires, jamais Assert.Equal ═══
result.Should().Be(42);
pokemon.Should().NotBeNull();
list.Should().HaveCount(6).And.Contain(x => x.Name == "Pikachu");
action.Should().Throw<ScriptRuntimeException>();
state.Weather.Should().Be(WeatherType.Rain);

// ═══ Fixtures SQLite pour les tests Data ═══
public class SqliteTestFixture : IAsyncLifetime
{
    public PokemonDbContext Context { get; private set; }

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<PokemonDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;
        Context = new PokemonDbContext(options);
        await Context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync() => await Context.DisposeAsync();
}

// ═══ Cibles de coverage ═══
// SDK.Battle   → >90% (moteur critique)
// SDK.Scripting → >80%
// SDK.Data     → >70%
// SDK.Core     → >60% (domaine pur, peu de logique)

// ═══ SeededRandom — pour tests déterministes ═══
// Prévoir dès Phase 2 pour Battle + futur Randomizer plugin
public sealed class SeededRandom : IRandom
{
    private readonly Random _rng;
    public SeededRandom(long seed) => _rng = new Random((int)seed);
    public int Next(int min, int max) => _rng.Next(min, max);
    public double NextDouble() => _rng.NextDouble();
}
```

---

## Nommage des fichiers

```
src/SDK.Battle/Engine/BattleState.cs          → public record BattleState
src/SDK.Battle/Engine/BattleEngine.cs         → public sealed class BattleEngine
src/SDK.Battle/Formulas/IDamageFormula.cs     → public interface IDamageFormula
src/SDK.Battle/Formulas/Gen13DamageFormula.cs → public sealed class Gen13DamageFormula
tests/SDK.Battle.Tests/Engine/BattleEngineTests.cs
tests/SDK.Battle.Tests/Formulas/DamageFormulaTests.cs
data/scripts/npcs/gym_leader_1.lua
data/scripts/zones/route_01.lua
data/maps/zone_01.tmx
assets/sprites/pokemon/00025_pikachu_front.png   ← 96×96
assets/sprites/pokemon/00025_pikachu_overworld.png ← 48×48
assets/sprites/tiles/route_01.png                ← 16×16 tileset
```
