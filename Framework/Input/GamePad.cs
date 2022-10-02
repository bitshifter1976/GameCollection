using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Framework;

public class GamePad
{
    private GamePadState currentState;
    private GamePadState prevState;
    private PlayerIndex index;

    public void Initialize(PlayerIndex playerIndex)
    {
        index = playerIndex;
        currentState = Microsoft.Xna.Framework.Input.GamePad.GetState(index);
    }

    public bool IsButtonPressed(Buttons button)
    {
        return currentState.IsConnected && prevState.IsButtonDown(button) && currentState.IsButtonUp(button);
    }

    public bool IsButtonDown(Buttons button)
    {
        return currentState.IsConnected && currentState.IsButtonDown(button);
    }

    public bool IsButtonUp(Buttons button)
    {
        return currentState.IsConnected && currentState.IsButtonUp(button);
    }

    public void Update()
    {
        prevState = currentState;
        currentState = Microsoft.Xna.Framework.Input.GamePad.GetState(index);
    }

    public void Reset()
    {
        prevState = new GamePadState();
        currentState = new GamePadState();
    }
}