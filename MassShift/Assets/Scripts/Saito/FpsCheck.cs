using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCheck : MonoBehaviour {

	[SerializeField]
	TextMesh mText;

	[SerializeField]
	float mCountFpsMinTime = 0.5f;

	int frameCount;
	float prevTime;

	// Use this for initialization
	void Start () {
		frameCount = 0;
		prevTime = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {

		frameCount++;

		float lTakeTime = Time.realtimeSinceStartup - prevTime;
		if(lTakeTime >= mCountFpsMinTime) {
			mText.text = string.Format("fps:{0}", frameCount /  lTakeTime);
			frameCount = 0;
			prevTime = Time.realtimeSinceStartup;
		}
	}
}
