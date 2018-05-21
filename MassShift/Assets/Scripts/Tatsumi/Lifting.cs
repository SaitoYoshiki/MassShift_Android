using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifting : MonoBehaviour {
	public enum LiftState {
		invalid,
		standby,
		liftUp,
		lifting,
		liftDown,
	}

	[SerializeField] Transform liftPoint = null;	// 持ち上げ位置
	[SerializeField] Transform liftUpCol = null;	// 持ち上げ可能判定
	[SerializeField] GameObject liftObj = null;		// 持ち上げ中オブジェクト
	[SerializeField] Collider standbyCol = null;	// 非持ち上げ中の本体当たり判定
	[SerializeField] Collider liftingCol = null;	// 持ち上げ中の本体当たり判定
	[SerializeField] LiftState st;
	public LiftState St {
		get {
			return st;
		}
	}

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

	MoveManager moveMng = null;
	MoveManager MoveMng {
		get {
			if (!moveMng) {
				moveMng = GetComponent<MoveManager>();
				if (!moveMng) {
					Debug.LogError("MoveManagerが見つかりませんでした。");
				}
			}
			return moveMng;
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
		LiftingObject();
	}

	void LiftingObject() {
		switch (st) {
		case LiftState.liftUp:
			// オブジェクトの位置を同期
			if (!MoveManager.MoveTo(liftPoint.position, liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box" }))) {
				// 同期できなければ下ろす
				LiftDown();
				return;
			}

			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

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
				MoveManager.MoveTo(liftPoint.position, liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "" }));
				liftObj = null;
			}
			break;
		default:
			break;
		}
	}

	public GameObject Lift() {
		Debug.Log("lift");
		switch (st) {
		case LiftState.standby:
			RaycastHit hitInfo;
			// 持ち上げれるオブジェクトがあれば
			if (Physics.BoxCast(transform.position, liftUpCol.lossyScale * 0.5f, (liftUpCol.position - transform.position), out hitInfo, liftPoint.rotation, Vector3.Distance(transform.position, liftUpCol.position), LayerMask.GetMask(new string[] { "Box" }))) {
				// 持ち上げ開始
				return LiftUp(hitInfo.collider.gameObject);
			} else {
				liftObj = null;
			}
			break;

		case LiftState.lifting:
			// 下ろし始める
			LiftDown();
			break;

		default:
			break;
		}
		return null;
	}
	
	GameObject LiftUp(GameObject _obj) {
		if (st == LiftState.standby) {
			Debug.Log("LiftUp:" + _obj.name);

			// 持ち上げ中オブジェクトの設定
			liftObj = _obj;

			// 持ち上げアニメーションの再生


			// 状態の変更
			st = LiftState.liftUp;

			return liftObj;
		}

		return null;
	}
	GameObject LiftDown() {
		Debug.Log("LiftDown:" + liftObj.name);

		// 持ち上げ中オブジェクトの判定を有効化
		liftObj.GetComponent<BoxCollider>().enabled = true;

		// 下ろしアニメーションの再生


		// 状態の変更
		st = LiftState.liftDown;

		return liftObj;
	}
}
