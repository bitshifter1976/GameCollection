using System;
using Microsoft.Xna.Framework;

namespace Framework;

public class Circle
{
    public Vector2 Center;
    public float Radius;

    public Circle(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public RectangleF BoundingBox => new(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2);

    public int IntersectSegment(Vector2 a, Vector2 b, out Vector2 intersection1, out Vector2 intersection2)
    {
        int count = 0;
        intersection1 = Vector2.Zero;
        intersection2 = Vector2.Zero;
        // First up, let's normalise our vectors so the circle is on the origin
        Vector2 normA = a - Center;
        Vector2 normB = b - Center;

        Vector2 d = normB - normA;

        // Want to solve as a quadratic equation, need 'a','b','c' components
        double aa = Vector2.Dot(d, d);
        double bb = 2 * (Vector2.Dot(normA, d));
        double cc = Vector2.Dot(normA, normA) - Math.Pow(Radius, 2);

        // Get determinant to see if LINE intersects
        double deter = Math.Pow(bb, 2.0) - 4 * aa * cc;
        if (deter > 0)
        {
            // Get t values (solve equation) to see if LINE SEGMENT intersects
            double t0 = (-bb - Math.Sqrt(deter)) / (2 * aa);
            double t1 = (-bb + Math.Sqrt(deter)) / (2 * aa);

            if (0.0 <= t0 && t0 <= 1.0)
            {
                // Interpolate to get collision point
                intersection1 = Center + Vector2.Lerp(normA, normB, (float)t0);
                count++;
            }
            if (0.0 <= t1 && t1 <= 1.0)
            {
                intersection2 = Center + Vector2.Lerp(normA, normB, (float)t1);
                count++;
            }
        }
        return count;
    }
}
