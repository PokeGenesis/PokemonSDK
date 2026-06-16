namespace SDK.MonoGame.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDK.Core.ValueObjects;
using SDK.MonoGame.Input;

public sealed class MoveLearnOverlay
{
    public bool IsVisible { get; private set; }
    public bool DecisionMade { get; private set; }
    public int ForgottenMoveIndex { get; private set; } = -1;

    private string _nickname = "";
    private BattleMove _newMove = default!;
    private IReadOnlyList<BattleMove> _currentMoves = Array.Empty<BattleMove>();
    private int _cursor;

    public void Trigger(string nickname, BattleMove newMove, IReadOnlyList<BattleMove> currentMoves)
    {
        _nickname = nickname;
        _newMove = newMove;
        _currentMoves = currentMoves;
        _cursor = 0;
        ForgottenMoveIndex = -1;
        DecisionMade = false;
        IsVisible = true;
    }

    public void Update(KeyboardState ks, KeyboardState prevKs)
    {
        if (!IsVisible) return;

        if (ks.IsKeyDown(InputMap.NavUp) && !prevKs.IsKeyDown(InputMap.NavUp))
            _cursor = Math.Max(0, _cursor - 1);

        if (ks.IsKeyDown(InputMap.NavDown) && !prevKs.IsKeyDown(InputMap.NavDown))
            _cursor = Math.Min(_currentMoves.Count - 1, _cursor + 1);

        if (ks.IsKeyDown(InputMap.Confirm) && !prevKs.IsKeyDown(InputMap.Confirm))
        {
            ForgottenMoveIndex = _cursor;
            DecisionMade = true;
            IsVisible = false;
        }

        if (ks.IsKeyDown(InputMap.Cancel) && !prevKs.IsKeyDown(InputMap.Cancel))
        {
            ForgottenMoveIndex = -1;
            DecisionMade = true;
            IsVisible = false;
        }
    }

    public void Draw(SpriteBatch sb, Texture2D pixel, SpriteFont? font)
    {
        if (!IsVisible || font is null) return;

        sb.Draw(pixel, new Rectangle(0, 179, 480, 91), new Color(15, 15, 15));

        sb.DrawString(font, $"{_nickname} wants to learn {_newMove.Identifier}!",
            new Vector2(8f, 182f), Color.Yellow, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
        sb.DrawString(font, "Forget which move? (X = cancel)",
            new Vector2(8f, 196f), new Color(180, 180, 180), 0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);

        for (int i = 0; i < _currentMoves.Count; i++)
        {
            var color = i == _cursor ? Color.Yellow : Color.White;
            string prefix = i == _cursor ? ">" : " ";
            sb.DrawString(font, $"{prefix} {_currentMoves[i].Identifier}",
                new Vector2(16f, 210f + i * 14f),
                color, 0f, Vector2.Zero, 0.50f, SpriteEffects.None, 0f);
        }
    }
}
