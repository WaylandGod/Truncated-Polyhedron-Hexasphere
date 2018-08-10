using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour {
    [SerializeField]
    private Icosahedron mIcoSphere;

    [SerializeField]
    private Hexagon mHexPrefab;

	void Start () {
        mIcoSphere.GenerateIcosahedron();
        foreach(var tile in mIcoSphere.Tiles)
        {
            var hex = Instantiate(mHexPrefab) as Hexagon;
            hex.Init(tile,transform);
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
