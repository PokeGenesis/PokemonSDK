namespace SDK.MonoGame.Scenes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDK.Core.Enums;
using SDK.Core.ValueObjects;
using SDK.MonoGame.Input;
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
        if (ks.IsKeyDown(InputMap.DebugBattle) && !_prevKs.IsKeyDown(InputMap.DebugBattle) && _wipe is null)
        {
            var testState = CreateTestBattleState();
            _wipe = new WipeTransition(_game1.GraphicsDevice, 15, () =>
            {
                _battleScene.LoadBattle(testState);
                _game1.SwitchToScene(_battleScene);
                _wipe = null;
            });
        }
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

    private static BattleState CreateTestBattleState()
    {
        var move1 = new BattleMove(1, "tackle", 1, MoveCategory.Physical, 40, 100, 35, 35);
        var move2 = new BattleMove(2, "growl", 1, MoveCategory.Status, null, 100, 40, 40);

        var player = new BattlePokemon(
            SpeciesId: 1, Nickname: "BULBASAUR", Level: 5,
            CurrentHp: 45, MaxHp: 45,
            Attack: 49, Defense: 49, SpecialAttack: 65, SpecialDefense: 65, Speed: 45,
            Type1Id: 12, Type2Id: 4,
            Moves: new[] { move1, move2 });

        var opponent = new BattlePokemon(
            SpeciesId: 4, Nickname: "CHARMANDER", Level: 5,
            CurrentHp: 39, MaxHp: 39,
            Attack: 52, Defense: 43, SpecialAttack: 60, SpecialDefense: 50, Speed: 65,
            Type1Id: 10, Type2Id: null,
            Moves: new[] { move1 });

        return new BattleState(player, opponent, Turn: 0, WeatherType.None,
            new BattleConfig(), Log: Array.Empty<string>());
    }
}
