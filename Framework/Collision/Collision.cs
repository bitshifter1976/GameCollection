using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Framework
{
    public static class Collision
    {
        public static CollisionDirection GetCollisionDirection(RectangleF r1, RectangleF r2)
        {
            var directions = new List<CollisionDirection>();
            var result = CollisionDirection.Undefined;
            // where did collision occur? on player top, left, right or bottom?
            if (r2.Top <= r1.Bottom && r2.Bottom >= r1.Bottom && r1.Top <= r2.Top)
                directions.Add(CollisionDirection.Bottom);
            if (r2.Top <= r1.Top && r2.Bottom >= r1.Top && r1.Bottom >= r2.Bottom)
                directions.Add(CollisionDirection.Top);
            if (r2.Left <= r1.Left && r2.Right >= r1.Left && r1.Right >= r2.Right)
                directions.Add(CollisionDirection.Left);
            if (r2.Left <= r1.Right && r2.Right >= r1.Right && r1.Left <= r2.Left)
                directions.Add(CollisionDirection.Right);

            // now combine directions
            switch (directions.Count)
            {
                // no collision
                case 0:
                {
                    result = CollisionDirection.None;
                    break;
                }
                // collision on one side
                case 1:
                {
                    result = directions[0];
                    break;
                }
                // collision on corner
                case 2:
                {
                    if ((directions[0] == CollisionDirection.Top && directions[1] == CollisionDirection.Left) || (directions[1] == CollisionDirection.Top && directions[2] == CollisionDirection.Left))
                        result = CollisionDirection.TopLeft;
                    if ((directions[0] == CollisionDirection.Top && directions[1] == CollisionDirection.Right) || (directions[1] == CollisionDirection.Top && directions[2] == CollisionDirection.Right))
                        result = CollisionDirection.TopRight;
                    if ((directions[0] == CollisionDirection.Bottom && directions[1] == CollisionDirection.Left) || (directions[1] == CollisionDirection.Bottom && directions[2] == CollisionDirection.Left))
                        result = CollisionDirection.BottomLeft;
                    if ((directions[0] == CollisionDirection.Bottom && directions[1] == CollisionDirection.Right) || (directions[1] == CollisionDirection.Bottom && directions[2] == CollisionDirection.Right))
                        result = CollisionDirection.BottomRight;
                    break;
                }
                case 3:
                {
                    int countTop = 0, countBottom = 0, countLeft = 0, countRight = 0;
                    foreach (var d2 in directions)
                    {
                        if (d2 == CollisionDirection.Top)
                            countTop++;
                        else if (d2 == CollisionDirection.Bottom)
                            countBottom++;
                        else if (d2 == CollisionDirection.Left)
                            countLeft++;
                        else if (d2 == CollisionDirection.Right)
                            countRight++;
                    }
                    if (countTop == 2)
                        result = CollisionDirection.Top;
                    if (countBottom == 2)
                        result = CollisionDirection.Bottom;
                    if (countLeft == 2)
                        result = CollisionDirection.Left;
                    if (countRight == 2)
                        result = CollisionDirection.Right;

                    break;
                }
                case 4:
                    result = CollisionDirection.Inside;
                    break;
            }

            return result;
        }

        public static bool Do(Sprite sprite, Sprite sprite2)
        {
            if (sprite.CollisionType == CollisionType.None || sprite2.CollisionType == CollisionType.None) 
                return false;
            if (sprite.CollisionType == CollisionType.BoundingBox && sprite2.CollisionType == CollisionType.BoundingBox && sprite.BoundingBox.Intersects(sprite2.BoundingBox))
                return true;
            if (sprite.CollisionType == CollisionType.BoundingBoxRotated && sprite2.CollisionType == CollisionType.BoundingBoxRotated && sprite.BoundingBoxRotated.Intersects(sprite2.BoundingBoxRotated))
                return true;
            if (sprite.CollisionType == CollisionType.BoundingBoxRotated && sprite2.CollisionType == CollisionType.BoundingBox && sprite.BoundingBoxRotated.Intersects(sprite2.BoundingBoxF))
                return true;
            if (sprite.CollisionType == CollisionType.BoundingBox && sprite2.CollisionType == CollisionType.BoundingBoxRotated && sprite2.BoundingBoxRotated.Intersects(sprite.BoundingBoxF))
                return true;

            return false;
        }
    }
}
