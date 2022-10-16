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
    private static Submarine player;

    public static void Init(Submarine player)
    {
        SpriteManager.player = player;
        Add(player);
    }

    public static void AddImmediate(Sprite s)
    {
        if (s != null)
            sprites.Add(s);
    }

    public static void AddImmediate(List<Sprite> list)
    {
        list.ForEach(AddImmediate);
    }

    public static void Add(Sprite s)
    {
        if (s != null)
            toAdd.Add(s);
    }

    public static void Add(List<Sprite> list)
    {
        list.ForEach(Add);
    }

    public static void ScrollX(float speed)
    {
        sprites.ForEach(s => s.ScrollX(speed));
    }

    public static int Update(GameTime time, bool checkCollision)
    {
        var numberOfEnemiesKilled = 0;
        // add new sprites
        sprites.AddRange(toAdd);
        toAdd.Clear(); 
        // update
        var remove = sprites.Select(s => s.Update(time)).ToList();
        // collision?
        if (checkCollision)
            remove.AddRange(CheckCollision(ref numberOfEnemiesKilled));
        // remove obsolete
        remove.ForEach(s => sprites.Remove(s));
        // sort by layer depth
        sprites.Sort();
        // return number of enemies killed
        return numberOfEnemiesKilled;
    }

    private static List<Sprite> CheckCollision(ref int enemyKillCount)
    {   
        var removeSprites = new List<Sprite>();
        enemyKillCount = 0;
        var list = sprites.Where(s => s.CollisionType != CollisionType.None).ToList();
        for (var i = 0; i < list.Count - 1; i++)
        {
            for (var j = i + 1; j < list.Count; j++)
            {
                if ((list[i] is Submarine s && list[j] is Torpedo t && t.Submarine == s) || (list[j] is Submarine s2 && list[i] is Torpedo t2 && t2.Submarine == s2))
                    continue;
                if (list[i].Collide(list[j]))
                    removeSprites.AddRange(DoCollisionReaction(list[i], list[j], ref enemyKillCount));
            }
        }
        var torpedos = sprites.OfType<Torpedo>().ToList();
        foreach (Torpedo torpedo in torpedos)
        {
            var rect = torpedo.BoundingBoxRotated.CollisionRectangle;
            for (var x = rect.Left < 0 ? 0 : (int)rect.Left; x <= (int)rect.Right && x <= Manager.DesignWidth; x++)
            {
                if (rect.Contains(new Vector2(x, Water.GroundPixel[x])))
                    removeSprites.AddRange(DoGroundCollisionReaction(torpedo, new Vector2(x, Water.GroundPixel[x])));
            }
        }

        return removeSprites;
    }

    private static List<Sprite> DoCollisionReaction(Sprite s1, Sprite s2, ref int enemyKillCount)
    {
        var toRemove = new List<Sprite>();

        if (s1 is Torpedo t && s2 is Submarine)
        {
            s2.Energy -= s1.Damage;
            if (s2.Energy <= 0)
            {
                if (t.Submarine == player)
                    enemyKillCount++;
                CreateExplosion(s2.Position, 1);
                toRemove.Add(s2);
            }
            CreateExplosion(s1.Center, 0.5f);
            toRemove.Add(s1);
        }
        if (s2 is Torpedo t2 && s1 is Submarine)
        {
            s1.Energy -= s2.Damage;
            if (s1.Energy <= 0)
            {
                if (t2.Submarine == player)
                    enemyKillCount++;
                CreateExplosion(s1.Position, 1);
                toRemove.Add(s1);
            }
            CreateExplosion(s2.Center, 0.5f);
            toRemove.Add(s2); 
        }
        if (s1 is Mine && s2 is Submarine)
        {
            s2.Energy -= s1.Damage;
            if (s2.Energy <= 0)
            {
                CreateExplosion(s2.Position, 1);
                toRemove.Add(s2);
            }
            CreateExplosion(s1.Center, 0.5f);
            toRemove.Add(s1);
        }
        if (s2 is Mine && s1 is Submarine)
        {
            s1.Energy -= s2.Damage;
            if (s1.Energy <= 0)
            {
                CreateExplosion(s1.Position, 1);
                toRemove.Add(s1);
            }
            CreateExplosion(s2.Center, 0.5f);
            toRemove.Add(s2);
        }

        return toRemove;
    }

    private static List<Sprite> DoGroundCollisionReaction(Sprite s, Vector2 collisionPoint)
    {
        var toRemove = new List<Sprite>();

        if (s is Torpedo)
        {
            CreateExplosion(collisionPoint, 0.5f);
            toRemove.Add(s);
        }

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

    public static void CreateExplosion(Vector2 pos, float scale)
    {
        Add(new Explosion(pos, scale));
        Add(ExplosionParticles.Create(Rand.Int(20, 50), new Color(Color.White.R, Color.White.G, Color.White.B, (byte)50), pos));
    }

    public static void Clear()
    {
        sprites.Clear();
        toAdd.Clear();
    }
}
