using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LandImpact))]
public class LandImpactSound : MonoBehaviour {

	[SerializeField, Tooltip("重さ2で落下したときのSE"), EditOnPrefab]
	GameObject mLandImpactHeavySE;

	[SerializeField, Tooltip("重さ1で落下したときのSE"), EditOnPrefab]
	GameObject mLandImpactLightSE;

	// Use this for initialization
	void Awake() {
		GetComponent<LandImpact>().OnLand += OnLand;
	}

	// Update is called once per frame
	void Update() {

	}

	void OnLand(WeightManager.Weight aWeight, bool aIsWater) {
		if (aWeight == WeightManager.Weight.heavy) {
			SoundManager.SPlay(mLandImpactHeavySE);
		}
		if (aWeight == WeightManager.Weight.light) {
			SoundManager.SPlay(mLandImpactLightSE);
		}
	}
}
