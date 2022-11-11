using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework;

public static class ContinuousEllipsoidCollision
{
    /// <summary>
    /// Continuous ellipsoid collision detection and reaction with debug information (preprocessor compiling symbol 'DBG_COLL').
    /// https://paulnettle.com/pub/FluidStudios/CollisionDetection/Fluid_Studios_Generic_Collision_Detection_for_Games_Using_Ellipsoids.pdf
    /// </summary>
    public static List<Sprite> Do(
        Sprite itself,
        Ellipse ellipseScaled,
        ref Vector2 center,
        ref Vector2 position,
        ref Vector2 velocity,
        Vector2 gravity,
        float friction,
        Vector2 worlScale,
        GameTime gameTime,
        List<Sprite> sprites)
    {
        // We need to do any pre-collision detection work here. Such as adding gravity to our veoclity vector. 
        // We want to do it in this separate routine because the following routine is recursive, and we don't want to recursively add gravity.
        velocity += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        LogDebugHeader(ellipseScaled, position, center, velocity, gravity*(float)gameTime.ElapsedGameTime.TotalSeconds);
        // scale to collision world
        var velocityScaled = velocity * worlScale;
        var centerScaled = center * worlScale;
        var positionScaled = position * worlScale;
        // collide
        int iteration = 0;
        var removeSprites = new List<Sprite>();
        CollideWithWorld(itself, removeSprites, ellipseScaled, ref centerScaled, ref positionScaled, ref velocityScaled, friction, worlScale, ref iteration, sprites);
        // unscale
        velocity = velocityScaled / worlScale;
        center = centerScaled / worlScale;
        position = positionScaled / worlScale;
        // log debug information
        DebugLog(position, velocity, iteration);

        return removeSprites;
    }

    private static void CollideWithWorld(
        Sprite itself,
        List<Sprite> removeSprites,
        Ellipse ellipse,
        ref Vector2 center,
        ref Vector2 position,
        ref Vector2 velocity,
        float friction,
        Vector2 worldScale,
        ref int iteration,
        List<Sprite> sprites)
    {
        try
        {
            // How far do we need to go?
            var distanceToTravel = velocity.Length();
            // What's our destination?
            var destinationPoint = center + velocity;

            // ++++++++++++++++++++++++++++++++++ Potential colliders +++++++++++++++++++++++++++++++++++++++++++++++++
            List<Sprite> potentialColliders;
            if (!IsCollisionPossible(itself, position, velocity, worldScale, distanceToTravel, ref iteration, sprites, out potentialColliders))
            {
                center += velocity;
                position += velocity;
                return;
            }

            // ++++++++++++++++++++++++++++++++++ Nearest collider ++++++++++++++++++++++++++++++++++++++++++++++++++++
            var collisionFound = DetermineNearestCollider(ellipse, ref center, ref velocity, worldScale, potentialColliders, distanceToTravel, out var nearestIntersectionPoint, out var nearestDistance, out var collidedSprites);
            // If we never found a collision, we can safely move to the destination
            if (!collisionFound)
            {
                center += velocity;
                position += velocity;
                return;
            }

            // ++++++++++++++++++++++++++++++++++ New velocity and position (slide) +++++++++++++++++++++++++++++++++++++
            var nearestCollVect = CalculateNearestCollVect(velocity, nearestDistance);
            center += nearestCollVect;
            position += nearestCollVect;
            velocity = CalculateVelocity(center, friction, destinationPoint, nearestIntersectionPoint); 

            // ++++++++++++++++++++++++++++++++++ React to collision ++++++++++++++++++++++++++++++++++++++++++++++++++++
            removeSprites.AddRange(collidedSprites.Select(s => itself.ReactToCollision(s)));
        }
        catch (Exception ex)
        {
            Log.Out(LogLevel.Error, "CollideWithWorld: {0}", ex);
        }

        // +++++++++++++++++++++++++++++++++++++++ Slide or bounce +++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Recursively slide (without adding gravity)
        CollideWithWorld(itself, removeSprites, ellipse, ref center, ref position, ref velocity, friction, worldScale, ref iteration, sprites);
    }

    private static bool IsCollisionPossible(
        Sprite itself,
        Vector2 position,
        Vector2 velocity,
        Vector2 worldScale,
        float distanceToTravel,
        ref int iteration,
        List<Sprite> sprites,
        out List<Sprite> potentialColliders)
    {
        potentialColliders = new List<Sprite>();

        if (AvoidEndlessLoop(ref iteration))
            return false;

        // Do we need to bother?
        if (distanceToTravel < float.Epsilon)
            return false;

        potentialColliders = GetPotentialColliders(itself, sprites, position / worldScale, velocity / worldScale);
#if DBG_COLL
        Log.Out(LogLevel.Dbg3, "++++++++++++++ {0} potential collider(s) iteration: {1} +++++++++++++++", potentialColliders.Count, iteration);
#endif

        return true;
    }

    /// <summary>
    /// Get potential colliders.
    /// </summary>
    /// <param name="itself">Sprite looking for collision.</param>
    /// <param name="position">It's unscaled position.</param>
    /// <param name="velocity">It's unscaled velocity.</param>
    /// <returns>List of potential colliders (Sprite).</returns>
    private static List<Sprite> GetPotentialColliders(Sprite itself, List<Sprite> sprites, Vector2 position, Vector2 velocity)
    {
#if DBG_COLL
        //Log.Out(LogLevel.Dbg3, "GetPotentialColliders");
#endif
        var potColl = new List<Sprite>();
        // 1. Find the bounding box of your sphere, centered on the source position
        // 2. Find the bounding box of your sphere, centered on the destination position
        // 3. Find the bounding box of those two bounding boxes
        // 4. Find all sprites that intersect that final bounding box
        var boundingRect = GetBoundingRect(itself, position, velocity);
        foreach (var s in sprites.Where(s => s.CollisionType != CollisionType.None && s.OnScreen && s != itself))
        {
            var boundingRect2 = GetBoundingRect(s, s.Position, s.Velocity);
            if (boundingRect.Intersects(boundingRect2))
                potColl.Add(s);

            Debug.Rects.Add(new RectDebug(new List<Line>(Line.GetFromRectangle(boundingRect)), "bb1", Color.Blue));
            Debug.Rects.Add(new RectDebug(new List<Line>(Line.GetFromRectangle(boundingRect2)), "bb2", Color.Green));
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
        bool collisionFound = false;
        nearestDistance = -1;
        Sprite nearestCollider = null;
        nearestIntersectionPoint = Vector2.Zero;
        Line nearestIntersectionLine = null;
        // go through collidalble sprites
        int spriteIdx = -1;
        collidedSprites = new List<Sprite>();
        foreach (Sprite colliderSprite in potentialColliders)
        {
            spriteIdx++;
#if DBG_COLL
            Log.Out(LogLevel.Dbg3, "---------------------- sprite {0} ---------------------------", spriteIdx);
            Log.Out(LogLevel.Dbg3, "    Collider:  {0}", colliderSprite.GetType().Name);
            Log.Out(LogLevel.Dbg3, "    Bounds:    {0}", colliderSprite.BoundingBoxRotated);
#endif
            var collLines = Line.GetFromRectangle(colliderSprite.BoundingBoxRotated);
            foreach (var t in collLines)
            {
                t.Start *= worldScale;
                t.End *= worldScale;
            }
            CollideLines(collidedSprites, ellipse, center, velocity, collLines, colliderSprite, distanceToTravel, ref collisionFound, ref nearestDistance, ref nearestCollider, ref nearestIntersectionPoint, ref nearestIntersectionLine);
        }
#if DBG_COLL
        Log.Out(LogLevel.Dbg3, "-------------------------------------------------");
#endif
        return collisionFound;
    }

    private static void CollideLines(
        List<Sprite> collidedSprites,
        Ellipse ellipse,
        Vector2 center,
        Vector2 velocity,
        IEnumerable<Line> collLines,
        Sprite colliderSprite,
        float distanceToTravel,
        ref bool collisionFound,
        ref float nearestDistance,
        ref Sprite nearestCollider,
        ref Vector2 nearestIntersectionPoint,
        ref Line nearestIntersectionLine)
    {
        int lineIdx = -1;
        foreach (Line collLine in collLines)
        {
            lineIdx++;
            // line origin/normal
            var lineNormal = collLine.Normal;
            var lineNormalNormalized = lineNormal;
            lineNormalNormalized.Normalize();
            // Determine the distance from the line to the position
            Vector2 lineNormalNegative = -lineNormalNormalized * 100;
            var lineRay = collLine.MakeEndless();
            var lineNormalNeg = new Line(center, lineNormalNegative);
#if DBG_COLL
            Debug.Lines.Add(new LineDebug(collLine, string.Format("collLine {0}", lineIdx), Color.Yellow));
            //Debug.Lines.Add(new LineDebug(lineNormalNeg, "lineNormalNeg", Color.LightBlue));
            //Debug.Lines.Add(new LineDebug(lineRay, string.Format("lineRay {0}", lineIdx), Color.LightPink));
#endif
            if (!lineRay.IntersectLine(lineNormalNeg, out var lineNormalIntersection))
            {
                Log.Out(LogLevel.Dbg3, "    lineRay not intersection normal");
                continue;
            }
            var lineNormalPosDistance = Vector2.Distance(center, lineNormalIntersection);
            var radius = ellipse.Width / 2f;
#if DBG_COLL
            //Debug.Points.Add(new PointDebug(new Dot(lineNormalIntersection), "lineNormalPosIntersectionPoint", Color.White));
            //Debug.Points.Add(new PointDebug(new Dot(position), "position", Color.LightGreen));
            Log.Out(LogLevel.Dbg3, "    lineNormalPosDistance: {0}", lineNormalPosDistance);
            Log.Out(LogLevel.Dbg3, "    radius: {0}", radius);
#endif
            // Is the line intersecting ellipse?
            Vector2 lineIntersectionPoint;
            if (lineNormalPosDistance <= radius)
            {
                // Calculate the line intersection point
                // -lineNormal with length set to lineDistance
                Vector2 temp = -lineNormalNormalized * lineNormalPosDistance;
                lineIntersectionPoint = center + temp;
#if DBG_COLL
                //Debug.Points.Add(new PointDebug(new Dot(lineIntersectionPoint), "lineIntersectionPoint", Color.DarkGoldenrod));
                //Log.Out(LogLevel.Dbg3, "    abs(lineNormalPosDistance) <= radius");
                //Log.Out(LogLevel.Dbg3, "    lineIntersectionPoint: {0}", lineIntersectionPoint);
#endif
            }
            else
            {
                // Calculate the ellipsoid intersection point
                // -lineNormal with length set to radius;
                Vector2 lineNormalInRadiusLength = -lineNormalNormalized * radius;
                Vector2 ellipseIntersectionPoint = center + lineNormalInRadiusLength;
                // Calculate the line velocity intersection point
                Vector2 velNormalized = velocity;
                velNormalized.Normalize();
                Vector2 ellipseLineVelPoint = ellipseIntersectionPoint + velNormalized * 100;
                collLine.IntersectLine(new Line(ellipseIntersectionPoint.X, ellipseIntersectionPoint.Y, ellipseLineVelPoint.X, ellipseLineVelPoint.Y), out lineIntersectionPoint);
#if DBG_COLL
                //Log.Out(LogLevel.Dbg3, "    abs(lineNormalPosDistance) > radius");
                Debug.Points.Add(new PointDebug(new Dot(ellipseIntersectionPoint), "ellipseIntersectionPoint", Color.OrangeRed));
                Debug.Points.Add(new PointDebug(new Dot(lineIntersectionPoint), "lineIntersectionPoint", Color.LightBlue));
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
                //Log.Out(LogLevel.Dbg3, "    lineIntersectionPoint is not on current line");
#endif
                // IntersectionPoint = nearest point on polygon's perimeter to planeIntersectionPoint;
                intersectionPoint = collLine.GetClosestPoint(lineIntersectionPoint);
            }
            // Using the lineIntersectionPoint, we need to reverse-intersect with the ellipse
            // float t = intersectEllipse(sourcePoint, radiusVector, polygonIntersectionPoint, negativeVelocityVector);
            Vector2 negativeVel = -velocity;
            negativeVel.Normalize();
            Vector2 pointOnNegativeVel = intersectionPoint + negativeVel * 100;
            ellipse.Position = center;
            Line reverseIntersectLine = new Line(intersectionPoint.X, intersectionPoint.Y, pointOnNegativeVel.X, pointOnNegativeVel.Y).MakeEndless();
            Vector2[] eIPs = new Vector2[2];
            var circle = new Circle(ellipse.Position, radius);
            int intersectionCount = circle.IntersectSegment(reverseIntersectLine.Start, reverseIntersectLine.End, out eIPs[0], out eIPs[1]);
            float ellipseIntersectionPointDistance = -1000;
            if (intersectionCount == 2)
            {
                var dist1 = Vector2.Distance(eIPs[0], intersectionPoint);
                var dist2 = Vector2.Distance(eIPs[1], intersectionPoint);
                ellipseIntersectionPointDistance = Math.Min(dist1, dist2);
            }
            if (intersectionCount == 1)
            {
                ellipseIntersectionPointDistance = eIPs[0] != Vector2.Zero ? Vector2.Distance(eIPs[0], intersectionPoint) : Vector2.Distance(eIPs[1], intersectionPoint);
            }
#if DBG_COLL
            Log.Out(LogLevel.Dbg3, "    lineIntersectionPoint: {0}", lineIntersectionPoint);
            Log.Out(LogLevel.Dbg3, "    ellipseIntersectionPoint1: {0}", eIPs[0]);
            Log.Out(LogLevel.Dbg3, "    ellipseIntersectionPoint2: {0}", eIPs[1]);
            Log.Out(LogLevel.Dbg3, "    ellipseIntersectionPointDistance: {0}", ellipseIntersectionPointDistance);
            Log.Out(LogLevel.Dbg3, "    distanceToTravel: {0}", distanceToTravel);
            Debug.Ellipses.Add(new EllipseDebug(new Ellipse(circle.Center, circle.Radius*2, circle.Radius*2), "Ellipse", Color.Gold, 100));
            Debug.Lines.Add(new LineDebug(reverseIntersectLine, "reverseIntersectLine", Color.Blue));
            Debug.Points.Add(new PointDebug(new Dot(intersectionPoint), "intersectionPoint" + lineIdx, Color.Pink));
#endif
            // Will there be an intersection with the ellipse?
            if (ellipseIntersectionPointDistance >= 0.0 && ellipseIntersectionPointDistance <= distanceToTravel)
            {
                // Vector V = negativeVelocityVector with length set to t;
                Vector2 v = negativeVel * ellipseIntersectionPointDistance;
                // Where did we intersect the ellipse?
                Vector2 intersection = intersectionPoint + v;
#if DBG_COLL
                Log.Out(LogLevel.Dbg3, "    collision detected at {0}", intersection);
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
                    //Log.Out(LogLevel.Dbg3, "    nearestIntersectionPoint {0}", nearestIntersectionPoint);
#endif
                }
            }
        }
    }

    private static Vector2 CalculateNearestCollVect(Vector2 velocity, float nearestDistance)
    {
        // calculate the nearest collision
        // Vector V = velocity with length set to (nearestDistance - EPSILON);
        Vector2 moveToNearestCollVect = velocity;
        moveToNearestCollVect.Normalize();
        moveToNearestCollVect *= nearestDistance * 0.9f;
        return moveToNearestCollVect;
    }

    private static Vector2 CalculateVelocity(Vector2 center, float friction, Vector2 destinationPoint, Vector2 nearestIntersectionPoint)
    {
        var slideLineNormalVect = nearestIntersectionPoint - center;
        var slideLineNormal = new Line(destinationPoint, slideLineNormalVect).MakeEndless();
        var slideLine = new Line(nearestIntersectionPoint, slideLineNormal.Normal).MakeEndless();
        slideLineNormal.IntersectLine(slideLine, out var slidingPoint);
        var velocity = slidingPoint - nearestIntersectionPoint;
        velocity *= friction;
        return velocity;
    }

    private static bool AvoidEndlessLoop(ref int iteration)
    {
        iteration++;
        if (iteration > 10)
        {
            Log.Out(LogLevel.Error, "Collision.CollideWithWorld Iteration > 10");
            return true;
        }
        return false;
    }

    private static void LogDebugHeader(Ellipse ellipseScaled, Vector2 position, Vector2 center, Vector2 velocity, Vector2 gravity)
    {
#if DBG_COLL
        Debug.Clear();
        Log.Out(LogLevel.Dbg3, "###############################################");
        Log.Out(LogLevel.Dbg3, "### DoContinuousEllipseDetectionAndReaction ###");
        Log.Out(LogLevel.Dbg3, "###############################################");
        Log.Out(LogLevel.Dbg3, "    Position:  {0}", position);
        Log.Out(LogLevel.Dbg3, "    Center:    {0}", center);
        Log.Out(LogLevel.Dbg3, "    Velocity:  {0}", velocity);
        Log.Out(LogLevel.Dbg3, "    Gravity:   {0}", gravity);
        //Debug.Points.Add(new PointDebug(new Dot(center), "Center", Color.Blue));
        Debug.Ellipses.Add(new EllipseDebug(ellipseScaled, "bounding ellipse", Color.Red, 30));
#endif
    }

    private static void DebugLog(Vector2 position, Vector2 velocity, int iteration)
    {
#if DBG_COLL
        Log.Out(LogLevel.Dbg3, "    new Position:  {0}", position);
        Log.Out(LogLevel.Dbg3, "    new Velocity:  {0}", velocity);
        Log.Out(LogLevel.Dbg3, "    iteration:     {0}", iteration);
        //Debug.Points.Add(new PointDebug(new Dot(center), "move to nearest collision (center)", Color.Green));
        //Debug.Points.Add(new PointDebug(new Dot(slidingPoint), "slidingPoint", Color.Pink));
        //Debug.Points.Add(new PointDebug(new Dot(destinationPoint), "destinationPoint", Color.Green));
        //Debug.Points.Add(new PointDebug(new Dot(nearestIntersectionPoint), "nearestIntersectionPoint", Color.Green));
        //Debug.Lines.Add(new LineDebug(new Line(nearestIntersectionPoint, velocity * 100), "newVelocity", Color.Green));
        //Debug.Lines.Add(new LineDebug(slideLine, "slideLine", Color.Red));
        //Debug.Lines.Add(new LineDebug(slideLineNormal, "slidingVect", Color.Green));
        //Debug.Lines.Add(new LineDebug(new Line(center, velocity*100), "new velocity", Color.Red));
        //Log.Out(LogLevel.Dbg3, "    move to nearest collision {0} (center)", center);
        //Log.Out(LogLevel.Dbg3, "    velCutOff (distanceToTravel - nearestDistance) {0}", velCutOff);
        //Log.Out(LogLevel.Dbg3, "    destinationPoint (nearestLineIntersectionPoint + velCutOff) {0}", destinationPoint);
        //Log.Out(LogLevel.Dbg3, "    newDestinationPoint (nearestIntersectionLine.Intersect(projectSlideOnLine)) {0}", newDestinationPoint);
        //Log.Out(LogLevel.Dbg3, "    slideDist {0}", slideDist);
        //Log.Out(LogLevel.Dbg3, "    slideLineNormal {0}", slideLineNormalVect);
        //Log.Out(LogLevel.Dbg3, "    slide to point {0}", newDestinationPoint);
        //Log.Out(LogLevel.Dbg3, "    new velocity {0}", velocity);
        //Log.Out(LogLevel.Dbg3, "    iteration {0}", iteration);
        //Log.Out(LogLevel.Dbg3, "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
#endif
    }
}
