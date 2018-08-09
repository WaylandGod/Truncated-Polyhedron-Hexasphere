﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// that looks funny but I need it to keep same references when projection hex to spheres, cause Unity Vector3 is not reftype
public class Point
{
    private Vector3 mP;
    public List<Face> faces = new List<Face>();

    public void RegisterFace(Face face)
    {
        faces.Add(face);
    }
 
    public Point(float x, float y, float z)
    {
        mP.Set(x, y, z);
    }

    public Point(Vector3 vec)
    {
        mP = vec;
    }

    public float x
    {
        get
        {
            return mP.x;
        }
    }

    public float y
    {
        get
        {
            return mP.y;
        }
    }

    public float z
    {
        get
        {
            return mP.z;
        }
    }

    public void Set(Vector3 vec)
    {
        mP = vec;
    }

    public Vector3 ToVector3()
    {
        return mP;
    }

    public override string ToString()
    {
        return mP.ToString();
    }

    public List<Face> getOrderedFaces()
    {
        List<Face> workingArray = new List<Face>();
        workingArray.AddRange(faces);

        var ret = new List<Face>();

        var i = 0;
        while (i < faces.Count)
        {
            if (i == 0)
            {
                ret.Add(workingArray[i]);
                workingArray.RemoveAt(i);
            }
            else
            {
                var hit = false;
                var j = 0;
                while (j < workingArray.Count && !hit)
                {
                    if (workingArray[j].isAdjacentTo(ret[i - 1]))
                    {
                        hit = true;
                        ret.Add(workingArray[j]);
                        workingArray.RemoveAt(j);
                    }
                    j++;
                }
            }
            i++;
        }

        return ret;
    }
}

