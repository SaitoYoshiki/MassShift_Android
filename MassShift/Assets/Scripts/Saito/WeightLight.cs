using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeightEffect))]
public class WeightLight : MonoBehaviour {

	[SerializeField]
	GameObject mLightModel;

	[SerializeField]
	Color mFlyingColor;

	[SerializeField]
	float mFlyingColorPower = 1.0f;

	[SerializeField]
	Color mLightColor;

	[SerializeField]
	float mLightColorPower = 1.0f;

	[SerializeField]
	Color mHeavyColor;

	[SerializeField]
	float mHeavyColorPower = 1.0f;

	//色を変更する対象のマテリアル
	[SerializeField]
	Material mLightMaterial;

	// Use this for initialization
	void Awake() {
		GetComponent<WeightEffect>().OnWeightChange += ChangeLightColor;
	}

	void ChangeLightColor(WeightManager.Weight aWeight) {
		//Utility.ChangeMaterialColor(mLightModel, mLightMaterial, "_EmissionColor", GetColor(aWeight));
		Utility.ChangeMaterialColor(mLightModel, mLightMaterial, "_Color", GetColor(aWeight));
	}


	//光る色を返す
	Color GetColor(WeightManager.Weight aWeight) {

		switch (aWeight) {
			case WeightManager.Weight.flying:
				return mFlyingColor * mFlyingColorPower;
			case WeightManager.Weight.light:
				return mLightColor * mLightColorPower;
			case WeightManager.Weight.heavy:
				return mHeavyColor * mHeavyColorPower;
		}
		Debug.Log("重さが不正です");
		return Color.black;
	}
}
