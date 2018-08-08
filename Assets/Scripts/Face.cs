using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Face
{
    public readonly List<Point> Points;
    public Face(Point p1, Point p2, Point p3)
    {
        Points = new List<Point>(3);
        Points.Add(p1);
        Points.Add(p2);
        Points.Add(p3);
    }

    public Vector3 GetCentroid()
    {
        return (Points[0].ToVector3() + Points[1].ToVector3() + Points[2].ToVector3()) / 3;
    }
}