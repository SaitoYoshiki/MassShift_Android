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
	[SerializeField] float stdLiftingColPoint = 1.0f;	// 接地方向が通常時の持ち上げ中の本体当たり判定の位置
	[SerializeField] float revLiftingColPoint = 0.0f;	// 接地方向が逆の時の持ち上げ中の本体当たり判定の位置
	[SerializeField] float liftObjMaxDisX = 0.9f;		// 持ち上げ時にx軸距離がこれ以上離れないように補正
	[SerializeField] LiftState st;
	public LiftState St {
		get {
			return st;
		}
		set {
			st = value;
		}
	}
	bool heavyFailedFlg = false;

	public bool IsLifting {
		get {
			return (St == LiftState.lifting);
		}
	}
	public bool IsLiftStop {
		get {
			return ((St == LiftState.liftUp) || (St == LiftState.liftUpFailed) || (St == LiftState.liftDown) || (St == LiftState.liftDownFailed));
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
		switch (St) {
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
				if (heavyFailedFlg || (!MoveManager.MoveTo(GetLiftUpBoxPoint(), liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box", "Fence" })))) {
					Debug.Log("持ち上げ失敗");

					// 同期できなければ下ろす
					St = LiftState.liftUpFailed;

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

				plAnim.ExitCatch();

				// 持ち上げ中状態
				St = LiftState.lifting;
			}
			break;

		case LiftState.liftDown:
			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			// オブジェクトの位置を同期
			if (!MoveManager.MoveTo(PlAnim.GetBoxPosition(), liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box", "Fence" }))) {
				Debug.Log("下ろし失敗");

				// 同期できなければ離す
				LiftEndObject(liftObj, false);

				// 待機状態に
				St = LiftState.standby;

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
				St = LiftState.standby;

				// アニメーション遷移
				PlAnim.ExitRelease();
			}
			break;

		case LiftState.liftUpFailed:
			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			// オブジェクトの位置を同期
			if (!MoveManager.MoveTo(PlAnim.GetBoxPosition(), liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box", "Fence" }))) {
				Debug.Log("持ち上げ失敗に失敗");

				// 同期できなければ下ろす
				St = LiftState.standby;

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
			// オブジェクトの位置を同期
			MoveManager.MoveTo(PlAnim.GetBoxPosition(), liftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box", "Fence" }));
			break;

		default:
			break;
		}
	}

	public GameObject Lift() {
		Debug.Log("lift");
		switch (St) {
		case LiftState.standby:
			// 重さ変更中は処理しない
			if (Pl.IsShift) return null;

			// 範囲内で最も近い持ち上げられるオブジェクトを取得
			List<RaycastHit> hitInfos = new List<RaycastHit>();
			hitInfos.AddRange(Physics.BoxCastAll(transform.position, liftUpCol.lossyScale * 0.5f, (liftUpCol.position - transform.position),
				liftPoint.rotation, Vector3.Distance(transform.position, liftUpCol.position), LayerMask.GetMask(new string[] { "Box" })));
			GameObject liftableObj = null;
			float dis = float.MaxValue;
			Debug.LogWarning(hitInfos.Count);
			foreach (var hitInfo in hitInfos) {
				Debug.LogWarning(hitInfo.collider.name + " " + hitInfo.collider.tag);
				if ((hitInfo.collider.tag == "LiftableObject") && (hitInfo.distance < dis)) {
					liftableObj = hitInfo.collider.gameObject;
					dis = hitInfo.distance;
				}
			}

			// 持ち上げれるオブジェクトがあれば
			if (liftableObj != null) {
				// 重さがプレイヤーより重ければ失敗フラグを立てる
				heavyFailedFlg = (Pl.GetComponent<WeightManager>().WeightLv < liftableObj.GetComponent<WeightManager>().WeightLv);

				// ジャンプ、重さ変更、振り向きを不可に
				Pl.CanJump = false;
				Pl.CanShift = false;
				Pl.CanRotation = false;

				// 持ち上げ開始
				return LiftUp(liftableObj);
			} else {
				liftObj = null;
			}
			break;

		case LiftState.lifting:
			// ジャンプ、重さ変更、振り向きを不可に
			Pl.CanJump = false;
			Pl.CanShift = false;
			Pl.CanRotation = false;

			// 下ろし始める
			LiftDown();

			break;

		default:
			break;
		}
		return null;
	}
	
	GameObject LiftUp(GameObject _obj) {
		if (St == LiftState.standby) {
			Debug.Log("LiftUp:" + _obj.name);

			// 持ち上げ中オブジェクトの設定
			liftObj = _obj;

			// 持ち上げアニメーションへの遷移
			PlAnim.StartCatch(_obj);

			// 状態の変更
			St = LiftState.liftUp;

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
		St = LiftState.liftDown;

		return liftObj;
	}

	void LiftEndObject(GameObject _obj, bool _liftUp) {
		// 持ち上げ中オブジェクトの判定と挙動を無効化/有効化
		liftObj.GetComponent<BoxCollider>().enabled = !_liftUp;
		liftObj.GetComponent<MoveManager>().enabled = !_liftUp;

		// 通常時のプレイヤー当たり判定を無効化/有効化
		standbyCol.enabled = !_liftUp;

		// 持ち上げ中のプレイヤー当たり判定有効化時に接地方向によって判定位置を移動
		if (_liftUp) {
			BoxCollider liftingBoxCol = ((BoxCollider)liftingCol);
			if (Pl.GetComponent<WeightManager>().WeightForce < 0.0f) {
				liftingBoxCol.center = new Vector3(liftingBoxCol.center.x, stdLiftingColPoint, liftingBoxCol.center.z);
			}else {
				liftingBoxCol.center = new Vector3(liftingBoxCol.center.x, revLiftingColPoint, liftingBoxCol.center.z);
			}
		}
		// 持ち上げ中のプレイヤー当たり判定を有効化/無効化
		liftingCol.enabled = _liftUp;

		// 有効な当たり判定をMoveManagerに登録
		if (standbyCol.enabled) {
			MoveMng.UseCol = standbyCol;
		}else {
			MoveMng.UseCol = liftingCol;
		}

		// 持ち上げきったのなら
		if (_liftUp) {
			// 持ち上げ状態に遷移
			St = LiftState.lifting;
		}
		// 下ろし切ったのなら
		else {
			// 待機状態に遷移
			St = LiftState.standby;

			// 持ち上げオブジェクト中をnullに
			liftObj = null;

			// プレイヤーの重さ移しを可能に
			Pl.CanShift = true;
		}

		// プレイヤーのジャンプ、振り向きを可能に
		Pl.CanJump = true;
		Pl.CanRotation = true;
	}

	Vector3 GetLiftUpBoxPoint() {
		if (liftObj == null) return Vector3.zero;

		// x軸が離れすぎていれば近づける
		Vector3 ret = PlAnim.GetBoxPosition();
		float dis = (ret.x - Pl.transform.position.x);
		if (Mathf.Abs(dis) > liftObjMaxDisX) {
			ret = new Vector3(Pl.transform.position.x + liftObjMaxDisX * Mathf.Sign(dis), ret.y, ret.z);
		}
		return ret;
	}
}
