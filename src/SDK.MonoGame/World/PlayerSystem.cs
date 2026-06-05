namespace SDK.MonoGame.World;

using Microsoft.Xna.Framework;
using SDK.Core.Interfaces;

public class PlayerSystem
{
    private readonly WorldSystem    _world;
    private readonly IInputProvider _input;

    public Vector2 Position { get; private set; } = new Vector2(240f, 135f);  // centre 480×270
    public const int TileSize = 16;  // D-14 : tiles 16×16

    public PlayerSystem(WorldSystem world, IInputProvider input)
    {
        _world = world;
        _input = input;
    }

    public void Update()
    {
        var moved = false;
        if (_input.IsActionJustPressed("Up"))    { Position -= new Vector2(0, TileSize); moved = true; }
        if (_input.IsActionJustPressed("Down"))  { Position += new Vector2(0, TileSize); moved = true; }
        if (_input.IsActionJustPressed("Left"))  { Position -= new Vector2(TileSize, 0); moved = true; }
        if (_input.IsActionJustPressed("Right")) { Position += new Vector2(TileSize, 0); moved = true; }

        if (moved) _ = _world.CheckWildEncounter();
    }
}
