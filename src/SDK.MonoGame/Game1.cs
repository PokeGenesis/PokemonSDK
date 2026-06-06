namespace SDK.MonoGame;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDK.Core.Interfaces;
using SDK.MonoGame.Input;
using SDK.MonoGame.Rendering;
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
    private WorldSystem            _world = null!;
    private PlayerSystem           _player = null!;
    private KeyboardInputProvider  _keyboard = null!;
    private ISaveSystem            _saveSystem = null!;
    private Func<IScriptEngine>    _scriptEngineFactory = null!;
    private LuaErrorOverlay        _luaErrorOverlay = new();
    private SpriteFont?            _font;
    private Microsoft.Xna.Framework.Input.KeyboardState _prevKeyState;
#if DEBUG
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

    protected override void Initialize()
    {
        _world   = _services.GetRequiredService<WorldSystem>();
        _player  = _services.GetRequiredService<PlayerSystem>();
        _keyboard = (KeyboardInputProvider)_services.GetRequiredService<IInputProvider>();
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
    }

    protected override void Update(GameTime gameTime)
    {
        _keyboard.Update();
        _world.Update(gameTime.ElapsedGameTime);
        _player.Update();
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
        // Plan 03-04+ : TilemapRenderer + PlayerSystem sprite
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
