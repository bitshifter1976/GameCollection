using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Framework
{
    public static class Rand
    {
        private static readonly Random rand = new();

        public static float Float(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }

        public static int Int(int min, int max)
        {
            return rand.Next(min, max+1);
        }

        public static bool Bool(int strikes, int outOfCount)
        {
            var i = Int(1, outOfCount);
            return i >= 1 && i <= strikes;
        }

        public static double Double(double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }

        public static Color Color(params Color[] colors)
        {
            return colors[rand.Next(colors.Length)];
        }
    } 
}
