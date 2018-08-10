using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float mAngleAlpha;

    [Range(0.0f, 1.0f)]
    public float mPositionAlpha;

    [SerializeField]
    private MeshFilter mFilter;
    [SerializeField]
    private MeshRenderer mRenderer;
    [SerializeField]
    private MeshCollider mCollider;

    [SerializeField]
    private float mSelectedOutDistance;

    private Vector3 mCenterDirection;
    private Vector3 mRotationDirection;

    private Mesh mHexMesh;
    private Transform mParent;

    private Vector3 mMouseOverPosition;
    private Vector3 mNoMousePosition;
    private Quaternion mNoRotation;
    private Quaternion mFlippedRotation;

    public void Init(Tile tile, Transform parent = null)
    {
        mParent = parent;
        if (parent != null)
        {
            transform.SetParent(parent);
        }

        var pointsList = new List<Vector3>(tile.Boundary.ToUnityVectorsArray());
        var center = new Vector3();
        foreach (var point in pointsList)
        {
            center += point;
        }

        center /= pointsList.Count;
        mCenterDirection = -center;
        mCenterDirection.Normalize();
        mRotationDirection = pointsList.Count == 6 ? pointsList[0] - pointsList[3] : pointsList[0] - (pointsList[1] + pointsList[4]) / 2;
        mRotationDirection.Normalize();

        //move them back
        for (int i = 0; i < pointsList.Count; i++)
        {
            pointsList[i] -= center;
        }

        var indices = new List<int>();
        for (int i = 0; i < pointsList.Count - 2; i++)
        {
            indices.Add(0);
            indices.Add(i + 1);
            indices.Add(i + 2);
        }

        transform.localPosition = center;

        mHexMesh.Clear();
        mHexMesh.SetVertices(pointsList);
        mHexMesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        mHexMesh.RecalculateNormals();

        mFilter.sharedMesh = mHexMesh;
        mCollider.sharedMesh = mHexMesh;

        BakeTransformations();
    }


    private void BakeTransformations()
    {
        mNoRotation = transform.localRotation;
        mFlippedRotation = Quaternion.AngleAxis(180f, mRotationDirection);
        mNoMousePosition = transform.localPosition;
        mMouseOverPosition = transform.localPosition - mCenterDirection * mSelectedOutDistance;
    }

    public void Update()
    {
        var rot = Quaternion.Lerp(mNoRotation, mFlippedRotation, mAngleAlpha);
        var pos = Vector3.Lerp(mNoMousePosition, mMouseOverPosition, mPositionAlpha);
        transform.localRotation = rot;
        transform.localPosition = pos;
    }

    public void Awake()
    {
        mHexMesh = new Mesh();
    }

    public void OnDestroy()
    {
        Destroy(mHexMesh);
    }
}
