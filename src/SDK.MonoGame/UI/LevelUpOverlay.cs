namespace SDK.MonoGame.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDK.Core.ValueObjects;
using SDK.MonoGame.Input;

public sealed class LevelUpOverlay
{
    public bool IsVisible { get; private set; }

    private string _nickname = "";
    private int _newLevel;
    private int _dAtk, _dDef, _dSpA, _dSpD, _dSpe, _dHp;

    public void Trigger(BattlePokemon before, BattlePokemon after)
    {
        _nickname = after.Nickname;
        _newLevel  = after.Level;
        _dAtk = after.Attack         - before.Attack;
        _dDef = after.Defense        - before.Defense;
        _dSpA = after.SpecialAttack  - before.SpecialAttack;
        _dSpD = after.SpecialDefense - before.SpecialDefense;
        _dSpe = after.Speed          - before.Speed;
        _dHp  = after.MaxHp          - before.MaxHp;
        IsVisible = true;
    }

    public void Update(KeyboardState ks, KeyboardState prevKs)
    {
        if (IsVisible && ks.IsKeyDown(InputMap.Confirm) && !prevKs.IsKeyDown(InputMap.Confirm))
            IsVisible = false;
    }

    public void Draw(SpriteBatch sb, Texture2D pixel, SpriteFont? font)
    {
        if (!IsVisible || font is null) return;

        sb.Draw(pixel, new Rectangle(0, 179, 480, 91), new Color(15, 15, 15));

        sb.DrawString(font, $"{_nickname} grew to Lv.{_newLevel}!",
            new Vector2(8f, 185f), Color.Yellow, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);

        string line1 = $"+ATK {_dAtk:+0;-0}   +DEF {_dDef:+0;-0}   +SpA {_dSpA:+0;-0}";
        string line2 = $"+SpD {_dSpD:+0;-0}   +Spe {_dSpe:+0;-0}   +HP {_dHp:+0;-0}";
        sb.DrawString(font, line1, new Vector2(8f, 205f), Color.White, 0f, Vector2.Zero, 0.50f, SpriteEffects.None, 0f);
        sb.DrawString(font, line2, new Vector2(8f, 220f), Color.White, 0f, Vector2.Zero, 0.50f, SpriteEffects.None, 0f);

        sb.DrawString(font, "Space", new Vector2(432f, 255f), Color.DimGray, 0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
    }
}
