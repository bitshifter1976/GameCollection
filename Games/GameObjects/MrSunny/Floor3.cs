using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Framework;
using System;
using System.Linq;

namespace AxeGameCollection.GameObjects.MrSunny;
public static class Floor3
{
    private static float width;
    private static readonly Dictionary<char, FloorTileProps3> tiles = new() 
    {
        { 'A', new FloorTileProps3("GroundTopLeft",   FloorTileType.GroundTopLeft,   "GroundBottomLeft",   FloorTileType.GroundBottomLeft,   0) },
        { 'B', new FloorTileProps3("GroundTopMiddle", FloorTileType.GroundTopMiddle, "GroundBottomMiddle", FloorTileType.GroundBottomMiddle, 0) },
        { 'C', new FloorTileProps3("GroundTopRight",  FloorTileType.GroundTopRight,  "GroundBottomRight",  FloorTileType.GroundBottomRight,  0) },
        { 'D', new FloorTileProps3("GroundTopSingle", FloorTileType.GroundTopSingle, "GroundBottomSingle", FloorTileType.GroundBottomSingle, 0) },
        { 'g', new FloorTileProps3("gap",             FloorTileType.GroundGap,       "gap",                FloorTileType.GroundGap,          0) }
    };
    private static string[] AllowedPatternPairs = new[] { "gg", "gA", "gD", "AB", "BB", "BC", "CA", "CD", "Cg", "DA", "DD", "Dg" };
    private static char[] HeightChangers = new[] { 'A', 'D' };
    private static char[] EndTiles = new[] { 'C', 'D', 'g' };

    public static float MinHeight => 256;
    public static float MaxHeight => 256*3;
    public static float Width => width;

    public static List<Sprite> Create(int distanceToBoss, int level, out List<Line> gaps)
    {
        var list = new List<FloorTile3>();
        gaps = new List<Line>();
        var x = 0;
        var pattern = "g";
        var currIdx = 0;
        char currChar = char.MinValue, prevChar = char.MinValue;
        var prevHeightLevel = 2;
        while (x < distanceToBoss)
        {
            currIdx++;
            prevChar = pattern[currIdx - 1];
            currChar = GetRandomChar(prevChar, level);
            var floorTile = CreateFloorTile(level, x, ref pattern, currChar, ref prevHeightLevel);
            var y = Manager.DesignHeight - floorTile.Height * prevHeightLevel;

            if (currChar == 'g')
                gaps.Add(new Line(x, y, x + floorTile.Width, y));
            else
                list.Add(floorTile);
            x += (int)floorTile.Width;
        }
        if (!EndTiles.Contains(currChar))
            CreateEnd(level, list, ref x, ref pattern, currChar, ref prevHeightLevel);

        width = x;

        gaps = CalculateGaps(gaps);
        return list.Select(s => (Sprite)s).ToList();
    }

    private static void CreateEnd(int level, List<FloorTile3> list, ref int x, ref string pattern, char currChar, ref int prevHeightLevel)
    {
        if (currChar == 'A')
        {
            var t1 = CreateFloorTile(level, x, ref pattern, 'B', ref prevHeightLevel);
            var t2 = CreateFloorTile(level, x, ref pattern, 'C', ref prevHeightLevel);
            list.Add(t1);
            list.Add(t2);
            x += (int)t1.Width;
            x += (int)t2.Width;
        }
        else if (currChar == 'B')
        {
            var t = CreateFloorTile(level, x, ref pattern, 'C', ref prevHeightLevel);
            list.Add(t);
            x += (int)t.Width;
        }
    }

    private static FloorTile3 CreateFloorTile(int level, int x, ref string pattern, char currChar, ref int prevHeightLevel)
    {
        pattern += currChar;
        var heightLevel = prevHeightLevel;
        if (HeightChangers.Contains(currChar) && !Rand.Bool(1, level))
            heightLevel += Rand.Int(-1, 1);
        heightLevel = Math.Clamp(heightLevel, 1, 3);
        var floorTileProps = tiles[currChar].Clone();
        floorTileProps.heightLevel = heightLevel;
        var floorTile = new FloorTile3(x, floorTileProps);
        prevHeightLevel = heightLevel;
        return floorTile;
    }

    private static List<Line> CalculateGaps(List<Line> gaps)
    {
        // if we have two gaps next to each other, we want one big gap
        Vector2 start, end;
        var gapPairFound = true;
        while (gapPairFound)
        {
            gapPairFound = false;
            var gaps2 = new List<Line>();
            for (var i = 0; i < gaps.Count - 1; i++)
            {
                start = gaps[i].Start;
                end = gaps[i].End;
                if (gaps[i].End == gaps[i + 1].Start)
                {
                    gapPairFound = true;
                    end = gaps[i + 1].End;
                    i++;
                }
                gaps2.Add(new Line(start.X, start.Y, end.X, end.Y));
            }
            gaps = gaps2;
        }

        return gaps;
    }

    private static char GetRandomChar(char prevChar, int level)
    {
        char currChar;
        var allowed = AllowedPatternPairs.Where(p => p[0] == prevChar).Select(p => p[1]).ToList();
        // create ground
        if (Rand.Bool(1, level))
        {
            if (allowed.Count > 1 && allowed.Contains('g'))
                allowed.Remove('g');
            currChar = allowed[Rand.Int(0, allowed.Count - 1)];
        }
        // create gap
        else if (Rand.Bool(1, 20 / level + 1) && allowed.Contains('g'))
        {
            currChar = 'g';
        }
        // anything else
        else
        {
            currChar = allowed[Rand.Int(0, allowed.Count - 1)];
        }

        return currChar;
    }
}
