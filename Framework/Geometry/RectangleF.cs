using System;
using Microsoft.Xna.Framework;

namespace Framework
{
    [Serializable]
    public class RectangleF
    {
        private float x;
        private float y;
        private float width;
        private float height;
        private float x2;
        private float y2;

        public float X
        {
            get { return x; }
            set
            {
                x = value;
                x2 = x + width;
            }
        }

        public float Y
        {
            get { return y; }
            set
            {
                y = value;
                y2 = y + height;
            }
        }

        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                x2 = x + width;
            }
        }

        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                y2 = y + height;
            }
        }

        public float X2
        {
            get { return x2; }
        }

        public float Y2
        {
            get { return y2; }
        }

        public float Top
        {
            get { return y; }
        }

        public float Bottom
        {
            get { return y2; }
        }

        public float Left
        {
            get { return x; }
        }

        public float Right
        {
            get { return x2; }
        }

        public Vector2 Center
        {
            get { return new Line(x, y, x2, y2, Color.White, false).Center; }
        }

        public RectangleF()
        {

        }

        public RectangleF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            x2 = x + width;
            y2 = y + height;
        }

        public RectangleF(Rectangle r) : this(r.X, r.Y, r.Width, r.Height) 
        {
        }

        public bool Contains(Vector2 point)
        {
            return point.X > x && point.X < x2 && point.Y > y && point.Y < y2;
        }

        public RectangleF Duplicate()
        {
            return new RectangleF(X, Y, Width, Height);
        }

        public bool Intersects(RectangleF rect)
        {
            return rect.X + rect.Width >= X && rect.Y + rect.Height >= Y && rect.X <= X + Width && rect.Y <= Y + Height;
        }

        public RectangleF Union(RectangleF rect)
        {
            var tempRect = new RectangleF
            {
                x = x < rect.x ? x : rect.x, 
                x2 = x2 > rect.x2 ? x2 : rect.x2,
                y = y < rect.y ? y : rect.y,
                y2 = y2 > rect.y2 ? y2 : rect.y2,
                height = y2 - y,
                width = x2 - x
            };
            return tempRect;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)x, (int)y, (int)width, (int)height);
        }

        public override string ToString()
        {
            return $"[{x}/{y}/{Width}/{Height}]";
        }
    }
}