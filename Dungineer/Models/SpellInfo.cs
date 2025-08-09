using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dungineer.Models;

public struct SpellInfo
{
    public string Name { get; set; }
    public string TextureName { get; set; }

    [JsonConverter(typeof(ContentLoader.RectangleJsonConverter))]
    public Rectangle Source { get; set; }
    public string Description { get; set; }
    public int Range { get; set; }
    public int Damage { get; set; }
    public int ManaCost { get; set; }
    public List<string> Effects { get; set; }
}

public enum SpellType
{
    BasicAttack,
    FireBolt,
}