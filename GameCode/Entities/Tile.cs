using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameCode.Entities;

public class Tile : Sprite
{
    public bool HasCollider { get; set; }
    public TileType TileType { get; private set; }
    public Tile(BaseGame game, TileType tileType, Texture2D texture, Vector2 pos, bool hasCollider = false) : 
        base(game, texture, pos)
    {
        TileType = tileType;
        HasCollider = hasCollider;
    }


    public override void Update(float dt)
    {
        base.Update(dt);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
    }
}