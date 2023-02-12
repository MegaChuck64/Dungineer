using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;

namespace Dungineer.Prefabs;

public class PlayerPrefab : IPrefab<Entity>
{
    private string name;
    private string description;
    private int portraitIndex;
    private int spriteIndex;
    public PlayerPrefab(string name, string description, int portraitIndex, int spriteIndex)
    {
        this.name = name;
        this.description = description;
        this.portraitIndex = portraitIndex;
        this.spriteIndex = spriteIndex;
    }
    public Entity Instantiate(BaseGame game)
    {
        var ent = new Entity(game);
        var tran = new Transform(ent)
        {
            Position = new Vector2(game.Width/2, game.Height/2),
            Size = new Vector2(64, 64), 
            Layer = 0.8f,
        };
        var player = new Player(ent)
        {
            Health = 20,
            MaxHealth = 20,
            MoveSpeed = 4,
            Name = name,
            Description = description,
            PotraitIndex = portraitIndex
        };
        var spr = new Sprite(ent)
        {
            Source = new Rectangle(32 * spriteIndex, 0, 32, 32),
            TextureName = "GnomeMage_32",
            Tint = Color.White,
        };

        var ms = new MouseInput(ent)
        {
            OnMousePressed = (mb) =>
            {
                player.Health -= player.Health > 0 ? 1 : 0;
            }
        };
        return ent;
    }
}