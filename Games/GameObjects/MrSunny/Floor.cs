using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Collections;
using Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;
public static class Floor
{
    private static float width;
    private const int gapWidth = 50;
    private static readonly Dictionary<char, FloorTileProps> tiles = new() 
    {
        { 'A', new FloorTileProps("beachA", "beachLeft", "beachRightA", 400) },
        { 'B', new FloorTileProps("beachB", "beachLeft", "beachRightB", 300) },
        { 'C', new FloorTileProps("beachC", "beachLeft", "beachRightC", 200) },
        { 'D', new FloorTileProps("beachD", "beachLeft", "beachRightD", 100) },
        { 'E', new FloorTileProps("beachE", "beachLeft", "beachRightE", 50)  },
        { 'F', new FloorTileProps("beachF", "beachLeft", "beachRightF", 25)  }
    };

    public static float Height => 100;
    public static float Width => width;

    public static void Create(Color color, string pattern)
    {
        var xNext = 0;
        for (int i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];

            if (c == 'g')
            {
                xNext += gapWidth;
            }
            else
            {
                // should we draw a left end
                tiles[c].leftEnd = false;
                if (i > 0)
                {
                    if (pattern[i - 1] == 'g')
                        tiles[c].leftEnd = true;
                }
                else
                {
                    tiles[c].leftEnd = true;
                }
                // should we draw a right end
                tiles[c].rightEnd = false;
                if (i + 1 < pattern.Length)
                {
                    if (pattern[i + 1] == 'g')
                        tiles[c].rightEnd = true;
                }
                else
                {
                    tiles[c].rightEnd = true;
                }
                // add tile
                tiles[c].color = color;
                var pos = new Vector2(xNext, Manager.DesignHeight - Height);
                var tile = new FloorTile(pos, 2, tiles[c].Clone());
                var width = tiles[c].width;
                tile.SetBoundingBox(new RectangleF(pos.X, pos.Y + FloorTile.TopOffset, width, Height - FloorTile.TopOffset));
                SpriteManager.AddImmediate(tile);
                xNext += width;
            }
        }
        width = xNext;
    }
}
