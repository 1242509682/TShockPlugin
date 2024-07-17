using Microsoft.Xna.Framework;
using Terraria;

namespace EconomicsAPI.Extensions;

public static class Vector2Ext
{
    public static bool HasNanOrInf(this Vector2 vec)
    {
        return float.IsNaN(vec.X) || float.IsInfinity(vec.X)
            || float.IsNaN(vec.Y) || float.IsInfinity(vec.Y)
            || float.IsPositiveInfinity(vec.X) || float.IsPositiveInfinity(vec.Y);

    }

    public static Vector2 RotationAngle(this Vector2 vel, float angle)
    {
        return vel.RotatedBy(angle * (Math.PI / 180));
    }

    public static Vector2 ToLenOf(this Vector2 vec, float len)
    {
        var newVec = vec.SafeNormalize(default);
        return newVec * len;
    }

    public static Vector2[] GetCircleEdgePoints(this Vector2 pos, float radius, float distanceBetweenPoints)
    {
        int numPoints = (int)Math.Ceiling(2 * Math.PI * radius / distanceBetweenPoints);
        Vector2[] points = new Vector2[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float angle = (float)(i * (2 * Math.PI) / numPoints);
            float x = radius * (float)Math.Cos(angle);
            float y = radius * (float)Math.Sin(angle);
            points[i] = pos + new Vector2(x, y);
        }
        return points;
    }

    public static Vector2[] GetArcPoints(this Vector2 vel, float startAngle, float endAngle, float radius, float interval)
    {
        var points = new List<Vector2>();
        int steps = (int)Math.Round((endAngle - startAngle + 360) % 360 / interval);
        for (int i = 0; i <= steps; i++)
        {
            float angle = (startAngle + i * interval + 360) % 360;
            float x = (float)(radius * Math.Cos(angle * Math.PI / 180));
            float y = (float)(radius * Math.Sin(angle * Math.PI / 180));
            points.Add(new Vector2(x, y) + vel);
        }
        return points.ToArray();
    }

    public static float Magnitude(this Vector2 vel)
    {
        return Convert.ToSingle(Math.Sqrt(vel.X * vel.X + vel.Y * vel.Y));
    }

    public static Vector2 Normalized(this Vector2 vel)
    {
        float magnitude = vel.Magnitude();
        return new Vector2(vel.X / magnitude, vel.Y / magnitude);
    }


    public static NPC? FindRangeNPC(this Vector2 pos, float distanceSquared)
    {
        foreach (var npc in Main.npc)
        {
            if (npc != null && npc.active && npc.CanBeChasedBy(null, false))
            {
                var val = npc.Center - pos;
                if (distanceSquared > val.LengthSquared())
                {
                    return npc;
                }
            }
        }
        return null;
    }

    public static List<Vector2> GenerateCurvePoints(Vector2 pointA, Vector2 pointB, float radius, float interval)
    {
        if (radius == 0)
        {
            return new List<Vector2> { pointA, pointB };
        }
        List<Vector2> curvePoints = new();
        double distance = (pointB - pointA).Magnitude();
        int numPoints = (int)Math.Round(distance / interval);
        for (int i = 0; i <= numPoints; i++)
        {
            float t = i / (float)numPoints;
            Vector2 midPoint = Vector2.Lerp(pointA, pointB, t);
            Vector2 direction = (pointB - pointA).Normalized();
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X);
            Vector2 curvePoint = midPoint + radius * perpendicular;
            if (radius < 0)
            {
                curvePoint = midPoint - Math.Abs(radius) * perpendicular;
            }
            curvePoints.Add(curvePoint);
        }
        return curvePoints;
    }
    
    public static List<Vector2> GetPointsOnCircle(this Vector2 pos1, Vector2 pos2, float startAngle, float angleIncrement, int maxCount)
    {
        List<Vector2> points = new List<Vector2>();
        float radius = (float)Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2));
        float currentAngle = startAngle;
        while (points.Count < maxCount && currentAngle < 360)
        {
            float x = pos2.X + radius * (float)Math.Cos(currentAngle * Math.PI / 180);
            float y = pos2.Y + radius * (float)Math.Sin(currentAngle * Math.PI / 180);
            points.Add(new Vector2(x, y));
            currentAngle += angleIncrement;
        }
        return points;
    }
    

    
    public static List<Vector2> GetPointsOnCircle(this Vector2 center, float radius, float startAngle, float angleIncrement, int loopCount)
    {
        List<Vector2> points = new List<Vector2>();
        float currentAngle = startAngle;
        for (int i = 0; i < loopCount; i++)
        {
            float x = center.X + radius * (float)Math.Cos(currentAngle * Math.PI / 180);
            float y = center.Y + radius * (float)Math.Sin(currentAngle * Math.PI / 180);
            points.Add(new Vector2(x, y));
            currentAngle += angleIncrement;
        }
        return points;
    }

    
    //public static List<Vector2> GenerateCircle(this Vector2 startPoint, Vector2 centerPoint, float radius, float angleIncrement)
    //{
    //    List<Vector2> points = new List<Vector2>();
    //    for (float angle = -360; angle <= 360; angle += angleIncrement)
    //    {
    //        float x = centerPoint.x + radius * (float)Math.Cos(angle * Math.PI / 180);
    //        float y = centerPoint.y + radius * (float)Math.Sin(angle * Math.PI / 180);
    //        points.Add(new Vector2(x, y));
    //    }
    //    return points;
    //}
    
    public static List<Vector2> CreateCircle(this Vector2 startPoint, Vector2 centerPoint, int angleIncrement, int numberOfPoints)
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < numberOfPoints; i++)
        {
            float angle = angleIncrement * i;
            float x = centerPoint.X + (float)Math.Cos(angle) * (startPoint.X - centerPoint.X);
            float y = centerPoint.Y + (float)Math.Sin(angle) * (startPoint.Y - centerPoint.Y);
            points.Add(new Vector2(x, y));
        }
        return points;
    }
 

}





