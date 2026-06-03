namespace SDK.Data.Seeding;

using SDK.Core.Entities;
using SDK.Core.Enums;

public static class BattleDataSeeder
{
    public static void SeedAll(PokemonDbContext ctx)
    {
        SeedTypeEffectiveness(ctx);
        SeedMoves(ctx);
        SeedAbilities(ctx);
    }

    public static void SeedTypeEffectiveness(PokemonDbContext ctx)
    {
        if (ctx.TypeEffectiveness.Any()) return;

        // Gen 1 type chart — 15 types (1-15), non-neutral entries only (factor 0/0.5/2).
        // Default for any missing entry = 1.0 (neutral).
        var entries = new (int atk, int def, decimal factor)[]
        {
            // Normal(1)
            (1, 13, 0.5m), (1, 14, 0m),
            // Fire(2)
            (2, 2, 0.5m), (2, 3, 0.5m), (2, 5, 2m), (2, 6, 2m), (2, 12, 2m), (2, 13, 0.5m), (2, 15, 0.5m),
            // Water(3)
            (3, 2, 2m), (3, 3, 0.5m), (3, 5, 0.5m), (3, 9, 2m), (3, 13, 2m),
            // Electric(4)
            (4, 3, 2m), (4, 4, 0.5m), (4, 5, 0.5m), (4, 9, 0m), (4, 10, 2m), (4, 15, 0.5m),
            // Grass(5)
            (5, 2, 0.5m), (5, 3, 2m), (5, 5, 0.5m), (5, 8, 0.5m), (5, 9, 2m),
            (5, 10, 0.5m), (5, 12, 0.5m), (5, 13, 2m), (5, 15, 0.5m),
            // Ice(6)
            (6, 3, 0.5m), (6, 5, 2m), (6, 6, 0.5m), (6, 9, 2m), (6, 10, 2m), (6, 15, 2m),
            // Fighting(7)
            (7, 1, 2m), (7, 6, 2m), (7, 8, 0.5m), (7, 10, 0.5m), (7, 11, 0.5m),
            (7, 12, 0.5m), (7, 13, 2m), (7, 14, 0m),
            // Poison(8)
            (8, 5, 2m), (8, 8, 0.5m), (8, 9, 0.5m), (8, 12, 2m), (8, 13, 0.5m), (8, 14, 0.5m),
            // Ground(9)
            (9, 2, 2m), (9, 4, 0m), (9, 5, 0.5m), (9, 8, 2m), (9, 10, 0m), (9, 12, 0.5m), (9, 13, 2m),
            // Flying(10)
            (10, 4, 0.5m), (10, 5, 2m), (10, 7, 2m), (10, 12, 2m), (10, 13, 0.5m),
            // Psychic(11)
            (11, 7, 2m), (11, 8, 2m), (11, 11, 0.5m), (11, 14, 0m),
            // Bug(12)
            (12, 2, 0.5m), (12, 5, 2m), (12, 7, 0.5m), (12, 10, 0.5m), (12, 8, 2m), (12, 14, 0.5m),
            // Rock(13)
            (13, 2, 2m), (13, 6, 2m), (13, 7, 0.5m), (13, 9, 0.5m), (13, 10, 2m), (13, 12, 2m),
            // Ghost(14)
            (14, 1, 0m), (14, 7, 0m), (14, 8, 2m), (14, 12, 0.5m), (14, 14, 2m),
            // Dragon(15)
            (15, 15, 2m),
        };

        foreach (var (atk, def, factor) in entries)
            ctx.TypeEffectiveness.Add(new TypeEffectiveness
            {
                AttackerTypeId = atk,
                DefenderTypeId = def,
                DamageFactor = factor,
                Generation = 1,
            });

        ctx.SaveChanges();
    }

    public static void SeedMoves(PokemonDbContext ctx)
    {
        if (ctx.Moves.Any()) return;

        // 15 Gen 1 representative moves — Physical/Special using Gen 4+ categorisation.
        ctx.Moves.AddRange(
            new Move { Id =  1, Identifier = "tackle",       TypeId = 1, Category = MoveCategory.Physical, Power =  35, Accuracy =  95, PP = 35, Generation = 1 },
            new Move { Id =  2, Identifier = "growl",        TypeId = 1, Category = MoveCategory.Status,   Power = null, Accuracy = 100, PP = 40, Generation = 1 },
            new Move { Id =  3, Identifier = "scratch",      TypeId = 1, Category = MoveCategory.Physical, Power =  40, Accuracy = 100, PP = 35, Generation = 1 },
            new Move { Id =  4, Identifier = "tail-whip",    TypeId = 1, Category = MoveCategory.Status,   Power = null, Accuracy = 100, PP = 30, Generation = 1 },
            new Move { Id =  5, Identifier = "thundershock", TypeId = 4, Category = MoveCategory.Special,  Power =  40, Accuracy = 100, PP = 30, Generation = 1 },
            new Move { Id =  6, Identifier = "ember",        TypeId = 2, Category = MoveCategory.Special,  Power =  40, Accuracy = 100, PP = 25, Generation = 1 },
            new Move { Id =  7, Identifier = "water-gun",    TypeId = 3, Category = MoveCategory.Special,  Power =  40, Accuracy = 100, PP = 25, Generation = 1 },
            new Move { Id =  8, Identifier = "vine-whip",    TypeId = 5, Category = MoveCategory.Special,  Power =  45, Accuracy = 100, PP = 25, Generation = 1 },
            new Move { Id =  9, Identifier = "flamethrower", TypeId = 2, Category = MoveCategory.Special,  Power =  90, Accuracy = 100, PP = 15, Generation = 1 },
            new Move { Id = 10, Identifier = "thunderbolt",  TypeId = 4, Category = MoveCategory.Special,  Power =  90, Accuracy = 100, PP = 15, Generation = 1 },
            new Move { Id = 11, Identifier = "surf",         TypeId = 3, Category = MoveCategory.Special,  Power =  90, Accuracy = 100, PP = 15, Generation = 1 },
            new Move { Id = 12, Identifier = "solarbeam",    TypeId = 5, Category = MoveCategory.Special,  Power = 120, Accuracy = 100, PP = 10, Generation = 1 },
            new Move { Id = 13, Identifier = "hyper-beam",   TypeId = 1, Category = MoveCategory.Special,  Power = 150, Accuracy =  90, PP =  5, Generation = 1 },
            new Move { Id = 14, Identifier = "swords-dance", TypeId = 1, Category = MoveCategory.Status,   Power = null, Accuracy = 100, PP = 20, Generation = 1 },
            new Move { Id = 15, Identifier = "quick-attack", TypeId = 1, Category = MoveCategory.Physical, Power =  40, Accuracy = 100, PP = 30, Generation = 1 }
        );
        ctx.SaveChanges();
    }

    public static void SeedAbilities(PokemonDbContext ctx)
    {
        if (ctx.Abilities.Any()) return;

        // Gen 3 fundamental abilities (Gen 1 had no abilities mechanic).
        ctx.Abilities.AddRange(
            new Ability { Id = 1, Identifier = "overgrow",   Generation = 3 },
            new Ability { Id = 2, Identifier = "blaze",      Generation = 3 },
            new Ability { Id = 3, Identifier = "torrent",    Generation = 3 },
            new Ability { Id = 4, Identifier = "static",     Generation = 3 },
            new Ability { Id = 5, Identifier = "intimidate", Generation = 3 },
            new Ability { Id = 6, Identifier = "keen-eye",   Generation = 3 }
        );
        ctx.SaveChanges();
    }
}
