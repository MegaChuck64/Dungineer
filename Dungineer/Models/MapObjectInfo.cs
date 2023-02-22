using Engine;
using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;

namespace Dungineer.Models;


public struct MapObjectInfo
{
    public string Name { get; set; }

    public string TextureName { get; set; }

    [JsonConverter(typeof(ContentLoader.RectangleJsonConverter))]
    public Rectangle Source { get; set; }
    public string Description { get; set; }

    public int LotteryValue { get; set; }

    public bool Collectable { get; set; }
}
public enum MapObjectType
{
    Human,
    Ghost,
    Arcanium
}