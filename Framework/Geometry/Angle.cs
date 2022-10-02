using Microsoft.Xna.Framework;
using System;

namespace Framework.Geometry
{
    public static class Angle
    {
        public static float DegreeToRadian(float angle)
        {
            return (float)(Math.PI * angle / 180.0f);
        }

        public static float RadianToDegree(float angle)
        {
            return (float)(angle * (180.0f / Math.PI));
        }

        public static Vector2 GetDirectionVectorFromDegrees(float degrees)
        {
            var north = new Vector2(1, 0);
            var radians = DegreeToRadian(degrees);
            var rotationMatrix = Matrix.CreateRotationZ(radians);
            return Vector2.Transform(north, rotationMatrix);
        }
    }
}
