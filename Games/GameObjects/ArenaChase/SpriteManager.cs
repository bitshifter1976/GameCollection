using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.ArenaChase;

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

        if (s1 is Tank && s2 is Shot shot && shot.Tank != s1)
        {
            s1.Energy -= s2.Damage;
            CreateExplosion(s2.Center, 0.5f);
            toRemove.Add(s2);
        }
        else if (s2 is Tank && s1 is Shot shot2 && shot2.Tank != s2)
        {
            s2.Energy -= s1.Damage;
            CreateExplosion(s1.Center, 0.5f);
            toRemove.Add(s1);
        }
        else if (s1 is Tank && s2 is Tank)
        {
            s1.BounceBack(4);
            s2.BounceBack(4);
        }
        else if (s1 is Tank && s2 is Stone)
        {
            s1.BounceBack(4);
        }
        else if (s1 is Stone && s2 is Tank)
        {
            s2.BounceBack(4);
        }
        else if (s1 is Tank && s2 is Mine)
        {
            s1.Energy -= s2.Damage;
            CreateExplosion(s2.Center, 0.5f);
            toRemove.Add(s2);
        }
        else if (s1 is Mine && s2 is Tank)
        {
            s2.Energy -= s1.Damage;
            CreateExplosion(s1.Center, 0.5f);
            toRemove.Add(s1);
        }
        else if (s1 is Shot && s2 is Stone)
        {
            CreateExplosion(s1.Center, 0.5f);
            toRemove.Add(s1);
        }
        else if (s1 is Stone && s2 is Shot)
        {
            CreateExplosion(s2.Center, 0.5f);
            toRemove.Add(s2);
        }
        else if (s1 is Shot && s2 is Mine)
        {
            CreateExplosion(s1.Center, 0.5f);
            CreateExplosion(s2.Center, 0.5f);
            toRemove.Add(s1);
            toRemove.Add(s2);
        }
        else if (s1 is Mine && s2 is Shot)
        {
            CreateExplosion(s1.Center, 0.5f);
            CreateExplosion(s2.Center, 0.5f);
            toRemove.Add(s1);
            toRemove.Add(s2);
        }
        else if (s1 is Shot && s2 is Shot)
        {
            CreateExplosion(s1.Center, 0.5f);
            CreateExplosion(s2.Center, 0.5f);
            toRemove.Add(s1);
            toRemove.Add(s2);
        }

        return toRemove;
    }

    public static void CreateExplosion(Vector2 pos, float scale)
    {
        Add(new Explosion(pos, scale));
        Add(ExplosionParticles.Create(Rand.Int(100, 200), Color.DarkGray, pos));
    }

    public static void Draw()
    {
        sprites.ForEach(s => s.Draw());
        if (Manager.Debug)
            Debug.Draw();
    }

    public static void Clear()
    {
        sprites.Clear();
        toAdd.Clear();
    }
}
