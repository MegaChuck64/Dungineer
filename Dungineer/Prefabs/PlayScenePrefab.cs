using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Dungineer.Prefabs;

public class PlayScenePrefab : IPrefab<List<Entity>>
{
    public List<Entity> Instantiate(BaseGame game)
    {
        var ents = new List<Entity>();


        //stats
        //info
        //terminal
        //inventory
        //play area


        //play area -----
        //middle top
        //map
        //player
        //items


        //info ----
        //right top


        //terminal ----
        //middle bottom

        //stats ----
        //left top

        //inventory
        //left middle


        //cursor
        var cursorPrefab = new CursorPrefab();
        var cursor = cursorPrefab.Instantiate(game);
        ents.Add(cursor);

        //stats
        var statEntity = new Entity(game)
            .With(new Transform
            {
                Layer = 0.7f,
                Position = new Vector2(2 * game.WindowRatio,2 * game.WindowRatio),
                Size = new Vector2(game.Width/5 - 2, game.Height - 4)
            })
            .With(new Sprite
            {
                Source = new Rectangle(0,0,1,1),
                TextureName = "_pixel",
                Tint = new Color(20,20,20),             
                Offset = Vector2.Zero,
            })
            .With(new Text
            {
                Content = string.Empty,
                Tint = Color.White,
                FontName = "consolas_12",
                Offset = new Vector2(2 * game.WindowRatio, 2 * game.WindowRatio)
            })
            .With(new Tag
            { 
                Value = "Stat Panel" 
            });

        ents.Add(statEntity);


        //portrait
        var potrait = new Entity(game)
            .With(new Transform
            {
                Position = new Vector2(game.Width/5 - (128f) - (2 * game.WindowRatio), 4 * game.WindowRatio),
                Layer = 0.7f,
                Size = new Vector2(128f, 128f),
            })
            .With(new Sprite
            {
                Offset = Vector2.Zero,
                Source = new Rectangle(0, 0, 512, 512),
                TextureName = "WizardPortraits_512",
                Tint = Color.White,
            })
            .With(new Tag
            {
                Value = "Portrait"
            });

        ents.Add(potrait);

        
        //map
        ents.Add(new MapPrefab().Instantiate(game));


        return ents;
    }
}