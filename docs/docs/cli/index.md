---
sidebar_position: 1
---

# CLI — pokeforge

`pokeforge` is a global .NET tool for creating and maintaining PokemonSDK projects.

## Install

```bash
dotnet tool install -g PokeForge.CLI
```

Verify:

```bash
pokeforge --version
```

## Commands

| Command | Description |
|---------|-------------|
| [`pokeforge new`](#new) | Scaffold a new project from the SDK starter template |
| [`pokeforge seed`](./seed.md) | Seed the SQLite database with Pokémon data |
| [`pokeforge doctor`](./doctor.md) | Health-check runtime dependencies |
| [`pokeforge asset-sync`](./asset-sync.md) | Validate, pack, and sync sprites |
| [`pokeforge fakemon list-parts`](./fakemon.md) | List available Fakemon parts |
| [`pokeforge fakemon assemble`](./fakemon.md) | Assemble a Fakemon sprite from parts |

## `pokeforge new` {#new}

Scaffolds a complete starter project using the SDK sample as a template.

```bash
pokeforge new MyGame
cd MyGame
dotnet run
```

The generated project references PokemonSDK packages via NuGet (not project references) and includes a pre-configured `data/`, `scripts/`, and `assets/` directory.

## Update

```bash
dotnet tool update -g PokeForge.CLI
```
