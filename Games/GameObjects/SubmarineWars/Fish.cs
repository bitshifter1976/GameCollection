using System;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.SubmarineWars;

[Serializable]
public sealed class Fish : SpriteAnimatedMultiLine
{
    private readonly float deltaX;
    private readonly int sign;

    public Fish() : base("graphic/submarineWars/fish", Vector2.Zero, 12, 3, 6, 0f, 0.5f, (int)Layer.Fish)
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
        scale = factor/800f;
        origin = new Vector2(texture.Width / (float)FrameCount / 2, (float)texture.Height/2);
        color = Color.White;
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
