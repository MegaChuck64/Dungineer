using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace GameCode.Entities;

public class CameraController : Entity
{
    public float MoveSpeed { get; set; } = 128f;
    public OrthographicCamera Camera { get; set; }
    public TileMap Map { get; set; }
    public CameraController(BaseGame game, OrthographicCamera cam, TileMap map) : base(game)
    {
        Camera = cam;
        Map = map;
    }

    public override void Update(float dt)
    {
        HandleCameraMovement(dt);
    }

    private void HandleCameraMovement(float dt)
    {
        if (Game.KeyState.IsKeyDown(Keys.W))
        {
            AttemptCameraMove(-Vector2.UnitY, dt);
        }
        if (Game.KeyState.IsKeyDown(Keys.S))
        {
            AttemptCameraMove(Vector2.UnitY, dt);
        }
        if (Game.KeyState.IsKeyDown(Keys.D))
        {
            AttemptCameraMove(Vector2.UnitX, dt);
        }
        if (Game.KeyState.IsKeyDown(Keys.A))
        {
            AttemptCameraMove(-Vector2.UnitX, dt);
        }
    }

    private void AttemptCameraMove(Vector2 movement, float dt)
    {
        var rect = Camera.BoundingRectangle;
        var pos = rect.Position;
        var size = rect.Size;
        var nextPos = pos + movement * MoveSpeed * dt;
        if (nextPos.X <= Map.TileSize * Map.Width - size.Width &&
            nextPos.X >= 0f &&
            nextPos.Y <= Map.TileSize * Map.Height - size.Height &&
            nextPos.Y >= 0f)
        {
            Camera.Move(movement * MoveSpeed * dt);
        }
    }

    public override void Draw(SpriteBatch sb)
    {
    }
}