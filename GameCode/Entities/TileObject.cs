using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameCode.Entities;

public class TileObject
{
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Texture2D Sprite { get; set; }
    public List<string> Flags { get; set; }

}

public class GroundTile : TileObject
{
    public float SpeedMod { get; set; } = 1f;
}

public class Weapon : TileObject
{
    public string Description { get; set; }
    public string Damage { get; set; }
    public string Rarity { get; set; }
    public string Range { get; set; }
    public int Weight { get; set; }
    public List<string> Requirements { get; set; }

}

public class Character : TileObject
{
    public string Description { get; set; }
    public string Race { get; set; }
    public string Class { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int HealthRegen { get; set; }
    public int Stamina { get; set; }
    public int MaxStamina { get; set; }
    public int StaminaRegen { get; set; }
    public int Strength { get; set; }
    public int Speed { get; set; }
    public int Armor { get; set; }
    public string Size { get; set; }
    public string Weapon { get; set; }

}