using Engine;
using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;

namespace Dungineer.Models;

public struct WardrobeInfo
{
    public string Name { get; set; }
    public string TextureName { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WardrobeSlot Slot { get; set; }

    public string Description { get; set; }
    public int Cost { get; set; }
    public float HealthMod { get; set; }
    public float StaminaMod { get; set; }
    public float MoveSpeedMod { get; set; }


    [JsonConverter(typeof(ContentLoader.RectangleJsonConverter))]
    public Rectangle Source { get; set; }
}

public enum WardrobeSlot
{
    Hair,
    Head,
    Beard,
    Body,
    Arm,
    Leg,
    Shoe,
}

public enum WardrobeType
{
    BasicRobe,
    ProperRobe,
    GrimRobe,
    MasterRobe
}
