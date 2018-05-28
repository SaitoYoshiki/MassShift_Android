using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LandImpact))]
public class LandImpactSound : MonoBehaviour {

	[SerializeField, Tooltip("重さ2で落下したときのSE"), EditOnPrefab]
	GameObject mLandImpactHeavySE;

	[SerializeField, Tooltip("重さ1で落下したときのSE"), EditOnPrefab]
	GameObject mLandImpactLightSE;

	[SerializeField, Tooltip("重さ2で水に落下したときのSE"), EditOnPrefab]
	GameObject mLandImpactWaterHeavySE;

	[SerializeField, Tooltip("重さ1で水に落下したときのSE"), EditOnPrefab]
	GameObject mLandImpactWaterLightSE;

	// Use this for initialization
	void Awake() {
		GetComponent<LandImpact>().OnLand += OnLand;
	}

	// Update is called once per frame
	void Update() {

	}

	void OnLand(WeightManager.Weight aWeight, LandImpact.CEnviroment aEnviroment) {

		//水面に落ちたなら
		if(aEnviroment == LandImpact.CEnviroment.cWaterSurface) {
			//重さが2なら
			if (aWeight == WeightManager.Weight.heavy) {
				SoundManager.SPlay(mLandImpactWaterHeavySE);	//音を鳴らす
			}
			else if (aWeight == WeightManager.Weight.light) {
				SoundManager.SPlay(mLandImpactWaterLightSE);
			}
		}

		//地上で落ちたなら
		else if (aEnviroment == LandImpact.CEnviroment.cGround) {
			if (aWeight == WeightManager.Weight.heavy) {
				SoundManager.SPlay(mLandImpactHeavySE);
			}
			else if (aWeight == WeightManager.Weight.light) {
				SoundManager.SPlay(mLandImpactLightSE);
			}
		}

		//水中で落ちたなら
		else if (aEnviroment == LandImpact.CEnviroment.cWater) {
			if (aWeight == WeightManager.Weight.heavy) {
				SoundManager.SPlay(mLandImpactHeavySE);
			}
			else if (aWeight == WeightManager.Weight.light) {
				SoundManager.SPlay(mLandImpactLightSE);
			}
		}
	}
}
