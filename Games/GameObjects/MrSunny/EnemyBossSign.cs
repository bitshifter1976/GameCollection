using System;
using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public class EnemyBossSign : Sprite
{
    public EnemyBossSign(float scale) : base("graphic/mrSunny/enemySign", Vector2.Zero, 0, scale, Color.White, (int)Layer.Platform, CollisionType.None)
    {
        origin= Vector2.Zero;
        scrolling = true;
    }
}
