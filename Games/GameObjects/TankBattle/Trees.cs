using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

public static class Trees
{
    public static void Create(int count)
    {
        var trees = new List<Tree>();

        for (var i=0; i<count; i++)
        {
            var x = Rand.Int(1, Manager.DesignWidth);
            var mountainTopY = (int)Mountain.TopPixel[x].Y;
            var scale = Rand.Float(0.3f, 0.6f);
            var yStart = mountainTopY + 10;
            var yEnd = Water.Top - 10;
            if (yEnd > yStart)
            {
                var position = new Vector2(x, Rand.Int(yStart, yEnd));
                var tree = new Tree(position, scale);
                var collisionDetected = trees.Any(t => tree.Collide(t));
                if (!collisionDetected)
                {
                    SpriteManager.Add(tree);
                    trees.Add(tree);
                }
            }
        }
    }
}
