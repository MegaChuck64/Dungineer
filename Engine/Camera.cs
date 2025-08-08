using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine;

public class Camera
{
    public float Zoom { get; private set; }
    public Vector2 Position { get; set; }
    public float Rotation { get; private set; }

    public Camera()
    {
        Zoom = 1.0f;
        Rotation = 0.0f;
        Position = Vector2.Zero;
    }

    public void PerformZoom(float amount)
    {
        Zoom += amount;
        if (Zoom < 0.1f) Zoom = 0.1f;
    }

    public void Rotate(float angle)
    {
        Rotation += angle;
    }

    public void Move(Vector2 amount)
    {
        Position += amount;
    }

    public Matrix Transform(GraphicsDevice graphicsDevice) =>
        Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
        Matrix.CreateRotationZ(Rotation) *
        Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
        Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));

}