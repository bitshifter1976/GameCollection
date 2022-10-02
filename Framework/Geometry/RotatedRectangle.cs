using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Framework
{
    public class RotatedRectangle
    {
        public RectangleF CollisionRectangle;
        public float Rotation;
        public Vector2 Origin;

        public float X
        {
            get { return CollisionRectangle.X; }
        }

        public float Y
        {
            get { return CollisionRectangle.Y; }
        }

        public float Width
        {
            get { return CollisionRectangle.Width; }
        }

        public float Height
        {
            get { return CollisionRectangle.Height; }
        }

        public Vector2 UpperLeftCorner
        {
            get
            {
                var upperLeft = new Vector2(CollisionRectangle.Left, CollisionRectangle.Top);
                upperLeft = RotatePoint(upperLeft, Origin, Rotation);
                return upperLeft;
            }
        }

        public Vector2 UpperRightCorner
        {
            get
            {
                var upperRight = new Vector2(CollisionRectangle.Right, CollisionRectangle.Top);
                upperRight = RotatePoint(upperRight, Origin, Rotation);
                return upperRight;
            }
        }

        public Vector2 LowerLeftCorner
        {
            get
            {
                var lowerLeft = new Vector2(CollisionRectangle.Left, CollisionRectangle.Bottom);
                lowerLeft = RotatePoint(lowerLeft, Origin, Rotation);
                return lowerLeft;
            }
        }

        public Vector2 LowerRightCorner
        {
            get
            {
                var lowerRight = new Vector2(CollisionRectangle.Right, CollisionRectangle.Bottom);
                lowerRight = RotatePoint(lowerRight, Origin, Rotation);
                return lowerRight;
            }
        }

        public Vector2 Center
        {
            get
            {
                var l1 = new Line(UpperLeftCorner.X,UpperLeftCorner.Y,LowerRightCorner.X,LowerRightCorner.Y, Color.White, false);
                var l2 = new Line(LowerLeftCorner.X, LowerLeftCorner.Y, UpperRightCorner.X, UpperRightCorner.Y, Color.White, false);
                l1.IntersectLine(l2, out var intersectionPoint);
                return intersectionPoint;
            }
        }

        public RotatedRectangle(RectangleF rect, float rotation)
        {
            CollisionRectangle = rect;
            Rotation = rotation;
            Origin = rect.Center;
        }

        /// <summary>
        /// Used for changing the X and Y position of the RotatedRectangle
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        public void ChangePosition(float posX, float posY)
        {
            CollisionRectangle.X += posX;
            CollisionRectangle.Y += posY;
        }

        /// <summary>
        /// This intersects method can be used to check a standard XNA framework Rectangle
        /// object and see if it collides with a Rotated Rectangle object
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool Intersects(RectangleF rect)
        {
            return Intersects(new RotatedRectangle(rect, 0.0f));
        }

        /// <summary>
        /// Check to see if two Rotated Rectangls have collided
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool Intersects(RotatedRectangle rect)
        {
            //Calculate the Axis we will use to determine if a collision has occurred
            //Since the objects are rectangles, we only have to generate 4 Axis (2 for
            //each rectangle) since we know the other 2 on a rectangle are parallel.
            List<Vector2> rectangleAxis = new List<Vector2>
            {
                UpperRightCorner - UpperLeftCorner,
                UpperRightCorner - LowerRightCorner,
                rect.UpperLeftCorner - rect.LowerLeftCorner,
                rect.UpperLeftCorner - rect.UpperRightCorner
            };

            //Cycle through all of the Axis we need to check. If a collision does not occur
            //on ALL of the Axis, then a collision is NOT occurring. We can then exit out 
            //immediately and notify the calling function that no collision was detected. If
            //a collision DOES occur on ALL of the Axis, then there is a collision occurring
            //between the rotated rectangles. We know this to be true by the Seperating Axis Theorem
            return rectangleAxis.All(aAxis => IsAxisCollision(rect, aAxis));
        }

        /// <summary>
        /// Determines if a collision has occurred on an Axis of one of the
        /// planes parallel to the Rectangle
        /// </summary>
        /// <param name="theRectangle"></param>
        /// <param name="aAxis"></param>
        /// <returns></returns>
        private bool IsAxisCollision(RotatedRectangle theRectangle, Vector2 aAxis)
        {
            //Project the corners of the Rectangle we are checking on to the Axis and
            //get a scalar value of that project we can then use for comparison
            var rectangleAScalars = new List<int>
            {
                GenerateScalar(theRectangle.UpperLeftCorner, aAxis),
                GenerateScalar(theRectangle.UpperRightCorner, aAxis),
                GenerateScalar(theRectangle.LowerLeftCorner, aAxis),
                GenerateScalar(theRectangle.LowerRightCorner, aAxis)
            };
            //Project the corners of the current Rectangle on to the Axis and
            //get a scalar value of that project we can then use for comparison
            var rectangleBScalars = new List<int>
            {
                GenerateScalar(UpperLeftCorner, aAxis),
                GenerateScalar(UpperRightCorner, aAxis),
                GenerateScalar(LowerLeftCorner, aAxis),
                GenerateScalar(LowerRightCorner, aAxis)
            };
            //Get the Maximum and Minium Scalar values for each of the Rectangles
            int rectangleAMinimum = rectangleAScalars.Min();
            int rectangleAMaximum = rectangleAScalars.Max();
            int rectangleBMinimum = rectangleBScalars.Min();
            int rectangleBMaximum = rectangleBScalars.Max();
            //If we have overlaps between the Rectangles (i.e. Min of B is less than Max of A)
            //then we are detecting a collision between the rectangles on this Axis
            if (rectangleBMinimum <= rectangleAMaximum && rectangleBMaximum >= rectangleAMaximum)
                return true;
            if (rectangleAMinimum <= rectangleBMaximum && rectangleAMaximum >= rectangleBMaximum)
                return true;

            return false;
        }

        /// <summary>
        /// Generates a scalar value that can be used to compare where corners of 
        /// a rectangle have been projected onto a particular axis. 
        /// </summary>
        /// <param name="theRectangleCorner"></param>
        /// <param name="theAxis"></param>
        /// <returns></returns>
        private static int GenerateScalar(Vector2 theRectangleCorner, Vector2 theAxis)
        {
            //Using the formula for Vector projection. Take the corner being passed in
            //and project it onto the given Axis
            float numerator = (theRectangleCorner.X * theAxis.X) + (theRectangleCorner.Y * theAxis.Y);
            float denominator = (theAxis.X * theAxis.X) + (theAxis.Y * theAxis.Y);
            float divisionResult = numerator / denominator;
            Vector2 cornerProjected = new Vector2(divisionResult * theAxis.X, divisionResult * theAxis.Y);
            //Now that we have our projected Vector, calculate a scalar of that projection
            //that can be used to more easily do comparisons
            float scalar = (theAxis.X * cornerProjected.X) + (theAxis.Y * cornerProjected.Y);
            return (int)scalar;
        }

        /// <summary>
        /// Rotate a point from a given location and adjust using the Origin we
        /// are rotating around
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector2 RotatePoint(Vector2 point, Vector2 origin, float rotation)
        {
            var rotated = new Vector2
            {
                X = (float)(Math.Cos(rotation)*(point.X - origin.X) - Math.Sin(rotation)*(point.Y - origin.Y) + origin.X),
                Y = (float)(Math.Sin(rotation)*(point.X - origin.X) + Math.Cos(rotation)*(point.Y - origin.Y) + origin.Y)
            };
            return rotated;
        }

        public override string ToString()
        {
            return $"[{UpperLeftCorner.X}/{UpperLeftCorner.Y}/{Width}/{Height}/{Rotation}]";
        }

        public List<Line> ToLines(Color color)
        {
            return Line.GetFromRectangle(this, color).ToList();
        }
    }
}
