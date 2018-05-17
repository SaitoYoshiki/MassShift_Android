using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterState : MonoBehaviour {
	[SerializeField] bool isInWater = false;
	public bool IsInWater {
		get {
			return isInWater;
		}
		set {
			// 変更がなかった
			if (isInWater == value) return;

			// 値を変更
			isInWater = value;
		}
	}

	[SerializeField] float waterFloatSpd = 1.0f;

	WeightManager weightMng = null;
	WeightManager WeightMng {
		get {
			if (weightMng == null) {
				weightMng = GetComponent<WeightManager>();
				if(weightMng == null) {
					Debug.LogError("WeightManagerが見つかりませんでした。");
				}
			}
			return weightMng;
		}
	}

	MoveManager moveMng = null;
	MoveManager MoveMng {
		get {
			if (moveMng == null) {
				moveMng = GetComponent<MoveManager>();
				if (moveMng == null) {
					Debug.LogError("MoveManagerが見つかりませんでした。");
				}
			}
			return moveMng;
		}
	}

	[SerializeField] List<float> inWaterMoveMax = new List<float>();	// 各重さレベルの入水時(常時)の最高移動量
	[SerializeField] List<float> outWaterMoveMax = new List<float>();   // 各重さレベルの出水時(瞬間)の最高移動量

	// Use this for initialization
	//	void Start () {}

	// Update is called once per frame
	void FixedUpdate () {
		isInWater = false;
		if (Support.GetColliderHitInfoList(GetComponent<Collider>(), Vector3.zero, LayerMask.GetMask("WaterArea")).Count > 0) {
			IsInWater = true;
		}

		// 水中の挙動
		if (IsInWater) {
			// 水による浮上
			FloatWater();
		}
	}

	void FloatWater() {
		if (WeightMng.WeightLv < WeightManager.Weight.heavy) {
			MoveMng.AddMove(new Vector3(0.0f, waterFloatSpd, 0.0f));
		}
	}
}
