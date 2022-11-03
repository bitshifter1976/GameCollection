using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AxeGameCollection.GameObjects.MrSunny;

public static class Floor2
{
    private static List<Line> topLines;
    private static List<Line> fillLines;
    private static Color color;

    public static void Load(int distanceToBoss, params Color[] colors)
    {
        topLines = new List<Line>();
        color = Rand.Color(colors);
        var x = 0;
        var y = Manager.DesignHeight / 2;
        while (x < distanceToBoss)
        {
            var xDelta = Rand.Int(100, 500);
            var yDelta = Rand.Int(-100, 100);
            var line = new Line(x, y, x + xDelta, y + yDelta);
            topLines.Add(line);
            x += xDelta;
            y += yDelta;
        }
    }

    public static void Update(GameTime gameTime)
    {
    }

    public static void Draw(GameTime gameTime)
    {
        foreach (var l in topLines)
        {
            if (l.Start.X >= 0 || l.End.X <= Manager.DesignWidth)
            {
                l.Draw(color, 1);
                var pointsOnLine = l.GetPoints((int)(l.End.X - l.Start.X));
                foreach (var p in pointsOnLine)
                    new Line(p.X, p.Y, p.X, Manager.DesignHeight).Draw(color, 1);
            }
        }
    }
}