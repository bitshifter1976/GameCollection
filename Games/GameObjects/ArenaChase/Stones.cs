using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.ArenaChase;

public static class Stones
{
    public static List<Sprite> Create(int count, List<Sprite> notToCollideWith)
    {
        var stones = new List<Sprite>();

        for (var i=0; i<count; i++)
        {
            var x = Rand.Int(1, Manager.DesignWidth);
            var y = Rand.Int(1, Manager.DesignHeight);
            var scale = Rand.Float(0.3f, 1f);
            var position = new Vector2(x, y);
            var stone = new Stone(position, scale);
            if (!notToCollideWith.Any(s => s.Collide(stone)) && !stones.Any(s => s.Collide(stone)))
                stones.Add(stone);
        }

        return stones;
    }
}
