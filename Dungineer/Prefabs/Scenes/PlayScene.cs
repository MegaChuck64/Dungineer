using Dungineer.Components.GameWorld;
using Dungineer.Components.UI;
using Dungineer.Prefabs.Entities;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dungineer.Prefabs.Scenes;

public class PlayScene : IPrefab<List<Entity>>
{
    public List<Entity> Instantiate(BaseGame game)
    {
        var ents = new List<Entity>();

        //cursor
        var cursorPrefab = new CursorPrefab();
        var cursor = cursorPrefab.Instantiate(game);
        ents.Add(cursor);

        //map
        var map = new MapPrefab().Instantiate(game);
        ents.Add(map);
        ents.AddRange(new MapItemsPrefab(map.GetComponent<Map>()).Instantiate(game));

        ents.Add(CreateExitButton(game));

        ents.Add(CreateTerminal(game));
        return ents;
    }

    private static Entity CreateTerminal(BaseGame game)
    {
        var terminal = new Entity()
            .With(new UIElement
            {
                Position = new Point(game.Width - ((game.Width / 5) + 1), game.Height - ((game.Height / 5) + 1) - 64 - 2 - 2),
                Size = new Point(game.Width / 5 - 2, game.Height / 5 - 2),
            })
            .With(new Terminal
            {

            });
        return terminal;
    }

    private static Entity CreateExitButton(BaseGame game)
    {

        var btn = new Entity()
            .With(new UIElement
            {
                Position = new Point(game.Width - 128 - 2, game.Height - 64 - 2),
                Size = new Point(128, 64),
                OnMouseReleased = (mb) =>
                {
                    game.Exit();
                }
            })
            .With(new SelectItem
            {
                PressedColor = new Color(82, 82, 82),
                HoverColor = new Color(65, 65, 65),
                DefaultColor = new Color(49, 49, 49),
                SelectedColor = new Color(49, 49, 49),
            })
            .With(new Image
            {
                Layer = 0.8f,
                Position = Point.Zero,
                Size = new Point(1, 1),
                Source = new Rectangle(0, 0, 1, 1),
                TextureName = "_pixel",
                Tint = Color.White,
            })
            .With(new TextBox
            {
                FontName = "ocra_22",
                Text = "Exit",
                TextColor = new Color(202, 62, 71),
                Layer = 0.9f,
            });

        return btn;

    }
}