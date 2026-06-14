namespace SDK.MonoGame;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDK.Core.Interfaces;
using SDK.MonoGame.Input;
using SDK.MonoGame.Rendering;
using SDK.MonoGame.Scenes;
using SDK.MonoGame.UI;
using SDK.MonoGame.World;
#if DEBUG
using SDK.Scripting.HotReload;
#endif

public class Game1 : Game
{
    private readonly IServiceProvider _services;
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch            _spriteBatch = null!;
    private RenderPipeline         _renderPipeline = null!;
    private IGameScene             _currentScene = null!;
    private IGameScene?            _pendingScene;
    private BattleScene            _battleScene = null!;
    private KeyboardInputProvider  _keyboard = null!;
    private ISaveSystem            _saveSystem = null!;
    private Func<IScriptEngine>    _scriptEngineFactory = null!;
    private LuaErrorOverlay        _luaErrorOverlay = new();
    private SpriteFont?            _font;
    private int                    _turboLevel;
    private Microsoft.Xna.Framework.Input.KeyboardState _prevKsGlobal;
#if DEBUG
    private Microsoft.Xna.Framework.Input.KeyboardState _prevKeyState;
    private LuaHotReloader?        _hotReloader;
    private LuaConsole             _luaConsole = new();
#endif

    public Game1(IServiceProvider services)
    {
        _services = services;
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth  = 1920,
            PreferredBackBufferHeight = 1080,
            IsFullScreen = false,
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public void SwitchToScene(IGameScene scene) => _pendingScene = scene;

    protected override void Initialize()
    {
        _battleScene  = _services.GetRequiredService<BattleScene>();
        var world  = _services.GetRequiredService<WorldSystem>();
        var player = _services.GetRequiredService<PlayerSystem>();
        var ws     = new WorldScene(world, player, _battleScene, this);
        _currentScene = ws;
        _battleScene.SetContext(ws, this);

        _keyboard             = (KeyboardInputProvider)_services.GetRequiredService<IInputProvider>();
        _saveSystem           = _services.GetRequiredService<ISaveSystem>();
        _scriptEngineFactory  = _services.GetRequiredService<Func<IScriptEngine>>();
#if DEBUG
        var hotReloadEngine = _scriptEngineFactory();
        _hotReloader = new LuaHotReloader("Content/Scripts", hotReloadEngine);
        _hotReloader.OnReloadError += (path, msg) =>
            _luaErrorOverlay.SetError($"{System.IO.Path.GetFileName(path)}: {msg}");
        Window.TextInput += (sender, e) => { if (_luaConsole.IsOpen) _luaConsole.Append(e.Character); };
#endif
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch    = new SpriteBatch(GraphicsDevice);
        _renderPipeline = new RenderPipeline(GraphicsDevice, Content, isHeadless: false);
        try { _font = Content.Load<SpriteFont>("Fonts/DefaultFont"); }
        catch (Microsoft.Xna.Framework.Content.ContentLoadException) { }
        catch (System.IO.FileNotFoundException) { }
        _battleScene.Initialize(GraphicsDevice, _font);
    }

    protected override void Update(GameTime gameTime)
    {
        _keyboard.Update();

        if (_pendingScene is not null)
        {
            _currentScene = _pendingScene;
            _pendingScene = null;
        }

        _currentScene.Update(gameTime);

        var ksGlobal = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        if ((ksGlobal.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift)
             || ksGlobal.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
            && !_prevKsGlobal.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift)
            && !_prevKsGlobal.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
        {
            _turboLevel = (_turboLevel + 1) % 3;
            TargetElapsedTime = _turboLevel switch
            {
                1 => TimeSpan.FromSeconds(1.0 / 120.0),
                2 => TimeSpan.FromSeconds(1.0 / 180.0),
                _ => TimeSpan.FromSeconds(1.0 / 60.0),
            };
        }
        _prevKsGlobal = ksGlobal;

#if DEBUG
        var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.OemTilde)
            && !_prevKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.OemTilde))
            _luaConsole.Toggle();
        if (_luaConsole.IsOpen)
        {
            if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Back)
                && !_prevKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Back))
                _luaConsole.Backspace();
            if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter)
                && !_prevKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
                _luaConsole.Submit(_scriptEngineFactory());
        }
        _prevKeyState = ks;
#endif
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        var clock = _services.GetRequiredService<IGameClock>();
        _renderPipeline.BeginScene(GraphicsDevice);
        _spriteBatch.Begin(samplerState: Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp);
        _currentScene.Draw(_spriteBatch, gameTime);
        _spriteBatch.End();
        _renderPipeline.EndScene(_spriteBatch, clock.GetTimeOfDay());
#if DEBUG
        _luaErrorOverlay.Draw(_spriteBatch, _font);
        _luaConsole.Draw(_spriteBatch, _font);
#endif
        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
#if DEBUG
        _hotReloader?.Dispose();
#endif
        base.Dispose(disposing);
    }
}
