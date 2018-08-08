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

// that looks funny but I need it to keep same references when projection hex to spheres :)
public class Point
{
    Vector3 p;

    public float x
    {
        get
        {
            return p.x;
        }
    }

    public float y
    {
        get
        {
            return p.y;
        }
    }

    public float z
    {
        get
        {
            return p.z;
        }
    }

    public Point(float x, float y, float z)
    {
        p.Set(x, y, z);
    }


    public Point(Vector3 vec)
    {
        p = vec;
    }

    public void Set(Vector3 vec)
    {
        p = vec;
    }

    public Vector3 ToVector3()
    {
        return p;
    }

    public override string ToString()
    {
        return p.ToString();
    }
}

public class Icosahedron : MonoBehaviour
{
    public delegate Point Checker(Point p);

    [SerializeField]
    private MeshFilter mFilter;
    [SerializeField]
    private MeshRenderer mRenderer;
    [SerializeField]
    private MeshCollider mCollider;

    private void Start()
    {
        GenerateIcosahedron();
    }

    private struct Face
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

    private void GenerateIcosahedron()
    {
        var tao = 1.61803399f;
        var d = 1;
        var basicCount = 20;

        var numDivisions = 3;

        var corners = new Point[12]
        {
            new Point(d, tao * d, 0),
            new Point(-d, tao * d, 0),
            new Point(d,-tao * d,0),
            new Point(-d,-tao * d,0),
            new Point(0,d,tao * d),
            new Point(0,-d,tao * d),
            new Point(0,d,-tao * d),
            new Point(0,-d,-tao * d),
            new Point(tao * d,0,d),
            new Point(-tao * d,0,d),
            new Point(tao * d,0,-d),
            new Point(-tao * d,0,-d)
        };

        HashSet<Point> points = new HashSet<Point>();
        foreach (var corner in corners)
        {
            points.Add(corner);
        }

        var faces = new Face[]
        {
            new Face(corners[0], corners[1], corners[4]),
            new Face(corners[1], corners[9], corners[4]),
            new Face(corners[4], corners[9], corners[5]),
            new Face(corners[5], corners[9], corners[3]),
            new Face(corners[2], corners[3], corners[7]),
            new Face(corners[3], corners[2], corners[5]),
            new Face(corners[7], corners[10], corners[2]),
            new Face(corners[0], corners[8], corners[10]),
            new Face(corners[0], corners[4], corners[8]),
            new Face(corners[8], corners[2], corners[10]),
            new Face(corners[8], corners[4], corners[5]),
            new Face(corners[8], corners[5], corners[2]),
            new Face(corners[1], corners[0], corners[6]),
            new Face(corners[11], corners[1], corners[6]),
            new Face(corners[3], corners[9], corners[11]),
            new Face(corners[6], corners[10], corners[7]),
            new Face(corners[3], corners[11], corners[7]),
            new Face(corners[11], corners[6], corners[7]),
            new Face(corners[6], corners[0], corners[10]),
            new Face(corners[9], corners[1], corners[11])
        };

        // We do not need this cause in unity it is not a class but struct. etc the extra
        Checker getPointIfExists = (point) =>
        {
            if (points.Contains(point))
            {
                return point;
            }
            else
            {
                points.Add(point);
                return point;
            }
        };


        List<Face> newFaces = new List<Face>();
        for (var f = 0; f < faces.Length; f++)
        {
            // console.log("-0---");
            List<Point> prev = null;
            var bottom = new List<Point>();
            bottom.Add(faces[f].Points[0]);
            var left = faces[f].Points[0].Subdivide(faces[f].Points[1], numDivisions, getPointIfExists);
            var right = faces[f].Points[0].Subdivide(faces[f].Points[2], numDivisions, getPointIfExists);
            for (var i = 1; i <= numDivisions; i++)
            {
                prev = bottom;
                bottom = left[i].Subdivide(right[i], i, getPointIfExists);
                for (var j = 0; j < i; j++)
                {
                    var nf = new Face(prev[j], bottom[j], bottom[j + 1]);
                    newFaces.Add(nf);

                    if (j > 0)
                    {
                        nf = new Face(prev[j - 1], prev[j], bottom[j]);
                        newFaces.Add(nf);
                    }
                }
            }
        }



        faces = newFaces.ToArray();

        HashSet<Point> newPoints = new HashSet<Point>();
        foreach (var point in points)
        {
            Vector3 vec = point.ToVector3();
            vec.Normalize();
            newPoints.Add(point);
        }

        points = newPoints;

        var ps = points.ToUnityVectorsArray();



        Mesh mesh = new Mesh();

        //verts
        var verts = new Vector3[3 * faces.Length];
        for (int i = 0; i < faces.Length; i++)
        {
            verts[i * 3] = faces[i].Points[0].ToVector3();
            verts[i * 3 + 1] = faces[i].Points[1].ToVector3();
            verts[i * 3 + 2] = faces[i].Points[2].ToVector3();
        }
        mesh.vertices = verts;

        //indices;
        var indices = new int[3 * faces.Length];
        for (int i = 0; i < indices.Length; ++i)
        {
            indices[i] = i;
        }


        mesh.triangles = indices;

        mesh.RecalculateNormals();
        mFilter.mesh = mesh;
        //  mCollider.sharedMesh = mesh;
    }
}
