using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFloor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//現在の重さの位置へ移動する
		mFloor.transform.localPosition = GetTargetLocalPosition();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateState();
	}


	enum CState {
		cStay,	//はまっている
		cToMoving,	//はまりから移動
		cFromMoving,	//移動からはまる
		cMoving,	//移動
		cTurn,	//方向転換
	}
	CState _mState = CState.cStay;  //現在の状態
	CState mState {
		get { return _mState; }
		set {
			mBeforeState = _mState;
			_mState = value;
			mNeedInitState = true;
		}
	}

	CState mBeforeState;  //以前の状態

	float mStateTime = 0.0f;

	bool mNeedInitState = true;	//状態が変化するとtrueになる

	void UpdateState() {

		if(mNeedInitState) {
			mStateTime = 0.0f;
		}

		switch (mState) {
			case CState.cStay:
				UpdateStay();
				break;
			case CState.cToMoving:
				UpdateToMoving();
				break;
			case CState.cFromMoving:
				UpdateFromMoving();
				break;
			case CState.cTurn:
				UpdateTurn();
				break;
			case CState.cMoving:
				UpdateMoving();
				break;
		}

		mStateTime += Time.deltaTime;
	}

	void UpdateStay() {
		//初期化
		if(mNeedInitState == true) {
			mNeedInitState = false;
		}
		//処理

		Vector3 lTargetLocalPosition = GetTargetLocalPosition();
		if(mFloor.transform.localPosition != lTargetLocalPosition) {
			mState = CState.cToMoving;
		}
	}

	void UpdateToMoving() {
		//初期化
		if (mNeedInitState == true) {
			mNeedInitState = false;
			mMoveDirection = GetTargetLocalPosition() - mFloor.transform.localPosition;
		}

		//処理

		Vector3 lVibration = GetFloorPositionAnimation(mStateTime, mToMovingTime, mToMovingHz, mToMovingAmp);
		VibrationFloor(lVibration);

		if (mStateTime >= mToMovingTime) {
			mState = CState.cMoving;
		}
	}

	void UpdateFromMoving() {
		//初期化
		if (mNeedInitState == true) {
			mNeedInitState = false;
		}

		//処理

		Vector3 lVibration = GetFloorPositionAnimation(mStateTime, mFromMovingTime, mFromMovingHz, mFromMovingAmp);
		VibrationFloor(lVibration);

		if (mStateTime >= mFromMovingTime) {
			mState = CState.cStay;
		}
	}

	void UpdateMoving() {
		//初期化
		if (mNeedInitState == true) {
			mNeedInitState = false;
			mMoveDirection = GetTargetLocalPosition() - mFloor.transform.localPosition;
		}

		//
		//処理
		//

		Vector3 lTargetLocalPosition = GetTargetLocalPosition();


		//方向転換のチェック
		//

		Vector3 lMoveDirection = lTargetLocalPosition - mFloor.transform.localPosition;

		//進行方向が逆向きなら
		if (Vector3.Dot(lMoveDirection, mMoveDirection) < 0.0f) {
			mState = CState.cTurn;
			return;
		}
		mMoveDirection = lMoveDirection;


		//移動
		//

		Vector3 lLocalPosition = MovePosition(mFloor.transform.localPosition, lTargetLocalPosition, mMoveSpeed * Time.deltaTime);
		MoveMoveFloor(lLocalPosition);

		if(mFloor.transform.localPosition == lTargetLocalPosition) {
			mState = CState.cFromMoving;
			return;
		}
	}

	void UpdateTurn() {
		//初期化
		if (mNeedInitState == true) {
			mNeedInitState = false;
		}

		//処理

		Vector3 lVibration = GetFloorPositionAnimation(mStateTime, mTurnTime, mTurnHz, mTurnAmp);
		VibrationFloor(lVibration);

		if (mStateTime >= mTurnTime) {
			mState = CState.cMoving;
		}
	}

	Vector3 mMoveDirection;

	Vector3 GetTargetLocalPosition() {
		switch (GetWeight()) {
			case WeightManager.Weight.flying:
				return Vector3.up * mUpHeight;
			case WeightManager.Weight.light:
				return Vector3.zero;
			case WeightManager.Weight.heavy:
				return Vector3.down * mDownHeight;
		}
		Debug.LogError("ErrorWeight", this);
		return Vector3.zero;
	}

	WeightManager.Weight GetWeight() {
		return mWeightManager.WeightLv;
	}


	Vector3 MovePosition(Vector3 aFrom, Vector3 aTo, float aDistance) {
		Vector3 lDir = aTo - aFrom;
		if (lDir.magnitude < aDistance) return aTo;
		return aFrom + lDir.normalized * aDistance;
	}
	void MoveMoveFloor(Vector3 aToLocalPosition) {
		Vector3 lWorldPosition = transform.TransformPoint(aToLocalPosition);
		Vector3 lWorldMoveDelta = lWorldPosition - mFloor.transform.position;

		Vector3 lBeforeColliderPosition = mFloor.transform.position;

		//行けるところを計算する
		MoveManager.Move(lWorldMoveDelta, mFloor.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Box", "Stage", "Player"}));

		//コライダーの位置をもとに戻す
		Vector3 lMoveRes = mFloor.transform.position - lBeforeColliderPosition;
	
		//動けた場合
		if(lMoveRes.magnitude != 0.0f){
			RotateGear(lMoveRes);	//動けた分歯車を回転
		}
		else {
			if(mStateTime % 2.0f < 0.5f) {
				Vector3 lVibration = GetFloorPositionAnimation(mStateTime, mStateTime + 1.0f, mTurnHz, mTurnAmp);
				Debug.Log("Vibration:" + lVibration);
				VibrationFloor(lVibration);
			}
		}
		
	}
	void VibrationFloor(Vector3 aToLocalPosition) {
		RotateGear(mFloorModel.transform.localPosition, aToLocalPosition);
		mFloorModel.transform.localPosition = aToLocalPosition;
	}

	Vector3 GetFloorPositionAnimation(float aNowTime, float aEndTime, int aHz, float aAmp) {
		if (aNowTime >= aEndTime) return Vector3.zero;
		float lRad = (aNowTime % (1.0f / aHz) ) * (aHz) * 2 * Mathf.PI;
		return Vector3.up * Mathf.Sin(lRad) * aAmp;
	}

	void RotateGear(Vector3 aFrom, Vector3 aTo) {
		RotateGear(aTo - aFrom);
	}
	void RotateGear(Vector3 aMove) {
		float lDistance = aMove.y;
		mGearLeftModel.transform.localRotation *= Quaternion.Euler(0.0f, 0.0f, lDistance * -mGearRotateSpeed);
		mGearRightModel.transform.localRotation *= Quaternion.Euler(0.0f, 0.0f, lDistance * mGearRotateSpeed);
	}


#if UNITY_EDITOR

	//床のサイズ変更
	void ResizeFloor() {

		//現在のモデルの削除
		for (int i = mFloorModel.transform.childCount - 1; i >= 0; i--) {
			if (EditorUtility.IsInPrefab(mFloorModel.transform.GetChild(i).gameObject, EditorUtility.GetPrefab(gameObject))) continue;
			EditorUtility.DestroyGameObject(mFloorModel.transform.GetChild(i).gameObject);
		}

		//モデルの配置
		
		//左端
		GameObject lLeft = EditorUtility.InstantiatePrefab(mFloorLeftPrefab, mFloorModel);
		lLeft.transform.localPosition = Vector3.left * (float)(mWidth - 1) / 2;

		//真ん中
		for (int i = 1; i < mWidth - 1; i++) {
			float lIndexFromMiddle =  i - (float)(mWidth - 1) / 2;
			GameObject lMiddle = EditorUtility.InstantiatePrefab(mFloorMiddlePrefab, mFloorModel);
			lMiddle.transform.localPosition = lIndexFromMiddle * Vector3.right;
		}

		//右端
		GameObject lRight = EditorUtility.InstantiatePrefab(mFloorRightPrefab, mFloorModel);
		lRight.transform.localPosition = Vector3.right * (float)(mWidth - 1) / 2;


		//コライダーの大きさ変更
		mFloor.GetComponent<BoxCollider>().size = new Vector3(mWidth, 1.0f, 1.0f);

		Vector3 lSelectAreaSize = mSelectArea.transform.localScale;
		lSelectAreaSize.x = mWidth + mSelectAreaWidth * 2.0f;
		lSelectAreaSize.y = 1.0f + mSelectAreaHeight * 2.0f;
		mSelectArea.transform.localScale = lSelectAreaSize;
	}

	//レールのサイズ変更
	void ResizeRail() {
		//現在のモデルの削除
		for (int i = mRailModel.transform.childCount - 1; i >= 0; i--) {
			if (EditorUtility.IsInPrefab(mRailModel.transform.GetChild(i).gameObject, EditorUtility.GetPrefab(gameObject))) continue;
			EditorUtility.DestroyGameObject(mRailModel.transform.GetChild(i).gameObject);
		}

		//モデルの配置

		//上端
		GameObject lTop = EditorUtility.InstantiatePrefab(mRailTopPrefab, mRailModel);
		lTop.transform.localPosition = Vector3.up * (mUpHeight - 0.5f);

		//真ん中
		for (int i = mUpHeight - 1; i > -mDownHeight; i--) {
			GameObject lMiddle;
			if (i == 0) {
				lMiddle = EditorUtility.InstantiatePrefab(mRailOnLightPrefab, mRailModel);
			}
			else {
				lMiddle = EditorUtility.InstantiatePrefab(mRailMiddlePrefab, mRailModel);
			}
			lMiddle.transform.localPosition = Vector3.up * i;
		}

		//下端
		GameObject lBottom = EditorUtility.InstantiatePrefab(mRailBottomPrefab, mRailModel);
		lBottom.transform.localPosition = Vector3.down * (mDownHeight - 0.5f);
	}

	[ContextMenu("Resize")]
	void Resize() {
		if (this == null) return;
		if (EditorUtility.IsPrefab(gameObject)) return;
		ResizeFloor();
		ResizeRail();

		//現在の重さの位置へ移動する
		mFloor.transform.localPosition = GetTargetLocalPosition();
	}

	private void OnValidate() {
		//UnityEditor.EditorApplication.delayCall += Resize;
	}

#endif

	[SerializeField, Tooltip("床の幅")]
	int mWidth;

	[SerializeField, Tooltip("上方向の高さ")]
	int mUpHeight;

	[SerializeField, Tooltip("下方向の高さ")]
	int mDownHeight;


	[SerializeField, Tooltip("重さ"), Space(16)]
	public WeightManager mWeightManager;

	[SerializeField, Tooltip("動き出すときの振動数")]
	int mToMovingHz = 5;

	[SerializeField, Tooltip("動き出すときの振動の強さ")]
	float mToMovingAmp = 0.05f;

	[SerializeField, Tooltip("止まっている状態から、動き始めるまでの時間")]
	float mToMovingTime = 1.0f;

	[SerializeField, Tooltip("止まる時の振動数")]
	int mFromMovingHz = 5;

	[SerializeField, Tooltip("止まる時の振動の強さ")]
	float mFromMovingAmp = 0.05f;

	[SerializeField, Tooltip("動いている状態から止まるまでの時間")]
	float mFromMovingTime = 1.0f;

	[SerializeField, Tooltip("ターンの時の振動数")]
	int mTurnHz = 5;

	[SerializeField, Tooltip("ターンの時の振動の強さ")]
	float mTurnAmp = 0.05f;

	[SerializeField, Tooltip("逆に動くときの、方向転換で止まる時間")]
	float mTurnTime = 1.0f;

	[SerializeField, Tooltip("床が1秒間に動く距離")]
	float mMoveSpeed = 1.0f;


	[SerializeField, Tooltip("床"), Space(16)]
	GameObject mFloor;

	[SerializeField, EditOnPrefab, Tooltip("選択範囲のコライダー")]
	GameObject mSelectArea;

	[SerializeField, EditOnPrefab, Tooltip("選択範囲の横幅を大きくする追加分")]
	float mSelectAreaWidth;

	[SerializeField, EditOnPrefab, Tooltip("選択範囲の縦幅を大きくする追加分")]
	float mSelectAreaHeight;

	[SerializeField, EditOnPrefab, Tooltip("床の全てのモデルの親")]
	GameObject mFloorModel;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("床の左端のモデル")]
	GameObject mFloorLeftPrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("床の真ん中モデル")]
	GameObject mFloorMiddlePrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("床の右端のモデル")]
	GameObject mFloorRightPrefab;


	[SerializeField, EditOnPrefab, Tooltip("レールの全てのモデルの親"), Space(16)]
	GameObject mRailModel;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("レールの上端のモデル")]
	GameObject mRailTopPrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("レールの真ん中モデル")]
	GameObject mRailMiddlePrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("レールの重さ1の時のモデル")]
	GameObject mRailOnLightPrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("レールの下端のモデル")]
	GameObject mRailBottomPrefab;

	[SerializeField, EditOnPrefab, Tooltip("左の歯車")]
	GameObject mGearLeftModel;

	[SerializeField, EditOnPrefab, Tooltip("右の歯車")]
	GameObject mGearRightModel;

	[SerializeField, EditOnPrefab, Tooltip("移動距離に応じた、歯車の回る速度")]
	float mGearRotateSpeed;
}
