using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField]
	bool canWalk = true;    // 左右移動可能フラグ
	public bool CanWalk {
		get {
			return canWalk;
		}
		set {
			canWalk = value;
		}
	}
	[SerializeField]
	bool canJump = true;    // ジャンプ可能フラグ
	public bool CanJump {
		get {
			return canJump;
		}
		set {
			canJump = value;
		}
	}
	[SerializeField]
	bool canShift = true;   // 重さ移し可能フラグ
	public bool CanShift {
		get {
			return canShift;
		}
		set {
			canShift = value;
		}
	}
	[SerializeField]
	bool canRotation = true;
	public bool CanRotation {
		get {
			return canRotation;
		}
		set {
			canRotation = value;
		}
	}
	[SerializeField]
	bool isShift = true;    // 重さ移し中フラグ
	public bool IsShift {
		get {
			return isShift;
		}
		set {
			isShift = value;
		}
	}
	public bool IsLanding {
		get {
			if (!Land) return false;
			return Land.IsLanding;
		}
	}
	[SerializeField]
	bool isRotation = false;
	bool IsRotation {
		get {
			return isRotation;
		}
		set {
			isRotation = value;
		}
	}

	[SerializeField]
	float walkSpd = 2.0f;       // 左右移動最高速度
	[SerializeField]
	float walkStopTime = 0.2f;  // 左右移動最高速度から停止までの時間

	[SerializeField]
	List<float> jumpWeightLvDis;       // 最大ジャンプ距離
	float JumpDis {
		get {
			return jumpWeightLvDis[(int)WeightMng.WeightLv];
		}
	}
	[SerializeField]
	List<float> jumpWeightLvHeight;    // 最大ジャンプ高度
	float JumpHeight {
		get {
			return jumpWeightLvHeight[(int)WeightMng.WeightLv];
		}
	}
	[SerializeField]
	List<float> jumpWeightLvTime;      // 最大ジャンプ滞空時間
	float JumpTime {
		get {
			return jumpWeightLvTime[(int)WeightMng.WeightLv];
		}
	}

	[SerializeField]
	float walkStandbyVec = 0.0f;    // 移動しようとしている方向
	[SerializeField]
	bool jumpStandbyFlg = false;   // ジャンプしようとしているフラグ
	bool prevJumpStandbyFlg = false;
	//	float jumpLimitTime = 0.0f;						// 次回ジャンプ可能時間

	[SerializeField]
	float remainJumpTime = 0.0f;

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

	PlayerAnimation plAnim = null;
	PlayerAnimation PlAnim {
		get {
			if (!plAnim) {
				plAnim = GetComponent<PlayerAnimation>();
				if (!plAnim) {
					Debug.LogError("PlayerAnimationが見つかりませんでした。");
				}
			}
			return plAnim;
		}
	}

	[SerializeField]
	Transform rotTransform = null;
	[SerializeField]
	Vector3 rotVec = new Vector3(1.0f, 0.0f, 0.0f); // 左右向きと非接地面
	[SerializeField]
	float rotSpd = 0.2f;
	[SerializeField]
	float turnRotBorderSpd = 1.0f;
	[SerializeField]
	float correctionaAngle = 1.0f;
	[SerializeField]
	float jumpStartOneTimeLimitSpd = 1.0f;

	// Use this for initialization
	//	void Start () {}

	// Update is called once per frame
	void Update() {
		// 左右移動入力
		walkStandbyVec = GetWalkInput();

		// ジャンプ入力
		jumpStandbyFlg |= GetJumpInput();

		// ジャンプ滞空時間
		remainJumpTime = (!Land.IsLanding ? remainJumpTime + Time.deltaTime : 0.0f);

		// 持ち上げ/下げ
		if ((Land.IsLanding || WaterStt.IsWaterSurface) && !IsRotation) {
			if (GetLiftInput()) {
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
		// 持ち下ろしアニメーション中以外なら
		if (!Lift.IsLiftStop) {
			// 左右移動
			Walk();
		}

		// ジャンプ
		bool isJump = Jump();
		prevJumpStandbyFlg = jumpStandbyFlg;
		jumpStandbyFlg = false;

		// 立ち止まり
		WalkDown();

		// 左右上下回転
		Rotate();

		// 着地アニメーション
		if (Land.IsLanding && Land.IsLandingChange) {
			Land.IsLandingChange = false;

			if (!Lift.IsLifting) {
				PlAnim.StartLand();
			}
			else {
				PlAnim.StartHoldLand();
			}
		}

		// 落下アニメーション
		if (!Land.IsLanding && Land.IsLandingChange) {
			Land.IsLandingChange = false;
			if (!isJump) {
				if (!Lift.IsLifting) {
					PlAnim.StartFall();
				}
				else {
					PlAnim.StartHoldFall();
				}
			}
		}
	}

	void Walk() {
		// 歩行アニメーション
		if ((walkStandbyVec != 0.0f) && CanWalk) {
			if (!Lift.IsLifting) {
				PlAnim.StartWalk();
			} else {
				PlAnim.StartHoldWalk();
			}
			PlAnim.SetSpeed(Mathf.Abs(walkStandbyVec));
		}
		// 待機アニメーション
		else {
			if (!Lift.IsLifting) {
				PlAnim.StartStandBy();
			} else {
				PlAnim.StartHoldStandBy();
			}
		}

		// 左右移動入力があれば
		if (walkStandbyVec == 0.0f) return;

		// 左右移動可能でなければ
		if (!canWalk) return;

		// 地上なら
		if (Land.IsLanding) {
			// 左右方向へ加速
			MoveMng.AddMove(new Vector3(walkStandbyVec * walkSpd, 0.0f, 0.0f));
		}
		// 空中なら
		else {
			// 左右方向へ加速
			MoveMng.AddMove(new Vector3(walkStandbyVec * (JumpDis / JumpTime) * Time.fixedDeltaTime, 0.0f, 0.0f));
		}
	}
	bool Jump() {
		// ジャンプ入力(トリガー)がなければ
		if (!jumpStandbyFlg || prevJumpStandbyFlg) return false;

		// ジャンプ可能でなければ
		if (!canJump) return false;

		// ステージに接地、又は水面で安定していなければ
		Debug.LogWarning("IsLanding:" + Land.IsLanding);
		//if (!Land.IsLanding && !WaterStt.IsWaterSurface) {
		if (!(Land.IsLanding || WaterStt.IsWaterSurface)) {
			PileWeight pile = GetComponent<PileWeight>();
			// 接地、又は安定しているオブジェクトにも接地していなければ
			List<Transform> pileObjs = pile.GetPileBoxList(new Vector3(0.0f, MoveMng.GravityForce, 0.0f));
			bool stagePile = false;
			foreach (var pileObj in pileObjs) {
				Landing pileLand = pileObj.GetComponent<Landing>();
				WaterState pileWaterStt = pileObj.GetComponent<WaterState>();
				if ((pileLand && (pileLand.IsLanding || pileLand.IsExtrusionLanding)) || (pileWaterStt && (pileWaterStt.IsWaterSurface))) {
					stagePile = true;
				}
			}
			if ((pileObjs.Count == 0) || !stagePile) {
				// ジャンプ不可
				return false;
			}
		}

		// ジャンプ直後であれば
		//		if (jumpLimitTime > Time.time) return;

		Debug.Log("Jump");

		// ジャンプアニメーション
		if (!Lift.IsLifting) {
			PlAnim.StartJump();
		} else {
			PlAnim.StartHoldJump();
		}

		// 前回までの上下方向の加速度を削除
		MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);

		// 左右方向の加速度を削除
		//		MoveMng.StopMoveHorizontalAll();

		// 左右方向の移動量も一更新だけ制限
		MoveMng.OneTimeMaxSpd = jumpStartOneTimeLimitSpd;

		// 上方向へ加速
		//float jumpGravityForce = (0.5f * Mathf.Pow(jumpTime * 0.5f, 2) + jumpHeight);	// ジャンプ中の重力加速度
		//		float jumpGravityForce = -100;   // ジャンプ中の重力加速度

		//		MoveMng.AddMove(new Vector3(0.0f, (-jumpGravityForce * JumpTime * 0.5f), 0.0f));
		//		Debug.Log(jumpGravityForce);

		MoveMng.AddMove(new Vector3(0.0f, (JumpHeight), 0.0f));

		// 離地
		Land.IsLanding = false;
		WaterStt.IsWaterSurface = false;
		WaterStt.BeginWaterStopIgnore();

		// ジャンプ入力を無効化
		jumpStandbyFlg = false;

		// 通常の重力加速度を一時的に無効
		//MoveMng.GravityCustomTime = (Time.time + JumpTime);
		//MoveMng.GravityForce = jumpGravityForce;

		// 次回ジャンプ可能時間を設定
		//		jumpLimitTime = Time.time + jumpTime * 0.5f;	// ジャンプしてからジャンプ滞空時間の半分の時間まではジャンプ不可

		return true;
	}
	void WalkDown() {
		// 接地中でなく、水上で安定状態もなければ
		if (!Land.IsLanding && !WaterStt.IsWaterSurface) {

			return;
		}

		// 進行方向側への左右入力があれば
		if ((walkStandbyVec != 0.0f) && (Mathf.Sign(MoveMng.PrevMove.x) == Mathf.Sign(walkStandbyVec))) return;

		// 減速
		float moveX = (Mathf.Min((walkSpd / walkStopTime), Mathf.Abs(MoveMng.PrevMove.x))) * -Mathf.Sign(MoveMng.PrevMove.x);
		MoveMng.AddMove(new Vector3(moveX, 0.0f, 0.0f));
	}

	void Rotate() {
		if (!CanRotation) return;

		//		// 持ち上げモーション中は処理しない
		//		if ((Lift.St == Lifting.LiftState.invalid) ||
		//			(Lift.St == Lifting.LiftState.standby)) {
//		// 接地中なら
//		if (Land.IsLanding || WaterStt.IsWaterSurface) {
			// 左右入力中なら
			if (walkStandbyVec != 0.0f) {
				// 一定の移動がある方向に向きを設定
				if (MoveMng.PrevMove.x > turnRotBorderSpd) {
					rotVec.x = 1.0f;
				} else if (MoveMng.PrevMove.x < -turnRotBorderSpd) {
					rotVec.x = -1.0f;
				} else {
					// 移動量が一定以下なら入力方向に向く
					if (walkStandbyVec > 0.0f) {
						rotVec.x = 1.0f;
					} else if (walkStandbyVec < 0.0f) {
						rotVec.x = -1.0f;
					}
				}
			}
//		}

		// 接地方向によって向きを設定
		if (WeightMng.WeightLv == WeightManager.Weight.flying) {
			rotVec.y = 1.0f;
		} else {
			rotVec.y = 0.0f;
		}

		// 結果の姿勢を求める
		Quaternion qt = Quaternion.Euler(rotVec.y * 180.0f, -90.0f + rotVec.x * 90.0f, 0.0f);

		// 現在の向きと結果の向きとの角度が一定以内なら
		float angle = Quaternion.Angle(rotTransform.rotation, qt);
		if (angle < correctionaAngle) {
			// 向きを合わせる
			rotTransform.rotation = Quaternion.Lerp(rotTransform.rotation, qt, 1);
			IsRotation = false;
		}
		// 角度が一定以上なら
		else {
			// 設定された向きにスラープ
			rotTransform.rotation = Quaternion.Slerp(rotTransform.rotation, qt, rotSpd);
			IsRotation = true;
		}
	}
	//	}

	float GetWalkInput() {
		if (Input.touchCount > 0) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Plane plane = new Plane(new Vector3(0.0f, 0.0f, -1.0f), 0.0f);

			float enter = 0.0f;
			if (plane.Raycast(ray, out enter)) {
				float lDistance = ray.GetPoint(enter).x - transform.position.x;
				return Mathf.Sign(lDistance);
			}
		}
		return Input.GetAxis("Horizontal");
	}

	bool GetJumpInput() {
		if(Input.touchCount > 0) {
			if(Input.touches[0].tapCount == 2) {
				return true;
			}
		}
		return Input.GetAxis("Jump") != 0.0f;
	}

	bool GetLiftInput() {
		return Input.GetAxis("Lift") != 0.0f;
	}
}
