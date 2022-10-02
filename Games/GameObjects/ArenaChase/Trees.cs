using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.ArenaChase;

public static class Trees
{
    public static List<Sprite> Create(int count, List<Sprite> notToCollideWith)
    {
        var trees = new List<Sprite>();

        for (var i=0; i<count; i++)
        {
            var x = Rand.Int(1, Manager.DesignWidth);
            var y = Rand.Int(1, Manager.DesignHeight);
            var scale = Rand.Float(0.3f, 1f);
            var position = new Vector2(x, y);
            var tree = new Tree(position, scale);
            if (!notToCollideWith.Any(s => s.Collide(tree)))
                trees.Add(tree);
        }

        return trees;
    }
}
