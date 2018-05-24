using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing : MonoBehaviour {
	[SerializeField] Transform landingCol = null;    // 接地判定用オブジェクト

	[SerializeField] bool isLanding = false;
	public bool IsLanding {
		get {
			return isLanding;
		}
		set {
			Debug.Log("isLanding " + value + " " + name);

			if (value == true) {
				// 縦方向の移動を停止
				MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);
				MoveMng.StopMoveVirtical(MoveManager.MoveType.gravity);
			}
	
			// 値に変化がない
			if (isLanding == value) return;

			// 値を変更
			isLanding = value;

			// 接地時
			if (value == true) {
				// ジャンプによる通常の重力加速度停止を解除
				MoveMng.GravityCustomTime = 0.0f;

				// 有効になった瞬間
				IsLandingChange = true;
			}
		}
	}

	[SerializeField] bool isLandingChange = false;
	public bool IsLandingChange {
		get {
			return isLandingChange;
		}
		set {
			isLandingChange = value;
		}
	}
		
	[SerializeField] bool isExtrusionLanding;
	public bool IsExtrusionLanding {
		get {
			return isExtrusionLanding;
		}
		set {
			isExtrusionLanding = value;

			// 押し出し接地時
			if (value == true) {
				// 縦方向の移動を停止
				MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);
				MoveMng.StopMoveVirtical(MoveManager.MoveType.gravity);
			}
		}
	}

	[SerializeField] List<Collider> landColList = new List<Collider>();				// 接地しているオブジェクト
	[SerializeField] List<Collider> landExtrusionColList = new List<Collider>();	// 押し出しによって接地しているオブジェクト

//	[SerializeField] bool upCollide = false;
//	[SerializeField] bool downCollide = false;
//	[SerializeField] bool leftCollide = false;
//	[SerializeField] bool rightCollide = false;

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

	FourSideCollider fourSideCol = null;
	FourSideCollider FourSideCol {
		get {
			if (fourSideCol == null) {
				fourSideCol = GetComponent<FourSideCollider>();
				if (fourSideCol == null) {
					Debug.LogError("FourSideColliderが見つかりませんでした。");
				}
			}
			return fourSideCol;
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

	// 当たり判定を行うレイヤーマスク
	int mask;

	// Use this for initialization
	void Start () {
		mask = LayerMask.GetMask(new string[] { "Stage", "Player", "Box", "Fence" });
	}

	// Update is called once per frame
	void Update() {
		if ((IsLanding) || (IsExtrusionLanding)) {
			CheckLandingFalse();
		}
	}

	// 接触時にその接触が指定方向への接触かを判定
	public bool GetIsLanding(Vector3 _move) {
		if (_move == Vector3.zero) return false;

		float dot = Vector3.Dot((Vector3.up * WeightMng.WeightForce).normalized, _move.normalized);
//		Debug.LogError(landingCol.localPosition + " " + _move + " " + (dot < 0.0f));

		// 指定方向の反対方向への接触
		if (dot < 0.0f) return false;

		// 指定方向への接触
		return true;
	}

	void CheckLandingFalse() {
		landColList.Clear();

		// 接地方向に移動していなければ接地していない
		if (WeightMng.WeightLv != WeightManager.Weight.flying) {
			if (MoveMng.TotalMove.y > 0.0f) {
				IsLanding = false;
				return;
			}
		} else {
			if (MoveMng.TotalMove.y < 0.0f) {
				IsLanding = false;
				return;
			}
		}

		// 接地側の判定オブジェクトを取得
		if (MoveMng.GravityForce <= 0.0f) {
			landingCol = FourSideCol.BottomCol;
		} else {
			landingCol = FourSideCol.TopCol;
		}

		// 離地判定
		landColList.AddRange(Physics.OverlapBox(landingCol.position, landingCol.localScale * 0.5f, landingCol.rotation, mask));

		// 自身は接地対象から除く
		for (int idx = landColList.Count - 1; idx >= 0; idx--) {
			if (landColList[idx].gameObject == gameObject) {
				landColList.RemoveAt(idx);
			}
		}

		// 接地しているオブジェクトが存在しなければ離地
		if (landColList.Count <= 0) {
			IsLanding = false;
			IsExtrusionLanding = false;
			Debug.Log("離地 " + Support.ObjectInfoToString(gameObject));
		}
	}

	void CheckExtrusionLandingFalse() {
		landExtrusionColList.Clear();

		// 接地方向の反対方向に移動していなければ接地していない
		if (WeightMng.WeightLv != WeightManager.Weight.flying) {
			if (MoveMng.TotalMove.y < 0.0f) {
				IsExtrusionLanding = false;
				return;
			}
		} else {
			if (MoveMng.TotalMove.y > 0.0f) {
				IsExtrusionLanding = false;
				return;
			}
		}

		// 非接地側の判定オブジェクトを取得
		if (MoveMng.GravityForce > 0.0f) {
			landingCol = FourSideCol.BottomCol;
		} else {
			landingCol = FourSideCol.TopCol;
		}

		// 離地判定
		landExtrusionColList.AddRange(Physics.OverlapBox(landingCol.position, landingCol.localScale * 0.5f, landingCol.rotation, mask));

		// 自身は反接地対象から除く
		for (int idx = landExtrusionColList.Count - 1; idx >= 0; idx--) {
			if (landExtrusionColList[idx].gameObject == gameObject) {
				landExtrusionColList.RemoveAt(idx);
			}
		}

		// 接地・反接地しているオブジェクトが存在しなければ離地
		if (landExtrusionColList.Count <= 0) {
			IsExtrusionLanding = false;
			Debug.Log("Ext離地");
		}
	}
}
