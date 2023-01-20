using Engine;
using Microsoft.Xna.Framework.Graphics;

namespace GameCode.Entities;

public class TileSelector : TileEntity
{
    Map map;
    int tileSize;
    public TileSelector(BaseGame game, MapTile tile, Map map, int tileSize) : base(game, tile)
    {
        this.map = map;
        this.tileSize = tileSize;
    }

    public override void Update(float dt)
    {
        var newPos =  (Game.MouseState.Position.X / tileSize, Game.MouseState.Position.Y / tileSize);
        if (newPos.Item1 >= 0 && newPos.Item1 < map.Width && newPos.Item2 >= 0 && newPos.Item2 < map.Height)
        {
            Tile.X = newPos.Item1;
            Tile.Y = newPos.Item2;
        }
    }

    public override void Draw(SpriteBatch sb)
    {
    }

}