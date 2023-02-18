using Dungineer.Components;
using Engine;
using Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungineer.Prefabs;

public class MapPrefab : IPrefab<Entity>
{
    public Entity Instantiate(BaseGame game)
    {

        var tiles = new byte[10, 10];
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y= 0; y < tiles.GetLength(1); y++)
            {
                tiles[x, y] = 0;
                if (game.Rand.NextDouble() > 0.75f)
                {
                    tiles[x, y] = 1;
                }
            }
        }
        var map = new Map
        {
            Tiles = tiles
        };

        var trn = new Transform
        {
            Size = new Microsoft.Xna.Framework.Vector2(64, 64),
            Position = new Microsoft.Xna.Framework.Vector2(),
            Layer = 0.5f,
        };


        var ent = new Entity(game)
            .With(map)
            .With(trn);

        return ent;
    }
}