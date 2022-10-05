using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework;

public static class ContinuousEllipsoidCollision
{
    /// <summary>
    /// Continuous ellipsoid collision detection and reaction with debug information (preprocessor compiling symbol 'DBG_COLL').
    /// </summary>
    public static List<Sprite> Do(
        Sprite itself,
        List<Sprite> allSprites,
        Ellipse ellipseScaled,
        ref Vector2 center,
        Vector2 position,
        ref Vector2 velocity,
        Vector2 gravity,
        float friction,
        Vector2 worlScale,
        GameTime gameTime)
    {
        LogDebugHeader(ellipseScaled, position, velocity, gravity);

        // We need to do any pre-collision detection work here. Such as adding gravity to our veoclity vector. 
        // We want to do it in this separate routine because the following routine is recursive, and we don't want to recursively add gravity.
        velocity += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // scale to collision world
        var velocityScaled = velocity * worlScale;
        var centerScaled = center * worlScale;
        var positionScaled = position * worlScale;

        // collide
        int iteration = 0;
        var removeSprites = new List<Sprite>();
        CollideWithWorld(itself, allSprites, removeSprites, ellipseScaled, ref centerScaled, ref positionScaled, ref velocityScaled, friction, worlScale, ref iteration);

        // unscale
        velocity = velocityScaled / worlScale;
        center = centerScaled / worlScale;

        return removeSprites;
    }

    private static void CollideWithWorld(
        Sprite itself,
        List<Sprite> allSprites,
        List<Sprite> removeSprites,
        Ellipse ellipse,
        ref Vector2 center,
        ref Vector2 position,
        ref Vector2 velocity,
        float friction,
        Vector2 worldScale,
        ref int iteration)
    {
        try
        {
            // How far do we need to go?
            var distanceToTravel = velocity.Length();
            // What's our destination?
            var destinationPoint = center + velocity;

            // ++++++++++++++++++++++++++++++++++ Potential colliders +++++++++++++++++++++++++++++++++++++++++++++++++
            if (!IsCollisionPossible(itself, allSprites, ref center, position, velocity, worldScale, distanceToTravel, ref iteration, out List<Sprite> potentialColliders))
                return;

            // ++++++++++++++++++++++++++++++++++ Nearest collider ++++++++++++++++++++++++++++++++++++++++++++++++++++
            var collisionFound = DetermineNearestCollider(ellipse, ref center, ref velocity, worldScale, potentialColliders, distanceToTravel, out Vector2 nearestIntersectionPoint, out float nearestDistance, out List<Sprite> collidedSprites);
            // If we never found a collision, we can safely move to the destination
            if (!collisionFound)
            {
                center += velocity;
                return;
            }

            // ++++++++++++++++++++++++++++++++++ New velocity and position (slide) +++++++++++++++++++++++++++++++++++++
            var nearestCollVect = CalculateNearestCollVect(velocity, nearestDistance);
            velocity = CalculateVelocity(center, friction, destinationPoint, nearestIntersectionPoint);
            position = CalculatePosition(ref position, ref center, nearestCollVect);

            // ++++++++++++++++++++++++++++++++++ React to collision ++++++++++++++++++++++++++++++++++++++++++++++++++++
            removeSprites.AddRange(collidedSprites.Select(s => itself.ReactToCollision(s)));

            // ++++++++++++++++++++++++++++++++++ Debug information +++++++++++++++++++++++++++++++++++++++++++++++++++++
            DebugLog(position, velocity, iteration, nearestIntersectionPoint);
        }
        catch (Exception ex)
        {
            Log.Out(LogLevel.Error, $"CollideWithWorld: {ex}");
        }

        // +++++++++++++++++++++++++++++++++++++++ Slide or bounce +++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Recursively slide (without adding gravity)
        CollideWithWorld(itself, allSprites, removeSprites, ellipse, ref center, ref position, ref velocity, friction, worldScale, ref iteration);
    }

    private static bool IsCollisionPossible(
        Sprite itself,
        List<Sprite> allSprites,
        ref Vector2 center,
        Vector2 position,
        Vector2 velocity,
        Vector2 worldScale,
        float distanceToTravel,
        ref int iteration,
        out List<Sprite> potentialColliders)
    {
        potentialColliders = new List<Sprite>();

        if (AvoidEndlessLoop(ref iteration))
            return false;

        // Do we need to bother?
        if (distanceToTravel < float.Epsilon)
            return false;

        potentialColliders = GetPotentialColliders(itself, allSprites, position / worldScale, velocity / worldScale);
#if DBG_COLL
        Debug.Log("++++++++++++++ CollideWithWorld for {0} sprite(s) iteration: {1} +++++++++++++++", potentialColliders.Count, iteration);
#endif
        // If there are none, we can safely move to the destination
        if (potentialColliders.Count == 0)
        {
            center += velocity;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Get potential colliders.
    /// </summary>
    /// <param name="itself">Sprite looking for collision.</param>
    /// <param name="position">It's unscaled position.</param>
    /// <param name="velocity">It's unscaled velocity.</param>
    /// <returns>List of potential colliders (Sprite).</returns>
    private static List<Sprite> GetPotentialColliders(Sprite itself, List<Sprite> allSprites, Vector2 position, Vector2 velocity)
    {
#if DBG_COLL
        //Debug.Log("GetPotentialColliders");
#endif
        var potColl = new List<Sprite>();
        // 1. Find the bounding box of your sphere, centered on the source position
        // 2. Find the bounding box of your sphere, centered on the destination position
        // 3. Find the bounding box of those two bounding boxes
        // 4. Find all sprites that intersect that final bounding box
        var boundingRect = GetBoundingRect(itself, position, velocity);
        foreach (var s in allSprites.Where(s => s.CollisionType != CollisionType.None && s.OnScreen && s != itself))
        {
            var boundingRect2 = GetBoundingRect(s, s.Position, s.Velocity);
            if (boundingRect.Intersects(boundingRect2))
                potColl.Add(s);
        }
        return potColl;
    }

    /// <summary>
    /// Gets the bounding box of enclosing source and destination. 
    /// </summary>
    /// <param name="s">Sprite.</param>
    /// <param name="position">Unscaled center position.</param>
    /// <param name="velocity">Unscaled velocity.</param>
    /// <returns>Bounding Box.</returns>
    private static RectangleF GetBoundingRect(Sprite s, Vector2 position, Vector2 velocity)
    {
        var destination = position + velocity;
        var sourceBB = s.GetBoundingBox(position);
        var destBB = s.GetBoundingBox(destination);
        var top = Math.Min(sourceBB.UpperLeftCorner.Y, destBB.UpperRightCorner.Y);
        var left = Math.Min(sourceBB.UpperLeftCorner.X, destBB.LowerLeftCorner.X);
        var bottom = Math.Max(sourceBB.LowerLeftCorner.Y, destBB.LowerRightCorner.Y);
        var right = Math.Max(sourceBB.UpperRightCorner.X, destBB.LowerRightCorner.X);

        return new RectangleF(left, top, right - left, bottom - top);
    }

    private static bool DetermineNearestCollider(Ellipse ellipse,
        ref Vector2 center,
        ref Vector2 velocity,
        Vector2 worldScale,
        IEnumerable<Sprite> potentialColliders,
        float distanceToTravel,
        out Vector2 nearestIntersectionPoint,
        out float nearestDistance,
        out List<Sprite> collidedSprites)
    {
        // Determine the nearest collider from the list potentialColliders
        var collisionFound = false;
        nearestDistance = -1;
        Sprite nearestCollider = null;
        nearestIntersectionPoint = Vector2.Zero;
        Line nearestIntersectionLine = null;
        // go through collidalble sprites
        var spriteIdx = -1;
        collidedSprites = new List<Sprite>();
        var collLines = new Line[4];
        foreach (Sprite colliderSprite in potentialColliders)
        {
            spriteIdx++;
#if DBG_COLL
            Debug.Log("---------------------- sprite {0} ---------------------------", spriteIdx);
#endif
            if (colliderSprite.CollisionType != CollisionType.None)
            {
#if DBG_COLL
                Debug.Log("    Collider:  {0}", colliderSprite.GetType().Name);
                Debug.Log("    Bounds:    {0}", colliderSprite.BoundingBox);
#endif
                collLines = Line.GetFromRectangle(colliderSprite.BoundingBox);
                foreach (var t in collLines)
                {
                    t.Start *= worldScale;
                    t.End *= worldScale;
                }
            }
            CollideLines(ref collidedSprites, ellipse, ref center, ref velocity,
                         collLines, colliderSprite, distanceToTravel, ref collisionFound, ref nearestDistance, ref nearestCollider, ref nearestIntersectionPoint, ref nearestIntersectionLine);
        }
#if DBG_COLL
        Debug.Log("-------------------------------------------------");
#endif
        return collisionFound;
    }

    private static void CollideLines(
        ref List<Sprite> collidedSprites,
        Ellipse ellipse,
        ref Vector2 position,
        ref Vector2 velocity,
        IEnumerable<Line> collLines,
        Sprite colliderSprite,
        float distanceToTravel,
        ref bool collisionFound,
        ref float nearestDistance,
        ref Sprite nearestCollider,
        ref Vector2 nearestIntersectionPoint,
        ref Line nearestIntersectionLine)
    {
        var lineIdx = -1;
        foreach (Line collLine in collLines)
        {
            lineIdx++;
            // line origin/normal
            var lineNormal = collLine.Normal;
            var lineNormalNormalized = lineNormal;
            lineNormalNormalized.Normalize();
#if DBG_COLL
            Debug.Lines.Add(new LineDebug(collLine, $"collLine {lineIdx}", Color.Yellow));

#endif
            // Determine the distance from the line to the position
            var lineNormalNegative = -lineNormalNormalized * 100;
            var lineRay = collLine.MakeEndless();
            var lineNormalNeg = new Line(position, lineNormalNegative);
            if (!lineRay.IntersectLine(lineNormalNeg, out Vector2 lineNormalIntersection))
                continue;
            var lineNormalPosDistance = Vector2.Distance(position, lineNormalIntersection);
            // The radius of the ellipse in the direction of the collision line normal 
            var directionalRadius = lineNormalNegative;
            directionalRadius.Normalize();
            directionalRadius *= ellipse.RadiusVector;
            var radius = directionalRadius.Length();
#if DBG_COLL
            Debug.Lines.Add(new LineDebug(lineNormalNeg, "lineNormalNegThroughPos", Color.LightBlue));
            Debug.Lines.Add(new LineDebug(lineRay, $"lineRay {lineIdx}", Color.LightPink));
            //Debug.Points.Add(new PointDebug(new Dot(lineNormalIntersection), "lineNormalPosIntersectionPoint", Color.LightBlue));
            //Debug.Points.Add(new PointDebug(new Dot(position), "position", Color.LightGreen));
            //Debug.Log("    lineNormalPosDistance: {0}", lineNormalPosDistance);
            //Debug.Log("    radius: {0}", radius);
#endif
            // Is the line intersecting ellipse?
            Vector2 lineIntersectionPoint;
            if (lineNormalPosDistance <= radius)
            {
                // Calculate the line intersection point
                // -lineNormal with length set to lineDistance
                Vector2 temp = -lineNormalNormalized * lineNormalPosDistance;
                lineIntersectionPoint = position + temp;
#if DBG_COLL
                //Debug.Points.Add(new PointDebug(new Dot(lineIntersectionPoint), "lineIntersectionPoint", Color.DarkGoldenrod));
                //Debug.Log("    abs(lineNormalPosDistance) <= radius");
                //Debug.Log("    lineIntersectionPoint: {0}", lineIntersectionPoint);
#endif
            }
            else
            {
                // Calculate the ellipsoid intersection point
                // -lineNormal with length set to radius;
                var lineNormalInRadiusLength = -lineNormalNormalized * radius;
                var ellipseIntersectionPoint = position + lineNormalInRadiusLength;
                // Calculate the line velocity intersection point
                var velNormalized = velocity;
                velNormalized.Normalize();
                var ellipseLineVelPoint = ellipseIntersectionPoint + velNormalized * 100;
                collLine.IntersectLine(new Line(ellipseIntersectionPoint.X, ellipseIntersectionPoint.Y, ellipseLineVelPoint.X, ellipseLineVelPoint.Y), out lineIntersectionPoint);
#if DBG_COLL
                //Debug.Log("    abs(lineNormalPosDistance) > radius");
                //Debug.Points.Add(new PointDebug(new Dot(ellipseIntersectionPoint), "ellipseIntersectionPoint", Color.Blue));
                //Debug.Points.Add(new PointDebug(new Dot(lineIntersectionPoint), "lineIntersectionPoint", Color.Green));
#endif
            }
            // Unless otherwise stated, our IntersectionPoint is the same point as lineIntersectionPoint
            Vector2 intersectionPoint = lineIntersectionPoint;
            // So… are they the same?
            // lineIntersectionPoint is not on current line
            if (!collLine.ContainsPoint(lineIntersectionPoint))
            {
#if DBG_COLL
                //Debug.Log("    lineIntersectionPoint is not on current line");
#endif
                // IntersectionPoint = nearest point on polygon's perimeter to planeIntersectionPoint;
                intersectionPoint = collLine.GetClosestPoint(lineIntersectionPoint);
            }
            // Using the lineIntersectionPoint, we need to reverse-intersect with the ellipse
            // float t = intersectEllipse(sourcePoint, radiusVector, polygonIntersectionPoint, negativeVelocityVector);
            Vector2 negativeVel = -velocity;
            negativeVel.Normalize();
            Vector2 pointOnNegativeVel = intersectionPoint + negativeVel * 100;
            ellipse.Position = position;
            var reverseIntersectLine = new Line(intersectionPoint.X, intersectionPoint.Y, pointOnNegativeVel.X, pointOnNegativeVel.Y);
            var eIPs = new Vector2[2];
            var intersectionCount = new Circle(ellipse.Position, radius).IntersectSegment(reverseIntersectLine.Start, reverseIntersectLine.End, out eIPs[0], out eIPs[1]);
            var ellipseIntersectionPointDistance = -100f;
            //ellipseIntersectionPoint = new Vector2(-100, -100);
            var dist2 = 100f;
            if (intersectionCount == 2)
                dist2 = Vector2.Distance(eIPs[1], intersectionPoint);
            if (intersectionCount > 0)
            {
                var dist1 = Vector2.Distance(eIPs[0], intersectionPoint);
                //ellipseIntersectionPoint = (dist1 <= dist2) ? eIPs[0] : eIPs[1];
                ellipseIntersectionPointDistance = (dist1 <= dist2) ? dist1 : dist2;
            }
#if DBG_COLL
            //Debug.Log("    lineIntersectionPoint: {0}", lineIntersectionPoint);
            //Debug.Log("    ellipseIntersectionPoint: {0}", ellipseIntersectionPoint);
            //Debug.Log("    ellipseIntersectionPointDistance: {0}", ellipseIntersectionPointDistance);
            //Debug.Lines.Add(new LineDebug(reverseIntersectLine, "reverseIntersectLine", Color.Black));
            //Debug.Points.Add(new PointDebug(new Dot(lineIntersectionPoint), "lineIntersectionPoint" + lineIdx, Color.Yellow));
            //Debug.Points.Add(new PointDebug(new Dot(intersectionPoint), "intersectionPoint" + lineIdx, Color.Yellow));
            //Debug.Points.Add(new PointDebug(new Dot(ellipseIntersectionPoint), "ellipseIntersectionPoint" + lineIdx, Color.Red));
#endif
            // Will there be an intersection with the ellipse?
            if (ellipseIntersectionPointDistance >= 0.0 && ellipseIntersectionPointDistance <= distanceToTravel)
            {
                // Vector V = negativeVelocityVector with length set to t;
                var v = negativeVel * ellipseIntersectionPointDistance;
                // Where did we intersect the ellipse?
                var intersection = intersectionPoint + v;
#if DBG_COLL
                //Debug.Log("    collision detected at {0}", intersection);
#endif
                if (!collidedSprites.Contains(colliderSprite))
                    collidedSprites.Add(colliderSprite);
                // Closest intersection thus far?
                if (!collisionFound || ellipseIntersectionPointDistance < nearestDistance)
                {
                    nearestDistance = ellipseIntersectionPointDistance;
                    nearestCollider = colliderSprite;
                    nearestIntersectionPoint = intersection;
                    nearestIntersectionLine = collLine;
                    collisionFound = true;
#if DBG_COLL
                    //Debug.Points.Add(new PointDebug(new Dot(nearestIntersectionPoint), "nearestIntersectionPoint", Color.Black));
                    //Debug.Log("    nearestIntersectionPoint {0}", nearestIntersectionPoint);
#endif
                }
            }
        }
    }

    private static Vector2 CalculateNearestCollVect(Vector2 velocity, float nearestDistance)
    {
        // calculate the nearest collision
        // Vector V = velocity with length set to (nearestDistance - EPSILON);
        var moveToNearestCollVect = velocity;
        moveToNearestCollVect.Normalize();
        //moveToNearestCollVect *= nearestDistance - 0.5f;
        moveToNearestCollVect *= nearestDistance - (velocity.Length() / 10.0f);
        return moveToNearestCollVect;
    }

    private static Vector2 CalculateVelocity(Vector2 center, float friction, Vector2 destinationPoint, Vector2 nearestIntersectionPoint)
    {
        var slideLineNormalVect = nearestIntersectionPoint - center;
        var slideLineNormal = new Line(destinationPoint, slideLineNormalVect).MakeEndless();
        var slideLine = new Line(nearestIntersectionPoint, slideLineNormal.Normal).MakeEndless();
        slideLineNormal.IntersectLine(slideLine, out Vector2 slidingPoint);
        var velocity = slidingPoint - nearestIntersectionPoint;
        velocity *= friction;

        return velocity;
    }

    private static Vector2 CalculatePosition(ref Vector2 position, ref Vector2 center, Vector2 moveToNearestCollVect)
    {
        center += moveToNearestCollVect;
        position += moveToNearestCollVect;
        return position;
    }

    private static bool AvoidEndlessLoop(ref int iteration)
    {
        iteration++;
        if (iteration > 10)
        {
            Log.Out(LogLevel.Error, "Collision.CollideWithWorld Iteration > 100");
            return true;
        }
        return false;
    }

    private static void LogDebugHeader(Ellipse ellipseScaled, Vector2 position, Vector2 velocity, Vector2 gravity)
    {
#if DBG_COLL
        Debug.Clear();
        Debug.Log("###############################################");
        Debug.Log("### DoContinuousEllipseDetectionAndReaction ###");
        Debug.Log("###############################################");
        Debug.Log("    Position:  {0}", position);
        Debug.Log("    Velocity:  {0}", velocity);
        Debug.Log("    Gravity:   {0}", gravity);
        //Debug.Points.Add(new PointDebug(new Dot(center), "Center", Color.Blue));
        Debug.Ellipses.Add(new EllipseDebug(ellipseScaled, "bounding ellipse", Color.Red, 30));
#endif
    }

    private static void DebugLog(Vector2 position, Vector2 velocity, int iteration, Vector2 nearestIntersectionPoint)
    {
#if DBG_COLL
        Debug.Log("    new Position:  {0}", position);
        Debug.Log("    new Velocity:  {0}", velocity);
        Debug.Log("    iteration:     {0}", iteration);
        //Debug.Points.Add(new PointDebug(new Dot(center), "move to nearest collision (center)", Color.Green));
        //Debug.Points.Add(new PointDebug(new Dot(slidingPoint), "slidingPoint", Color.Pink));
        //Debug.Points.Add(new PointDebug(new Dot(destinationPoint), "destinationPoint", Color.Green));
        Debug.Points.Add(new PointDebug(new Dot(nearestIntersectionPoint), "nearestIntersectionPoint", Color.Black));
        //Debug.Lines.Add(new LineDebug(new Line(nearestIntersectionPoint, velocity * 100), "newVelocity", Color.Green));
        //Debug.Lines.Add(new LineDebug(slideLine, "slideLine", Color.Red));
        //Debug.Lines.Add(new LineDebug(slideLineNormal, "slidingVect", Color.Green));
        //Debug.Lines.Add(new LineDebug(new Line(center, velocity*100), "new velocity", Color.Red));
        //Debug.Log("    move to nearest collision {0} (center)", center);
        //Debug.Log("    velCutOff (distanceToTravel - nearestDistance) {0}", velCutOff);
        //Debug.Log("    destinationPoint (nearestLineIntersectionPoint + velCutOff) {0}", destinationPoint);
        //Debug.Log("    newDestinationPoint (nearestIntersectionLine.Intersect(projectSlideOnLine)) {0}", newDestinationPoint);
        //Debug.Log("    slideDist {0}", slideDist);
        //Debug.Log("    slideLineNormal {0}", slideLineNormalVect);
        //Debug.Log("    slide to point {0}", newDestinationPoint);
        //Debug.Log("    new velocity {0}", velocity);
        //Debug.Log("    iteration {0}", iteration);
        //Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
#endif
    }
}
