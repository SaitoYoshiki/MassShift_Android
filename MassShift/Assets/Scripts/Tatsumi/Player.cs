using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField] bool walkFlg = true;    // 左右移動可能フラグ
	[SerializeField] bool jumpFlg = true;    // ジャンプ可能フラグ
	[SerializeField] bool shotFlg = true;    // ショット可能フラグ

	[SerializeField] float walkSpd = 2.0f;       // 左右移動最高速度
	[SerializeField] float walkStopTime = 0.2f;  // 左右移動最高速度から停止までの時間(秒)

	[SerializeField] List<float> jumpWeightLvDis;       // 最大ジャンプ距離
	float JumpDis {
		get {
			return jumpWeightLvDis[(int)WeightMng.WeightLv];
		}
	}
	[SerializeField] List<float> jumpWeightLvHeight;    // 最大ジャンプ高度
	float JumpHeight {
		get {
			return jumpWeightLvHeight[(int)WeightMng.WeightLv];
		}
	}
	[SerializeField] List<float> jumpWeightLvTime;      // 最大ジャンプ滞空時間(秒)
	float JumpTime {
		get {
			return jumpWeightLvTime[(int)WeightMng.WeightLv];
		}
	}

	[SerializeField] float walkStandbyVec = 0.0f;	// 移動しようとしている方向
	[SerializeField] bool jumpStandbyFlg = false;	// ジャンプしようとしているフラグ
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

	Lifting lift = null;
	Lifting Lift {
		get {
			if (!lift) {
				lift = GetComponent<Lifting>();
				if (!lift) {
					Debug.LogError("Liftingが見つかりませんでした。");
				}
			}
			return lift;
		}
	}
	bool liftTrg = false;

	WaterState waterStt = null;
	WaterState WaterStt {
		get {
			if (waterStt == null) {
				waterStt = GetComponent<WaterState>();
			}
			return waterStt;
		}
	}

	[SerializeField] Transform rotTransform = null;
	[SerializeField] Vector3 rotVec = new Vector3(1.0f, 0.0f, 0.0f); // 左右向きと非接地面
	[SerializeField] float rotSpd = 0.2f;
	[SerializeField] float turnRotBorderSpd = 1.0f;

	// Use this for initialization
	//	void Start () {}

	// Update is called once per frame
	void Update() {
		// 左右移動入力
		walkStandbyVec = Input.GetAxis("Horizontal");

		// ジャンプ入力
		jumpStandbyFlg = (Input.GetAxis("Jump") != 0.0f);

		// ジャンプ滞空時間
		remainJumpTime = (!Land.IsLanding ? remainJumpTime + Time.deltaTime : 0.0f);

		// 持ち上げ/下げ
		if (Land.IsLanding) {
			if ((Input.GetAxis("Lift") != 0.0f)) {
				if (!liftTrg) {
					Lift.Lift();
				}
				liftTrg = true;
			} else {
				liftTrg = false;
			}
		}
	}

	void FixedUpdate() {
		// 左右移動
		Walk();

		// ジャンプ
		Jump();

		// 立ち止まり
		WalkDown();

		// 左右上下回転
		Rotate();
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
			MoveMng.AddMove(new Vector3(walkStandbyVec * (JumpDis / JumpTime) * Time.deltaTime, 0.0f, 0.0f));
		}
	}
	void Jump() {
		// ジャンプ入力がなければ
		if (!jumpStandbyFlg) return;

		// ジャンプ可能でなければ
		if (!jumpFlg) return;

		// ステージ又は水面に接地していなければ
		if (!Land.IsLanding && !WaterStt.IsWaterSurface) {
			PileWeight pile = GetComponent<PileWeight>();
			// 接地しているオブジェクトにも接地していなければ
			List<Transform> pileObjs = pile.GetPileBoxList(new Vector3(0.0f, MoveMng.GravityForce, 0.0f));
			bool stagePile = false;
			foreach (var pileObj in pileObjs) {
				Landing pileLand = pileObj.GetComponent<Landing>();
				WaterState pileWaterStt = pileObj.GetComponent<WaterState>();
				if ((pileLand && (pileLand.IsLanding || pileLand.IsExtrusionLanding)) ||
					(pileWaterStt && (pileWaterStt.IsWaterSurface))) {
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
		float jumpGravityForce = -10;   // ジャンプ中の重力加速度

		MoveMng.AddMove(new Vector3(0.0f, (-jumpGravityForce * JumpTime * 0.5f), 0.0f));
		Debug.Log(jumpGravityForce);

		// 離地
		Land.IsLanding = false;
		WaterStt.IsWaterSurface = false;
		WaterStt.BeginWaterStopIgnore();

		// ジャンプ入力を無効化
		jumpStandbyFlg = false;

		// 通常の重力加速度を一時的に無効
		MoveMng.GravityCustomTime = (Time.time + JumpTime);
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

	void Rotate() {
		// 持ち上げモーション中は処理しない
		if ((Lift.St == Lifting.LiftState.invalid) ||
			(Lift.St == Lifting.LiftState.standby)) {
			// 接地中なら
			if (Land.IsLanding || WaterStt.IsWaterSurface) {
				// 移動方向によって向きを設定
				if (MoveMng.PrevMove.x > turnRotBorderSpd) {
					rotVec.x = 1.0f;
				} else if (MoveMng.PrevMove.x < -turnRotBorderSpd) {
					rotVec.x = -1.0f;
				}
			}

			// 接地方向によって向きを設定
			if (WeightMng.WeightLv == WeightManager.Weight.flying) {
				rotVec.y = 1.0f;
			} else if (MoveMng.PrevMove.y > 0.0f) {
				rotVec.y = 0.0f;
			}

			// 設定された向きにスラープ
			Quaternion qt = Quaternion.Euler(rotVec.y * 180.0f, -90.0f + rotVec.x * 90.0f, 0.0f);
			rotTransform.rotation = Quaternion.Slerp(rotTransform.rotation, qt, rotSpd);
		}
	}
}
