using Engine;
using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;

namespace Dungineer.Models;

public struct TileInfo
{
    public string Name { get; set; }
    public string TextureName { get; set; }

    [JsonConverter(typeof(ContentLoader.RectangleJsonConverter))]
    public Rectangle Source { get; set; }

    public bool Solid { get; set; }
}


public enum TileType
{
    Grass,
    Water,
    PineTree,
    DungeonWall,
    DungeonFloor,
    DungeonFloorHole,
}