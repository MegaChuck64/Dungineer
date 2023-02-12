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
        var statEntity = new Entity(game);
        var stTran = new Transform(statEntity)
        {
            Layer = 0.7f,
            Position = new Vector2(2,2),
            Size = new Vector2(game.Width/5 - 2, game.Height - 4)
        };
        var stSpr = new Sprite(statEntity)
        {
            Source = new Rectangle(0,0,1,1),
            TextureName = "_pixel",
            Tint = new Color(20,20,20),             
        };
        var stTxt = new Text(statEntity)
        {
            Content = string.Empty,
            Tint = Color.White,
            FontName = "consolas_12",
            Offset = new Vector2(2, 2)
        };
        var stat = new StatPanel(statEntity);

        ents.Add(statEntity);

        //terminal
        

        return ents;
    }
}