using System;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

[Serializable]
public sealed class Bird : SpriteAnimated
{
    private readonly float deltaX;
    private readonly int sign;

    public Bird() : base("graphic/tankBattle/bird", Vector2.Zero, 8, 12, 0f, 1f, (int)Layer.Bird)
    {
    	scrolling = true;
        var direction = Rand.Int(0, 1);
        var pos = new Vector2();
        if (direction == 1)
        {
            sign = -1;
            pos.X = Manager.DesignWidth;
            flip = false;
        }
        else
        {
    	    sign = 1;
            pos.X = -Width;
            flip = true;
        }
        var factor = Rand.Int(1, 200);
			deltaX = factor / 5f + 0.3f;
        pos.Y = factor;
        position = pos;
        scale = factor/800f;
        origin = new Vector2(texture.Width / (float)FrameCount / 2, (float)texture.Height/2);
        color = Color.Black;
    }

    public override Sprite Update(GameTime gameTime)
    {
        rotation += Rand.Float(-0.005f, 0.005f);
        rotation = MathHelper.Clamp(rotation, -0.7f, 0.7f);
        position.X += deltaX * sign * (float)gameTime.ElapsedGameTime.TotalSeconds;
        position.Y += Rand.Float(-0.5f, 0.5f);

        return base.Update(gameTime);
    }
}
