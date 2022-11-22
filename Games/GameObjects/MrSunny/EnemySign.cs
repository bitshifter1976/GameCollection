using System;
using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public class EnemySign : Sprite
{
    public EnemySign(float scale) : base("graphic/mrSunny/enemySign", Vector2.Zero, 0, scale, Color.White, (int)Layer.Platform, CollisionType.None)
    {
        origin= Vector2.Zero;
        scrolling = true;
    }
}
