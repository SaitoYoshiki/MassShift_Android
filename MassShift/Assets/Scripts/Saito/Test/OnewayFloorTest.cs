using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnewayFloorTest : MonoBehaviour {

	[SerializeField]
	OnewayFloor mOnewayFloor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log(mOnewayFloor.IsThrough(Vector3.down, gameObject));
	}
}
