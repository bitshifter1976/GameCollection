using Microsoft.Xna.Framework.Input;

namespace Framework;

public class Keyboard
{
    public KeyboardState State { get; private set; }
    public KeyboardState PrevState { get; private set; }

    public void Initialize()
    {
        State = new KeyboardState();
    }

    public void Update()
    {
        PrevState = State;
        State = Microsoft.Xna.Framework.Input.Keyboard.GetState();
    }

    public void Reset()
    {               
        PrevState = new KeyboardState();
        State = new KeyboardState();
    }

    public bool IsKeyPressed(Keys key)
    {
        return PrevState.IsKeyDown(key) && State.IsKeyUp(key);
    }

    public bool IsKeyUp(Keys key)
    {
        return State.IsKeyUp(key);
    }

    public bool IsKeyDown(Keys key)
    {
        return State.IsKeyDown(key);
    }
}