using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Framework;
using System;
using System.Linq;
using System.Net.WebSockets;

namespace AxeGameCollection.GameObjects.MrSunny;
public static class Platforms
{
    private static readonly int GapLengthMin = 250;
    private static readonly int JumpHeight = 100;
    private static readonly int MinGapXOffset = 1;
    private static readonly int MaxGapXOffset = 20;
    private static readonly float MinRotation = MathHelper.ToRadians(-45);
    private static readonly float MaxRotation = MathHelper.ToRadians(45);
    private static readonly float MinScale = 1;
    private static readonly float MaxScale = 3;

    public static List<Sprite> Create(List<Line> gaps, int distanceToBoss, int level)
    {
        var list = new List<Platform>();
        foreach(var gap in gaps)
        {
            var gapLength = gap.End.X - gap.Start.X;
            if (gapLength > GapLengthMin)
            {
                var platform = new Platform(
                    new Vector2(gap.Start.X + Rand.Float(MinGapXOffset, MaxGapXOffset), gap.Start.Y - JumpHeight),
                    Rand.Color(Color.White, Color.Black, Color.Red, Color.Blue, Color.Green, Color.Yellow),
                    Rand.Float(MinRotation, MaxRotation),
                    Rand.Float(MinScale, MaxScale));
                list.Add(platform);
            }
        }
        return list.Select(s => (Sprite)s).ToList();
    }
}
