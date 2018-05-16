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
		if(Input.GetKeyDown(KeyCode.U)) {
			Debug.Log("Up        :" + mOnewayFloor.IsThrough(Vector3.up));
			Debug.Log("Down      :" + mOnewayFloor.IsThrough(Vector3.down));
			Debug.Log("Left      :" + mOnewayFloor.IsThrough(Vector3.left));
			Debug.Log("Right     :" + mOnewayFloor.IsThrough(Vector3.right));
			Debug.Log("UpRight   :" + mOnewayFloor.IsThrough(Vector3.up + Vector3.right));
			Debug.Log("UpLeft    :" + mOnewayFloor.IsThrough(Vector3.up + Vector3.left));
			Debug.Log("DownRight :" + mOnewayFloor.IsThrough(Vector3.down + Vector3.right));
			Debug.Log("DownLeft  :" + mOnewayFloor.IsThrough(Vector3.down + Vector3.left));
		}
	}
}
