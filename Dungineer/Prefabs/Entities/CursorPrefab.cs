using Dungineer.Components.UI;
using Engine;
using Microsoft.Xna.Framework;

namespace Dungineer.Prefabs.Entities;

public class CursorPrefab : IPrefab<Entity>
{
    public Entity Instantiate(BaseGame game)
    {
        var cursor = new Entity()
            .With(new UIElement
            {
                Position = Point.Zero,
                Size = new Point(16, 16),
            })
            .With(new Image
            {
                Layer = 0.9f,
                Position = Point.Zero,
                Size = new Point(1, 1),
                Source = new Rectangle(0, 0, 16, 16),
                TextureName = "cursor_16",
                Tint = Color.White
            })
            .WithTag("Cursor");

        return cursor;
    }
}
