using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;

namespace Dungineer.Prefabs.Entities;

public class CursorPrefab : IPrefab<Entity>
{
    public Entity Instantiate(BaseGame game)
    {
        var entity = new Entity(game)
            .With(new Transform
            {
                Position = Vector2.Zero,
                Size = new Vector2(16, 16),
                Layer = 0.5f
            })
            .With(new Sprite
            {
                TextureName = "cursor_16",
                Source = new Rectangle(0, 0, 16, 16),
                Tint = Color.White,
                Offset = Vector2.Zero
            })
            .With(new Tag
            {
                Value = "Cursor"
            });


        return entity;
    }
}
