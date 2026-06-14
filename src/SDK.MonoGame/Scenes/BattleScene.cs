namespace SDK.MonoGame.Scenes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using SDK.MonoGame.Input;
using SDK.MonoGame.UI;

public sealed class BattleScene : IGameScene
{
    private enum BattlePhase { Init, SelectMove, ShowLog, ShowLevelUp, BattleEnd }

    private readonly IBattleEngine _engine;
    private readonly IExpFormula? _expFormula;
    private HpBar? _hpBar;
    private StatusIcon? _statusIcon;
    private MoveMenu? _moveMenu;
    private readonly BattleEndOverlay _battleEndOverlay = new();
    private ExpBar? _expBar;
    private LevelUpOverlay? _levelUpOverlay;
    private Texture2D? _pixel;
    private SpriteFont? _font;

    private BattleState? _state;
    private BattlePhase _phase = BattlePhase.Init;
    private BattleMove? _selectedMove;
    private IReadOnlyList<string> _lastLog = Array.Empty<string>();
    private KeyboardState _prevKs;
    private bool _leveledUp;
    private BattlePokemon _playerBeforeTurn = default!;

    private WorldScene? _worldScene;
    private Game1? _game1;

    public BattleScene(IBattleEngine engine, IExpFormula? expFormula = null)
    {
        _engine = engine;
        _expFormula = expFormula;
    }

    public void Initialize(GraphicsDevice graphicsDevice, SpriteFont? font = null)
    {
        _hpBar = new HpBar(graphicsDevice);
        _statusIcon = new StatusIcon(graphicsDevice);
        _expBar = new ExpBar(graphicsDevice);
        _levelUpOverlay = new LevelUpOverlay();
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
        _font = font;
        _graphicsDevice = graphicsDevice;
    }

    private GraphicsDevice? _graphicsDevice;

    public void SetContext(WorldScene worldScene, Game1 game1)
    {
        _worldScene = worldScene;
        _game1 = game1;
    }

    public void LoadBattle(BattleState initialState)
    {
        _state = initialState;
        _phase = BattlePhase.SelectMove;
        _selectedMove = null;
        _prevKs = default;
        if (_graphicsDevice != null)
            _moveMenu = new MoveMenu(initialState.Player.Moves, _graphicsDevice);
    }

    public void SetPlayerMove(BattleMove move) => _selectedMove = move;

    public void Update(GameTime gameTime)
    {
        if (_state is null) return;

        switch (_phase)
        {
            case BattlePhase.SelectMove:
                _moveMenu?.Update(Keyboard.GetState());
                if (_moveMenu?.SelectedMove != null)
                {
                    SetPlayerMove(_moveMenu.SelectedMove);
                    _moveMenu.ResetSelection();
                }
                if (_selectedMove != null)
                {
                    ExecuteTurn();
                    _selectedMove = null;
                }
                break;

            case BattlePhase.ShowLog:
                var ksLog = Keyboard.GetState();
                if (ksLog.IsKeyDown(InputMap.Confirm) && !_prevKs.IsKeyDown(InputMap.Confirm))
                {
                    _prevKs = Keyboard.GetState();
                    if (_leveledUp)
                    {
                        _phase = BattlePhase.ShowLevelUp;
                        _leveledUp = false;
                    }
                    else
                    {
                        _phase = (_state.Player.CurrentHp <= 0 || _state.Opponent.CurrentHp <= 0)
                            ? BattlePhase.BattleEnd
                            : BattlePhase.SelectMove;
                    }
                }
                else
                    _prevKs = ksLog;
                break;

            case BattlePhase.ShowLevelUp:
                var ksLvl = Keyboard.GetState();
                _levelUpOverlay!.Update(ksLvl, _prevKs);
                _prevKs = ksLvl;
                if (!_levelUpOverlay.IsVisible)
                    _phase = (_state.Player.CurrentHp <= 0 || _state.Opponent.CurrentHp <= 0)
                        ? BattlePhase.BattleEnd
                        : BattlePhase.SelectMove;
                break;

            case BattlePhase.BattleEnd:
                var ks = Keyboard.GetState();
                if (ks.IsKeyDown(InputMap.Confirm) && !_prevKs.IsKeyDown(InputMap.Confirm))
                    _game1?.SwitchToScene(_worldScene!);
                _prevKs = ks;
                break;
        }
    }

    public void Draw(SpriteBatch sb, GameTime gameTime)
    {
        if (_pixel is null) return;

        DrawRect(sb, new Rectangle(0, 0, 480, 270), Color.Black);

        if (_state is null) return;

        // Field: opponent top-right, player bottom-left — no overlap
        DrawRect(sb, new Rectangle(262, 8,  88, 88), Color.Gray);      // opponent sprite
        DrawRect(sb, new Rectangle(48,  90, 88, 88), Color.DarkGray);  // player sprite

        // HP bars: opponent top-left, player bottom-right
        _hpBar!.Draw(sb, _state.Opponent.CurrentHp, _state.Opponent.MaxHp,
            new Vector2(10, 12),  140, 8, "FOE", _font);
        _hpBar.Draw(sb, _state.Player.CurrentHp, _state.Player.MaxHp,
            new Vector2(256, 130), 140, 8, "PLR", _font);

        _statusIcon?.Draw(sb, _state.Opponent.Status, new Vector2(10,  28), _font);
        _statusIcon?.Draw(sb, _state.Player.Status,   new Vector2(256, 146), _font);

        if (_expFormula != null && _state.Player.Level < 100)
        {
            int nextThreshold = _expFormula.ExpThreshold(_state.Player.Level + 1, _state.Player.GrowthRate);
            _expBar!.Draw(sb, _state.Player.CurrentExp, nextThreshold, _state.Player.Level,
                new Vector2(256f, 162f), 140, 4, _font);
        }

        // UI panel at bottom (y 178–270)
        DrawRect(sb, new Rectangle(0, 178, 480, 1),  new Color(60, 60, 60));
        DrawRect(sb, new Rectangle(0, 179, 480, 91), new Color(15, 15, 15));

        if (_phase == BattlePhase.SelectMove)
            _moveMenu?.Draw(sb, new Vector2(5, 183), _font);

        if (_phase == BattlePhase.ShowLog && _font != null)
        {
            int lines = Math.Min(_lastLog.Count, 5);
            for (int i = 0; i < lines; i++)
                sb.DrawString(_font, _lastLog[i], new Vector2(8f, 185f + i * 16f), Color.White,
                    0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
            sb.DrawString(_font, "Space", new Vector2(432, 255), Color.DimGray,
                0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
        }

        if (_phase == BattlePhase.ShowLevelUp)
        {
            DrawRect(sb, new Rectangle(0, 178, 480, 1), new Color(60, 60, 60));
            _levelUpOverlay!.Draw(sb, _pixel!, _font);
        }

        if (_phase == BattlePhase.BattleEnd)
        {
            DrawRect(sb, new Rectangle(0, 178, 480, 92), Color.Black);
            _battleEndOverlay.Draw(sb, _state.Player.CurrentHp > 0, _font);
        }
    }

    private void ExecuteTurn()
    {
        _playerBeforeTurn = _state!.Player;
        var opponentMove = _engine.SelectOpponentMove(_state!);
        _state = _engine.RunTurn(_state!, _selectedMove!, opponentMove);
        _lastLog = _state.Log;
        _leveledUp = _state.Log.Any(m => m.Contains("grew to level"));
        if (_leveledUp)
            _levelUpOverlay!.Trigger(_playerBeforeTurn, _state.Player);
        _phase = BattlePhase.ShowLog;
        _prevKs = Keyboard.GetState();
    }

    private void DrawRect(SpriteBatch sb, Rectangle rect, Color color) =>
        sb.Draw(_pixel!, rect, color);
}
