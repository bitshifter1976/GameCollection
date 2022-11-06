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

    public static List<Sprite> Sprites => sprites;

    public static void Init(Submarine player)
    {
        SpriteManager.player = player;
        Add(player);
        Manager.Sound.LoadEffect("metalSlide");
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
        var collisionSprites = sprites.Where(s => s.CollisionType != CollisionType.None).ToList();
        for (var i = 0; i < collisionSprites.Count - 1; i++)
        {
            for (var j = i + 1; j < collisionSprites.Count; j++)
            {
                if ((collisionSprites[i] is Submarine s && collisionSprites[j] is Torpedo t && t.Submarine == s) || (collisionSprites[j] is Submarine s2 && collisionSprites[i] is Torpedo t2 && t2.Submarine == s2))
                    continue;
                if (collisionSprites[i].Collide(collisionSprites[j]))
                    removeSprites.AddRange(DoCollisionReaction(collisionSprites[i], collisionSprites[j], ref enemyKillCount));
            }
        }
        foreach (var s in collisionSprites)
        {
            var rect = s.BoundingBoxF;
            if (s.CollisionType == CollisionType.BoundingBoxRotated)
                rect = s.BoundingBoxRotated.Rect;
            var possibleCollisionPixel = Water.GroundPixel.Where(p => p.Key >= rect.Left && p.Key <= rect.Right).Select(p => new Vector2(p.Key, p.Value)).ToList();
            foreach (var p in possibleCollisionPixel)
            {
                if (rect.Contains(p))
                    removeSprites.AddRange(DoGroundCollisionReaction(s, p));
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
                CreateExplosion(s2.Position, 2);
                toRemove.Add(s2);
            }
            CreateExplosion(s1.Center, 1);
            toRemove.Add(s1);
        }
        if (s2 is Torpedo t2 && s1 is Submarine)
        {
            s1.Energy -= s2.Damage;
            if (s1.Energy <= 0)
            {
                if (t2.Submarine == player)
                    enemyKillCount++;
                CreateExplosion(s1.Position, 2);
                toRemove.Add(s1);
            }
            CreateExplosion(s2.Center, 1);
            toRemove.Add(s2); 
        }
        if (s1 is Torpedo t3 && s2 is EnemyShip)
        {
            s2.Energy -= s1.Damage;
            if (s2.Energy <= 0)
            {
                if (t3.Submarine == player)
                    enemyKillCount++;
                CreateExplosion(s2.Position, 2, false);
                toRemove.Add(s2);
            }
            CreateExplosion(s1.Center, 2, false);
            toRemove.Add(s1);
        }
        if (s2 is Torpedo t4 && s1 is EnemyShip)
        {
            s1.Energy -= s2.Damage;
            if (s1.Energy <= 0)
            {
                if (t4.Submarine == player)
                    enemyKillCount++;
                CreateExplosion(s1.Position, 2, false);
                toRemove.Add(s1);
            }
            CreateExplosion(s2.Center, 2, false);
            toRemove.Add(s2);
        }
        if (s1 is Torpedo && s2 is TankShip)
        {
            s2.Energy -= s1.Damage;
            if (s2.Energy <= 0)
            {
                CreateExplosion(s2.Position, 2, false);
                toRemove.Add(s2);
            }
            CreateExplosion(s1.Center, 2, false);
            toRemove.Add(s1);
        }
        if (s2 is Torpedo && s1 is EnemyShip)
        {
            s1.Energy -= s2.Damage;
            if (s1.Energy <= 0)
            {
                CreateExplosion(s1.Position, 2, false);
                toRemove.Add(s1);
            }
            CreateExplosion(s2.Center, 2, false);
            toRemove.Add(s2);
        }
        if (s1 is Mine && s2 is Submarine)
        {
            s2.Energy -= s1.Damage;
            if (s2.Energy <= 0)
            {
                CreateExplosion(s2.Position, 2);
                toRemove.Add(s2);
            }
            CreateExplosion(s1.Center, 1);
            toRemove.Add(s1);
        }
        if (s2 is Mine && s1 is Submarine)
        {
            s1.Energy -= s2.Damage;
            if (s1.Energy <= 0)
            {
                CreateExplosion(s1.Position, 2);
                toRemove.Add(s1);
            }
            CreateExplosion(s2.Center, 1);
            toRemove.Add(s2);
        }
        if (s1 is Bomb && s2 is Submarine)
        {
            s2.Energy -= s1.Damage;
            if (s2.Energy <= 0)
            {
                CreateExplosion(s2.Position, 2);
                toRemove.Add(s2);
            }
            CreateExplosion(s1.Center, 1);
            toRemove.Add(s1);
        }
        if (s2 is Bomb && s1 is Submarine)
        {
            s1.Energy -= s2.Damage;
            if (s1.Energy <= 0)
            {
                CreateExplosion(s1.Position, 2);
                toRemove.Add(s1);
            }
            CreateExplosion(s2.Center, 1);
            toRemove.Add(s2);
        }
        if ((s1 is Mine && s2 is Torpedo) || (s1 is Torpedo && s2 is Mine))
        {
            CreateExplosion(s1.Center, 1);
            toRemove.Add(s1);
            CreateExplosion(s2.Center, 1);
            toRemove.Add(s2);
        }
        if ((s1 is Bomb && s2 is Torpedo) || (s1 is Torpedo && s2 is Bomb))
        {
            CreateExplosion(s1.Center, 1);
            toRemove.Add(s1);
            CreateExplosion(s2.Center, 1);
            toRemove.Add(s2);
        }
        if ((s1 is Bomb && s2 is Mine) || (s1 is Mine && s2 is Bomb))
        {
            CreateExplosion(s1.Center, 1);
            toRemove.Add(s1);
            CreateExplosion(s2.Center, 1);
            toRemove.Add(s2);
        }

        return toRemove;
    }

    private static List<Sprite> DoGroundCollisionReaction(Sprite s, Vector2 collisionPoint)
    {
        var toRemove = new List<Sprite>();

        if (s is Torpedo)
        {
            CreateExplosion(collisionPoint, 1f);
            toRemove.Add(s);
        }
        else if (s is Bomb)
        {
            CreateExplosion(collisionPoint, 1f);
            toRemove.Add(s);
        }
        else if (s is Submarine s2)
        {
            s2.Position += new Vector2(0, -2); 
            if (!Manager.Sound.IsEffectPlaying("metalSlide"))
                Manager.Sound.PlayEffect("metalSlide");
            s2.Energy -= 0.5f;
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

    public static void CreateExplosion(Vector2 pos, float scale, bool withParticles = true)
    {
        Add(new Explosion(pos, scale));
        if (withParticles)
            Add(ExplosionParticles.Create(pos, scale));
    }

    public static void Clear()
    {
        sprites.Clear();
        toAdd.Clear();
    }
}
