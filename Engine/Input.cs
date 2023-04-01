using Microsoft.Xna.Framework.Input;

namespace Engine;

public static class Input
{
    public static bool WasPressed(MouseButton mb, MouseState mouseState, MouseState lastMouseState) => mb switch
    {
        MouseButton.Left => mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released,
        MouseButton.Middle => mouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released,
        MouseButton.Right => mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released,
        _ => false,
    };

    public static bool WasReleased(MouseButton mb, MouseState mouseState, MouseState lastMouseState) => mb switch
    {
        MouseButton.Left => mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed,
        MouseButton.Middle => mouseState.MiddleButton == ButtonState.Released && lastMouseState.MiddleButton == ButtonState.Pressed,
        MouseButton.Right => mouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed,
        _ => false,
    };

}