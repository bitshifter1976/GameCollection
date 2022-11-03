using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Framework;
using System;
using System.Linq;

namespace AxeGameCollection.GameObjects.MrSunny;
public static class Floor
{
    private static float width;
    private static readonly Dictionary<char, FloorTileProps> tiles = new() 
    {
        { 'A', new FloorTileProps("beachA", "beachLeft", "beachRightA", 400) },
        { 'B', new FloorTileProps("beachB", "beachLeft", "beachRightB", 300) },
        { 'C', new FloorTileProps("beachC", "beachLeft", "beachRightC", 200) },
        { 'D', new FloorTileProps("beachD", "beachLeft", "beachRightD", 100) },
        { 'E', new FloorTileProps("beachE", "beachLeft", "beachRightE", 50)  },
        { 'F', new FloorTileProps("beachF", "beachLeft", "beachRightF", 25)  },
        { 'g', new FloorTileProps("gap",    "gap",       "gap",         50)  }
    };
    private static string[] AllowedPatternPairs = new[] { "AA", "AB", "AC", "AD", "AE", "AF", "Ag", "Bg", "Cg", "Dg", "Eg", "Fg", "gg", "gA", "gB", "gC", "gD", "gE", "gF" };

    public static float Height => 100;
    public static float Width => width;

    public static List<Sprite> Create(Color color, int distanceToBoss, int level, out List<Line> gaps)
    {
        var list = new List<FloorTile>();
        gaps = new List<Line>();
        var xNext = 0;
        var pattern = string.Empty;
        var currIdx = -1;
        char currChar, prevChar = 'g';
        FloorTile floorTile;
        while (xNext < distanceToBoss)
        {
            currIdx++;
            if (currIdx > 0)
                prevChar = pattern[currIdx - 1];
            currChar = GetRandomChar(prevChar, level);
            pattern += currChar;

            var pos = new Vector2(xNext, Manager.DesignHeight - Height);
            var floorTileProps = tiles[currChar].Clone();
            floorTileProps.color = color;
            floorTile = new FloorTile(pos, floorTileProps);
            xNext += (int)floorTile.Width;

            if (currChar == 'g')
            {
                gaps.Add(new Line(pos.X, pos.Y, xNext, pos.Y));
                if (prevChar != 'g' && list.Count > 0)
                    list[list.Count-1].RightEnd = true;
            }
            else
            {
                if (prevChar == 'g')
                    floorTile.LeftEnd = true;
                if (xNext >= distanceToBoss)
                    floorTile.RightEnd = true;
                list.Add(floorTile);
            }
        }
        width = xNext;
        // if we have two gaps next to each other, we want one big gap
        var gaps2 = new List<Line>();
        Vector2 start, end;
        var gapPairFound = true;
        while (gapPairFound)
        {
            gapPairFound = false;
            gaps2 = new List<Line>();
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
        return list.Select(s => (Sprite)s).ToList();
    }

    private static char GetRandomChar(char prevChar, int level)
    {
        char currChar;
        if (Rand.Bool(1, level))
        {
            currChar = 'A';
        }
        else
        {
            if (Rand.Bool(1, 20 / level + 1))
            {
                currChar = 'g';
            }
            else
            {
                var allowed = AllowedPatternPairs.Where(p => p[0] == prevChar).Select(p => p[1]).ToList();
                currChar = allowed[Rand.Int(0, allowed.Count - 1)];
            }
        }

        return currChar;
    }

    private static char GetCharFromPattern(string possiblePatterns)
    {
        return possiblePatterns[Rand.Int(0, possiblePatterns.Length - 1)];
    }
}
