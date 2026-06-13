---
sidebar_position: 3
---

# Scripting Lua : événements NPC et badges

Ce guide montre comment créer le moteur de script, écrire des scripts Lua pour les événements NPC et les badges, et persister l'état du jeu.

## Installer

```bash
dotnet add package PokeForge.SDK.Scripting
```

## Créer le moteur

`LuaScriptEngine` prend un `GameState` et un `SaveSystem`. Injectez les deux depuis votre racine de composition :

```csharp
using PokeForge.SDK.Scripting;

var scriptEngine = new LuaScriptEngine(gameState, saveSystem);
```

## Écrire un script

Les scripts s'exécutent dans un `Preset_SoftSandbox` : les bibliothèques Lua standard sont disponibles, mais `os`, `io` et l'accès réseau sont bloqués. Les scripts interagissent avec le jeu via les globaux `game` et `player` :

```lua
-- award_badge.lua
-- Appelé quand le joueur bat le champion d'arène

if not game.get_flag("gym1_beaten") then
  game.set_flag("gym1_beaten", true)
  player.give_badge(1)
  player.heal_party()
  game.show_dialog("Félicitations ! Vous avez obtenu le Badge Pierre.")
  game.save()
end
```

Exécuter un fichier de script :

```csharp
await scriptEngine.RunFileAsync("scripts/award_badge.lua");
```

## Contraintes de la sandbox

Le `Preset_SoftSandbox` de MoonSharp impose ces limites :

| Bloqué | Autorisé |
|--------|----------|
| `os.*` | `math.*`, `string.*`, `table.*` |
| `io.*` | `game.*`, `player.*`, `sdk.*` |
| Réseau | `coroutine.*` |

Tenter d'appeler un global bloqué lève une `ScriptRuntimeException` que le moteur attrape et journalise.

## Flags GameState

Les flags sont des valeurs `JsonElement` typées stockées dans `GameState.Flags`. L'API Lua les encapsule :

```lua
-- Définir un flag booléen
game.set_flag("npc_talked", true)

-- Définir un flag numérique
game.set_flag("coins_collected", 42)

-- Lire un flag (retourne nil si non défini)
local talked = game.get_flag("npc_talked")
if talked then
  game.show_dialog("Nous avons déjà parlé.")
end
```

Depuis C# vous pouvez lire les flags directement :

```csharp
if (gameState.Flags.TryGetValue("gym1_beaten", out var val)
    && val.GetBoolean())
{
    // le passage est déverrouillé
}
```

## Rechargement à chaud en debug

En mode debug, `LuaHotReloader` surveille les fichiers de script et les ré-exécute à la sauvegarde :

```csharp
#if DEBUG
var hotReloader = new LuaHotReloader(scriptEngine, "scripts/");
hotReloader.Start();  // ré-exécute les scripts modifiés en <500ms
#endif
```

Le rechargement à chaud s'applique uniquement aux scripts sûrs à ré-exécuter (sans effets de bord à la ré-entrée). Les scripts d'attribution de badge doivent être protégés par un flag, comme montré ci-dessus.
