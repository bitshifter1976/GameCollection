using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.MrSunny;

public static class SpriteManager
{
    private static readonly List<Sprite> sprites = new();
    private static readonly List<Sprite> toAdd = new();

    public static List<Sprite> Sprites => sprites;

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
            remove.AddRange(CheckCollision(time));
        // remove obsolete
        remove.ForEach(s => sprites.Remove(s));
        // sort by layer depth
        sprites.Sort();
        // return number of enemies killed
        return numberOfEnemiesKilled;
    }

    private static List<Sprite> CheckCollision(GameTime time)
    {
        // test if there is any collision
        // player collision
        var ellipse = Player.Sprite.GetScaledBEllipse(Player.WorldScale);
        var center = Player.Sprite.CenterBEllipse;
        var position = Player.Sprite.Position;
        var velocity = Player.Sprite.Velocity;
        var gravity = new Vector2(0, Player.Mass);
        // collision
        var removeSprites = ContinuousEllipsoidCollision.Do(Player.Sprite, ellipse, ref center, ref position, ref velocity, gravity, Player.Sprite.Friction, Player.WorldScale, time, sprites.ToList());
        // update player
        var scrollX = center.X - Player.Sprite.CenterBEllipse.X + velocity.X;
        ScrollX(-scrollX);
        Player.Position = new Vector2(Player.PositionX, position.Y);
        //Player.Sprite.CenterBEllipse = new Vector2(Player.Sprite.CenterBEllipse.X, center.Y);
        Player.Sprite.Velocity = velocity;
        // do all other collisions
        var collisionSprites = sprites.Where(s => s.CollisionType != CollisionType.None).ToList();
        for (var i = 0; i < collisionSprites.Count - 1; i++)
        {
            for (var j = i + 1; j < collisionSprites.Count; j++)
            {
                if (collisionSprites[i].Collide(collisionSprites[j]))
                    removeSprites.AddRange(DoCollisionReaction(collisionSprites[i], collisionSprites[j]));
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
            //Debug.Clear();
        }
    }

    public static void Clear()
    {
        sprites.Clear();
        toAdd.Clear();
    }
}
