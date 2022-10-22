using Framework;
using Microsoft.Xna.Framework.Input;

namespace AxeGameCollection.GameObjects.MrSunny;

public static class Action
{
    private const float holdingKeyTime = 1.2f;

    static Action()
    {
        Manager.Input.CreateHoldingKeys(holdingKeyTime, Keys.A, Keys.Left, Keys.D, Keys.Right);
    }

    public static bool WalkLeft
    {
        get { return Manager.Input.KeyDown(Keys.A) || Manager.Input.KeyDown(Keys.Left); }
    }

    public static bool WalkRight
    {
        get { return Manager.Input.KeyDown(Keys.D) || Manager.Input.KeyDown(Keys.Right); }
    }
    
    public static bool RunLeft
    {
        get { return Manager.Input.HoldingKey(Keys.A) || Manager.Input.HoldingKey(Keys.Left); }
    }

    public static bool RunRight
    {
        get { return Manager.Input.HoldingKey(Keys.D) || Manager.Input.HoldingKey(Keys.Right); }
    }

    public static bool Jump
    {
        get { return Manager.Input.KeyDown(Keys.Space) || Manager.Input.KeyDown(Keys.Enter); }
    }

    public static bool Duck
    {
        get { return Manager.Input.KeyDown(Keys.S) || Manager.Input.KeyDown(Keys.Down); }
    }

    public static bool UnDuck
    {
        get { return Manager.Input.KeyUp(Keys.S) || Manager.Input.KeyUp(Keys.Down); }
    }

    public static bool Shoot
    {
        get
        {
            return Manager.Input.KeyPressed(Keys.LeftShift)   || Manager.Input.KeyPressed(Keys.RightShift)   ||
                   Manager.Input.KeyPressed(Keys.LeftControl) || Manager.Input.KeyPressed(Keys.RightControl) ||
                   Manager.Input.KeyPressed(Keys.LeftAlt)     || Manager.Input.KeyPressed(Keys.RightAlt);
        }
    }

    public static bool ShootUp
    {
        get { return Shoot && (Manager.Input.KeyDown(Keys.Up) || Manager.Input.KeyUp(Keys.Up)); }
    }

    public static bool Weapon1
    {
        get { return Manager.Input.KeyPressed(Keys.D1); }
    }

    public static bool Weapon2
    {
        get { return Manager.Input.KeyPressed(Keys.D2); }
    }

    public static bool Weapon3
    {
        get { return Manager.Input.KeyPressed(Keys.D3); }
    }

    public static bool Weapon4
    {
        get { return Manager.Input.KeyPressed(Keys.D4); }
    }

    public static bool Weapon5
    {
        get { return Manager.Input.KeyPressed(Keys.D5); }
    }

    public static bool Weapon6
    {
        get { return Manager.Input.KeyPressed(Keys.D6); }
    }

    public static bool Weapon7
    {
        get { return Manager.Input.KeyPressed(Keys.D7); }
    }

    public static bool Weapon8
    {
        get { return Manager.Input.KeyPressed(Keys.D8); }
    }

    public static bool Weapon9
    {
        get { return Manager.Input.KeyPressed(Keys.D9); }
    }
}
