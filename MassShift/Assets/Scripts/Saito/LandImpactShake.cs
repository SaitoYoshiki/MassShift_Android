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

	void OnLand(WeightManager.Weight aWeight, LandImpact.CEnviroment aEnviroment) {

		//重さが2で
		if(aWeight == WeightManager.Weight.heavy) {
			//水中か、地上に着地したら
			if(aEnviroment == LandImpact.CEnviroment.cWater || aEnviroment == LandImpact.CEnviroment.cGround) {
				ShakeCamera.ShakeAll(mShakeTime);	//カメラを揺らす
			}
		}
	}
}
