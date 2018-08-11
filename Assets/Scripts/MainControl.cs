using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour
{
    [SerializeField]
    InputController mUserInput;

    [SerializeField]
    private Icosahedron mIcoSphere;

    [SerializeField]
    private Hexagon mHexPrefab;

    private List<Hexagon> mTiles;

    private void Start()
    {
        mTiles = new List<Hexagon>();
        mIcoSphere.GenerateIcosahedron();
        foreach (var tile in mIcoSphere.Tiles)
        {
            var hexagon = Instantiate(mHexPrefab) as Hexagon;
            hexagon.Init(tile, transform);
            mTiles.Add(hexagon);
            mUserInput.AddTrackingElement(hexagon);
        }
    }
}
