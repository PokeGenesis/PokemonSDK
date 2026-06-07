using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarterGame;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly bool _headless;
    private SpriteBatch _spriteBatch = null!;
    private SpriteFont _defaultFont = null!;

    public Game1(bool headless = false)
    {
        _headless = headless;
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth  = 1920,
            PreferredBackBufferHeight = 1080
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        if (_headless)
        {
            Console.WriteLine("StarterGame: headless mode — exiting cleanly");
            Exit();
        }
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _defaultFont = Content.Load<SpriteFont>("Fonts/DefaultFont");
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        _spriteBatch.DrawString(
            _defaultFont,
            "PokeForge StarterGame — Appuyer sur Echap pour quitter",
            new Vector2(20, 20),
            Color.White);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
