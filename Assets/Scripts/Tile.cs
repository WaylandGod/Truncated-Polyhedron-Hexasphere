using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    // Tiles
    public Point centerPoint;


    public Vector3 vector(Vector3 p1, Vector3 p2)
    {
        return new Vector3(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z);
    }

    public Vector3 calculateSurfaceNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var U = vector(p1, p2);
        var V = vector(p1, p3);

        var N = new Vector3(
            U.y * V.z - U.z * V.y,
            U.z * V.x - U.x * V.z,
            U.x * V.y - U.y * V.x
            );

        return N;
    }

    public List<Face> faces;
    public List<Point> neighborIds;
    public List<Point> boundary;
    public List<Tile> neighbors;

    public Tile(Point centerPoint, float? hexSize)
    {
        if (!hexSize.HasValue)
            hexSize = 1;

        hexSize = Mathf.Clamp(hexSize.Value, 0.01f, 1);
        this.centerPoint = centerPoint;
        faces = centerPoint.getOrderedFaces();
        boundary = new List<Point>();
        neighborIds = new List<Point>(); // this holds the centerpoints, will resolve to references after
        neighbors = new List<Tile>(); // this is filled in after all the tiles have been created

        var neighborHash = new Dictionary<Point, int>();

        for (var f = 0; f < faces.Count; f++)
        {
            // build boundary
            boundary.Add(faces[f].GetCentroid().segment(centerPoint, hexSize.Value));

            // get neighboring tiles
            var otherPoints = faces[f].GetOtherPoints(this.centerPoint);
            for (var o = 0; o < 2; o++)
            {
                neighborHash[otherPoints[o]] = 1;
            }
        }


        foreach (var key in neighborHash.Keys)
        {
            neighborIds.Add(key);
        }


        var normal = calculateSurfaceNormal(this.boundary[0].ToVector3(), this.boundary[1].ToVector3(), this.boundary[2].ToVector3());

        if (!pointingAwayFromOrigin(this.centerPoint.ToVector3(), normal))
        {
            this.boundary.Reverse();
        }

    }


    bool pointingAwayFromOrigin(Vector3 p, Vector3 v)
    {
        return ((p.x * v.x) >= 0) && ((p.y * v.y) >= 0) && ((p.z * v.z) >= 0);
    }
}
