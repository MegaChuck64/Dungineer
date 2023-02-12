using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Systems;
public class PlayerSystem : BaseSystem
{
    private KeyboardState keyState;
    private KeyboardState lastKeyState;
    private float moveTimer = 0f;
    public PlayerSystem(BaseGame game) : base(game)
    {
    }

    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        lastKeyState = keyState;
        keyState = Keyboard.GetState();

        var playerEntity = entities.FirstOrDefault(t => t.Components.Any(c => c is Player));
        var player = playerEntity?.GetComponent<Player>();
        if (player == null) return;
        var transform = playerEntity.GetComponent<Transform>();
        if (transform == null) return;

        moveTimer += dt;
        if (moveTimer > 1f/player.MoveSpeed)
        {
            moveTimer = 0f;

            if (keyState.IsKeyDown(Keys.W))
            {
                transform.Position -= Vector2.UnitY * transform.Size.Y;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                transform.Position += Vector2.UnitY * transform.Size.Y;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                transform.Position += Vector2.UnitX * transform.Size.X;
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                transform.Position -= Vector2.UnitX * transform.Size.X;
            }
        }

        
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
    }
}