using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Framework;

namespace AxeGameCollection.GameObjects.MrSunny
{
    public static class Weapon
    {
        private static float angle;
        private const float shotPower = 3f;
        private const float duckShotPower = 5f;
        private const float shotScale = 0.2f;
        private static int shotCount;

        public static int Count { get => shotCount; set => shotCount = value; }

        public static void ChangeCount(int count)
        {
            shotCount = count;
        }

        public static void Shoot(Vector2 pos, int power, bool ducking, Direction direction)
        {
            if (shotCount > 0)
            {
                switch (direction)
                {
                    case Direction.Up:
                        angle = 0;
                        break;
                    case Direction.UpRight:
                        angle = 55;
                        break;
                    case Direction.UpLeft:
                        angle = 305;
                        break;
                    case Direction.Right:
                        angle = 80;
                        break;
                    case Direction.Left:
                        angle = 280;
                        break;
                    case Direction.DownRight:
                        angle = 135;
                        break;
                    case Direction.DownLeft:
                        angle = 235;
                        break;
                    case Direction.Down:
                        angle = 180;
                        break;
                    default:
                        angle = 0;
                        break;
                }
                var up = new Vector2(0, -1);
                var rotMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(angle));
                var velocity = Vector2.Transform(up, rotMatrix);
                var p = pos + velocity * 50;
                var pwr = ducking ? power / (duckShotPower / duckShotPower) : power / (duckShotPower / shotPower);
                SpriteManager.Add(new Shot(new Vector2(p.X, p.Y), velocity * pwr, shotScale));
                shotCount--;
            }
        }
    }
}
