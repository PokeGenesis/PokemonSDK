using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

    public OverworldScene(GraphicsDevice gfx) => _gfx = gfx;

    public void LoadContent(ContentManager content)
    {
        _tileset = content.Load<Texture2D>("Sprites/Tileset");
        _font    = content.Load<SpriteFont>("Fonts/DefaultFont");

        _playerTex = new Texture2D(_gfx, 1, 1);
        _playerTex.SetData(new[] { Color.Yellow });

        _npcTex = new Texture2D(_gfx, 1, 1);
        _npcTex.SetData(new[] { Color.Magenta });
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

        _dialogue = string.Empty;
        if (kb.IsKeyDown(Keys.Space))
        {
            var px = (int)MathF.Round(_playerPos.X);
            var py = (int)MathF.Round(_playerPos.Y);
            foreach (var (dx, dy) in new[] { (0, -1), (0, 1), (-1, 0), (1, 0) })
            {
                var nx = px + dx; var ny = py + dy;
                if (nx >= 0 && nx < TilemapData.Width &&
                    ny >= 0 && ny < TilemapData.Height &&
                    TilemapData.IsNpc(TilemapData.Map[ny, nx]))
                {
                    _dialogue = "PNJ : Bienvenue dans PokeForge StarterGame !";
                    // TODO 09-04 : LuaScriptEngine.Execute("npc_dialogue.lua") via SDK.Scripting
                }
            }
        }
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

        if (!string.IsNullOrEmpty(_dialogue))
        {
            sb.Draw(_playerTex,
                new Rectangle(10, 10, TilemapData.Width * TD - 20, 40), new Color(0, 0, 0, 180));
            sb.DrawString(_font, _dialogue, new Vector2(20, 18), Color.White);
        }

        sb.End();
    }
}
