using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLifting : MonoBehaviour {
	enum LiftState {
		invalid,
		standby,
		liftUp,
		lifting,
		liftDown,
	}

	[SerializeField] Transform liftPoint = null;	// 持ち上げ位置
	[SerializeField] GameObject liftObj = null;		// 持ち上げ中オブジェクト
	[SerializeField] Collider standbyCol = null;	// 非持ち上げ中の本体当たり判定
	[SerializeField] Collider liftingCol = null;	// 持ち上げ中の本体当たり判定
	[SerializeField] Transform liftEndPoint = null;	// 下ろし完了時の下ろす地点
	[SerializeField] LiftState st;

	[SerializeField] Player pl = null;
	public Player Pl {
		get {
			if (!pl) {
				pl = GetComponent<Player>();
				if (!pl) {
					Debug.LogError("Playerが見つかりませんでした。");
				}
			}
			return pl;
		}
	}

	// Use this for initialization
	void Start () {
		if (liftPoint == null) {
			Debug.LogError("LiftPointが設定されていません。");
			enabled = false;
		}
	}

	// Update is called once per frame
	void Update() {
		Lifting();
	}

	void Lifting() {
		switch (st) {
		case LiftState.liftUp:
			// オブジェクトの位置を同期
			if (!MoveManager.MoveTo(liftPoint.position, liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box" }))) {
				// 同期できなければ下ろす
				LiftDown();
				return;
			}

			// 持ち上げ完了時
			if (false) {
				// 持ち上げ中オブジェクトの判定を無効化
				liftObj.GetComponent<BoxCollider>().enabled = false;

				// 持ち上げ中の当たり判定を有効化
				standbyCol.enabled = false;
				liftingCol.enabled = true;
			}
			break;
		case LiftState.liftDown:
			// オブジェクトの位置を同期
			if (!MoveManager.MoveTo(liftPoint.position, liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box" }))) {
				// 同期できなければ持ち上げる
				LiftUp(liftObj);
				return;
			}

			// 下ろし完了時
			if (false) {
				// 持ち上げ中の当たり判定を無効化
				standbyCol.enabled = true;
				liftingCol.enabled = false;

				// 持ち上げ中オブジェクトを下ろし切る
				MoveManager.MoveTo(liftEndPoint.position, liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "" }));
				liftObj = null;
			}
			break;
		default:
			break;
		}
	}

	public void LiftUp(GameObject _obj) {
		if (st == LiftState.standby) {
			// 持ち上げ中オブジェクトの設定
			liftObj = _obj;

			// 持ち上げアニメーションの再生

		}
	}
	public void LiftDown() {
		// 持ち上げ中オブジェクトの判定を有効化
		liftObj.GetComponent<BoxCollider>().enabled = true;

		// 下ろしアニメーションの再生

	}
}
