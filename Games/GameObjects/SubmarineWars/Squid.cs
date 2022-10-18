using System;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public sealed class Squid : SpriteAnimated
{
    private readonly float deltaX;
    private readonly int sign;

    public static Sprite Create(int probabilityToCreateNewOne)
    {
        Squid obj = null;
        if (probabilityToCreateNewOne > 0 && Rand.Bool(1, probabilityToCreateNewOne))
            obj = new Squid();
        return obj;
    }

    public Squid() : base("graphic/submarineWars/squid", Vector2.Zero, 5, 2.5f, 0, 1, (int)Layer.Fish)
    {
    	scrolling = true;
        var direction = Rand.Int(0, 1);
        if (direction == 1)
        {
            sign = -1;
            position.X = Manager.DesignWidth;
            flip = false;
        }
        else
        {
    	    sign = 1;
            position.X = -Width;
            flip = true;
        }
        var factor = Rand.Int(1, 200);
		deltaX = factor / 5f + 0.3f;
        position.Y = Rand.Float(Water.TopPixel.Max(p => p.Value) + Height*2, Manager.DesignHeight - Water.GroundYOffset - Height*2);
        scale = factor/300f;
        origin = new Vector2(texture.Width / (float)FrameCount / 2, (float)texture.Height/2);
        color = Color.White;
    }

    public override Sprite Update(GameTime gameTime)
    {
        rotation += Rand.Float(-0.005f, 0.005f);
        rotation = MathHelper.Clamp(rotation, -0.7f, 0.7f);
        position.X += deltaX * sign * (float)gameTime.ElapsedGameTime.TotalSeconds;
        position.Y += Rand.Float(-2f, 2f);

        return base.Update(gameTime);
    }
}
