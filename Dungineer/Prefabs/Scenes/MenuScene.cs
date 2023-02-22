using Dungineer.Components.UI;
using Dungineer.Prefabs.Entities;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Dungineer.Prefabs.Scenes;

public class MenuScene : IPrefab<List<Entity>>
{
    public List<Entity> Instantiate(BaseGame game)
    {
        var ents = new List<Entity>();
        /*
            Maybe a palette?

            #525252         rgb(82, 82, 82)  /grays
            #414141         rgb(65, 65, 65)
            #313131         rgb(49, 49, 49)                            
            #CA3E47         rgb(202, 62, 71) /red

        */

        //wizard imaged
        var imageEnt = new Entity()
            .With(new UIElement
            {
                Position = new Point((game.Width / 2) - 128, game.Height / 2 - 128),
                Size = new Point(256, 256),
            })
            .With(new Image
            {
                Position = Point.Zero,
                Size = new Point(1, 1),
                Source = new Rectangle(512 * game.Rand.Next(0, 8), 0, 512, 512),
                TextureName = "WizardPortraits_512",
                Tint = Color.White
            });
        ents.Add(imageEnt);


        //play button
        var btn = new Entity()
            .With(new UIElement
            {
                Position = new Point((game.Width / 2) - 64, game.Height - (64 * 3)),
                Size = new Point(128, 64),
                OnMouseReleased = (mb) =>
                {
                    SceneManager.ChangeScene("CharacterCreation");
                }
            })
            .With(new SelectItem
            {
                DefaultColor = new Color(82, 82, 82),
                HoverColor = new Color(65, 65, 65),
                PressedColor = new Color(49, 49, 49),
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
                FontName = "consolas_22",
                Text = "Play",
                TextColor = new Color(202, 62, 71),
                Layer = 0.8f,
            });
        ents.Add(btn);

        //cursor
        ents.Add(new CursorPrefab().Instantiate(game));

        return ents;
    }

}
