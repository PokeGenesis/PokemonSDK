---
sidebar_position: 4
---

# Étape 3 : Script Lua et badge

## Créer le script

Créez `scripts/gym_first.lua` dans votre projet :

```lua
-- Attribuer le premier badge et enregistrer la progression
game.give_badge("Badge Herbier")
game.set_flag("GYM1_DONE", true)
game.save()
```

Le global `game` est exposé par `LuaScriptEngine`. Il s'exécute dans `Preset_SoftSandbox` : pas d'accès au système de fichiers, pas de réseau, pas d'appels OS.

## Brancher dans Game1.cs

Ajoutez le code suivant après le bloc de combat de l'Étape 2 :

```csharp
using PokeForge.SDK.Scripting;

// Créer l'engine avec le GameState courant et le SaveSystem
var scriptEngine = new LuaScriptEngine(gameState, saveSystem);

// Exécuter le script de la salle
scriptEngine.ExecuteFile("scripts/gym_first.lua");

// Relire le flag pour confirmer la persistance
bool gymCleared = gameState.Flags.TryGetValue("GYM1_DONE", out var v)
    && v.GetBoolean();

Console.WriteLine($"GYM1_DONE :  {gymCleared}");
Console.WriteLine($"Badges :     {string.Join(", ", gameState.Badges)}");
```

`gameState` et `saveSystem` viennent du DI. Le `Program.cs` généré par le scaffold les enregistre déjà.

## Lancer

```bash
dotnet run -- --headless
```

Sortie attendue :

```
Vainqueur : Bulbasaur
Tours :     7
GYM1_DONE :  True
Badges :     Badge Herbier
```

Vérifiez que le fichier de sauvegarde a été créé :

```bash
ls saves/
# default.json
```

Contenu de `saves/default.json` :

```json
{
  "Flags": { "GYM1_DONE": true },
  "Badges": ["Badge Herbier"],
  "Money": 0
}
```

:::note
`JsonSaveSystem` écrit dans `saves/{slot}.json` dans le dossier de données de l'application. Le slot par défaut est `"default"`.
:::

## Et ensuite ?

Vous avez un fan game fonctionnel : scaffold, combat, badge Lua, sauvegarde.

Pour aller plus loin :

| Sujet | Guide |
|-------|-------|
| Système de combat en profondeur | [Package Battle](../packages/battle) |
| Patterns de scripting Lua | [Package Scripting](../packages/scripting) |
| Plugins (Nuzlocke, Randomizer) | [Plugins](../packages/plugins) |
| Référence des commandes CLI | [CLI pokeforge](../cli) |
