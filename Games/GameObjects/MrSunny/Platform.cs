using System;
using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public class Platform : Sprite
{
    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);

    public Platform(Vector2 position, Color color, float rotation, float scale) : base("graphic/mrSunny/platform", position, rotation, scale, color, (int)Layer.Platform, CollisionType.BoundingBoxRotated)
    {
        scrolling = true;
    }

    public override RotatedRectangle GetBoundingBox(Vector2 pos)
    {
        return new RotatedRectangle(new RectangleF(pos.X - Width / 2f, pos.Y - Height / 2f, Width, Height), rotation);
    }
}
