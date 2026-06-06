# Developer Experience — PokemonSDK

## Asset Pipeline (Phase 7 — DX-01)

### Convention de nommage obligatoire (D-16 + D-23)

```
{dexid5}_{identifier}_{view}.png
Views : front | back | overworld | portrait | icon

Tailles figées (D-23) :
  front     → 96×96    (384×384 affiché ×4)  combat ennemi
  back      → 96×96    (384×384 affiché ×4)  combat joueur
  overworld → 48×48    (192×192 affiché ×4)  map + followers
  portrait  → 128×128  (512×512 affiché ×4)  gym leaders, légendaires
  icon      → 32×32    (128×128 affiché ×4)  party, PC box, Pokédex

Exemples :
00025_pikachu_front.png              ← 96×96   combat face
00025_pikachu_back.png               ← 96×96   combat dos
00025_pikachu_overworld.png          ← 48×48   overworld + follower
00025_pikachu_icon.png               ← 32×32   party / PC box
00025_pikachu_shiny_front.png        ← 96×96   shiny (identifier=pikachu_shiny)
00025_pikachu_shiny_icon.png         ← 32×32   shiny icon
00006_charizard_mega_x_front.png     ← 96×96   mega forme
00130_gyarados_portrait.png          ← 128×128 portrait gym leader
route_01.png                         ← WARN    tileset sans dexid
```

Regex D-16 : `^(\d{5})_([a-z0-9_]+)_(front|back|overworld|portrait|icon)\.png$`

### Structure du dossier assets attendue

```
assets/sprites/
├── pokemon/
│   ├── 00025_pikachu_front.png      ← 96×96
│   ├── 00025_pikachu_back.png       ← 96×96
│   ├── 00025_pikachu_overworld.png  ← 48×48
│   ├── 00025_pikachu_icon.png       ← 32×32
│   └── 00025_pikachu_shiny_front.png ← 96×96
├── tiles/
│   ├── route_01.png                 ← tileset 16×16
│   └── cave_01.png
└── portraits/
    └── gym_leader_01.png            ← 128×128
assets/sounds/
└── cries/
    ├── 00025_pikachu.ogg            ← OGG q8 mono 22050Hz
    ├── 00006_charizard.ogg
    └── 00006_charizard_mega_x.ogg
```

### SpriteValidator — règles bloquantes vs warnings

```
ERROR (bloque l'import, exit code 1 en CI) :
  - Taille incorrecte   : front/back ≠ 96×96, overworld ≠ 48×48, portrait ≠ 128×128, icon ≠ 32×32
  - Canal alpha absent  : PNG sans transparence (color type ≠ 4 ou 6)
  - Fichier corrompu    : PNG non lisible / signature invalide

WARNING (import mais signalé dans import.json) :
  - Nommage non conforme : pikachu_25_front.png au lieu de 00025_pikachu_front.png
  - Sprite dupliqué      : même asset_key deux fois (dernier gagne)
```

### Audio — Cries (D-24)

```
Format    : OGG Vorbis q8, mono, 22050Hz
Nommage   : {dexid5}_{identifier}.ogg
Dossier   : assets/sounds/cries/

00025_pikachu.ogg
00006_charizard.ogg
00006_charizard_mega_x.ogg     ← forme spéciale
00351_castform_rainy.ogg       ← forme météo
Shinies → MÊME cry (pas de fichier séparé)
Musique BGM + SFX → Phase 6+, format non encore décidé
```

### Flux complet `pokeforge asset-sync`

```
1. SpriteScanner    → liste tous les PNG dans assets/sprites/ récursivement
2. SpriteValidator  → valide chaque PNG (taille, nommage, alpha)
3. AtlasPacker      → génère Content/atlas.json + Content/sprites.mgcb
4. SqliteSyncer     → parse asset_key depuis nommage → UPDATE pokemon_forms SET asset_key
5. Rapport          → import.json (OK / WARN / ERROR par fichier)

Commandes :
pokeforge asset-sync      # flux complet
pokeforge asset-validate  # validation seule (CI-friendly, exit 1 si ERROR)
pokeforge asset-report    # affiche import.json formaté en console
```

### Intégration CI (GitHub Actions)

```yaml
# Ajouter dans ci.yml après le build
- name: Validate sprites
  run: dotnet run --project src/SDK.Tools -- asset-validate
  # exit code 1 si ERROR → PR bloquée automatiquement
```

---

## Hot Reload Lua (Phase 7 — DX-02)

### LuaHotReloader — `#if DEBUG` uniquement

```csharp
// SDK.Tools/LuaDevTools/LuaHotReloader.cs
public sealed class LuaHotReloader : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private readonly LuaScriptEngine   _engine;
    private readonly TimeSpan          _debounce = TimeSpan.FromMilliseconds(200);
    private DateTime                   _lastReload = DateTime.MinValue;

    public LuaHotReloader(LuaScriptEngine engine, string scriptsPath)
    {
        _engine  = engine;
        _watcher = new FileSystemWatcher(scriptsPath, "*.lua")
        {
            EnableRaisingEvents = true,
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite
        };
        _watcher.Changed += OnScriptChanged;
    }

    private void OnScriptChanged(object sender, FileSystemEventArgs e)
    {
        // Anti-bounce : ignorer les événements < 200ms
        if (DateTime.Now - _lastReload < _debounce) return;
        _lastReload = DateTime.Now;

        // Rechargement ciblé — GameState et flags préservés
        _engine.Reload(e.FullPath);
        Console.WriteLine($"[HotReload] {Path.GetFileName(e.FullPath)} rechargé");
    }

    public void Dispose() => _watcher.Dispose();
}

// Activation dans Game1 uniquement en DEBUG
#if DEBUG
_hotReloader = new LuaHotReloader(_scriptEngine, "data/scripts/");
#endif
```

### LuaErrorOverlay — affichage ingame

```
┌──────────────────────────────────────────────────────┐
│ ⚠  LUA ERROR                                         │
│ Fichier : data/scripts/npcs/gym_leader_1.lua         │
│ Ligne   : 23                                         │
│ Erreur  : attempt to index a nil value (global       │
│           'player')                                  │
│                                                      │
│ [R] Recharger après correction   [Esc] Ignorer       │
└──────────────────────────────────────────────────────┘
```

```csharp
// LuaErrorOverlay.cs — rendu MonoGame #if DEBUG
public sealed class LuaErrorOverlay
{
    private LuaError? _currentError;

    public void Show(LuaError error) => _currentError = error;
    public void Hide() => _currentError = null;

    public void Draw(SpriteBatch sb, SpriteFont font)
    {
        if (_currentError is null) return;

        // Fond semi-transparent
        sb.Draw(_pixel, _overlayBounds, new Color(0, 0, 0, 200));

        // Texte erreur
        sb.DrawString(font, $"⚠ LUA ERROR", pos, Color.OrangeRed);
        sb.DrawString(font, $"Fichier : {_currentError.File}", pos + lineOffset, Color.White);
        sb.DrawString(font, $"Ligne   : {_currentError.Line}", pos + lineOffset * 2, Color.White);
        sb.DrawString(font, _currentError.Message, pos + lineOffset * 3, Color.Yellow);
    }
}
```

---

## Console Lua REPL (Phase 7 — DX-02)

```
Touche ~ → toggle console ingame (DEBUG uniquement)

Exemples de commandes disponibles :
> player.position()                        → "Zone: route_01, X: 320, Y: 240"
> player.teleport("zone_02", 100, 100)     → téléporte le joueur
> flags.set("gym_1_defeated", true)        → set un flag
> flags.get("gym_1_defeated")              → "true"
> battle.start({ trainer_id = 1 })         → démarre un combat
> pokemon.spawn("pikachu", level=5)        → spawn un Pokémon sauvage
> time.set("evening")                      → change le TimeOfDay
> weather.set("rain")                      → change la météo
> reload("npcs/gym_leader_1.lua")          → recharge un script spécifique
```

```csharp
// LuaConsole.cs
public sealed class LuaConsole
{
    private bool          _visible  = false;
    private string        _input    = "";
    private List<string>  _history  = new(50);
    private List<string>  _output   = new(20);
    private int           _histIdx  = -1;

    public void Toggle() => _visible = !_visible;

    public void HandleKeyInput(Keys key, char? character)
    {
        if (!_visible) return;

        if (key == Keys.Enter && !string.IsNullOrWhiteSpace(_input))
        {
            Execute(_input);
            _history.Insert(0, _input);
            _input = "";
            _histIdx = -1;
        }
        else if (key == Keys.Up && _history.Count > 0)
        {
            _histIdx = Math.Min(_histIdx + 1, _history.Count - 1);
            _input = _history[_histIdx];
        }
        // ... gestion caractères, backspace, etc.
    }

    private void Execute(string command)
    {
        try
        {
            var result = _engine.Eval(command);
            _output.Add($"> {command}");
            _output.Add($"  → {result ?? "nil"}");
        }
        catch (ScriptRuntimeException ex)
        {
            _output.Add($"> {command}");
            _output.Add($"  ❌ {ex.DecoratedMessage}");
        }
    }
}
```

---

## Règles DX (NON NÉGOCIABLES)

1. `SDK.Tools` ne référence **jamais** `SDK.MonoGame` — tourne en CI headless (D-17)
2. Hot reload et console REPL uniquement `#if DEBUG` — zero overhead en prod
3. `SpriteValidator` : exit code 1 si ERROR — intégrable dans GitHub Actions
4. `import.json` machine-readable — pour intégration future dans l'éditeur Avalonia
5. `LuaScriptEngine.Reload(path)` : rechargement **ciblé**, GameState préservé
6. Overlay erreur Lua : jamais un crash silencieux — toujours fichier + ligne + message
