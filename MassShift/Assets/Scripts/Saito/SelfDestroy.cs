using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {

	[SerializeField]
	float mDestroyTime = float.MaxValue;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		mDestroyTime -= Time.deltaTime;
		if(mDestroyTime <= 0.0f) {
			Destroy(gameObject);
		}
	}
}
