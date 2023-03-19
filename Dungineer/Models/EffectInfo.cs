using Engine;
using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;

namespace Dungineer.Models;

public struct EffectInfo
{
    public string Name { get; set; }
    public string TextureName { get; set; }

    [JsonConverter(typeof(ContentLoader.RectangleJsonConverter))]
    public Rectangle Source { get; set; }
    public string Description { get; set; }

    public int Turns { get; set; }
    public int Damage { get; set; }
}

public enum EffectType
{
    Fire
}