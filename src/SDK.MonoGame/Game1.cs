namespace SDK.MonoGame;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDK.Core.Interfaces;
using SDK.MonoGame.Input;
using SDK.MonoGame.Rendering;
using SDK.MonoGame.World;

public class Game1 : Game
{
    private readonly IServiceProvider _services;
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch            _spriteBatch = null!;
    private RenderPipeline         _renderPipeline = null!;
    private WorldSystem            _world = null!;
    private PlayerSystem           _player = null!;
    private KeyboardInputProvider  _keyboard = null!;

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
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch    = new SpriteBatch(GraphicsDevice);
        _renderPipeline = new RenderPipeline(GraphicsDevice, Content, isHeadless: false);
    }

    protected override void Update(GameTime gameTime)
    {
        _keyboard.Update();
        _world.Update(gameTime.ElapsedGameTime);
        _player.Update();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        var clock = _services.GetRequiredService<IGameClock>();
        _renderPipeline.BeginScene(GraphicsDevice);
        // Plan 03-04+ : TilemapRenderer + PlayerSystem sprite
        _renderPipeline.EndScene(_spriteBatch, clock.GetTimeOfDay());
        base.Draw(gameTime);
    }
}
