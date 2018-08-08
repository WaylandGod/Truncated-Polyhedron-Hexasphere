using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static List<Point> Subdivide(this Point sourcePoint, Point point, int count, Icosahedron.Checker checkPoint)
    {
        List<Point> segments = new List<Point>();
        segments.Add(sourcePoint);

        for (int i = 1; i < count; i++)
        {
            float _i = i;
            var np = new Point(sourcePoint.x * (1 - (_i / count)) + point.x * (_i / count),
                sourcePoint.y * (1 - (_i / count)) + point.y * (_i / count),
                sourcePoint.z * (1 - (_i / count)) + point.z * (_i / count));
            np = checkPoint(np);
            segments.Add(np);
        }

        segments.Add(point);
        return segments;
    }

    public static Vector3[] ToUnityVectorsArray(this IEnumerable<Point> points)
    {
        var vecList = new List<Vector3>();
        foreach (var p in points)
        {
            vecList.Add(p.ToVector3());
        }
        return vecList.ToArray();
    }
}
