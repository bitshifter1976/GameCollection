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
        var tileBlock = new List<FloorTile>();
        var xNext = 0;
        var blockWidth = 0;
        for (int i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];
                
            if (c != 'g')
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
                tileBlock.Add(new FloorTile(new Vector2(xNext, Manager.DesignHeight - Height), 2, tiles[c].Clone()));
                xNext += tiles[c].width;
                blockWidth += tiles[c].width;
            }
            else
            {
                for (int j = 0; j < tileBlock.Count; j++)
                {
                    if (j == 0)
                        tileBlock[j].SetBoundingBox(new RectangleF(tileBlock[0].Position.X, tileBlock[0].Position.Y + FloorTile.TopOffset, blockWidth, Height - FloorTile.TopOffset));
                    else
                        tileBlock[j].CollisionType = CollisionType.None;
                    SpriteManager.AddImmediate(tileBlock[j]);
                }
                tileBlock.Clear();
                xNext += gapWidth;
                blockWidth = 0;
            }
        }
        // calculate the new bounding boxes
        for (int j = 0; j < tileBlock.Count; j++)
        {
            SpriteManager.AddImmediate(tileBlock[j]);
        }
        width = xNext;
    }
}
