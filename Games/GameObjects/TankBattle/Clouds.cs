using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

public static class Clouds
{
    public static void Create(int probabilityToCreateNewOne)
    {
        var create = Rand.Bool(1, probabilityToCreateNewOne);
        if (create)
        {
            var windDirectionLeft = Rand.Bool(1, 2);
            var idx = Rand.Int(1, 7);
            var sign = windDirectionLeft ? -1 : 1;
            var speed = Rand.Float(5f, 10f);
            var scale = Rand.Float(0.2f, 0.7f);
            var cloud = new Cloud(idx, speed * sign, Vector2.Zero, 0, scale);
            cloud.Position = new Vector2(windDirectionLeft ? Manager.DesignWidth : -cloud.Width, Rand.Float(-128 * scale, (float)Manager.DesignHeight / 5));

            SpriteManager.Add(cloud);
        }
    } 
}
