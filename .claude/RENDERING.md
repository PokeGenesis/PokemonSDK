# Rendering HD — PokemonSDK

## Principe : résolution interne + upscale entier (D-14)

```
Résolution interne : 480×270   (= 1920÷4 × 1080÷4)
Upscale shader xBR : ×4
Résolution finale  : 1920×1080 — pixels nets garantis

Jamais de résolution interne différente — changer = redessiner tous les assets.
```

---

## Tailles de sprites (D-16 — figées)

| Élément | Taille native | Affiché ×4 | Notes |
|---------|--------------|------------|-------|
| Sprite combat front/back | **96×96** | 384×384 px | Pokémon en combat |
| Sprite overworld | **48×48** | 192×192 px | Joueur, NPC, Pokémon sur map |
| Tile de terrain | **16×16** | 64×64 px | Chunks 16×16 tiles |
| Portrait / boss / splash | **128×128** | 512×512 px | Gym leaders, legendaires |

---

## Pipeline de rendu — 3 passes obligatoires

```csharp
// ═══ Game1.cs ═══

private RenderTarget2D _internalTarget;   // 480×270
private Rectangle      _fullscreenRect;   // 1920×1080
private Effect         _xbrEffect;
private BloomEffect    _bloomEffect;
private DayNightEffect _dayNightEffect;

protected override void LoadContent()
{
    _internalTarget  = new RenderTarget2D(GraphicsDevice, 480, 270);
    _fullscreenRect  = new Rectangle(0, 0, 1920, 1080);
    _xbrEffect       = Content.Load<Effect>("Shaders/xBR");
    _bloomEffect     = new BloomEffect(GraphicsDevice, Content);
    _dayNightEffect  = new DayNightEffect(GraphicsDevice, Content);
}

protected override void Draw(GameTime gameTime)
{
    // ── Passe 1 : rendu monde dans RenderTarget interne (480×270) ──────────
    GraphicsDevice.SetRenderTarget(_internalTarget);
    GraphicsDevice.Clear(Color.Black);

    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);  // pixel-perfect
    _tilemapRenderer.Draw(_spriteBatch, _camera);   // ground → decor → overhead
    _entityRenderer.Draw(_spriteBatch, _camera);    // joueur, NPC, Pokémon
    _spriteBatch.End();

    // ── Passe 2 : upscale xBR ×4 → backbuffer 1920×1080 ────────────────────
    GraphicsDevice.SetRenderTarget(null);
    GraphicsDevice.Clear(Color.Black);

    _spriteBatch.Begin(
        effect:       _xbrEffect,
        samplerState: SamplerState.LinearClamp);    // interpolation xBR
    _spriteBatch.Draw(_internalTarget, _fullscreenRect, Color.White);
    _spriteBatch.End();

    // ── Passe 3 : post-process (conditionnels) ──────────────────────────────
    if (_bloomEnabled)    _bloomEffect.Draw(gameTime);
    if (_dayNightEnabled) _dayNightEffect.Draw(_worldState.TimeOfDay);

    base.Draw(gameTime);
}
```

---

## Shaders — rôles et conditions d'activation

| Shader | Fichier | Activation | Usage |
|--------|---------|------------|-------|
| **xBR** | `Shaders/xBR.fx` | **Toujours** — Passe 2 systématique | Upscaling 480×270 → 1920×1080 |
| **Bloom** | `Shaders/Bloom.fx` | Pendant attaques combat uniquement | Flash, Tonnerre, Psyko, Laser... |
| **DayNight** | `Shaders/DayNight.fx` | En overworld si cycle actif | Tint chaud/froid + saturation selon TimeOfDay |
| **PaletteSwap** | `Shaders/PaletteSwap.fx` | Sur sprite ciblé uniquement | Shinies, Fakemons, couleurs rivaux |

---

## Tilemap — Règles chunk-based

```csharp
// Chunk size : 16×16 tiles — FIGÉ (D-14), ne jamais changer
public const int CHUNK_SIZE = 16;

// Layers ordre strict — toujours dans cet ordre
public enum TilemapLayer
{
    Ground   = 0,  // sol, eau, dalles
    Decor    = 1,  // herbes basses, décorations
    Collision = 2, // tiles bloquantes (invisibles en prod)
    Overhead = 3,  // arbres, toits (au-dessus du joueur)
    Triggers = 4,  // zones déclencheurs Lua (invisibles)
}

// Rendu chunk-based — ne rendre QUE les chunks visibles
public void Draw(SpriteBatch sb, Camera2D camera)
{
    var visibleBounds = camera.GetVisibleBounds();

    foreach (var chunk in _chunks)
    {
        if (!chunk.Bounds.Intersects(visibleBounds)) continue;  // frustum culling

        for (int layer = 0; layer < LAYER_COUNT; layer++)
        {
            if (layer == (int)TilemapLayer.Collision) continue;  // invisible en prod
            DrawChunkLayer(sb, chunk, layer);
        }
    }
}

// INTERDIT : ne jamais itérer tous les tiles de toute la map
foreach (var tile in _allTiles) { }  // ← O(n) sans culling = crash sur grandes maps
```

---

## DayNight — Configuration

```csharp
public enum TimeMode
{
    RealTime,    // horloge PC — 24h = 24h réelles
    InternalClock // horloge interne — vitesse configurable
}

// Transitions de TimeOfDay
// Morning  : 06h00 → 09h59
// Day      : 10h00 → 17h59
// Evening  : 18h00 → 20h59
// Night    : 21h00 → 05h59

// Tints correspondants (passés au shader DayNight.fx)
Morning → Color(255, 220, 180)  // chaud, orangé
Day     → Color(255, 255, 255)  // neutre
Evening → Color(255, 180, 100)  // orangé-rouge
Night   → Color(80,  100, 180)  // bleu foncé, saturation réduite
```

---

## Ressources de référence rendu

- **Bulbapedia damage formula Gen IX** : https://bulbapedia.bulbagarden.net/wiki/Damage
- **MonoGame docs** : https://docs.monogame.net
- **MonoGame.Extended Tiled** : https://monogameextended.net/docs
- **xBR shader HLSL** : https://github.com/libretro/glsl-shaders/tree/master/xbr
- **Tiled map editor** : https://doc.mapeditor.org/en/stable/
