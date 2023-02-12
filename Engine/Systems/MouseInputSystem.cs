using System.Collections.Generic;
using Dungineer.Components;
using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Systems;

public class MouseInputSystem : BaseSystem
{
    private readonly List<MouseInput> entered;
    private MouseState mouseState;
    private MouseState lastMouseState;

    public MouseInputSystem(BaseGame game) : base(game)
    {
        entered = new List<MouseInput>();        
    }

    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        lastMouseState = mouseState;

        mouseState = Mouse.GetState();

        foreach (var entity in entities)
        {
            var transform = entity.GetComponent<Transform>();

            if (transform == null || !transform.IsActive)
                continue;

            var bounds = transform.Bounds;


            if (entity.GetComponent<Cursor>() is Cursor cursor && cursor.IsActive) //todo, move out of engine, or move cursor into engine
            {
                transform.Position = mouseState.Position.ToVector2();
            }

            if (entity.GetComponent<MouseInput>() is MouseInput mi && mi.IsActive)
            {
                if (bounds.Contains(mouseState.Position))
                {
                    HandleMouseEnter(mi);
                    HandleMousePressed(mi);
                    HandleMouseReleased(mi);
                }
                else
                {
                    HandleMouseLeave(mi);
                }

            }         

        }

    }

    private void HandleMouseEnter(MouseInput mi)
    {
        if (!entered.Contains(mi))
        {
            entered.Add(mi);
            mi.OnMouseEnter?.Invoke();
        }
    }

    private void HandleMouseLeave(MouseInput mi)
    {
        if (entered.Contains(mi))
        {
            entered.Remove(mi);
            mi.OnMouseLeave?.Invoke();
        }
    }
    //todo
    private void HandleMousePressed(MouseInput mi)
    {
        if (WasPressed(MouseButton.Left))
            mi.OnMousePressed?.Invoke(MouseButton.Left);

        if (WasPressed(MouseButton.Right))
            mi.OnMousePressed?.Invoke(MouseButton.Right);

        if (WasPressed(MouseButton.Middle))
            mi.OnMousePressed?.Invoke(MouseButton.Middle);
    }

    private void HandleMouseReleased(MouseInput mi)
    {
        if (WasReleased(MouseButton.Left))
            mi.OnMouseReleased?.Invoke(MouseButton.Left);

        if (WasReleased(MouseButton.Right))
            mi.OnMouseReleased?.Invoke(MouseButton.Right);

        if (WasReleased(MouseButton.Middle))
            mi.OnMouseReleased?.Invoke(MouseButton.Middle);
    }

    private bool WasPressed(MouseButton mb) => mb switch
    {
        MouseButton.Left => mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released,
        MouseButton.Middle => mouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released,
        MouseButton.Right => mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released,
        _ => false,
    };

    private bool WasReleased(MouseButton mb) => mb switch
    {
        MouseButton.Left => mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed,
        MouseButton.Middle => mouseState.MiddleButton == ButtonState.Released && lastMouseState.MiddleButton == ButtonState.Pressed,
        MouseButton.Right => mouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed,
        _ => false,
    };


    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {

    }
}

public enum MouseButton
{
    Left,
    Middle,
    Right
}