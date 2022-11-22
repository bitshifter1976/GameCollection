using System;
using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public class Stone : Sprite
{
    public float TopOffset => 10 * scale;
    public float RightOffset => 50 * scale;

    public override Rectangle BoundingBox => new((int)position.X, (int)(position.Y + TopOffset), (int)(Width - RightOffset), (int)(Height- TopOffset));

    public Stone(float scale) : base("graphic/mrSunny/stone", Vector2.Zero, 0, scale, Color.White, (int)Layer.Platform, CollisionType.BoundingBox)
    {
        origin= Vector2.Zero;
        scrolling = true;
    }
}
