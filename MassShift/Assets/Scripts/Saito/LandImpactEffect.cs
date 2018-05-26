using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LandImpact))]
public class LandImpactEffect : MonoBehaviour {

	[SerializeField, Tooltip("重さ2で地面に落ちた時のエフェクト"), EditOnPrefab]
	GameObject mLandImpactGroundHeavyPrefab;

	[SerializeField, Tooltip("重さ1で地面に落ちた時のエフェクト"), EditOnPrefab]
	GameObject mLandImpactGroundLightPrefab;

	[SerializeField, Tooltip("重さ1で地面に当たった時のエフェクト"), EditOnPrefab]
	GameObject mLandImpactGroundFlyingPrefab;

	[SerializeField, Tooltip("重さ2で水に落ちた時のエフェクト"), EditOnPrefab]
	GameObject mLandImpactWaterHeavyPrefab;

	[SerializeField, Tooltip("重さ1で水に落ちた時のエフェクト"), EditOnPrefab]
	GameObject mLandImpactWaterLightPrefab;


	// Use this for initialization
	void Awake() {
		GetComponent<LandImpact>().OnLand += OnLand;
	}

	// Update is called once per frame
	void Update() {

	}

	void OnLand(WeightManager.Weight aWeight, bool aIsWater) {
		if(aIsWater) {
			if (aWeight == WeightManager.Weight.heavy) {
				var g = Instantiate(mLandImpactWaterHeavyPrefab, transform);
			}
			if (aWeight == WeightManager.Weight.light) {
				var g = Instantiate(mLandImpactWaterLightPrefab, transform);
			}
		}
		else {
			if (aWeight == WeightManager.Weight.heavy) {
				var g = Instantiate(mLandImpactGroundHeavyPrefab, transform);
			}
			if (aWeight == WeightManager.Weight.light) {
				var g =Instantiate(mLandImpactGroundLightPrefab, transform);
			}
			if (aWeight == WeightManager.Weight.flying) {
				var g = Instantiate(mLandImpactGroundFlyingPrefab, transform);
			}
		}
		
	}
}