using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameCode.Entities;

//Items
//Entities
//Actions

//Components that are capabilities
//turn verb into noun... operation into object

//
/*
    
class item:
    Attack melee;
    Attack ranged;
    Defense defense;
    Use use;


class Attack:
    int minDmg;
    int maxDmg;
    
    void hit();

 
class Defense:
    int armor;
    int dodgeBonus;
    
    void defend();


abstract class Use:
    void use();

class HealUse extends Use:
    void use() {
        hero.health += 20;
    }

class FireBallUse extends Use:
    void use() {
        //Cast fireball
    }



var sword = new Item {
    melee: new Attack(10, 20),
};

var crossbow = new Item {
    ranged: new Attack(10, 20),
};

var shield = new Item {
    melee: new Attack(5, 8),
    defense: new Defense(3, 0),
};

var healPotion = new Item {
    use: new HealUse(),
};

var fireSword = new Item {
    melee: Attack(30, 40),
    use: new FireBallUse()
};


 */
public class TileObject
{
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Texture2D Sprite { get; set; }
    public List<string> Flags { get; set; }
    public TileObject Copy => this.MemberwiseClone() as TileObject;
    
}

public class GroundTile : TileObject
{
    public float SpeedMod { get; set; } = 1f;

    
}

public class ItemTile : TileObject
{
    public string Description { get; set; }
    public int Health { get; set; }
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