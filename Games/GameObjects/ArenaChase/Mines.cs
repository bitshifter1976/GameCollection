using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.ArenaChase;

public static class Mines
{
    public static List<Sprite> Create(int count, List<Sprite> notToCollideWith)
    {
        var mines = new List<Sprite>();

        for (var i=0; i<count; i++)
        {
            var x = Rand.Int(1, Manager.DesignWidth);
            var y = Rand.Int(1, Manager.DesignHeight);
            var scale = 0.5f;
            var position = new Vector2(x, y);
            var mine = new Mine(position, scale, Rand.Float(0f, MathHelper.ToRadians(359)));
            if (!notToCollideWith.Any(s => s.Collide(mine)) && !mines.Any(s => s.Collide(mine)))
                mines.Add(mine);
        }

        return mines;
    }
}
