using Dungineer.Components.GameWorld;
using Dungineer.Prefabs.Entities;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        return ents;
    }
}