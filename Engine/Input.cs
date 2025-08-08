using Dungineer.Systems;
using Microsoft.Xna.Framework.Input;

namespace Engine;

public static class Input
{
    public static KeyboardState KeyState { get; private set; }
    public static KeyboardState PrevKeyState { get; private set; }
    public static MouseState MouseState { get; private set; }
    public static MouseState PrevMouseState { get; private set; }

    public static void Update()
    {
        PrevKeyState = KeyState;
        KeyState = Keyboard.GetState();

        PrevMouseState = MouseState;
        MouseState = Mouse.GetState();
    }

    public static bool IsDown(Keys key)
    {
        return KeyState.IsKeyDown(key);
    }

    public static bool IsDown(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => MouseState.LeftButton == ButtonState.Pressed,
            MouseButton.Right => MouseState.RightButton == ButtonState.Pressed,
            MouseButton.Middle => MouseState.MiddleButton == ButtonState.Pressed,
            _ => false,
        };
    }

    public static bool WasPressed(Keys key)
    {
        return KeyState.IsKeyDown(key) && PrevKeyState.IsKeyUp(key);
    }

    public static bool WasPressed(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => MouseState.LeftButton == ButtonState.Pressed && PrevMouseState.LeftButton == ButtonState.Released,
            MouseButton.Right => MouseState.RightButton == ButtonState.Pressed && PrevMouseState.RightButton == ButtonState.Released,
            MouseButton.Middle => MouseState.MiddleButton == ButtonState.Pressed && PrevMouseState.MiddleButton == ButtonState.Released,
            _ => false,
        };
    }

    public static bool WasReleased(Keys key)
    {
        return KeyState.IsKeyUp(key) && PrevKeyState.IsKeyDown(key);
    }

    public static bool WasReleased(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => MouseState.LeftButton == ButtonState.Released && PrevMouseState.LeftButton == ButtonState.Pressed,
            MouseButton.Right => MouseState.RightButton == ButtonState.Released && PrevMouseState.RightButton == ButtonState.Pressed,
            MouseButton.Middle => MouseState.MiddleButton == ButtonState.Released && PrevMouseState.MiddleButton == ButtonState.Pressed,
            _ => false,
        };
    }

}