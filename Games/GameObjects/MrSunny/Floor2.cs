using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AxeGameCollection.GameObjects.MrSunny;

public static class Floor2
{
    public static List<Sprite> Create(int distanceToBoss, params Color[] colors)
    {
        var tiles = new List<Sprite>();
        var color = Rand.Color(colors);
        var x = 0;
        var y = Manager.DesignHeight / 2;
        while (x < distanceToBoss)
        {
            var xDelta = Rand.Int(100, 500);
            var yDelta = Rand.Int(-100, 100);
            var tile = new FloorTile2(new Vector2(x, y), new Vector2(x + xDelta, y + yDelta), color);
            tiles.Add(tile);
            x += xDelta;
            y += yDelta;
        }
        return tiles;
    }
}