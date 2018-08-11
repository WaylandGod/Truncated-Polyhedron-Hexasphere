using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour, IInputElement
{
    [SerializeField]
    InputController mUserInput;

    [SerializeField]
    private Icosahedron mIcoSphere;

    [SerializeField]
    private Hexagon mHexPrefab;

    [SerializeField]
    private SphereCollider mSphereCollider;

    private List<Hexagon> mTiles;

    public Collider MainCollider
    {
        get
        {
            return mSphereCollider;
        }
    }

    public bool ProcessClick(int mouseIndex)
    {
        return false;
    }

    private float rotSpeed = 0;

    public bool ProcessDrag(Vector3 delta)
    {
        rotSpeed = delta.x;
        return true;
    }

    public void ProcessMouseLost()
    {
        Debug.Log("Big sphere Lost mouse");
    }

    public bool ProcessMouseOver()
    {
        Debug.Log("Big sphere mouse over");
        return false;
    }

    private void Update()
    {
        if (Mathf.Abs(rotSpeed) > 0.1)
        {
            Quaternion rot = transform.localRotation;
            Vector3 rotAngle = rot.eulerAngles;
            rotAngle.y += rotSpeed;
            transform.localRotation = Quaternion.Euler(rotAngle);
            rotSpeed = Mathf.Lerp(rotSpeed, 0, 0.05f);
        }


    }

    private void Start()
    {
        mTiles = new List<Hexagon>();
        mIcoSphere.GenerateIcosahedron();
        mSphereCollider.radius = 11f;
        mUserInput.AddTrackingElement(this);

        foreach (var tile in mIcoSphere.Tiles)
        {
            var hexagon = Instantiate(mHexPrefab) as Hexagon;
            hexagon.Init(tile, transform);
            mTiles.Add(hexagon);
            mUserInput.AddTrackingElement(hexagon);
        }
    }
}
