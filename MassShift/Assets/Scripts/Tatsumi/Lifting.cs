using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifting : MonoBehaviour {
	public enum LiftState {
		invalid,
		standby,
		liftUp,
		liftUpFailed,
		lifting,
		liftDown,
		liftDownFailed,
	}

	[SerializeField] Transform liftPoint = null;    // 持ち上げ位置
	[SerializeField] Transform liftUpCol = null;    // 持ち上げ可能判定
	[SerializeField] GameObject liftObj = null;     // 持ち上げ中オブジェクト
	[SerializeField] Collider standbyCol = null;    // 非持ち上げ中の本体当たり判定
	[SerializeField] Collider liftingCol = null;    // 持ち上げ中の本体当たり判定
	[SerializeField] LiftState st;
	public LiftState St {
		get {
			return st;
		}
	}
	bool heavyFailedFlg = false;

	public bool IsLifting {
		get {
			return (St == LiftState.lifting);
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

	[SerializeField] PlayerAnimation plAnim = null;
	public PlayerAnimation PlAnim {
		get {
			if (plAnim == null) {
				plAnim = GetComponent<PlayerAnimation>();
				if (plAnim == null) {
					Debug.LogError("PlayerAnimationが見つかりませんでした。");
				}
			}
			return plAnim;
		}
		set {
			plAnim = value;
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
		UpdateLifting();
	}

	void UpdateLifting() {
		switch (st) {
		case LiftState.liftUp:
			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			// 持つオブジェクトの補間位置が現在のオブジェクトより高ければ
			bool liftMoveFlg = false;
			if (MoveMng.GravityForce < 0.0f) {	// 接地方向が下
				if (PlAnim.GetBoxPosition().y > liftObj.transform.position.y) {
					liftMoveFlg = true;
				}
			} else {							// 接地方向が上
				if (PlAnim.GetBoxPosition().y < liftObj.transform.position.y) {
					liftMoveFlg = true;
				}
			}

			// オブジェクトの位置を同期
			if (liftMoveFlg) {
				if (heavyFailedFlg || (!MoveManager.MoveTo(PlAnim.GetBoxPosition(), liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box" })))) {
					Debug.Log("持ち上げ失敗");

					// 同期できなければ下ろす
					st = LiftState.liftUpFailed;

					// 失敗アニメーションへの遷移
					PlAnim.FailedCatch();

					return;
				}
			}

			// 持ち上げ完了時
			if (PlAnim.CompleteCatch()) {
				LiftEndObject(liftObj, true);
//				// 持ち上げ中オブジェクトの判定と挙動を無効化
//				liftObj.GetComponent<BoxCollider>().enabled = false;
//				liftObj.GetComponent<MoveManager>().enabled = false;
//
//				// 持ち上げ中のプレイヤー当たり判定を有効化
//				standbyCol.enabled = false;
//				liftingCol.enabled = true;

				// 持ち上げ中状態
				st = LiftState.lifting;
			}
			break;

		case LiftState.liftDown:
			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			// オブジェクトの位置を同期
			if (!MoveManager.MoveTo(liftPoint.position, liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box" }))) {
				Debug.Log("下ろし失敗");

				// 同期できなければ離す
				LiftEndObject(liftObj, false);

				// 待機状態に
				st = LiftState.standby;

				// アニメーション遷移
				PlAnim.ExitRelease();

				return;
			}

			// 下ろし完了時
			if (PlAnim.CompleteRelease()) {
				// 
				LiftEndObject(liftObj, false);

				//				// 持ち上げ中オブジェクトの判定と挙動を有効化
				//				liftObj.GetComponent<BoxCollider>().enabled = true;
				//				liftObj.GetComponent<MoveManager>().enabled = true;
				//
				//				// 持ち上げ中のプレイヤー当たり判定を無効化
				//				standbyCol.enabled = true;
				//				liftingCol.enabled = false;
				//
				//				// 持ち上げ中オブジェクトを下ろし切る
				//				MoveManager.MoveTo(liftPoint.position, liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "" }));
				//				liftObj = null;

				// 待機状態に
				st = LiftState.standby;

				// アニメーション遷移
				PlAnim.ExitRelease();
			}
			break;

		case LiftState.liftUpFailed:
			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			// オブジェクトの位置を同期
			if (!MoveManager.MoveTo(PlAnim.GetBoxPosition(), liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box" }))) {
				Debug.Log("持ち上げ失敗に失敗");

				// 同期できなければ下ろす
				st = LiftState.standby;

				LiftEndObject(liftObj, false);

				// 待機アニメーションへの遷移
				PlAnim.ExitCatchFailed();

				return;
			}

			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			// 持ち上げ失敗完了時
			if (PlAnim.CompleteCatchFailed()) {
				// 下ろし時のオブジェクト挙動を変更
				LiftEndObject(liftObj, false);

				PlAnim.ExitCatchFailed();

				liftObj = null;
			}
			break;

/// ひとまず下ろし失敗状態に遷移する事はない
//		case LiftState.liftDownFailed:
//			// 移動不可
//			MoveMng.StopMoveVirticalAll();
//			MoveMng.StopMoveHorizontalAll();
//
//			// オブジェクトの位置を同期
//			if (!MoveManager.MoveTo(liftPoint.position, liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box" }))) {
//				Debug.Log("下ろし失敗");
//
//				// 同期できなければ持ち上げる
//				LiftUp(liftObj);
//				return;
//			}
//
//			// 下ろし完了時
//			if (PlAnim.CompleteRelease()) {
//				// 持ち上げ中オブジェクトの判定と挙動を有効化
//				liftObj.GetComponent<BoxCollider>().enabled = true;
//				liftObj.GetComponent<MoveManager>().enabled = true;
//
//				// 持ち上げ中のプレイヤー当たり判定を無効化
//				standbyCol.enabled = true;
//				liftingCol.enabled = false;
//
//				// 持ち上げ中オブジェクトを下ろし切る
//				MoveManager.MoveTo(liftPoint.position, liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "" }));
//				liftObj = null;
//			}
//			break;

		case LiftState.lifting:
			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			// オブジェクトの位置を同期
			MoveManager.MoveTo(PlAnim.GetBoxPosition(), liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box" }));
			break;

		default:
			break;
		}
	}

	public GameObject Lift() {
		Debug.Log("lift");
		switch (st) {
		case LiftState.standby:
			// 重さ変更中は処理しない
			if (false) return null;	//test

			// ジャンプと重さ変更を不可に
			Pl.CanJump = false;
			Pl.CanShift = false;

			RaycastHit hitInfo;
			// 持ち上げれるオブジェクトがあれば
			if (Physics.BoxCast(transform.position, liftUpCol.lossyScale * 0.5f, (liftUpCol.position - transform.position),
				out hitInfo, liftPoint.rotation, Vector3.Distance(transform.position, liftUpCol.position), LayerMask.GetMask(new string[] { "Box" }))) {
				// 重さがプレイヤーより重ければ失敗フラグを立てる
				heavyFailedFlg = (Pl.GetComponent<WeightManager>().WeightLv < hitInfo.collider.GetComponent<WeightManager>().WeightLv);
				// 持ち上げ開始
				return LiftUp(hitInfo.collider.gameObject);
			} else {
				liftObj = null;
			}
			break;

		case LiftState.lifting:
			// ジャンプと重さ変更を不可に
			Pl.CanJump = false;
			Pl.CanShift = false;

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

			// 持ち上げアニメーションへの遷移
			PlAnim.StartCatch(_obj);

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

		// 下ろしアニメーションへの遷移
		PlAnim.StartRelease();

		// 状態の変更
		st = LiftState.liftDown;

		return liftObj;
	}

	void LiftEndObject(GameObject _obj, bool _liftUp) {
		// 持ち上げ中オブジェクトの判定と挙動を無効化/有効化
		liftObj.GetComponent<BoxCollider>().enabled = !_liftUp;
		liftObj.GetComponent<MoveManager>().enabled = !_liftUp;

		// 通常時のプレイヤー当たり判定を無効化/有効化
		standbyCol.enabled = !_liftUp;

		// 持ち上げ中のプレイヤー当たり判定を有効化/無効化
		liftingCol.enabled = _liftUp;

		// 持ち上げきったのなら
		if (_liftUp) {
			// 持ち上げ状態に遷移
			st = LiftState.lifting;
		}
		// 下ろし切ったのなら
		else {
			// 待機状態に遷移
			st = LiftState.standby;

			// 持ち上げオブジェクト中をnullに
			liftObj = null;

			// プレイヤーの重さ移しを可能に
			Pl.CanShift = true;
		}

		// プレイヤーのジャンプを可能に
		Pl.CanJump = true;
	}
}
