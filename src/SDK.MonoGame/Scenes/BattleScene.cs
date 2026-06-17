namespace SDK.MonoGame.Scenes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDK.Core.Enums;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using SDK.MonoGame.Input;
using SDK.MonoGame.UI;

public sealed class BattleScene : IGameScene
{
    private enum BattlePhase { Init, SelectAction, SelectMove, ShowLog, ShowLevelUp, ShowMoveLearn, ShowEvolution, BattleEnd }

    private readonly IBattleEngine _engine;
    private readonly IExpFormula? _expFormula;
    private HpBar? _hpBar;
    private StatusIcon? _statusIcon;
    private MoveMenu? _moveMenu;
    private readonly BattleEndOverlay _battleEndOverlay = new();
    private ExpBar? _expBar;
    private LevelUpOverlay? _levelUpOverlay;
    private MoveLearnOverlay? _moveLearnOverlay;
    private readonly EvolutionOverlay _evolutionOverlay = new();
    private Texture2D? _pixel;
    private SpriteFont? _font;

    private BattleState? _state;
    private BattlePhase _phase = BattlePhase.Init;
    private BattleMove? _selectedMove;
    private IReadOnlyList<string> _lastLog = Array.Empty<string>();
    private KeyboardState _prevKs;
    private bool _leveledUp;
    private BattlePokemon _playerBeforeTurn = default!;

    private Queue<BattleMove>? _pendingMoveQueue;
    private BattleMove? _currentLearnMove;

    private WorldScene? _worldScene;
    private Game1? _game1;

    private float _playerDisplayHp;
    private float _opponentDisplayHp;
    private int   _actionIndex  = 0;
    private bool  _playerRanAway = false;
    private float _fleeTimer = 0f;

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
        _moveLearnOverlay = new MoveLearnOverlay();
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
        _phase = BattlePhase.SelectAction;
        _selectedMove = null;
        _prevKs = default;
        _pendingMoveQueue = null;
        _currentLearnMove = null;
        _leveledUp = false;
        _actionIndex = 0;
        _playerRanAway = false;
        _fleeTimer = 0f;
        _playerDisplayHp  = initialState.Player.CurrentHp;
        _opponentDisplayHp = initialState.Opponent.CurrentHp;
        if (_graphicsDevice != null)
            _moveMenu = new MoveMenu(initialState.Player.Moves, _graphicsDevice);
    }

    public void SetPlayerMove(BattleMove move) => _selectedMove = move;

    public void Update(GameTime gameTime)
    {
        if (_state is null) return;

        if (_expFormula != null && _state.Player.Level < 100)
        {
            int currentThreshold = _expFormula.ExpThreshold(_state.Player.Level, _state.Player.GrowthRate);
            int nextThreshold    = _expFormula.ExpThreshold(_state.Player.Level + 1, _state.Player.GrowthRate);
            int intraExp   = Math.Max(0, _state.Player.CurrentExp - currentThreshold);
            int intraRange = Math.Max(1, nextThreshold - currentThreshold);
            _expBar?.Update(gameTime, intraExp, intraRange);
        }

        float lerpSpeed = 8f * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _playerDisplayHp   = Math.Clamp(MathHelper.Lerp(_playerDisplayHp,   _state.Player.CurrentHp,   lerpSpeed), 0f, _state.Player.MaxHp);
        _opponentDisplayHp = Math.Clamp(MathHelper.Lerp(_opponentDisplayHp, _state.Opponent.CurrentHp, lerpSpeed), 0f, _state.Opponent.MaxHp);
        if (_state.Player.CurrentHp   == 0 && _playerDisplayHp   < 0.5f) _playerDisplayHp   = 0f;
        if (_state.Opponent.CurrentHp == 0 && _opponentDisplayHp < 0.5f) _opponentDisplayHp = 0f;

        switch (_phase)
        {
            case BattlePhase.SelectAction:
            {
                var ksAct = Keyboard.GetState();
                if (ksAct.IsKeyDown(InputMap.NavUp)   && !_prevKs.IsKeyDown(InputMap.NavUp))
                    _actionIndex = (_actionIndex - 1 + 2) % 2;
                if (ksAct.IsKeyDown(InputMap.NavDown) && !_prevKs.IsKeyDown(InputMap.NavDown))
                    _actionIndex = (_actionIndex + 1) % 2;
                if (ksAct.IsKeyDown(InputMap.Confirm) && !_prevKs.IsKeyDown(InputMap.Confirm))
                {
                    if (_actionIndex == 0) // FIGHT
                    {
                        _moveMenu = new MoveMenu(_state!.Player.Moves, _graphicsDevice!, Keyboard.GetState());
                        _phase = BattlePhase.SelectMove;
                    }
                    else // RUN
                    {
                        _lastLog = new List<string> { "Got away safely!" };
                        _playerRanAway = true;
                        _fleeTimer = 5f;
                        _phase = BattlePhase.ShowLog;
                        _prevKs = Keyboard.GetState();
                    }
                }
                _prevKs = ksAct;
                break;
            }

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
                if (_playerRanAway)
                {
                    _fleeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_fleeTimer <= 0f)
                    {
                        Serilog.Log.Information("[BATTLE] Fled — {Player} Lv{Level} vs {Opponent} Lv{OppLevel}",
                            _state!.Player.Nickname, _state.Player.Level,
                            _state.Opponent.Nickname, _state.Opponent.Level);
                        _game1?.SwitchToScene(_worldScene!);
                    }
                    break;
                }
                var ksLog = Keyboard.GetState();
                if (ksLog.IsKeyDown(InputMap.Confirm) && !_prevKs.IsKeyDown(InputMap.Confirm))
                {
                    if (_leveledUp)
                    {
                        _phase = BattlePhase.ShowLevelUp;
                        _leveledUp = false;
                    }
                    else if (_pendingMoveQueue?.Count > 0)
                    {
                        TriggerNextMoveLearn();
                    }
                    else if (_state!.PendingEvolution != null)
                        TriggerEvolution();
                    else
                    {
                        _phase = NextPhaseAfterBattle();
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
                {
                    if (_pendingMoveQueue?.Count > 0)
                        TriggerNextMoveLearn();
                    else if (_state!.PendingEvolution != null)
                        TriggerEvolution();
                    else
                        _phase = NextPhaseAfterBattle();
                }
                break;

            case BattlePhase.ShowMoveLearn:
                var ksMl = Keyboard.GetState();
                _moveLearnOverlay!.Update(ksMl, _prevKs);
                _prevKs = ksMl;
                if (_moveLearnOverlay.DecisionMade)
                {
                    ApplyMoveLearnDecision();
                    _phase = BattlePhase.ShowLog;
                }
                break;

            case BattlePhase.ShowEvolution:
            {
                var ksEvo = Keyboard.GetState();
                _evolutionOverlay.Update(ksEvo, _prevKs, gameTime);
                _prevKs = ksEvo;
                if (_evolutionOverlay.IsComplete)
                {
                    var evo = _state!.PendingEvolution!;
                    if (!_evolutionOverlay.WasCancelled)
                    {
                        _state = _state with
                        {
                            Player = _state.Player with
                            {
                                Nickname  = evo.NewName,
                                SpeciesId = evo.NewSpeciesId
                            },
                            PendingEvolution = null
                        };
                        Serilog.Log.Information("[BATTLE] Evolution: {Old} → {New} (SpeciesId {Id})",
                            evo.OldName, evo.NewName, evo.NewSpeciesId);
                    }
                    else
                    {
                        _state = _state with { PendingEvolution = null };
                        Serilog.Log.Information("[BATTLE] Evolution cancelled: {Old} stays as-is", evo.OldName);
                    }
                    _phase = NextPhaseAfterBattle();
                }
                break;
            }

            case BattlePhase.BattleEnd:
                var ks = Keyboard.GetState();
                if (ks.IsKeyDown(InputMap.Confirm) && !_prevKs.IsKeyDown(InputMap.Confirm))
                {
                    var result = _state!.Player.CurrentHp > 0 ? "Victory" : "Defeat";
                    Serilog.Log.Information("[BATTLE] {Result} — {Player} Lv{Level} vs {Opponent} Lv{OppLevel}",
                        result, _state.Player.Nickname, _state.Player.Level,
                        _state.Opponent.Nickname, _state.Opponent.Level);
                    _game1?.SwitchToScene(_worldScene!);
                }
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
        _hpBar!.Draw(sb, (int)Math.Ceiling(_opponentDisplayHp), _state.Opponent.MaxHp,
            new Vector2(10, 12),  140, 8,
            _state.Opponent.Nickname[..Math.Min(_state.Opponent.Nickname.Length, 10)], _font);
        if (_font != null)
            sb.DrawString(_font, $"Lv.{_state.Opponent.Level}",
                new Vector2(130f, 1f), Color.White, 0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
        _hpBar.Draw(sb, (int)Math.Ceiling(_playerDisplayHp), _state.Player.MaxHp,
            new Vector2(256, 130), 140, 8,
            _state.Player.Nickname[..Math.Min(_state.Player.Nickname.Length, 10)], _font);

        _statusIcon?.Draw(sb, _state.Opponent.Status, new Vector2(10,  28), _font);
        _statusIcon?.Draw(sb, _state.Player.Status,   new Vector2(256, 146), _font);

        if (_expFormula != null && _state.Player.Level < 100)
            _expBar!.Draw(sb, _state.Player.Level, new Vector2(256f, 162f), 140, 4, _font);

        if (_state.Weather != WeatherType.None && _font != null)
        {
            string weatherLabel = _state.Weather switch
            {
                WeatherType.Sun  => "SUN",
                WeatherType.Rain => "RAIN",
                WeatherType.Sand => "SAND",
                WeatherType.Hail => "HAIL",
                _                => ""
            };
            if (weatherLabel.Length > 0)
                sb.DrawString(_font, weatherLabel, new Vector2(220f, 2f), Color.Yellow,
                    0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }

        // UI panel at bottom (y 178–270)
        DrawRect(sb, new Rectangle(0, 178, 480, 1),  new Color(60, 60, 60));
        DrawRect(sb, new Rectangle(0, 179, 480, 91), new Color(15, 15, 15));

        if (_phase == BattlePhase.SelectMove)
            _moveMenu?.Draw(sb, new Vector2(5, 183), _font);

        if (_phase == BattlePhase.SelectAction && _font != null)
        {
            string fightTxt = _actionIndex == 0 ? "> FIGHT" : "  FIGHT";
            string runTxt   = _actionIndex == 1 ? "> RUN"   : "  RUN";
            sb.DrawString(_font, fightTxt, new Vector2(8f,  187f), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            sb.DrawString(_font, runTxt,   new Vector2(8f,  207f), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
        }

        if (_phase == BattlePhase.ShowLog && _font != null)
        {
            int lines = Math.Min(_lastLog.Count, 5);
            int start = _lastLog.Count - lines;
            for (int i = 0; i < lines; i++)
                sb.DrawString(_font, _lastLog[start + i], new Vector2(8f, 185f + i * 16f), Color.White,
                    0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
            sb.DrawString(_font, "Space", new Vector2(432, 255), Color.DimGray,
                0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
        }

        if (_phase == BattlePhase.ShowLevelUp)
        {
            DrawRect(sb, new Rectangle(0, 178, 480, 1), new Color(60, 60, 60));
            _levelUpOverlay!.Draw(sb, _pixel!, _font);
        }

        if (_phase == BattlePhase.ShowMoveLearn)
        {
            DrawRect(sb, new Rectangle(0, 178, 480, 1), new Color(60, 60, 60));
            _moveLearnOverlay!.Draw(sb, _pixel!, _font);
        }

        if (_phase == BattlePhase.ShowEvolution)
        {
            DrawRect(sb, new Rectangle(0, 178, 480, 1), new Color(60, 60, 60));
            _evolutionOverlay.Draw(sb, _pixel!, _font);
        }

        if (_phase == BattlePhase.BattleEnd)
        {
            DrawRect(sb, new Rectangle(0, 178, 480, 92), Color.Black);
            if (!_playerRanAway)
                _battleEndOverlay.Draw(sb, _state.Player.CurrentHp > 0, _font);
        }
    }

    private void ExecuteTurn()
    {
        _playerBeforeTurn = _state!.Player;
        var opponentMove = _engine.SelectOpponentMove(_state!);
        _state = _engine.RunTurn(_state!, _selectedMove!, opponentMove);
        _lastLog = _state.Log;
        Serilog.Log.Information("[BATTLE] Turn {Turn}: {Messages}", _state.Turn, string.Join(" | ", _state.Log));
        _leveledUp = _state.Log.Any(m => m.Contains("grew to level"));
        if (_leveledUp)
            _levelUpOverlay!.Trigger(_playerBeforeTurn, _state.Player);
        _pendingMoveQueue = _state.PendingLearnedMoves is { Count: > 0 } p
            ? new Queue<BattleMove>(p)
            : null;
        _phase = BattlePhase.ShowLog;
        var ksNow = Keyboard.GetState();
        _moveMenu = new MoveMenu(_state.Player.Moves, _graphicsDevice!, ksNow);
        _prevKs = ksNow;
    }

    private void TriggerEvolution()
    {
        var evo = _state!.PendingEvolution!;
        _evolutionOverlay.Trigger(evo.OldName, evo.NewName);
        _phase = BattlePhase.ShowEvolution;
        _prevKs = Keyboard.GetState();
    }

    private void TriggerNextMoveLearn()
    {
        _currentLearnMove = _pendingMoveQueue!.Dequeue();
        _moveLearnOverlay!.Trigger(_state!.Player.Nickname, _currentLearnMove, _state.Player.Moves);
        _phase = BattlePhase.ShowMoveLearn;
    }

    private void ApplyMoveLearnDecision()
    {
        int idx = _moveLearnOverlay!.ForgottenMoveIndex;
        var log = _lastLog.ToList();
        if (idx >= 0 && _currentLearnMove != null)
        {
            var oldMoves = _state!.Player.Moves;
            string forgotten = oldMoves[idx].Identifier;
            string learned   = _currentLearnMove.Identifier;
            var newMoves = oldMoves.ToList();
            newMoves[idx] = _currentLearnMove;
            _state = _state with { Player = _state.Player with { Moves = newMoves } };
            _moveMenu = new MoveMenu(_state.Player.Moves, _graphicsDevice!, Keyboard.GetState());
            log.Add($"{_state.Player.Nickname} forgot {forgotten} and learned {learned}!");
            Serilog.Log.Information("[BATTLE] {Pokemon} forgot {Forgotten} and learned {Learned}",
                _state.Player.Nickname, forgotten, learned);
        }
        else if (_currentLearnMove != null)
        {
            log.Add($"{_state!.Player.Nickname} did not learn {_currentLearnMove.Identifier}.");
        }
        _lastLog = log;
        _currentLearnMove = null;
    }

    private BattlePhase NextPhaseAfterBattle()
    {
        if (_state!.Player.CurrentHp <= 0 || _state.Opponent.CurrentHp <= 0)
            return BattlePhase.BattleEnd;
        return BattlePhase.SelectAction;
    }

    private void DrawRect(SpriteBatch sb, Rectangle rect, Color color) =>
        sb.Draw(_pixel!, rect, color);
}
