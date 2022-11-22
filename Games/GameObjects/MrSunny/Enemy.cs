using System;
using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public class Enemy : Sprite
{
    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);

    public Enemy(float scale) : base("graphic/mrSunny/enemy", Vector2.Zero, 0, scale, Color.White, (int)Layer.Player, CollisionType.BoundingBoxRotated)
    {
        scrolling = true;
    }

    public override RotatedRectangle GetBoundingBox(Vector2 pos)
    {
        return new RotatedRectangle(new RectangleF(pos.X - Width / 2f, pos.Y - Height / 2f, Width, Height), rotation);
    }
}
