using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDK.Battle;
using SDK.Battle.Difficulty;
using SDK.Battle.Formulas;
using SDK.Battle.Plugins;
using SDK.Core.Enums;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using SDK.Plugins.Nuzlocke;
using SDK.Scripting.Bindings;
using SDK.Scripting.Engine;
using StarterGame.World;

namespace StarterGame.Scenes;

public sealed class OverworldScene
{
    private readonly GraphicsDevice _gfx;
    private Texture2D _tileset  = null!;
    private Texture2D _playerTex = null!;
    private Texture2D _npcTex   = null!;
    private SpriteFont _font    = null!;

    private Vector2 _playerPos = new(2f, 7f);
    private const float Speed = 5f;
    private const int   TD = TilemapData.TilePixels * TilemapData.DisplayScale; // 48px

    // Source rects dans l'atlas Kenney Tiny Town (192×176, grille 12×11 de 16×16)
    private static Rectangle TileRect(int col, int row) => new(col * 16, row * 16, 16, 16);
    private static readonly Rectangle GrassSrc = TileRect(0, 0);   // vert herbe
    private static readonly Rectangle WallSrc  = TileRect(1, 9);   // gris-blanc mur bâtiment
    private static readonly Rectangle WaterSrc = TileRect(0, 4);   // bleu-gris eau
    private static readonly Rectangle WarpSrc  = TileRect(1, 2);   // sable brun, sortie/entrée

    private string _dialogue = string.Empty;

    private GameState _gameState = new();
    private readonly ISaveSystem _saveSystem;
    private readonly LuaScriptEngine _scriptEngine;
    private readonly BattleEngine _battleEngine;
    private string _lastBattleResult = string.Empty;
    private string _scriptPath = string.Empty;
    private KeyboardState _prevKb;

    public OverworldScene(GraphicsDevice gfx, ISaveSystem saveSystem, LuaScriptEngine scriptEngine)
    {
        _gfx = gfx;
        _saveSystem = saveSystem;
        _scriptEngine = scriptEngine;

        var plugins = new PluginRegistry();
        plugins.Register(new NuzlockePlugin((flagKey, val) =>
            _gameState = _gameState.WithFlag(flagKey, val)));

        _battleEngine = new BattleEngine(
            new Gen1DamageFormula(),
            new StoryDifficultyMode(),
            new StoryDifficultyMode(),
            new NeutralTypeChart(),
            plugins);
    }

    public void LoadContent(ContentManager content)
    {
        _tileset = content.Load<Texture2D>("Sprites/Tileset");
        _font    = content.Load<SpriteFont>("Fonts/DefaultFont");

        _playerTex = new Texture2D(_gfx, 1, 1);
        _playerTex.SetData(new[] { Color.Yellow });

        _npcTex = new Texture2D(_gfx, 1, 1);
        _npcTex.SetData(new[] { Color.Magenta });

        _scriptPath = Path.Combine(AppContext.BaseDirectory, "Content", "Scripts", "npc_dialogue.lua");
    }

    public void Update(GameTime gameTime, KeyboardState kb)
    {
        var dt   = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var move = Vector2.Zero;

        if (kb.IsKeyDown(Keys.Left))  move.X -= Speed * dt;
        if (kb.IsKeyDown(Keys.Right)) move.X += Speed * dt;
        if (kb.IsKeyDown(Keys.Up))    move.Y -= Speed * dt;
        if (kb.IsKeyDown(Keys.Down))  move.Y += Speed * dt;

        var next  = _playerPos + move;
        var tileX = (int)MathF.Round(next.X);
        var tileY = (int)MathF.Round(next.Y);

        if (tileX >= 0 && tileX < TilemapData.Width &&
            tileY >= 0 && tileY < TilemapData.Height &&
            TilemapData.IsWalkable(TilemapData.Map[tileY, tileX]))
        {
            _playerPos = next;

            if (TilemapData.IsWarp(TilemapData.Map[tileY, tileX]))
                _playerPos.X = _playerPos.X <= 1f ? TilemapData.Width - 2f : 1f;
        }

        // F5 — save (just-pressed)
        if (kb.IsKeyDown(Keys.F5) && !_prevKb.IsKeyDown(Keys.F5))
        {
            Directory.CreateDirectory("data");
            _saveSystem.Save(_gameState, "data/save1.json");
            _dialogue = "Sauvegardé!";
        }
        // F9 — load (just-pressed)
        else if (kb.IsKeyDown(Keys.F9) && !_prevKb.IsKeyDown(Keys.F9))
        {
            var loaded = _saveSystem.Load("data/save1.json");
            if (loaded != null)
            {
                _gameState = loaded;
                _dialogue = "Chargé!";
            }
        }
        // Espace — interaction NPC
        else if (kb.IsKeyDown(Keys.Space) && !_prevKb.IsKeyDown(Keys.Space))
        {
            var px = (int)MathF.Round(_playerPos.X);
            var py = (int)MathF.Round(_playerPos.Y);
            bool npcFound = false;
            foreach (var (dx, dy) in new[] { (0, -1), (0, 1), (-1, 0), (1, 0) })
            {
                var nx = px + dx; var ny = py + dy;
                if (nx >= 0 && nx < TilemapData.Width &&
                    ny >= 0 && ny < TilemapData.Height &&
                    TilemapData.IsNpc(TilemapData.Map[ny, nx]))
                {
                    npcFound = true;
                    break;
                }
            }

            if (npcFound)
            {
                var tackle = new BattleMove(
                    MoveId: 33, Identifier: "tackle", TypeId: 1,
                    Category: MoveCategory.Physical, Power: 40, Accuracy: 100,
                    CurrentPP: 35, MaxPP: 35);

                var player = new BattlePokemon(
                    SpeciesId: 1, Nickname: "Bulbasaur", Level: 5,
                    CurrentHp: 45, MaxHp: 45,
                    Attack: 49, Defense: 49, SpecialAttack: 65, SpecialDefense: 65, Speed: 45,
                    Type1Id: 12, Type2Id: 4,
                    Moves: new[] { tackle });

                var opponent = new BattlePokemon(
                    SpeciesId: 19, Nickname: "Rattata", Level: 3,
                    CurrentHp: 30, MaxHp: 30,
                    Attack: 56, Defense: 35, SpecialAttack: 25, SpecialDefense: 35, Speed: 72,
                    Type1Id: 1, Type2Id: null,
                    Moves: new[] { tackle });

                var request = new BattleRequest(player, opponent, new BattleConfig());
                var result  = _battleEngine.RunBattle(request);

                if (result.PlayerWon)
                {
                    _lastBattleResult = $"Victoire en {result.TurnsElapsed} tours!";
                    if (File.Exists(_scriptPath))
                    {
                        var api = new BadgeApi(_gameState);
                        _scriptEngine.RegisterApi("badges", api);
                        _scriptEngine.LoadFile(_scriptPath);
                        _gameState = api.GetState();
                    }
                }
                else
                {
                    _lastBattleResult = "Défaite...";
                }

                _dialogue = _lastBattleResult;
            }
        }
        else if (!kb.IsKeyDown(Keys.F5) && !kb.IsKeyDown(Keys.F9) && !kb.IsKeyDown(Keys.Space))
        {
            _dialogue = string.Empty;
        }

        _prevKb = kb;
    }

    public void Draw(SpriteBatch sb)
    {
        sb.Begin(samplerState: SamplerState.PointClamp);

        for (var row = 0; row < TilemapData.Height; row++)
        for (var col = 0; col < TilemapData.Width;  col++)
        {
            var dest = new Rectangle(col * TD, row * TD, TD, TD);
            var tile = TilemapData.Map[row, col];
            var src  = tile switch
            {
                1 => WallSrc,
                2 => WaterSrc,
                3 => WarpSrc,
                _ => GrassSrc,
            };
            sb.Draw(_tileset, dest, src, Color.White);

            if (TilemapData.IsNpc(tile))
                sb.Draw(_npcTex, dest, Color.White);
        }

        var px = (int)(_playerPos.X * TD);
        var py = (int)(_playerPos.Y * TD);
        sb.Draw(_playerTex, new Rectangle(px, py, TD, TD), Color.White);

        if (_gameState.GetFlag<bool>("badge_boulder"))
            sb.DrawString(_font, "Badge: boulder ✓", new Vector2(20, TilemapData.Height * TD - 30), Color.Gold);

        if (!string.IsNullOrEmpty(_dialogue))
        {
            sb.Draw(_playerTex,
                new Rectangle(10, 10, TilemapData.Width * TD - 20, 40), new Color(0, 0, 0, 180));
            sb.DrawString(_font, _dialogue, new Vector2(20, 18), Color.White);
        }

        sb.End();
    }

    private sealed class NeutralTypeChart : ITypeChart
    {
        public decimal GetFactor(int attackerTypeId, int defenderTypeId, int generation) => 1.0m;
    }
}
