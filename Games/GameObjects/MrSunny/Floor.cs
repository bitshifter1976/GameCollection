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

    public static List<Sprite> Create(Color color, int distanceToBoss, int level)
    {
        var list = new List<FloorTile>();
        var xNext = 0;
        var pattern = string.Empty;
        var currIdx = -1;
        char currChar, prevChar = 'g', prevprevChar = 'g';
        FloorTile floorTile;
        while (xNext < distanceToBoss)
        {
            currIdx++;
            if (currIdx > 0)
                prevChar = pattern[currIdx - 1];
            if (currIdx > 1)
                prevprevChar = pattern[currIdx - 2];
            currChar = GetRandomChar(prevChar, prevprevChar, level);
            pattern += currChar;

            var pos = new Vector2(xNext, Manager.DesignHeight - Height);
            var floorTileProps = tiles[currChar].Clone();
            floorTileProps.color = color;
            floorTile = new FloorTile(pos, floorTileProps);
            xNext += (int)floorTile.Width;

            if (currChar == 'g')
            {
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
        return list.Select(s => (Sprite)s).ToList();
    }

    private static char GetRandomChar(char prevChar, char prevprevChar, int level)
    {
        char currChar;
        if (Rand.Bool(1, level))
        {
            currChar = 'A';
        }
        else
        {
            var allowed = AllowedPatternPairs.Where(p => p[0] == prevChar).Select(p => p[1]).ToList();
            if (prevChar == 'g' && prevprevChar == 'g')
                allowed.Remove('g');
            currChar = allowed[Rand.Int(0, allowed.Count - 1)];
        }

        return currChar;
    }

    private static char GetCharFromPattern(string possiblePatterns)
    {
        return possiblePatterns[Rand.Int(0, possiblePatterns.Length - 1)];
    }
}
