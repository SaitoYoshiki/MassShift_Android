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

	[SerializeField, Tooltip("上に発生するエフェクトの発生位置のオフセット"), EditOnPrefab]
	Vector3 mUpEffectOffset;

	[SerializeField, Tooltip("水のエフェクトの発生位置のオフセット"), EditOnPrefab]
	Vector3 mWaterEffectOffset;

	[SerializeField, Tooltip("下に発生するエフェクトの発生位置のオフセット"), EditOnPrefab]
	Vector3 mDownEffectOffset;


	// Use this for initialization
	void Awake() {
		GetComponent<LandImpact>().OnLand += OnLand;
	}

	// Update is called once per frame
	void Update() {

	}

	void OnLand(WeightManager.Weight aWeight, LandImpact.CEnviroment aEnviroment) {

		//水面に落下したなら
		if(aEnviroment == LandImpact.CEnviroment.cWaterSurface) {
			if (aWeight == WeightManager.Weight.heavy) {
				var g = Instantiate(mLandImpactWaterHeavyPrefab, transform);
				g.transform.localPosition = mWaterEffectOffset;
			}
			if (aWeight == WeightManager.Weight.light) {
				var g = Instantiate(mLandImpactWaterLightPrefab, transform);
				g.transform.localPosition = mWaterEffectOffset;
			}
		}

		//地上に落下したなら
		else if (aEnviroment == LandImpact.CEnviroment.cGround) {
			if (aWeight == WeightManager.Weight.heavy) {
				var g = Instantiate(mLandImpactGroundHeavyPrefab, transform);
				g.transform.localPosition = mDownEffectOffset;
			}
			if (aWeight == WeightManager.Weight.light) {
				var g =Instantiate(mLandImpactGroundLightPrefab, transform);
				g.transform.localPosition = mDownEffectOffset;
			}
			if (aWeight == WeightManager.Weight.flying) {
				var g = Instantiate(mLandImpactGroundFlyingPrefab, transform);
				g.transform.localPosition = mUpEffectOffset;
			}
		}
		
	}
}