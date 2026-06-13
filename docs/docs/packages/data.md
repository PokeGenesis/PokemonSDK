---
sidebar_position: 3
---

# PokeForge.SDK.Data

EF Core 10 + SQLite data layer — migrations, 9-generation seed data, central translations table.

```bash
dotnet add package PokeForge.SDK.Data
```

## Setup

```csharp
// Register in DI
services.AddDbContext<PokemonDbContext>(options =>
    options.UseSqlite(Configuration.GetConnectionString("Default")));

// Apply migrations + seed on startup
await using var scope = app.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<PokemonDbContext>();
await db.Database.MigrateAsync();
```

Or from the CLI:

```bash
pokeforge seed --db data/PokemonSDK.db
```

## Migrations

Run from the repository root:

```bash
dotnet ef migrations add InitialCreate \
  --project src/SDK.Data \
  --startup-project src/SDK.MonoGame

dotnet ef database update \
  --project src/SDK.Data \
  --startup-project src/SDK.MonoGame
```

## Translations

All user-visible text is stored in a central `translations` table — never as columns on entities (no `name_fr` column on `Species`). This lets you add locales without a schema migration.

```csharp
// Fetch a Pokémon name in French
var name = await db.Translations
    .Where(t => t.EntityType == "Species"
             && t.EntityId == 25
             && t.Locale == "fr")
    .Select(t => t.Value)
    .FirstOrDefaultAsync();
```

Six locales are seeded out-of-the-box: **en, es, fr, de, it, ja**.

## Seeded data

| Table | Content |
|-------|---------|
| `species` | All species, generations 1–9 |
| `moves` | All moves |
| `items` | All items |
| `abilities` | All abilities |
| `translations` | Names + descriptions in 6 locales |

## Filtering by generation

```csharp
// All Gen-1 species with French name
var gen1 = await db.Species
    .Where(s => s.Generation == 1)
    .Include(s => s.Translations.Where(t => t.Locale == "fr"))
    .ToListAsync();
```

## Fakemon

Custom species (Fakemons) share the same `species` table. Use `pokeforge seed` with a custom JSON manifest or the `FakemonDataSeeder` class directly:

```csharp
var seeder = new FakemonDataSeeder(db);
await seeder.SeedAsync("data/fakemons.json");
```
