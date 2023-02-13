using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;

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
        
        var mapEntity = entities.FirstOrDefault(t => t.Components.Any(c => c is Map));
        if (mapEntity == null) return;
        var map = mapEntity.GetComponent<Map>();
        if (map == null) return;

        var playerEntity = entities.FirstOrDefault(t => t.Components.Any(c => c is Player));
        var player = playerEntity?.GetComponent<Player>();
        if (player == null) return;
        var transform = playerEntity.GetComponent<Transform>();
        if (transform == null) return;

        //moveTimer += dt;
        //if (moveTimer > 1f/player.MoveSpeed)
        //{
        //    moveTimer = 0f;

        //    if (keyState.IsKeyDown(Keys.W))
        //    {
        //        transform.Position -= Vector2.UnitY * transform.Size.Y;
        //    }
        //    if (keyState.IsKeyDown(Keys.S))
        //    {
        //        transform.Position += Vector2.UnitY * transform.Size.Y;
        //    }
        //    if (keyState.IsKeyDown(Keys.D))
        //    {
        //        transform.Position += Vector2.UnitX * transform.Size.X;
        //    }
        //    if (keyState.IsKeyDown(Keys.A))
        //    {
        //        transform.Position -= Vector2.UnitX * transform.Size.X;
        //    }
        //}

        var nextPos = transform.Position / transform.Size;

        if (WasPressed(Keys.W))
            nextPos -= Vector2.UnitY;

        if (WasPressed(Keys.S))
            nextPos += Vector2.UnitY;

        if (WasPressed(Keys.D))
            nextPos += Vector2.UnitX;

        if (WasPressed(Keys.A))
            nextPos -= Vector2.UnitX;

        var nxt = nextPos.ToPoint();
        if (nxt.X >= 0 && nxt.Y >= 0 && nxt.X < map.Tiles.GetLength(0) && nxt.Y < map.Tiles.GetLength(1))
        {
            if (map.Tiles[nxt.X, nxt.Y] == 0)
            {
                transform.Position = nextPos * transform.Size;
            }
        }
        
    }

    private bool WasPressed(Keys key) => lastKeyState.IsKeyUp(key) && keyState.IsKeyDown(key);

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
    }
}