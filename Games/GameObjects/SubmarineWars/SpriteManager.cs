using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public static class SpriteManager
{
    private static readonly List<Sprite> sprites = new();
    private static readonly List<Sprite> toAdd = new();

    public static void AddImmediate(Sprite s)
    {
        sprites.Add(s);
    }

    public static void AddImmediate(List<Sprite> list)
    {
        list.ForEach(AddImmediate);
    }

    public static void Add(Sprite s)
    {
        toAdd.Add(s);
    }

    public static void Add(List<Sprite> list)
    {
        toAdd.AddRange(list);
    }

    public static void ScrollX(float speed)
    {
        sprites.ForEach(s => s.ScrollX(speed));
    }

    public static void Update(GameTime time)
    {
        // add new sprites
        sprites.AddRange(toAdd);
        toAdd.Clear(); 
        // update
        var remove = sprites.Select(s => s.Update(time)).ToList();
        // collision?
        remove.AddRange(CheckCollision());
        // remove obsolete
        remove.ForEach(s => sprites.Remove(s));
        // sort by layer depth
        sprites.Sort();
    }

    private static List<Sprite> CheckCollision()
    {   
        var removeSprites = new List<Sprite>();
        var list = sprites.Where(s => s.CollisionType != CollisionType.None).ToList();
        for (var i = 0; i < list.Count - 1; i++)
        {
            for (var j = i + 1; j < list.Count; j++)
            {
                if (list[i].Collide(list[j]))
                    removeSprites.AddRange(DoCollisionReaction(list[i], list[j]));
            }
        }

        return removeSprites;
    }

    private static List<Sprite> DoCollisionReaction(Sprite s1, Sprite s2)
    {
        var toRemove = new List<Sprite>();

        return toRemove;
    }

    public static void Draw()
    {
        sprites.ForEach(s => s.Draw());
        if (Manager.Debug)
        {
            Debug.Draw();
            Debug.Clear();
        }
    }

    public static void Clear()
    {
        sprites.Clear();
        toAdd.Clear();
    }
}
