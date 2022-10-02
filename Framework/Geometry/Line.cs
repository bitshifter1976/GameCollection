using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework
{
    public class Line
    {
        private Texture2D sprite;
        public Color Color;

        public Vector2 Normal
        {
            get 
            {
                float dx = End.X - Start.X;
                float dy = End.Y - Start.Y;
                // counter clockwise normal
                return new Vector2(dy, -dx);
            }
        }
        public Vector2 Center
        {
            get
            {
                Vector2 v = new Vector2();
                v.X = Math.Min(Start.X, End.X) + Math.Abs(End.X - Start.X) / 2;
                v.Y = Math.Min(Start.Y, End.Y) + Math.Abs(End.Y - Start.Y) / 2;
                return v;
            }
        }
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

        public Line(float x1, float y1, float x2, float y2) : this(x1, y1, x2, y2, Color.Black, false)
        {
        }

        public Line(float x1, float y1, float x2, float y2, Color color, bool thick) : this(new Vector2(x1, y1), new Vector2(x2, y2), color, thick)
        {
        }

        public Line(Vector2 p1, Vector2 p2) : this(p1, p2, Color.Black, false)
        {
        }

        public Line(Vector2 p1, Vector2 p2, Color color, bool thick)
        {
            Color = color;
            Start = p1;
            End = p2;
            sprite = Manager.Content.Load<Texture2D>(thick ? "graphic/common/dotBig" : "graphic/common/dot");
        }

        public static Line[] GetFromRectangle(Rectangle rect, Color color)
        {
            Line[] lines = new Line[4];
            lines[0] = new Line(rect.Left, rect.Top, rect.Right, rect.Top, color, false);
            lines[1] = new Line(rect.Right, rect.Bottom, rect.Left, rect.Bottom, color, false);
            lines[2] = new Line(rect.Left, rect.Bottom, rect.Left, rect.Top, color, false);
            lines[3] = new Line(rect.Right, rect.Top, rect.Right, rect.Bottom, color, false);
            return lines;
        }

        public static Line[] GetFromRectangle(RectangleF rect, Color color)
        {
            Line[] lines = new Line[4];
            lines[0] = new Line(rect.Left, rect.Top, rect.Right, rect.Top, color, false);
            lines[1] = new Line(rect.Right, rect.Bottom, rect.Left, rect.Bottom, color, false);
            lines[2] = new Line(rect.Left, rect.Bottom, rect.Left, rect.Top, color, false);
            lines[3] = new Line(rect.Right, rect.Top, rect.Right, rect.Bottom, color, false);
            return lines;
        }

        public static Line[] GetFromRectangle(RotatedRectangle rect, Color color)
        {
            Line[] lines = new Line[4];
            lines[0] = new Line(rect.UpperLeftCorner.X, rect.UpperLeftCorner.Y, rect.UpperRightCorner.X, rect.UpperRightCorner.Y, color, false);
            lines[1] = new Line(rect.LowerRightCorner.X, rect.LowerRightCorner.Y, rect.LowerLeftCorner.X, rect.LowerLeftCorner.Y, color, false);
            lines[2] = new Line(rect.LowerLeftCorner.X, rect.LowerLeftCorner.Y, rect.UpperLeftCorner.X, rect.UpperLeftCorner.Y, color, false);
            lines[3] = new Line(rect.UpperRightCorner.X, rect.UpperRightCorner.Y, rect.LowerRightCorner.X, rect.LowerRightCorner.Y, color, false);
            return lines;
        }

        public Line MakeEndless()
        {
            Vector2 direction = End - Start;
            direction *= 100;
            Vector2 oldEnd = End;
            Vector2 newEnd = Start + direction;
            Vector2 newStart = oldEnd - direction;
            return new Line(newStart.X, newStart.Y, newEnd.X, newEnd.Y, Color, false);
        }

        public bool ContainsPoint(Vector2 point)
        {
            float bottomY = Math.Min(Start.Y, End.Y);
            float topY = Math.Max(Start.Y, End.Y);
            bool heightIsRight = point.Y >= bottomY &&
                                 point.Y <= topY;
            //Vertical line, slope is divideByZero error!
            if (Start.X == End.X)
            {
                if (point.X == Start.X && heightIsRight)
                {
                    return true;
                }
                return false;
            }
            float slope = (End.X - Start.X) / (End.Y - Start.Y);
            bool onLine = (Start.Y - point.Y) == (slope * (Start.X - point.X));
            if (onLine && heightIsRight)
            {
                return true;
            }
            return false;
        }

        public Vector2 GetClosestPoint(Vector2 point)
        {
            Vector2 ap = point - Start;  
            Vector2 ab = End - Start;    
            float ab2 = ab.X * ab.X + ab.Y * ab.Y;    
            float ap_ab = ap.X * ab.X + ap.Y * ab.Y;    
            float t = ap_ab / ab2;

            if (t < 0.0f)
                return Start;
            if (t > 1.0f)
                return End;

            Vector2 closest = Start + ab * t;    
            return closest;
        }

        public Vector2 GetDirectionVector()
        {
            Vector2 delta = End - Start;

            float distance = delta.Length();

            if (distance == 0.0f)
            {
                return Start;
            }
            Vector2 direction = delta / distance;
            return Start + direction * distance;
        }

        public bool IntersectLine(Line line, out Vector2 intersectionPoint)
        {
            intersectionPoint = new Vector2();
            // Denominator for ua and ub are the same, so store this calculation
            float d = (line.End.Y - line.Start.Y) * (End.X - Start.X) - (line.End.X - line.Start.X) * (End.Y - Start.Y);
            // Make sure there is not a division by zero - this also indicates that the lines are parallel.  
            // If n_a and n_b were both equal to zero the lines would be on top of each other (coincidental).  
            // This check is not done because it is not necessary for this implementation (the parallel check accounts for this).
            if (d == 0)
                return false;
            //n_a and n_b are calculated as seperate values for readability
            float n_a = (line.End.X - line.Start.X) * (Start.Y - line.Start.Y) - (line.End.Y - line.Start.Y) * (Start.X - line.Start.X);
            float n_b = (End.X - Start.X) * (Start.Y - line.Start.Y) - (End.Y - Start.Y) * (Start.X - line.Start.X);
            // Calculate the intermediate fractional point that the lines potentially intersect.
            float ua = n_a / d;
            float ub = n_b / d;
            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                intersectionPoint.X = Start.X + (ua * (End.X - Start.X));
                intersectionPoint.Y = Start.Y + (ua * (End.Y - Start.Y));
                return true;
            }
            return false;
        }

        public void Draw()
        {
            Vector2 origin = new Vector2(0.5f, 0.0f);
            Vector2 diff = End - Start;
            Vector2 scale = new Vector2(1.0f, diff.Length() / sprite.Height);
            float angle = (float)(Math.Atan2(diff.Y, diff.X)) - MathHelper.PiOver2;
            Manager.SpriteBatch.Draw(sprite, Start, null, Color, angle, origin, scale, SpriteEffects.None, 1.0f);
        }

        public override string ToString()
        {
            return Start + " - " + End;
        }
    }
}
