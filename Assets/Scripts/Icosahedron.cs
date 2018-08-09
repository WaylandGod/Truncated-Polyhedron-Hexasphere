﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Icosahedron : MonoBehaviour
{
    public delegate Point Checker(Point p);

    [SerializeField]
    private MeshFilter mFilter;
    [SerializeField]
    private MeshRenderer mRenderer;
    [SerializeField]
    private MeshCollider mCollider;
    [SerializeField]
    private int numDivisions = 1;
    [SerializeField]
    private float mDiameter;

    private List<Face> faces;
    private Point[] corners;
    private HashSet<Point> points;

    private List<Tile> tiles;
    private Dictionary<Point, Tile> tileLookup;

    private void Start()
    {
        GenerateIcosahedron();
    }

    private void GenerateIcosahedron()
    {
        var tao = 1.61803399f; // this is not magic but science
        var d = mDiameter;

        corners = new Point[12]
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

        points = new HashSet<Point>();
        foreach (var corner in corners)
        {
            points.Add(corner);
        }

        var fArr = new Face[]
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

        faces = new List<Face>();
        faces.AddRange(fArr);

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
        for (var f = 0; f < faces.Count; f++)
        {
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
                    var nf = new Face(prev[j], bottom[j], bottom[j + 1], true);
                    newFaces.Add(nf);

                    if (j > 0)
                    {
                        nf = new Face(prev[j - 1], prev[j], bottom[j], true);
                        newFaces.Add(nf);
                    }
                }
            }
        }

        faces = newFaces;

        HashSet<Point> newPoints = new HashSet<Point>();
        foreach (var point in points)
        {
            Vector3 vec = point.ToVector3();
          //  vec.Normalize();
            point.Set(vec);
            newPoints.Add(point);
        }

        points = newPoints;

        tiles = new List<Tile>();
        tileLookup = new Dictionary<Point, Tile>();

        foreach (var p in points)
        {
            var newTile = new Tile(p, 1);
            this.tiles.Add(newTile);
            this.tileLookup.Add(newTile.centerPoint, newTile);

        }

        foreach (var t in this.tiles)
        {
            var _this = this;
            t.neighbors = t.neighborIds.Select((f) => tileLookup[f]).ToList();
        }



        // TEST VISUALIZATION----------------------------------------------


      //  faces = new List<Face>();
      //  faces.AddRange(tiles[15].faces);
      //  foreach (var n in tiles[15].neighbors)
      //  {
      ////      if (n.faces.Count == tiles[15].faces.Count)
      //          faces.AddRange(n.faces);
      //  }

        Mesh mesh = new Mesh();



        //verts
        var verts = new Vector3[3 * faces.Count];
        for (int i = 0; i < faces.Count; i++)
        {
            verts[i * 3] = faces[i].Points[0].ToVector3();
            verts[i * 3 + 1] = faces[i].Points[1].ToVector3();
            verts[i * 3 + 2] = faces[i].Points[2].ToVector3();
        }
        mesh.vertices = verts;

        //indices;
        var indices = new int[3 * faces.Count];
        for (int i = 0; i < indices.Length; ++i)
        {
            indices[i] = i;
        }

        mesh.triangles = indices;

        mesh.RecalculateNormals();
        mFilter.mesh = mesh;
        mCollider.sharedMesh = mesh;
        // ----------------------------------------------------------------------
    }







}
