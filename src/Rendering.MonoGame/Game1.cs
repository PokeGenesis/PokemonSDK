using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PokemonSDK.Rendering.MonoGame
{
    public enum ScreenSize
    {
        S,  // Petite
        M,  // 640x480
        L,  // 1280x720 (défaut)
        XL  // 1920x1080
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Taille actuelle (changeable plus tard via un menu/options)
        private ScreenSize _currentSize = ScreenSize.L; // L = 1280x720 par défaut

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            ApplyScreenSize(_currentSize);
        }

        private void ApplyScreenSize(ScreenSize size)
        {
            switch (size)
            {
                case ScreenSize.S:
                    _graphics.PreferredBackBufferWidth  = 480;
                    _graphics.PreferredBackBufferHeight = 360;
                    break;
                case ScreenSize.M:
                    _graphics.PreferredBackBufferWidth  = 640;
                    _graphics.PreferredBackBufferHeight = 480;
                    break;
                case ScreenSize.L: // 720p
                    _graphics.PreferredBackBufferWidth  = 1280;
                    _graphics.PreferredBackBufferHeight = 720;
                    break;
                case ScreenSize.XL:
                    _graphics.PreferredBackBufferWidth  = 1920;
                    _graphics.PreferredBackBufferHeight = 1080;
                    break;
            }

            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            // Exemple : changer la résolution au clavier (tu pourras remplacer ça par un menu plus tard)
            if (keyboard.IsKeyDown(Keys.F1)) { _currentSize = ScreenSize.S;  ApplyScreenSize(_currentSize); }
            if (keyboard.IsKeyDown(Keys.F2)) { _currentSize = ScreenSize.M;  ApplyScreenSize(_currentSize); }
            if (keyboard.IsKeyDown(Keys.F3)) { _currentSize = ScreenSize.L;  ApplyScreenSize(_currentSize); }
            if (keyboard.IsKeyDown(Keys.F4)) { _currentSize = ScreenSize.XL; ApplyScreenSize(_currentSize); }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            // Pas encore de texte/sprites : juste un fond bleu
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
