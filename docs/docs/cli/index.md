---
sidebar_position: 1
---

# CLI pokeforge

`pokeforge` is a global .NET tool for PokemonSDK projects.

```bash
dotnet tool install -g PokeForge.CLI
```

## Commands

| Command | Description |
|---------|-------------|
| `pokeforge new` | Scaffold a new StarterGame project |
| `pokeforge seed` | Populate the SQLite database |
| `pokeforge doctor` | Health check (SDL, piper, aplay) |
| `pokeforge asset-sync` | Validate + pack + sync sprites |
| `pokeforge fakemon list-parts` | List available Fakemon parts |
| `pokeforge fakemon assemble` | Assemble a Fakemon sprite |
