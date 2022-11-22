using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Framework;
using System;
using System.Linq;

namespace AxeGameCollection.GameObjects.MrSunny;
public static class Level
{
    private static float width;
    private static string pattern;

    private static readonly Dictionary<char, FloorTileProps3> tileProps = new() 
    {
        { 'A', new FloorTileProps3(FloorTileType.groundTopLeft,   FloorTileType.groundBottomLeft,   FloorTileType.platformLeft  ) },
        { 'B', new FloorTileProps3(FloorTileType.groundTopMiddle, FloorTileType.groundBottomMiddle, FloorTileType.platformMiddle) },
        { 'C', new FloorTileProps3(FloorTileType.groundTopRight,  FloorTileType.groundBottomRight,  FloorTileType.platformRight ) },
        { 'D', new FloorTileProps3(FloorTileType.groundTopSingle, FloorTileType.groundBottomSingle, FloorTileType.platformSingle) },
        { 'g', new FloorTileProps3(FloorTileType.gap,             FloorTileType.gap,                FloorTileType.gap           ) }
    };

    private static string[] AllowedPatternPairs = new[] { "gg", "gA", "gD", "AB", "BB", "BC", "CA", "CD", "Cg", "DA", "DD", "Dg" };
    private static char[] HeightChangers = new[] { 'A', 'D' };
    private static char[] EndTiles = new[] { 'C', 'D', 'g' };
    private static int MaxGapCount = 2;
    public static float Width => width;

    public static List<Sprite> Create(int distanceToBoss, int level)
    {
        var sprites = new List<Sprite>();
        var x = 0f;
        char currChar = ' '; 
        char prevChar;
        var heightLevel = 1;
        var gapCount = 0;
        // create start floor where player can land
        var startPattern = "gABBBBB";
        foreach (var c in startPattern)
        {
            var floorTile = CreateFloorTile(level, x, ref pattern, c, ref heightLevel);
            sprites.Add(floorTile);
            x += floorTile.Width;
        }
        // create random floor and platforms
        while (x < distanceToBoss)
        {
            prevChar = pattern.Last();
            currChar = (gapCount == MaxGapCount ? 'A' : GetRandomChar(prevChar, level));
            var floorTile = CreateFloorTile(level, x, ref pattern, currChar, ref heightLevel);
            if (currChar == 'g')
            {
                gapCount++;
            }
            else
            {
                gapCount = 0;
                sprites.Add(floorTile);
                RandomlyCreateItemOnFloor(floorTile, sprites);
            }
            x += floorTile.Width;
        }
        // create end floor, where enemy boss awaits you
        var endPattern = string.Empty;
        if (currChar == 'C' || currChar == 'D' || currChar == 'g')
            endPattern = "A";
        endPattern += "BBBBBBC";
        var idx = 0;
        foreach (var c in endPattern)
        {
            idx++;
            var floorTile = CreateFloorTile(level, x, ref pattern, c, ref heightLevel);
            sprites.Add(floorTile);
            x += (int)floorTile.Width;
            pattern += c;
            if (idx == 2)
            {
                var sign = new EnemySign(1);
                sign.Position = new Vector2(floorTile.BoundingBox.X, floorTile.BoundingBox.Y - sign.Height);
                sprites.Add(sign);
            }
            if (idx == 5)
            {
                var enemy = new Enemy(1);
                enemy.Position = new Vector2(floorTile.BoundingBox.X, floorTile.BoundingBox.Y - enemy.Height/2f);
                sprites.Add(enemy);
            }
        }
        // remember final width
        width = x;
        // return the created sprite list
        return sprites;
    }

    private static void RandomlyCreateItemOnFloor(GroundTile floorTile, List<Sprite> list)
    {
        if (Rand.Bool(1, 10))
        {
            var stone = new Stone(Rand.Float(0.5f, 1.5f));
            stone.Position = new Vector2(Rand.Float(floorTile.BoundingBox.X, floorTile.BoundingBox.X + floorTile.Width - stone.Width), floorTile.BoundingBox.Y - stone.Height);
            list.Add(stone);
        }
    }

    private static GroundTile CreateFloorTile(int level, float x, ref string pattern, char currChar, ref int heightLevel)
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
        return new GroundTile(x, heightLevel, tileProps[currChar].Clone());
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
