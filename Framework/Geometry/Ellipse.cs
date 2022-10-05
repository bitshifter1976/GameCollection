using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Framework
{
    public class Ellipse
    {
        #region members

        public Vector2 Position;
        public float Width;
        public float Height;

        #endregion

        #region properties

        public Vector2 RadiusVector
        {
            get { return new Vector2(Width/2, Height/2); }
        }

        #endregion

        #region methods

        public Ellipse(Vector2 pos, float width, float height)
        {
            Position = pos;
            Width = width;
            Height = height;
        }

        /*public float DistanceToPoint(Vector2 point)
        {
            float x = Width * Height * point.X / (float)Math.Sqrt((double)((Height*point.X)*(Height*point.X) + (Width*point.Y)*(Width*point.Y)));
            float y = Width * Height * point.Y / (float)Math.Sqrt((double)((Height*point.X)*(Height*point.X) + (Width*point.Y)*(Width*point.Y)));
            return (float)Math.Sqrt((double)((point.X-x)*(point.X-x) + (point.Y-y)*(point.Y-y)));
        }*/

        /*public Vector2[] Intersect(Line line) 
        {
            ArrayList result = new ArrayList();
            Vector2 origin = line.Start;
            Vector2 dir    = line.GetDirectionVector();
            Vector2 center = Position;
            Vector2 diff   = origin - center;
            Vector2 mDir = new Vector2(dir.X / (Width / 2 * Width / 2), dir.Y / (Height / 2 * Height / 2));
            Vector2 mDiff = new Vector2(diff.X / (Width / 2 * Width / 2), diff.Y / (Height / 2 * Height / 2));

            var a = Vector2.Dot(dir, mDir);
            var b = Vector2.Dot(dir, mDiff);
            var c = Vector2.Dot(diff,mDiff) - 1.0;
            var d = b*b - a*c;

            if (d < 0) 
            {
                // result = new Intersection("Outside");
                return new Vector2[0];
            } 
            else if (d > 0) 
            {
                double root = Math.Sqrt(d);
                double t_a  = (-b - root) / a;
                double t_b  = (-b + root) / a;

                if ((t_a < 0 || 1 < t_a) && (t_b < 0 || 1 < t_b)) 
                {
                    if ((t_a < 0 && t_b < 0) || (t_a > 1 && t_b > 1))
                    {
                        //result = new Intersection("Outside");
                        return new Vector2[0];
                    }
                    else
                    {
                        //result = new Intersection("Inside");
                        return new Vector2[0];
                    }
                } 
                else 
                {
                    //result = new Intersection("Intersection");
                    if (0 <= t_a && t_a <= 1)
                    {
                        //result.appendPoint(a1.lerp(a2, t_a));
                        //line.Start
                        result.Add(Vector2.Lerp(line.Start, line.End, (float)t_a));
                    }
                    if (0 <= t_b && t_b <= 1)
                    {
                        //((result.appendPoint(a1.lerp(a2, t_b));
                        result.Add(Vector2.Lerp(line.Start, line.End, (float)t_b));
                    }
                }
            } 
            else 
            {
                double t = -b/a;
                if (0 <= t && t <= 1) 
                {
                    //result = new Intersection("Intersection");
                    //result.appendPoint( a1.lerp(a2, t) );
                    result.Add(Vector2.Lerp(line.Start, line.End, (float)t));
                } 
                else 
                {
                    //result = new Intersection("Outside");
                    return new Vector2[0];
                }
            }
            Vector2[] r = new Vector2[result.Count];
            result.CopyTo(r);
            return r;
        }*/

        public override string ToString()
        {
            return String.Format("Pos {0} Width [{1}] Height [{2}]", Position, Width, Height);
        }

        public void Draw(Color color, int resolution)
        {
            ArrayList vectors = CreateEllipsePoints(Width / 2, Height / 2, 0, resolution);
            Vector2 oldPos = new Vector2(0, 0);
            Vector2 firstPos = new Vector2(0, 0);
            for (int i=0; i<vectors.Count; i++)
            {
                Vector2 v = (Vector2)vectors[i];
                Vector2 pos = Position + v;
                if (i > 0)
                    new Line(oldPos.X, oldPos.Y, pos.X, pos.Y).Draw(color, 1);
                else
                    firstPos = pos;
                oldPos = pos;
            }
            new Line(firstPos.X, firstPos.Y, oldPos.X, oldPos.Y).Draw(color, 1);
        }

        /// <summary>
        /// Creates an ellipse starting from 0, 0 with the given width and height.
        /// Vectors are generated using the parametric equation of an ellipse.
        /// </summary>
        /// <param name="semimajor_axis">The width of the ellipse at its center.</param>
        /// <param name="semiminor_axis">The height of the ellipse at its center.</param>
        /// <param name="angle_offset">The counterlockwise rotation in radians.</param>
        /// <param name="sides">The number of sides on the ellipse (a higher value yields more resolution).</param>
        public ArrayList CreateEllipsePoints(float semimajor_axis, float semiminor_axis, float angle_offset, int sides)
        {
            ArrayList vectors = new ArrayList();
            float max = 2.0f * (float)Math.PI;
            float step = max / (float)sides;
            float h = 0.0f;
            float k = 0.0f;

            for (float t = 0.0f; t < max; t += step)
            {
                // center point: (h,k); add as argument if you want (to circumvent modifying this.Position)
                // x = h + a*cos(t)  -- a is semimajor axis, b is semiminor axis
                // y = k + b*sin(t)
                vectors.Add(new Vector2((float)(h + semimajor_axis * Math.Cos(t)),
                                        (float)(k + semiminor_axis * Math.Sin(t))));
            }

            // then add the first vector again so it's a complete loop
            vectors.Add(new Vector2((float)(h + semimajor_axis * Math.Cos(step)),
                                    (float)(k + semiminor_axis * Math.Sin(step))));

            // now rotate it as necessary
            Matrix m = Matrix.CreateRotationZ(angle_offset);
            for (int i = 0; i < vectors.Count; i++)
            {
                vectors[i] = Vector2.Transform((Vector2)vectors[i], m);
            }
            return vectors;
        }

        #endregion
    }
}
