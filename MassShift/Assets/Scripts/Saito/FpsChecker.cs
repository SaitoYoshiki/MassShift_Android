using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsChecker : MonoBehaviour {

	int frameCount = 0;
	float prevTime = 0.0f;

	[SerializeField]
	TextMesh mText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		frameCount++;
		float time = Time.realtimeSinceStartup - prevTime;

		if(time >= 0.5f) {
			mText.text = string.Format("fps:{0}", frameCount / time);

			frameCount = 0;
			prevTime = Time.realtimeSinceStartup;
		}
	}
}
