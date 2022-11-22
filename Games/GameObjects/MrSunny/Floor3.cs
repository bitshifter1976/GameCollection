using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Framework;
using System;
using System.Linq;

namespace AxeGameCollection.GameObjects.MrSunny;
public static class Floor3
{
    public static float width;
    private static readonly Dictionary<char, FloorTileProps3> tileProps = new() 
    {
        { 'A', new FloorTileProps3(FloorTileType.GroundTopLeft,   FloorTileType.GroundBottomLeft,   FloorTileType.PlatformLeft  ) },
        { 'B', new FloorTileProps3(FloorTileType.GroundTopMiddle, FloorTileType.GroundBottomMiddle, FloorTileType.PlatformMiddle) },
        { 'C', new FloorTileProps3(FloorTileType.GroundTopRight,  FloorTileType.GroundBottomRight,  FloorTileType.PlatformRight ) },
        { 'D', new FloorTileProps3(FloorTileType.GroundTopSingle, FloorTileType.GroundBottomSingle, FloorTileType.PlatformSingle) },
        { 'g', new FloorTileProps3(FloorTileType.gap,             FloorTileType.gap,                FloorTileType.gap           ) }
    };
    private static string[] AllowedPatternPairs = new[] { "gg", "gA", "gD", "AB", "BB", "BC", "CA", "CD", "Cg", "DA", "DD", "Dg" };
    private static char[] HeightChangers = new[] { 'A', 'D' };
    private static char[] EndTiles = new[] { 'C', 'D', 'g' };
    private static int MaxGapCount = 2;

    public static float Width => width;

    public static List<Sprite> Create(int distanceToBoss, int level)
    {
        var list = new List<FloorTile3>();
        var x = 0f;
        var pattern = "g";
        var currIdx = 0;
        char currChar = char.MinValue, prevChar = char.MinValue;
        var heightLevel = 1;
        var gapCount = 0;
        while (x < distanceToBoss)
        {
            currIdx++;
            prevChar = pattern[currIdx - 1];
            currChar = (gapCount == MaxGapCount ? 'A' : GetRandomChar(prevChar, level));

            var floorTile = CreateFloorTile(level, x, ref pattern, currChar, ref heightLevel);

            if (currChar == 'g')
            {
                gapCount++;
            }
            else
            {
                gapCount = 0;
                list.Add(floorTile);
            }
            x += floorTile.Width;
        }

        if (!EndTiles.Contains(currChar))
            CreateEnd(level, list, ref x, ref pattern, currChar, ref heightLevel);
        width = x;

        return list.Select(s => (Sprite)s).ToList();
    }

    private static void CreateEnd(int level, List<FloorTile3> list, ref float x, ref string pattern, char currChar, ref int heightLevel)
    {
        if (currChar == 'A')
        {
            var t1 = CreateFloorTile(level, x, ref pattern, 'B', ref heightLevel);
            var t2 = CreateFloorTile(level, x, ref pattern, 'C', ref heightLevel);
            list.Add(t1);
            list.Add(t2);
            x += (int)t1.Width;
            x += (int)t2.Width;
            pattern += 'B';
            pattern += 'C';
        }
        else if (currChar == 'B')
        {
            var t = CreateFloorTile(level, x, ref pattern, 'C', ref heightLevel);
            list.Add(t);
            x += (int)t.Width;
            pattern += 'C';
        }
    }

    private static FloorTile3 CreateFloorTile(int level, float x, ref string pattern, char currChar, ref int heightLevel)
    {
        pattern += currChar;
        if (HeightChangers.Contains(currChar) && !Rand.Bool(1, level))
        {
            if (heightLevel == 1)
                heightLevel++;
            else if (heightLevel == 2)
                heightLevel += Rand.Bool(1, 3) ? -1 : 1;
            else if (heightLevel == 3)
                heightLevel += Rand.Bool(1, 3) ? 1 : Rand.Int(-2,-1);
            else if (heightLevel == 4)
                heightLevel += Rand.Int(-3, -1);
            heightLevel = Math.Clamp(heightLevel, 1, 4);
        }
        return new FloorTile3(x, heightLevel, tileProps[currChar].Clone());
    }

    private static char GetRandomChar(char prevChar, int level)
    {
        char currChar;
        var allowed = AllowedPatternPairs.Where(p => p[0] == prevChar).Select(p => p[1]).ToList();
        // create ground
        if (Rand.Bool(1, level))
        {
            if (allowed.Count > 1)
                allowed.Remove('g');
            if (allowed.Count > 1)
                allowed.Remove('C');
            currChar = allowed[Rand.Int(0, allowed.Count - 1)];
        }
        // anything else
        else
        {
            currChar = allowed[Rand.Int(0, allowed.Count - 1)];
        }

        return currChar;
    }
}
