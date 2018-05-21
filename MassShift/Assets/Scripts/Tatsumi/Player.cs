using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField] bool walkFlg = true;	// 左右移動可能フラグ
	[SerializeField] bool jumpFlg = true;	// ジャンプ可能フラグ
	[SerializeField] bool shotFlg = true;	// ショット可能フラグ
	
	[SerializeField] float walkSpd = 2.0f;		// 左右移動最高速度
	[SerializeField] float walkStopTime = 0.2f;	// 左右移動最高速度から停止までの時間(秒)
	[SerializeField] float jumpDis = 2.0f;		// 最大ジャンプ距離
	[SerializeField] float jumpHeight = 2.0f;	// 最大ジャンプ高度
	[SerializeField] float jumpTime = 1.0f;		// 最大ジャンプ滞空時間(秒)

	[SerializeField] float walkStandbyVec = 0.0f;	// 移動しようとしている方向
	[SerializeField] bool jumpStandbyFlg = false;   // ジャンプしようとしているフラグ
//	float jumpLimitTime = 0.0f;						// 次回ジャンプ可能時間
	
	[SerializeField] float remainJumpTime = 0.0f;

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

	Landing land = null;
	Landing Land {
		get {
			if (land == null) {
				land = GetComponent<Landing>();
				if (land == null) {
					Debug.LogError("Landingが見つかりませんでした。");
				}
			}
			return land;
		}
	}

	WaterState waterStt = null;
	WaterState WaterStt {
		get {
			if (waterStt == null) {
				waterStt = GetComponent<WaterState>();
			}
			return waterStt;
		}
	}

	[SerializeField] Transform modelTransform = null;

	// Use this for initialization
	//	void Start () {}

	// Update is called once per frame
	void Update() {
		// 左右移動入力
		walkStandbyVec = Input.GetAxis("Horizontal");

		// ジャンプ入力
		jumpStandbyFlg = (Input.GetAxis("Jump") != 0.0f);

		// 残りジャンプ滞空時間
		remainJumpTime = (!Land.IsLanding ? remainJumpTime + Time.deltaTime : 0.0f);
	}

	void FixedUpdate() {
		// 左右移動
		Walk();

		// ジャンプ
		Jump();

		// 立ち止まり
		WalkDown();

		// モデル左右回転
		RotateModel();
	}

	void Walk() {
		// 左右移動入力があれば
		if (walkStandbyVec == 0.0f) return;

		// 左右移動可能でなければ
		if (!walkFlg) return;

		// 地上なら
		if (Land.IsLanding) {
			// 左右方向へ加速
			MoveMng.AddMove(new Vector3(walkStandbyVec * walkSpd, 0.0f, 0.0f));
		}
		// 空中なら
		else {
			// 左右方向へ加速
			MoveMng.AddMove(new Vector3(walkStandbyVec * (jumpDis / jumpTime) * Time.deltaTime, 0.0f, 0.0f));
		}
	}
	void Jump() {
		// ジャンプ入力がなければ
		if (!jumpStandbyFlg) return;

		// ジャンプ可能でなければ
		if (!jumpFlg) return;

		// ステージに接地していなければ
		if (!Land.IsLanding) {
			PileWeight pile = GetComponent<PileWeight>();
			// 接地しているオブジェクトにも接地していなければ
			List<Transform> pileObjs = pile.GetPileBoxList(new Vector3(0.0f, MoveMng.GravityForce, 0.0f));
			bool stagePile = false;
			foreach (var pileObj in pileObjs) {
				Landing pileLand = pileObj.GetComponent<Landing>();
				if (pileLand && (pileLand.IsLanding || pileLand.IsExtrusionLanding)) {
					stagePile = true;
				}
			}
			if ((pileObjs.Count == 0) || !stagePile) {
				// ジャンプ不可
				return;
			}
		}

		// ジャンプ直後であれば
//		if (jumpLimitTime > Time.time) return;

		Debug.Log("Jump");

		// 前回までの上下方向の加速度を削除
		MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);

		// 左右方向の加速度を削除
		MoveMng.StopMoveHorizontalAll();

		// 上方向へ加速
//		float jumpGravityForce = (0.5f * Mathf.Pow(jumpTime * 0.5f, 2) + jumpHeight);	// ジャンプ中の重力加速度
		float jumpGravityForce = -10;	// ジャンプ中の重力加速度

		MoveMng.AddMove(new Vector3(0.0f, (-jumpGravityForce * jumpTime * 0.5f), 0.0f));
		Debug.Log(jumpGravityForce);

		// 離地
		Land.IsLanding = false;

		// ジャンプ入力を無効化
		jumpStandbyFlg = false;

		// 通常の重力加速度を一時的に無効
		MoveMng.GravityCustomTime = (Time.time + jumpTime);
		MoveMng.GravityForce = jumpGravityForce;

		// 次回ジャンプ可能時間を設定
//		jumpLimitTime = Time.time + jumpTime * 0.5f;	// ジャンプしてからジャンプ滞空時間の半分の時間まではジャンプ不可
	}
	void WalkDown() {
		// 接地中でなければ
		if (!Land.IsLanding) return;

		// 進行方向側への左右入力があれば
		if ((walkStandbyVec != 0.0f) && (Mathf.Sign(MoveMng.PrevMove.x) == Mathf.Sign(walkStandbyVec))) return;

		// 減速
		float moveX = (Mathf.Min((walkSpd / walkStopTime), Mathf.Abs(MoveMng.PrevMove.x))) * -Mathf.Sign(MoveMng.PrevMove.x);
		MoveMng.AddMove(new Vector3(moveX, 0.0f, 0.0f));
	}

	void RotateModel() {

	}
}
