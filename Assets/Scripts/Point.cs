using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// that looks funny but I need it to keep same references when projection hex to spheres, cause Unity Vector3 is not reftype
public class Point
{
    private Vector3 mP;

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
}

