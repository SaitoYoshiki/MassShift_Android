using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterState : MonoBehaviour {
	const float DefaultWaterIgnoreTime = 0.1f;

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

			// 入水時
			if (isInWater) {
				Debug.Log("InWater OneTimeMaxSpd:" + weightLvEnterWaterMoveMax[(int)WeightMng.WeightLv] + " StayMaxSpd:" + weightLvStayWaterMoveMax[(int)WeightMng.WeightLv] + Support.ObjectInfoToString(gameObject));
				SetWaterMaxSpeed(weightLvEnterWaterMoveMax, weightLvStayWaterMoveMax);
			}
			// 出水時
			else {
				Debug.Log("OutWater OneTimeMaxSpd:" + weightLvExitWaterMoveMax[(int)WeightMng.WeightLv] + " StayMaxSpd:null" + Support.ObjectInfoToString(gameObject));
				SetWaterMaxSpeed(weightLvExitWaterMoveMax, null);
			}
		}
	}

	[SerializeField] bool isWaterSurface = false;
	public bool IsWaterSurface {
		get {
			return isWaterSurface;
		}
		set {
			isWaterSurface = value;
		}
	}

	[SerializeField] List<float> waterFloatSpd = new List<float>(); // 重さ毎の上昇量

	WeightManager weightMng = null;
	WeightManager WeightMng {
		get {
			if (weightMng == null) {
				weightMng = GetComponent<WeightManager>();
				if (weightMng == null) {
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

	[SerializeField] List<float> weightLvEnterWaterMoveMax = new List<float>(3);    // 各重さレベルの入水時の最高移動量
	[SerializeField] List<float> weightLvStayWaterMoveMax = new List<float>(3);     // 各重さレベルの入水中の最高移動量
	[SerializeField] List<float> weightLvExitWaterMoveMax = new List<float>(3);     // 各重さレベルの出水時の最高移動量
	[SerializeField] float cutOutSpd = 1.0f;                                        // 水面に浮く重さレベルでの入出水時に完全に移動を停止する移動量基準
	[SerializeField] float waterStopIgnoreRemainTime = 0.0f;
	float WaterStopIgnoreRemainTime {
		get {
			return waterStopIgnoreRemainTime;
		}
		set {
			waterStopIgnoreRemainTime = value;
		}
	}

	// Use this for initialization
	//	void Start () {}

	// Update is called once per frame
	void FixedUpdate () {
		IsInWater = (Support.GetColliderHitInfoList(GetComponent<Collider>(), Vector3.zero, LayerMask.GetMask("WaterArea")).Count > 0);

		// 水中/水上の挙動
		if (IsInWater) {
			// 水中なら
			if (!IsWaterSurface) {
				// 水による浮上
				MoveMng.AddMove(new Vector3(0.0f, waterFloatSpd[(int)WeightMng.WeightLv], 0.0f));
			}
			// 水上なら
			else {
				// 重さに変化が無ければ
				if (WeightMng.WeightLv == WeightManager.Weight.light) {
					// 落下しない
					MoveMng.StopMoveVirtical(MoveManager.MoveType.gravity);
					MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);
				}
				// 重さが変化していれば
				else {
					// 水面状態を解除
					IsWaterSurface = false;
				}
			}
		}
	}

	void SetWaterMaxSpeed(List<float> _oneTimeWeightLvMaxSpd, List<float> _stayWeightLvMaxSpd) {
		// 水面に浮かぶ重さレベルでの入出水時に入出水速度が一定以下なら
		//		Debug.LogError("(" + MoveMng.TotalMove.magnitude + " <= " + cutOutSpd + ")");
		if ((WeightMng.WeightLv == WeightManager.Weight.light) && (MoveMng.PrevMove.magnitude <= cutOutSpd)) {
			// 停止
			Debug.Log("WaterState CutOut" + MoveMng.PrevMove.magnitude);
			MoveMng.OneTimeMaxSpd = 0.0f;
			IsWaterSurface = true;
		} else {
			// 一度の更新に限り最大速度を制限
			MoveMng.OneTimeMaxSpd = _oneTimeWeightLvMaxSpd[(int)WeightMng.WeightLv];
		}

		// 継続的に最大速度を制限
		if (_stayWeightLvMaxSpd != null) {
			MoveMng.CustomWeightLvMaxSpd.AddRange(_stayWeightLvMaxSpd);
		} else {
			moveMng.CustomWeightLvMaxSpd.Clear();
		}
	}

	public void BeginWaterStopIgnore(float _time = DefaultWaterIgnoreTime) {
		WaterStopIgnoreRemainTime = _time;
	}
}
