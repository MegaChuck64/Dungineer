using Engine;
using Microsoft.Xna.Framework.Graphics;

namespace GameCode.Entities;

public abstract class TileEntity : Entity
{
    public MapTile Tile { get; private set; }

    public TileEntity(BaseGame game, MapTile tile) : base(game)
    {
        Tile = tile;
    }
}