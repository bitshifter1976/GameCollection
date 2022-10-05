using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AxeGameCollection.GameObjects.TankBattle;

public static class Mountain
{
    private static Texture2D texture;
    private static Dictionary<int, Line> explosionLines;
    private static Color color;

    public static Dictionary<int, Vector2> TopPixel { get; private set; }

    public static void Load(params Color[] colors)
    {
        texture = new Texture2D(Manager.Graphics.GraphicsDevice, 1, 1);
        texture.SetData(new Color[1] { Color.White });

        TopPixel = new Dictionary<int, Vector2>();
        explosionLines = new Dictionary<int, Line>();
        color = Rand.Color(colors);

        for (var x = 0; x <= Manager.DesignWidth; x++)
            TopPixel.Add(x, new Vector2(x, Manager.DesignHeight / 2f));

        GenerateTerrain(0, Manager.DesignWidth, Manager.DesignHeight / 3f, 0.8f, 0);
    }

    private static void GenerateTerrain(int leftIndex, int rightIndex, float displacement, float roughness, int recursionDepth)
    {
        const int offset = 30;
        if (recursionDepth++ >= Rand.Int(3, 7) || leftIndex+offset >= rightIndex-offset)
            return;

        var midIndex = Rand.Int(leftIndex+offset, rightIndex-offset);
        var change = (float)(Rand.Double(-1,1) * displacement);
        var y = (TopPixel[leftIndex].Y + TopPixel[rightIndex].Y) / 2f + change;
        y = MathHelper.Clamp(y, 300, Water.Top-20);
        TopPixel[midIndex] = new Vector2(TopPixel[midIndex].X, y);

        displacement *= roughness;

        ApplyTopPixelOnLine(leftIndex, midIndex);
        GenerateTerrain(leftIndex, midIndex, displacement, roughness, recursionDepth);

        ApplyTopPixelOnLine(midIndex, rightIndex);
        GenerateTerrain(midIndex, rightIndex, displacement, roughness, recursionDepth);
    }

    private static void ApplyTopPixelOnLine(int leftIndex, int rightIndex)
    {
        for (var x = leftIndex; x < rightIndex; x++)
        {
            var y = GetYOnLine(leftIndex, rightIndex, x);
            y = MathHelper.Clamp(y, 100, Water.Top - 20);
            TopPixel[x] = new Vector2(x, y);
        }
    }

    private static float GetYOnLine(int leftIndex, int rightIndex, int x)
    {
        var m = (TopPixel[rightIndex].Y - (double)TopPixel[leftIndex].Y) / (TopPixel[rightIndex].X - (double)TopPixel[leftIndex].X);
        var t = TopPixel[rightIndex].Y - TopPixel[rightIndex].X * m;
        return (float)(m * TopPixel[x].X + t);
    }

    public static bool Update(GameTime gameTime)
    {
        var remove = new Dictionary<int, Line>();
        foreach (var l in explosionLines)
        {
            l.Value.Start = new Vector2(l.Key, l.Value.Start.Y + 1f);
            l.Value.End = new Vector2(l.Key, l.Value.End.Y + 1f);
            if (l.Value.End.Y >= TopPixel[l.Key].Y)
            {
                remove.Add(l.Key, l.Value);
                var height = l.Value.End.Y - l.Value.Start.Y;
                TopPixel[l.Key] = new Vector2(l.Key, TopPixel[l.Key].Y - height);
            }
        }
        foreach (var l in remove)
            explosionLines.Remove(l.Key);

        return explosionLines.Count > 0;
    }

    public static void Draw(GameTime gameTime, int drawWidth)
    {
        foreach (var p in TopPixel.Where(p => p.Key <= drawWidth))
            Manager.SpriteBatch.Draw(
                texture, 
                new Rectangle(p.Key, (int)p.Value.Y, 1, (int)(Manager.DesignHeight - p.Value.Y)), 
                null, 
                color, 
                0, 
                Vector2.Zero, 
                SpriteEffects.None, 
                0);

        foreach (var l in explosionLines)
            l.Value.Draw(color, 1);
    }

    public static bool Collide(Rectangle rect, out Vector2 collisionPoint)
    {
        collisionPoint = Vector2.Zero;
        for (var x = rect.Left; x <= rect.Right; x++)
        {
            var p = new Vector2(x, rect.Bottom);
            if (Collide(p, out var collisionPoint2))
            {
                collisionPoint = collisionPoint2;
                return true;
            }
        }
        return false;
    }

    public static bool Collide(Vector2 pos, out Vector2 collisionPoint)
    {
        if ((pos.X < 0 || pos.X > Manager.DesignWidth) && pos.Y > Manager.DesignHeight)
        {
            collisionPoint = pos;
            return true;
        }

        var x = (int)Math.Round(pos.X, 0);
        if (TopPixel.ContainsKey(x) && pos.Y >= TopPixel[x].Y)
        {
            collisionPoint = TopPixel[x];
            return true;
        }

        collisionPoint = Vector2.Zero;
        return false;
    }

    public static void ReactToCollision(Sprite sprite, Vector2 collisionPoint)
    {
        if (sprite is Shot shot)
        {
            var circle = new Circle(collisionPoint, shot.DamageCircle.Radius);

            Debug.Ellipses.Add(new EllipseDebug(new Ellipse(circle.Center, circle.Radius * 2, circle.Radius * 2), "ExplosionCircle", Color.Orange, 25));

            var startX = (int)Math.Round(collisionPoint.X - circle.Radius, 0);
            var endX = (int)Math.Round(collisionPoint.X + circle.Radius, 0);

            for (var x = startX; x <= endX; x++)
            {
                var pointA = new Vector2(x, TopPixel.ContainsKey(x) ? TopPixel[x].Y : Manager.DesignHeight - 1);
                var pointB = new Vector2(x, Manager.DesignHeight);

                if (circle.IntersectSegment(pointA, pointB, out var intersection1, out var intersection2) > 0)
                {
                    var posX = (int)(Math.Round(intersection1.X, 0));
                    if (intersection1 != Vector2.Zero && TopPixel.ContainsKey(posX) && !explosionLines.ContainsKey(posX))
                        explosionLines.Add(posX, new Line(posX, TopPixel[posX].Y, posX, intersection1.Y));

                    posX = (int)(Math.Round(intersection2.X, 0));
                    if (intersection2 != Vector2.Zero && TopPixel.ContainsKey(posX))
                        TopPixel[posX] = new Vector2(posX,intersection2.Y);
                }
            }
        }
    }

    public static void Unload()
    {
        texture.Dispose();
    }
}