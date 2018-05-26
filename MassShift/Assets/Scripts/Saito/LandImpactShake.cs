using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LandImpact))]
public class LandImpactShake : MonoBehaviour {

	[SerializeField, Tooltip("揺れる時間"), EditOnPrefab]
	float mShakeTime = 0.2f;

	// Use this for initialization
	void Awake () {
		GetComponent<LandImpact>().OnLand += OnLand;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnLand(WeightManager.Weight aWeight, bool aIsWater) {
		if(aWeight == WeightManager.Weight.heavy) {
			if(aIsWater == false) {
				ShakeCamera.ShakeAll(mShakeTime);
			}
		}
	}
}
