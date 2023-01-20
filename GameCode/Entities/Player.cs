using Engine;
using Microsoft.Xna.Framework.Graphics;

namespace GameCode.Entities;

public class Player : TileEntity
{
    PathFinder pathFinder;
    Map map;
    public Player(BaseGame game, Map map, MapTile tile) : base(game, tile)
    {
        this.map = map;
        pathFinder = new PathFinder(game, map.ToShortCollisionMap(), 32);        
    }


    public void Target(int x, int y)
    {
        pathFinder.Map = map.ToShortCollisionMap();
        pathFinder.CreatePath((Tile.X, Tile.Y), (x, y), true, false);
    }

    public void TakeStep()
    {
        var chr = (Tile as MapCharacter);
        var steps = chr.Speed;
        var moved = false;
        for (int i = 0; i < steps; i++)
        {
            if (pathFinder.TryStepForward(out (int x, int y)? nextPos))
            {
                if (nextPos != null)
                {
                    moved = true;
                    chr.X = nextPos.Value.x;
                    chr.Y = nextPos.Value.y;
                }
                else break;
            }
            else break;
        }

        if (moved) chr.Stamina--;
    }

    public override void Update(float dt)
    {
        pathFinder.Update(dt);
    }
    public override void Draw(SpriteBatch sb)
    {
        pathFinder.Draw(sb);
    }
}