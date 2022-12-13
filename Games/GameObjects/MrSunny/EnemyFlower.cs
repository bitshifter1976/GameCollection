using System;
using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public class EnemyFlower : SpriteAnimatedMultiLine
{
    private int level;

    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);

    public override RotatedRectangle GetBoundingBox(Vector2 pos)
    {
        return new RotatedRectangle(new RectangleF(pos.X - Width / 2f, pos.Y - Height / 2f, Width, Height), rotation);
    }


    public EnemyFlower(float scale, int level) : base("graphic/mrSunny/flower", Vector2.Zero, 30, 5, 10, 0, scale, (int)Layer.Player, true, CollisionType.BoundingBox)
    {
        scrolling = true;
        energy = level;
        damage = Player.MaxEnergy / 5;
        this.level = level;
    }

    public override Sprite Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (energy <= 0)
            return this;

        if (OnScreen && Rand.Bool(1, 100/level))
        {
            // randomly shoot

        }

        return null;
    }
}
