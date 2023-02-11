using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;

namespace Dungineer.Prefabs;

public class CursorPrefab : IPrefab<Entity>
{
    public Entity Instantiate (BaseGame game)
    {
        var entity = new Entity(game);

        var trn = new Transform(entity)
        {
            Position = Vector2.Zero,
            Size = new Vector2(16,16),
            Layer = 0.9f
        };
        var spr = new Sprite(entity)
        {
            TextureName = "cursor_16",
            Source = new Rectangle(0,0, 16, 16),
            Tint = Color.White,
        };
        var cursor = new Cursor(entity);


        return entity;
    }
}
