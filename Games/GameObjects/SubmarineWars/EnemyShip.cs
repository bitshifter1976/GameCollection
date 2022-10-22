using System.Linq;
using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public sealed class EnemyShip : Sprite
{
    private readonly int sign;
    private int bombAmount;
    
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
        bombAmount = 5;
    }

    public override Sprite Update(GameTime gameTime)
    {
        if (Rand.Bool(1, 200))
        {
            Shoot();
        }
        if (Rand.Bool(1, 50))
        {
            rotation += Rand.Float(-0.01f, 0.01f);
            rotation = MathHelper.Clamp(rotation, -0.05f, 0.05f);
        }
        position.X += speed * sign * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (Water.TopPixel.ContainsKey((int)position.X))
            position.Y = Water.TopPixel[(int)position.X] - Height / 3f;
        if (position.X < 0)
            return position.X < -Manager.DesignWidth * 2 ? this : null;
        if (position.X > Manager.DesignWidth)
            return position.X > Manager.DesignWidth * 2 ? this : null;
        return null;
    }

    public void Shoot()
    {
        if (OnScreen && bombAmount > 0)
        {
            bombAmount--;
            var up = new Vector2(0, -1);
            var rot = MathHelper.ToRadians(flip ? 45 : -45);
            var rotMatrix = Matrix.CreateRotationZ(rot);
            var shotVelocity = Vector2.Transform(up, rotMatrix);
            var shotPos = new Vector2(flip ? position.X + Width / 3f : position.X - Width / 3f, position.Y);
            SpriteManager.Add(new Bomb(shotPos, shotVelocity, flip, rot));
            Manager.Sound.LoadEffect("shot");
            Manager.Sound.PlayEffect("shot");
        }
    }
}
