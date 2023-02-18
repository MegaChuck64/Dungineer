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
        var player = new Player
        {
            Health = 20,
            MaxHealth = 20,
            MoveSpeed = 4,
            Name = name,
            Description = description,
            PotraitIndex = portraitIndex,
            Source = new Rectangle(32 * spriteIndex, 0, 32, 32),
            Tint = Color.White,
        };

        return new Entity(game)
            .With(new Transform
            {
                Position = new Vector2(4, 4),
                Layer = 0.8f,
            })
            .With(player)
            .With(new MouseInput
            {
                OnMousePressed = (mb) =>
                {
                    player.Health -= player.Health > 0 ? 1 : 0;
                }
            });
    }
}