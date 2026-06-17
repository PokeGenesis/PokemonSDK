namespace SDK.MonoGame.Scenes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDK.Core.Enums;
using SDK.Core.ValueObjects;
using SDK.MonoGame.World;

public sealed class WorldScene : IGameScene
{
    private readonly WorldSystem _world;
    private readonly PlayerSystem _player;
    private readonly BattleScene _battleScene;
    private readonly Game1 _game1;
    private KeyboardState _prevKs;
    private WipeTransition? _wipe;

    public WorldScene(WorldSystem world, PlayerSystem player, BattleScene battleScene, Game1 game1)
    {
        _world       = world;
        _player      = player;
        _battleScene = battleScene;
        _game1       = game1;
    }

    public void Update(GameTime gameTime)
    {
        var ks = Keyboard.GetState();

#if DEBUG
        void StartBattle(BattleState state)
        {
            if (_wipe is not null) return;
            _wipe = new WipeTransition(_game1.GraphicsDevice, 15, () =>
            {
                _battleScene.LoadBattle(state);
                _game1.SwitchToScene(_battleScene);
                _wipe = null;
            });
        }

        // F1 — EXP bar animation sans level-up
        if (ks.IsKeyDown(Keys.F1) && !_prevKs.IsKeyDown(Keys.F1))
            StartBattle(ScenarioExpAnimation());

        // F2 — Level-up + auto-learn (< 4 moves)
        if (ks.IsKeyDown(Keys.F2) && !_prevKs.IsKeyDown(Keys.F2))
            StartBattle(ScenarioLevelUpAutoLearn());

        // F3 — Level-up + MoveLearnOverlay (4 moves pleins)
        if (ks.IsKeyDown(Keys.F3) && !_prevKs.IsKeyDown(Keys.F3))
            StartBattle(ScenarioMoveLearnOverlay());

        // F4 — Level cap bloqué (0 badge → cap Lv13, BW2 preset)
        if (ks.IsKeyDown(Keys.F4) && !_prevKs.IsKeyDown(Keys.F4))
            StartBattle(ScenarioLevelCap());

        // F5 — Multi level-up (3 niveaux, 2 moves auto-appris)
        if (ks.IsKeyDown(Keys.F5) && !_prevKs.IsKeyDown(Keys.F5))
            StartBattle(ScenarioMultiLevelUp());

        // F6 — HP lerp test : adversaire HP plein, lerp animation visible
        if (ks.IsKeyDown(Keys.F6) && !_prevKs.IsKeyDown(Keys.F6))
            StartBattle(ScenarioHpLerp());

        // F7 — Evolution test : BULBASAUR Lv5 → Lv6 avec évolution vers IVYSAUR (annulable par X)
        if (ks.IsKeyDown(Keys.F7) && !_prevKs.IsKeyDown(Keys.F7))
            StartBattle(ScenarioEvolution());
#endif
        _wipe?.Update();
        _prevKs = ks;

        _world.Update(gameTime.ElapsedGameTime);
        _player.Update();
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        _wipe?.Draw(spriteBatch);
    }

#if DEBUG
    // ────────────────────────────────────────────────────────────────────────
    // F1 : EXP bar animée, pas de level-up
    // Player Lv10 MediumFast, 950/1331 EXP (≈71%).
    // Adversaire donne ≈42 EXP → total 992, reste sous Lv11 (1331).
    // Attendu : barre EXP monte en douceur, rien d'autre.
    // ────────────────────────────────────────────────────────────────────────
    private static BattleState ScenarioExpAnimation()
    {
        var tackle = new BattleMove(1, "tackle", 1, MoveCategory.Physical, 40, 100, 35, 35);

        var player = new BattlePokemon(
            SpeciesId: 1, Nickname: "BULBASAUR", Level: 10,
            CurrentHp: 60, MaxHp: 60,
            Attack: 54, Defense: 54, SpecialAttack: 70, SpecialDefense: 70, Speed: 50,
            Type1Id: 12, Type2Id: 4,
            Moves: new[] { tackle },
            CurrentExp: 1235,
            GrowthRate: GrowthRate.MediumFast);

        var opponent = new BattlePokemon(
            SpeciesId: 16, Nickname: "PIDGEY", Level: 10,
            CurrentHp: 1, MaxHp: 40,
            Attack: 40, Defense: 35, SpecialAttack: 35, SpecialDefense: 35, Speed: 56,
            Type1Id: 1, Type2Id: null,
            Moves: new[] { tackle },
            BaseExpYield: 30);

        return new BattleState(player, opponent, Turn: 0, WeatherType.None,
            new BattleConfig(), Log: Array.Empty<string>());
    }

    // ────────────────────────────────────────────────────────────────────────
    // F2 : Level-up + auto-learn (2 moves → slot libre)
    // Player Lv5 MediumFast, 210 EXP (seuil Lv6 = 216, à 6 EXP).
    // Adversaire baseYield=64, Lv5 → gains=45 → total=255 ≥ 216.
    // Learnset : vine-whip au Lv6.
    // Attendu : LevelUpOverlay Lv5→6, "learned vine-whip!" dans le log, pas d'overlay choix.
    // ────────────────────────────────────────────────────────────────────────
    private static BattleState ScenarioLevelUpAutoLearn()
    {
        var tackle   = new BattleMove(1, "tackle",    1,  MoveCategory.Physical, 40,  100, 35, 35);
        var growl    = new BattleMove(2, "growl",     1,  MoveCategory.Status,   null,100, 40, 40);
        var vineWhip = new BattleMove(3, "vine-whip", 12, MoveCategory.Physical, 45,  100, 25, 25);

        var player = new BattlePokemon(
            SpeciesId: 1, Nickname: "BULBASAUR", Level: 5,
            CurrentHp: 45, MaxHp: 45,
            Attack: 49, Defense: 49, SpecialAttack: 65, SpecialDefense: 65, Speed: 45,
            Type1Id: 12, Type2Id: 4,
            Moves: new[] { tackle, growl },
            CurrentExp: 210,
            GrowthRate: GrowthRate.MediumFast,
            FullLearnset: new[] { (6, vineWhip) });

        var opponent = new BattlePokemon(
            SpeciesId: 4, Nickname: "CHARMANDER", Level: 5,
            CurrentHp: 1, MaxHp: 39,
            Attack: 52, Defense: 43, SpecialAttack: 60, SpecialDefense: 50, Speed: 65,
            Type1Id: 10, Type2Id: null,
            Moves: new[] { tackle },
            BaseExpYield: 64);

        return new BattleState(player, opponent, Turn: 0, WeatherType.None,
            new BattleConfig(), Log: Array.Empty<string>());
    }

    // ────────────────────────────────────────────────────────────────────────
    // F3 : Level-up + MoveLearnOverlay (4 moves pleins)
    // Même EXP/learnset que F2, mais 4 moves déjà présents.
    // Attendu : LevelUpOverlay → MoveLearnOverlay "Forget which? (X=cancel)".
    //   - Space sur un move → "BULBASAUR forgot X and learned vine-whip!"
    //   - X → décline, vine-whip non appris.
    // ────────────────────────────────────────────────────────────────────────
    private static BattleState ScenarioMoveLearnOverlay()
    {
        var tackle   = new BattleMove(1, "tackle",    1,  MoveCategory.Physical, 40,  100, 35, 35);
        var growl    = new BattleMove(2, "growl",     1,  MoveCategory.Status,   null,100, 40, 40);
        var scratch  = new BattleMove(4, "scratch",   1,  MoveCategory.Physical, 40,  100, 35, 35);
        var leer     = new BattleMove(5, "leer",      1,  MoveCategory.Status,   null,100, 30, 30);
        var vineWhip = new BattleMove(3, "vine-whip", 12, MoveCategory.Physical, 45,  100, 25, 25);

        var player = new BattlePokemon(
            SpeciesId: 1, Nickname: "BULBASAUR", Level: 5,
            CurrentHp: 45, MaxHp: 45,
            Attack: 49, Defense: 49, SpecialAttack: 65, SpecialDefense: 65, Speed: 45,
            Type1Id: 12, Type2Id: 4,
            Moves: new[] { tackle, growl, scratch, leer },
            CurrentExp: 210,
            GrowthRate: GrowthRate.MediumFast,
            FullLearnset: new[] { (6, vineWhip) });

        var opponent = new BattlePokemon(
            SpeciesId: 4, Nickname: "CHARMANDER", Level: 5,
            CurrentHp: 1, MaxHp: 39,
            Attack: 52, Defense: 43, SpecialAttack: 60, SpecialDefense: 50, Speed: 65,
            Type1Id: 10, Type2Id: null,
            Moves: new[] { tackle },
            BaseExpYield: 64);

        return new BattleState(player, opponent, Turn: 0, WeatherType.None,
            new BattleConfig(), Log: Array.Empty<string>());
    }

    // ────────────────────────────────────────────────────────────────────────
    // F4 : Level cap bloqué — preset BW2, 0 badge → cap Lv13
    // Player Lv13 MediumFast, 2150 EXP (seuil Lv14 = 2744).
    // Adversaire donne ≈214 EXP → total 2364 ≥ 2744? Non: 2150+214=2364 < 2744.
    // Mais level cap = 13, donc AwardExp bloque dès que Level >= cap.
    // Attendu : "EXP bloquée ! (Cap niveau 13 — prochain badge requis)" dans le log.
    // ────────────────────────────────────────────────────────────────────────
    private static BattleState ScenarioLevelCap()
    {
        var tackle = new BattleMove(1, "tackle", 1, MoveCategory.Physical, 40, 100, 35, 35);

        var player = new BattlePokemon(
            SpeciesId: 1, Nickname: "BULBASAUR", Level: 13,
            CurrentHp: 75, MaxHp: 75,
            Attack: 60, Defense: 60, SpecialAttack: 80, SpecialDefense: 80, Speed: 55,
            Type1Id: 12, Type2Id: 4,
            Moves: new[] { tackle },
            CurrentExp: 2300,
            GrowthRate: GrowthRate.MediumFast);

        var opponent = new BattlePokemon(
            SpeciesId: 6, Nickname: "CHARIZARD", Level: 15,
            CurrentHp: 1, MaxHp: 120,
            Attack: 84, Defense: 78, SpecialAttack: 109, SpecialDefense: 85, Speed: 100,
            Type1Id: 10, Type2Id: 3,
            Moves: new[] { tackle },
            BaseExpYield: 100);

        return new BattleState(player, opponent, Turn: 0, WeatherType.None,
            new BattleConfig(PlayerBadges: 0, LevelCapTable: BattleConfig.LevelCaps8Badges),
            Log: Array.Empty<string>());
    }

    // ────────────────────────────────────────────────────────────────────────
    // F5 : Multi level-up (3 niveaux d'un coup, 2 moves auto-appris)
    // Player Lv5 MediumFast, 100 EXP.
    // Adversaire GYARADOS baseYield=200, Lv20 → gains=(int)(200*20/7)=571.
    // Total=671 → dépasse Lv6(216), Lv7(343), Lv8(512) = 3 level-ups.
    // FullLearnset: razor-leaf Lv7, sleep-powder Lv8 (auto-appris car moves.count < 4).
    // Attendu : 3× LevelUpOverlay (Space pour passer chacun) +
    //           "learned razor-leaf!" + "learned sleep-powder!" dans le log.
    // ────────────────────────────────────────────────────────────────────────
    private static BattleState ScenarioMultiLevelUp()
    {
        var tackle      = new BattleMove(1, "tackle",       1,  MoveCategory.Physical, 40,  100, 35, 35);
        var growl       = new BattleMove(2, "growl",        1,  MoveCategory.Status,   null,100, 40, 40);
        var razorLeaf   = new BattleMove(6, "razor-leaf",  12,  MoveCategory.Physical, 55,   95, 25, 25);
        var sleepPowder = new BattleMove(7, "sleep-powder",12,  MoveCategory.Status,   null, 75, 15, 15);

        var player = new BattlePokemon(
            SpeciesId: 1, Nickname: "BULBASAUR", Level: 5,
            CurrentHp: 45, MaxHp: 45,
            Attack: 49, Defense: 49, SpecialAttack: 65, SpecialDefense: 65, Speed: 45,
            Type1Id: 12, Type2Id: 4,
            Moves: new[] { tackle, growl },
            CurrentExp: 150,
            GrowthRate: GrowthRate.MediumFast,
            FullLearnset: new[]
            {
                (7, razorLeaf),
                (8, sleepPowder),
            });

        var opponent = new BattlePokemon(
            SpeciesId: 130, Nickname: "GYARADOS", Level: 20,
            CurrentHp: 1, MaxHp: 150,
            Attack: 125, Defense: 79, SpecialAttack: 60, SpecialDefense: 100, Speed: 81,
            Type1Id: 11, Type2Id: 3,
            Moves: new[] { tackle },
            BaseExpYield: 200);

        return new BattleState(player, opponent, Turn: 0, WeatherType.None,
            new BattleConfig(), Log: Array.Empty<string>());
    }

    // F7 : Evolution — BULBASAUR Lv5, 210 EXP (seuil Lv6=216).
    // CHARMANDER yield=64, Lv5 → gains=45 → total=255 ≥ 216 → Lv6 déclenché.
    // EvolvesAtLevel=6 → PendingEvolution → EvolutionOverlay flash 2s.
    // X pendant flash annule (ROADMAP SC5). Space en phase Done confirme.
    private static BattleState ScenarioEvolution()
    {
        var tackle = new BattleMove(1, "tackle", 1, MoveCategory.Physical, 40, 100, 35, 35);

        var player = new BattlePokemon(
            SpeciesId: 1, Nickname: "BULBASAUR", Level: 5,
            CurrentHp: 45, MaxHp: 45,
            Attack: 49, Defense: 49, SpecialAttack: 65, SpecialDefense: 65, Speed: 45,
            Type1Id: 12, Type2Id: 4,
            Moves: new[] { tackle },
            CurrentExp: 210,
            GrowthRate: GrowthRate.MediumFast,
            EvolvesAtLevel: 6,
            EvolvesToSpeciesId: 2,
            EvolvesToName: "IVYSAUR");

        var opponent = new BattlePokemon(
            SpeciesId: 4, Nickname: "CHARMANDER", Level: 5,
            CurrentHp: 1, MaxHp: 39,
            Attack: 52, Defense: 43, SpecialAttack: 60, SpecialDefense: 50, Speed: 65,
            Type1Id: 10, Type2Id: null,
            Moves: new[] { tackle },
            BaseExpYield: 64);

        return new BattleState(player, opponent, Turn: 0, WeatherType.None,
            new BattleConfig(), Log: Array.Empty<string>());
    }

    // F6 : HP lerp visible — adversaire HP plein (50/50), Tackle fait ~12-18 dégâts.
    // Barre HP adverse glisse visuellement pendant ~0.4s après chaque attaque.
    private static BattleState ScenarioHpLerp()
    {
        var tackle = new BattleMove(1, "tackle", 1, MoveCategory.Physical, 40, 100, 35, 35);
        var growl  = new BattleMove(2, "growl",  1, MoveCategory.Status,   null, 100, 40, 40);

        var player = new BattlePokemon(
            SpeciesId: 1, Nickname: "BULBASAUR", Level: 15,
            CurrentHp: 60, MaxHp: 60,
            Attack: 60, Defense: 55, SpecialAttack: 65, SpecialDefense: 65, Speed: 45,
            Type1Id: 12, Type2Id: 4,
            Moves: new[] { tackle, growl },
            CurrentExp: 0,
            GrowthRate: GrowthRate.MediumFast,
            FullLearnset: Array.Empty<(int, BattleMove)>());

        var opponent = new BattlePokemon(
            SpeciesId: 16, Nickname: "PIDGEY", Level: 10,
            CurrentHp: 50, MaxHp: 50,
            Attack: 45, Defense: 40, SpecialAttack: 35, SpecialDefense: 35, Speed: 56,
            Type1Id: 1, Type2Id: 3,
            Moves: new[] { tackle },
            BaseExpYield: 55);

        return new BattleState(player, opponent, Turn: 0, WeatherType.None,
            new BattleConfig(), Log: Array.Empty<string>());
    }
#endif
}
