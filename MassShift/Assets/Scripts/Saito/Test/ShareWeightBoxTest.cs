using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareWeightBoxTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		ShareWeightBox s = GetComponent<ShareWeightBox>();
		if(Input.GetKeyDown(KeyCode.A)) {
			foreach(var ts in s.GetShareAllListExceptOwn()) {
				Debug.Log(ts, this);
			}
		}
	}
}
