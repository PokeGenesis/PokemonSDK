namespace SDK.MonoGame.Rendering;

using Microsoft.Xna.Framework;

public class Camera2D
{
    public Vector2 Position { get; private set; }

    private const int InternalWidth  = 480;
    private const int InternalHeight = 270;

    public Matrix GetTransform()
        => Matrix.CreateTranslation(
            -Position.X + InternalWidth  / 2f,
            -Position.Y + InternalHeight / 2f,
            0f);

    public void Follow(Vector2 target)
        => Position = Vector2.Lerp(Position, target, 0.12f);

    public Rectangle GetVisibleBounds()
        => new Rectangle(
            (int)(Position.X - InternalWidth  / 2f) - 32,
            (int)(Position.Y - InternalHeight / 2f) - 32,
            InternalWidth  + 64,
            InternalHeight + 64);
}
