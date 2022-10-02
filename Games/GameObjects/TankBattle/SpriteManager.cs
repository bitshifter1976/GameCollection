using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

public static class SpriteManager
{
    private static readonly List<Sprite> sprites = new();
    private static readonly List<Sprite> toAdd = new();
    private static Tank tank1;
    private static Tank tank2;

    public static void Initialize(Tank tank1, Tank tank2)
    {
        SpriteManager.tank1 = tank1;
        SpriteManager.tank2 = tank2;
        AddImmediate(tank1);
        AddImmediate(tank2);
    }

    public static void AddImmediate(Sprite s)
    {
        sprites.Add(s);
    }

    public static void AddImmediate(List<Sprite> list)
    {
        sprites.AddRange(list);
    }

    public static void Add(Sprite s)
    {
        toAdd.Add(s);
    }

    public static void Add(List<Sprite> list)
    {
        toAdd.AddRange(list);
    }

    public static bool Update(GameTime time)
    {
        // add new sprites
        sprites.AddRange(toAdd);
        toAdd.Clear(); 
        // update
        var remove = sprites.Select(s => s.Update(time)).ToList();
        // collision?
        remove.AddRange(CheckCollision(time));
        // remove obsolete
        remove.ForEach(s => sprites.Remove(s));
        // sort by layer depth
        sprites.Sort();
        return remove.Any(p => p is Shot);
    }

    private static List<Sprite> CheckCollision(GameTime time)
    {
        var removeSprites = new List<Sprite>();
        foreach (var shot in sprites.OfType<Shot>().ToList())
        {
            // collide shot
            var collisionDetected = false;
            // ... with tank
            if (tank1.Collide(shot.Circle))
            {
                collisionDetected = true;
                CreateExplosion(shot.Center, 2);
                tank1.ReactToCollision(shot);
                Mountain.ReactToCollision(shot, shot.Center);
            }
            if (tank2.Collide(shot.Circle))
            {
                collisionDetected = true;
                CreateExplosion(shot.Center, 2);
                tank2.ReactToCollision(shot);
                Mountain.ReactToCollision(shot, shot.Center);
            }
            // ... with mountain
            if (Mountain.Collide(shot.BoundingBox, out var collisionPoint))
            {
                collisionDetected = true;
                CreateExplosion(shot.Center, 2);
                Mountain.ReactToCollision(shot, collisionPoint);
                if (tank1.Collide(shot.DamageCircle))
                    tank1.ReactToCollision(shot);
                if (tank2.Collide(shot.DamageCircle))
                    tank2.ReactToCollision(shot);
            }
            // ... with trees
            foreach (var tree in sprites.OfType<Tree>().ToList())
            {
                if (tree.Collide(shot))
                {
                    collisionDetected = true;
                    removeSprites.Add(tree);
                    CreateExplosion(shot.Center, 1);
                    if (tank1.Collide(shot.DamageCircle))
                        tank1.ReactToCollision(shot);
                    if (tank2.Collide(shot.DamageCircle))
                        tank2.ReactToCollision(shot);
                }
            }
            // finish if collision detected
            if (collisionDetected)
            {
                removeSprites.Add(shot);
                tank1.IsShooting = tank2.IsShooting = false;
                if (tank2.IsActive)
                    tank2.LastAiShotCollision = shot;
            }
        }
        return removeSprites;
    }

    public static void CreateExplosion(Vector2 pos, float scale)
    {
        Add(new Explosion(pos, scale));
        Add(ExplosionParticles.Create(Rand.Int(600, 900), Color.Gray, pos));
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
