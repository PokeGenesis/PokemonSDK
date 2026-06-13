---
sidebar_position: 1
---

# Tutorial: 30 Minutes to a Working Fan Game

This tutorial walks you from zero to a fan game that runs a real battle and awards a badge via Lua.

Estimated time: 30 minutes.

## What You Will Build

- A scaffolded fan game project using the `pokeforge` CLI
- A 1v1 battle between two Pokémon resolved programmatically
- A Lua script that awards a badge and persists a save file

## Prerequisites

| Requirement | Version |
|-------------|---------|
| .NET SDK | 10+ |
| SQLite | 3.x |
| pokeforge CLI | latest |
| Terminal | any |

Install the CLI:

```bash
dotnet tool install -g PokeForge.CLI
```

Verify:

```bash
pokeforge --version
```

## Steps

| Step | What You Do | Time |
|------|-------------|------|
| [Step 1: Create a Project](tutorial/create) | Scaffold, seed, run headless | 10 min |
| [Step 2: First Battle](tutorial/battle) | Wire BattleEngine, run a 1v1 loop | 10 min |
| [Step 3: Lua Script and Badge](tutorial/lua-badge) | Write a Lua script, award a badge, save | 10 min |

Start with [Step 1](tutorial/create).
