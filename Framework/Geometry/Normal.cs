using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Framework
{
    public class Normal
    {
        public static Vector2 GetFromLine(Line line)
        {
            float dx=line.End.X - line.Start.X;
            float dy=line.End.Y - line.Start.Y;
            return new Vector2(dy, -dx);
        }

        public static Vector2[] GetFromRectangle(Rectangle rect)
        {
            Vector2[] normals = new Vector2[4];
            normals[0] = GetFromLine(new Line(rect.Left, rect.Top, rect.Right, rect.Top));
            normals[1] = GetFromLine(new Line(rect.Right, rect.Bottom, rect.Left, rect.Bottom));
            normals[2] = GetFromLine(new Line(rect.Left, rect.Bottom, rect.Right, rect.Top));
            normals[3] = GetFromLine(new Line(rect.Right, rect.Top, rect.Right, rect.Bottom));
            return normals;
        }
    }
}
