using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LandImpact))]
public class LandImpactEffect : MonoBehaviour {

	[SerializeField, Tooltip("重さ2で地面に落ちた時のエフェクト"), EditOnPrefab]
	GameObject mLandImpactEffectHeavyPrefab;

	[SerializeField, Tooltip("重さ1で地面に落ちた時のエフェクト"), EditOnPrefab]
	GameObject mLandImpactEffectLightPrefab;

	// Use this for initialization
	void Awake() {
		GetComponent<LandImpact>().OnLand += OnLand;
	}

	// Update is called once per frame
	void Update() {

	}

	void OnLand(WeightManager.Weight aWeight, bool aIsWater) {
		if (aWeight == WeightManager.Weight.heavy)
			Instantiate(mLandImpactEffectHeavyPrefab);
	}
}