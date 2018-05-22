using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightManager : MonoBehaviour {
	// 列挙型
	public enum Weight {    // 重さレベル
		flying = 0,
		light,
		heavy,
	}

	// 定数
	const Weight WeightDefLv = Weight.light;    // 標準重さレベル

	// 重さレベル
	[SerializeField] Weight weightLv = WeightDefLv;
	public Weight WeightLv {
		get {
			return weightLv;
		}
		set {
			weightLv = value;

			// 一応の同期
			WeightLvSeem = value;

			// 通常の重力加速度設定に戻す
			if (MoveMng) {
				MoveMng.GravityCustomTime = 0.0f;
			}
		}
	}

	[SerializeField] float[] weightLvGravityForce = new float[3];

	public float WeightForce {
		get {
			return weightLvGravityForce[(int)PileMaxWeightLv];
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

	PileWeight pile = null;
	PileWeight Pile {
		get {
			if (!pile) {
				pile = GetComponent<PileWeight>();
				if (!pile) {
					Debug.LogError("PileWeightが見つかりませんでした。");
				}
			}
			return pile;
		}
	}

	// 自身に積み重なっている重さオブジェクトの中で最も重いオブジェクトの重さレベルを返す
	[SerializeField] Weight pileMaxWeightLv =  WeightDefLv;
	public Weight PileMaxWeightLv {
		get {
			if (pileWeightUpdateTime < Time.time) {
				pileWeightUpdateTime = Time.time;
				pileMaxWeightLv = WeightLv;
				if (Pile && MoveMng) {
					List<Transform> pileObjs = Pile.GetPileBoxList(Vector3.up * MoveMng.GravityForce);
					foreach (var pileObj in pileObjs) {
						WeightManager pileObjWeightMng = pileObj.GetComponent<WeightManager>();
						if (pileMaxWeightLv < pileObjWeightMng.weightLv) {
							pileMaxWeightLv = pileObjWeightMng.weightLv;
						}
					}
				}
			}
			return pileMaxWeightLv;
		}
	}

	float pileWeightUpdateTime = 0.0f;

	[SerializeField] Weight weightLvSeem = WeightDefLv;
	public Weight WeightLvSeem {
		get {
			return weightLvSeem;
		}
		set {
			weightLvSeem = value;
		}
	}
		
	// pull元からpush先へ指定数の重さレベルを移し、移す事に成功したレベル数を返す
	public int PullWeight(WeightManager _from, int _num = 1) {
		int cnt = 0;
		// 指定数のレベルを移しきるか、値が変更できなくなるまでループ
		while ((_num > 0) && _from.SubWeightLevel(true) && AddWeightLevel(true)) {
			// 値を変更
			_from.SubWeightLevel(false);
			AddWeightLevel(false);

			// 変更回数を加算
			cnt++;

			// 残り変更回数を減算
			_num--;
		}
		// 変更回数を返す
		return cnt;
	}
	// 相手側のpull処理を行う事で間接的なpush処理を実装
	public int PushWeight(WeightManager _to, int _num = 1) {
		return _to.PullWeight(this, _num);
	}

	// pull元とpush先を指定しての処理
	static public int ShiftWeight(WeightManager _from, WeightManager _to, int _num = 1) {
		return _from.PushWeight(_to, _num);
	}

	// 重さレベルを一段階変化させる
	// 成功したらtrueを返す
	// _checkOnlyがtrueなら成功するかどうかだけを返し、レベルを変更しない
	public bool AddWeightLevel(bool _checkOnly) {
		switch (WeightLv) {
		case Weight.flying:
			if (!_checkOnly) {
				// 変更
				WeightLv = Weight.light;
			}
			return true;
		case Weight.light:
			if (!_checkOnly) {
				// 変更
				WeightLv = Weight.heavy;
			}
			return true;
		case Weight.heavy:
			// 変更不可
			return false;
		default:
			Debug.LogError("不明な重さレベルに変更されようとしました。");
			return false;
		}
	}
	public bool SubWeightLevel(bool _checkOnly) {
		switch (WeightLv) {
		case Weight.flying:
			// 変更不可
			return false;
		case Weight.light:
			if (!_checkOnly) {
				// 変更
				WeightLv = Weight.flying;
			}
			return true;
		case Weight.heavy:
			// 変更
			if (!_checkOnly) {
				WeightLv = Weight.light;
			}
			return true;
		default:
			Debug.LogError("不明な重さレベルに変更されようとしました。");
			return false;
		}
	}

	void Start() {
		WeightLvSeem = WeightLv;
	}
}
