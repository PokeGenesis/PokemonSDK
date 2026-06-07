using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using StarterGame.Scenes;
using StarterGame.World;

namespace StarterGame;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly bool _headless;
    private SpriteBatch _spriteBatch = null!;
    private OverworldScene _scene = null!;

    public Game1(bool headless = false)
    {
        _headless = headless;
        _graphics = new GraphicsDeviceManager(this)
        {
            // 20 tuiles × 48px = 960 | 15 tuiles × 48px = 720
            PreferredBackBufferWidth  = TilemapData.Width  * TilemapData.TilePixels * TilemapData.DisplayScale,
            PreferredBackBufferHeight = TilemapData.Height * TilemapData.TilePixels * TilemapData.DisplayScale,
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize(); // appelle LoadContent() avant de retourner
        if (_headless)
        {
            Console.WriteLine("StarterGame: headless mode — exiting cleanly");
            Exit();
        }
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _scene = new OverworldScene(GraphicsDevice);
        _scene.LoadContent(Content);

        if (!_headless)
        {
            try
            {
                var bgm = Content.Load<Song>("Music/bgm");
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.5f;
                MediaPlayer.Play(bgm);
            }
            catch { /* pas de hardware audio en CI headless */ }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        var kb = Keyboard.GetState();
        if (kb.IsKeyDown(Keys.Escape)) Exit();
        _scene.Update(gameTime, kb);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _scene.Draw(_spriteBatch);
        base.Draw(gameTime);
    }
}
