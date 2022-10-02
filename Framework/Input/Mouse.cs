using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Framework;

public partial class InputManager
{
    public enum MouseButton
    {
        Left,
        Middle,
        Right
    }
    
    private class Mouse
    {
        public MouseState State { get; private set; }
        public MouseState PrevState { get; private set; }
        public Point Position => new(State.X, State.Y);

        public void Initialize()
        {
            State = new MouseState();
        }

        public void Update()
        {
            PrevState = State;
            State = Microsoft.Xna.Framework.Input.Mouse.GetState();
        }

        public void Reset()
        {
            PrevState = new MouseState();
            State = new MouseState();
        }

        public bool IsButtonPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return State.LeftButton == ButtonState.Released && PrevState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return State.MiddleButton == ButtonState.Released && PrevState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return State.RightButton == ButtonState.Released && PrevState.LeftButton == ButtonState.Pressed;
            }

            return false;
        }
    }
}