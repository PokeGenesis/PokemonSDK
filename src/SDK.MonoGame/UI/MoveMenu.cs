namespace SDK.MonoGame.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDK.Core.ValueObjects;
using SDK.MonoGame.Input;

public sealed class MoveMenu
{
    private static readonly Dictionary<int, Color> TypeColors = new()
    {
        [1]  = Color.Gray,
        [2]  = new Color(255, 140, 0),
        [3]  = Color.DodgerBlue,
        [4]  = Color.LimeGreen,
        [5]  = Color.Yellow,
        [6]  = Color.Orchid,
        [7]  = Color.Sienna,
        [8]  = Color.MediumPurple,
        [9]  = Color.SandyBrown,
        [10] = Color.SkyBlue,
        [11] = Color.Violet,
        [12] = Color.OliveDrab,
        [13] = Color.SlateGray,
        [14] = Color.DarkViolet,
        [15] = Color.DarkGray,
        [16] = Color.DimGray,
        [17] = Color.Silver,
        [18] = Color.Pink,
    };

    private readonly IReadOnlyList<BattleMove> _moves;
    private readonly Texture2D _pixel;
    private int _cursorIndex;
    private KeyboardState _prevKeyState;

    public BattleMove? SelectedMove { get; private set; }

    public MoveMenu(IReadOnlyList<BattleMove> moves, GraphicsDevice gd)
    {
        _moves = moves;
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Update(KeyboardState ks)
    {
        if (_moves.Count == 0) return;

        if (ks.IsKeyDown(InputMap.NavUp) && !_prevKeyState.IsKeyDown(InputMap.NavUp))
            _cursorIndex = (_cursorIndex - 1 + _moves.Count) % _moves.Count;
        if (ks.IsKeyDown(InputMap.NavDown) && !_prevKeyState.IsKeyDown(InputMap.NavDown))
            _cursorIndex = (_cursorIndex + 1) % _moves.Count;
        if (ks.IsKeyDown(InputMap.Confirm) && !_prevKeyState.IsKeyDown(InputMap.Confirm))
            SelectedMove = _moves[_cursorIndex];

        _prevKeyState = ks;
    }

    public void ResetSelection() => SelectedMove = null;

    public void Draw(SpriteBatch sb, Vector2 origin, SpriteFont? font)
    {
        for (int i = 0; i < _moves.Count; i++)
        {
            var move = _moves[i];
            var bgColor = TypeColors.TryGetValue(move.TypeId, out var c) ? c * 0.6f : Color.Gray * 0.6f;
            sb.Draw(_pixel, new Rectangle((int)origin.X, (int)origin.Y + i * 14, 110, 12), bgColor);

            if (font != null)
            {
                var label = (i == _cursorIndex ? "> " : "  ") + move.Identifier;
                sb.DrawString(font, label, new Vector2(origin.X + 2f, origin.Y + i * 14f + 1f),
                    Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
            }
        }
    }

    public void Dispose() => _pixel.Dispose();
}
