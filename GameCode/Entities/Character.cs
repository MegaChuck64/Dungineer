using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameCode.Entities;

public class Character : TileObject
{
    PathFinder pathFinder;
    TileMap map;

    public CharRace Race { get; set; }
    public CharClass @Class { get; set; }
    public int MaxHealth { get; set; } = 20;
    public int Health { get; set; } = 20;

    /// <summary>
    /// Ability to use strength
    /// Maybe just percent. If stamina = max, then combat hit damage = strength * 1
    /// could use that technique and give bonus, like add 20%. if stam = max then dmg = strength * 1.2
    /// </summary>
    public int MaxStamina { get; set; } = 20;
    public int Stamina { get; set; } = 20;    
    
    /// <summary>
    /// Used in combat
    /// </summary>
    public int Strength { get; set; } = 10;

    /// <summary>
    /// Tiles Per Tick
    /// </summary>
    public int Speed { get; set; } = 1;
    

    public Character(
        CharRace race,
        CharClass @class,
        BaseGame game, 
        string name, 
        TileType tileType, 
        Texture2D texture, 
        Vector2 pos, 
        TileMap map,
        bool hasCollider = false) 
        : base(game, name, tileType, texture, pos, hasCollider)
    {
        Race = race;
        Class = @class;
        pathFinder = new PathFinder(game, map);
        this.map = map;
    }

    public void SetTarget((int x, int y) targetPos)
    {
        pathFinder.CreatePath(map.WorldToMapPosition(Transform.Position), targetPos, true, false);
    }


    public override void Update(float dt)
    {
        base.Update(dt);
        if (Game.KeyState.WasKeyJustDown(Keys.OemTilde))
        {
            pathFinder.showDebug = !pathFinder.showDebug;
        }
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
        pathFinder.Draw(sb);
    }
    public enum CharRace
    {
        Unknown,
        Human,
    }
    public enum CharClass
    {
        Unknown,
        Fighter,
    }
}