using System;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public sealed class EnemyShip : Sprite
{
    private readonly int sign;
    
    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)position.Y, (int)Width, (int)(Height / 2f));

    public static Sprite Create(int probabilityToCreateNewOne)
    {
        EnemyShip obj = null;
        if (probabilityToCreateNewOne > 0 && Rand.Bool(1, probabilityToCreateNewOne))
            obj = new EnemyShip();
        return obj;
    }

    public EnemyShip() : base("graphic/submarineWars/ship", Vector2.Zero, 0, 1, (int)Layer.Submarine, CollisionType.BoundingBox)
    {
    	scrolling = true;
        var direction = Rand.Int(0, 1);
        if (direction == 1)
        {
            sign = -1;
            position.X = Manager.DesignWidth;
            flip = true;
        }
        else
        {
    	    sign = 1;
            position.X = -Width;
            flip = false;
        }
        position.Y = Water.TopPixel.Max(p => p.Value) - Height / 3f;
		speed = Rand.Float(40,80);
        color = Color.White;
    }

    public override Sprite Update(GameTime gameTime)
    {
        if (Rand.Bool(1, 50))
        {
            rotation += Rand.Float(-0.01f, 0.01f);
            rotation = MathHelper.Clamp(rotation, -0.05f, 0.05f);
        }
        position.X += speed * sign * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (Water.TopPixel.ContainsKey((int)position.X))
            position.Y = Water.TopPixel[(int)position.X] - Height / 3f;
        return base.Update(gameTime);
    }
}
