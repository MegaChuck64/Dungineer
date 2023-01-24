using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Security.Cryptography.X509Certificates;

namespace GameCode.Entities;

public class TileSelector : Entity
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int TileSize { get; set; }
    public Texture2D Texture { get; set; }
    public Color Tint { get; set; }
    public TileSelector(BaseGame game, Texture2D texture, int width, int height, int tileSize) : base(game)
    {
        Texture = texture;
        TileSize = tileSize;
        Width = width;
        Height = height;
        Tint = Color.White;
    }

    public override void Update(float dt)
    {
        (int newX, int newY) = 
            (Game.MouseState.Position.X / TileSize, Game.MouseState.Position.Y / TileSize);

        if (newX >= 0 && newX < Width && newY >= 0 && newY < Height)
        {
            X = newX;
            Y = newY;
        }
    }

    public override void Draw(SpriteBatch sb)
    {
        sb.Draw(Texture, new Rectangle(TileSize * X, TileSize * Y, TileSize, TileSize), null, Tint, 0f,
            Vector2.Zero, SpriteEffects.None, 0.9f);
    }

}