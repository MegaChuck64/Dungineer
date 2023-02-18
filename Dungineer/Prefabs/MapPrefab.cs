using Dungineer.Components;
using Engine;
using Engine.Components;

namespace Dungineer.Prefabs;

public class MapPrefab : IPrefab<Entity>
{
    public Entity Instantiate(BaseGame game)
    {
        int mapWidth = 25;
        int mapHeight = 20;
        var tiles = new byte[mapWidth, mapHeight];
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
            Position = new Microsoft.Xna.Framework.Vector2(),
            Layer = 0.5f,
        };


        var ent = new Entity(game)
            .With(map)
            .With(trn);

        return ent;
    }
}