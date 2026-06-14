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
    private enum BattlePhase { Init, SelectMove, ShowLog, BattleEnd }

    private readonly IBattleEngine _engine;
    private HpBar? _hpBar;
    private StatusIcon? _statusIcon;
    private MoveMenu? _moveMenu;
    private BattleEndOverlay _battleEndOverlay = new();
    private Texture2D? _pixel;
    private SpriteFont? _font;

    private BattleState? _state;
    private BattlePhase _phase = BattlePhase.Init;
    private BattleMove? _selectedMove;
    private IReadOnlyList<string> _lastLog = Array.Empty<string>();
    private KeyboardState _prevKs;

    private WorldScene? _worldScene;
    private Game1? _game1;

    public BattleScene(IBattleEngine engine) => _engine = engine;

    public void Initialize(GraphicsDevice graphicsDevice, SpriteFont? font = null)
    {
        _hpBar = new HpBar(graphicsDevice);
        _statusIcon = new StatusIcon(graphicsDevice);
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
                    _phase = (_state.Player.CurrentHp <= 0 || _state.Opponent.CurrentHp <= 0)
                        ? BattlePhase.BattleEnd
                        : BattlePhase.SelectMove;
                }
                else
                    _prevKs = ksLog;
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

        // UI panel at bottom (y 178–270)
        DrawRect(sb, new Rectangle(0, 178, 480, 1),  new Color(60, 60, 60));
        DrawRect(sb, new Rectangle(0, 179, 480, 91), new Color(15, 15, 15));

        if (_phase == BattlePhase.SelectMove)
            _moveMenu?.Draw(sb, new Vector2(5, 183), _font);

        if (_phase == BattlePhase.ShowLog && _font != null)
        {
            int lines = Math.Min(_lastLog.Count, 5);
            for (int i = 0; i < lines; i++)
                sb.DrawString(_font, _lastLog[i], new Vector2(8, 185 + i * 16), Color.White,
                    0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
            sb.DrawString(_font, "Space", new Vector2(432, 255), Color.DimGray,
                0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
        }

        if (_phase == BattlePhase.BattleEnd)
        {
            DrawRect(sb, new Rectangle(0, 178, 480, 92), Color.Black);
            _battleEndOverlay.Draw(sb, _state.Player.CurrentHp > 0, _font);
        }
    }

    private void ExecuteTurn()
    {
        var opponentMove = _engine.SelectOpponentMove(_state!);
        _state = _engine.RunTurn(_state!, _selectedMove!, opponentMove);
        _lastLog = _state.Log;
        _phase = BattlePhase.ShowLog;
        _prevKs = Keyboard.GetState();
    }

    private void DrawRect(SpriteBatch sb, Rectangle rect, Color color) =>
        sb.Draw(_pixel!, rect, color);
}
