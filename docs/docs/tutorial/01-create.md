---
sidebar_position: 2
---

# Step 1: Create a Project

## Scaffold

Run the `pokeforge new` command to create a fan game project:

```bash
pokeforge new MonJeu
```

This creates the following structure:

```
MonJeu/
  MonJeu.csproj        # References PokeForge.SDK.* NuGet packages
  Game1.cs             # Entry point: MonoGame Game class
  Program.cs           # DI composition root
  scripts/             # Lua scripts go here
  assets/              # Sprites, tilemaps, sounds
```

## Seed the Database

Navigate into the project folder and seed the Pokémon database:

```bash
cd MonJeu && pokeforge seed
```

This fills `PokemonSDK.db` with all 9 generations of Pokémon, moves, abilities, and type charts.

Expected output:

```
Seeding generation 1... OK (151 species)
Seeding generation 2... OK (100 species)
...
Seeding generation 9... OK (103 species)
Seed complete. 1010 species total.
```

## Check Project Health

Run the doctor command to confirm everything is wired correctly:

```bash
pokeforge doctor
```

Expected output (all green):

```
[OK] .NET SDK 10.x found
[OK] PokemonSDK.db exists and is readable
[OK] EF Core migrations applied (8 tables)
[OK] pokeforge CLI version matches SDK
```

If any item shows `[ERROR]`, the output includes a fix hint. The most common issue is a missing database: run `pokeforge seed` again.

## Run Headless

Start the project in headless mode (no graphics window):

```bash
dotnet run -- --headless
```

Expected output:

```
[PokemonSDK] Headless mode active
[World] Loaded: 0 tilemaps
[Game] Loop started
```

:::tip
To launch with a graphics window, omit `--headless`. This requires SDL2 and an OpenGL-capable GPU. For this tutorial, headless is enough.
:::

Next: [Step 2: First Battle](./battle)
