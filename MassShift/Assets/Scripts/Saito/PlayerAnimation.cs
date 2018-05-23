using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerAnimation : MonoBehaviour {

	[SerializeField]
	GameObject mAnimationModel;

	[SerializeField]
	Transform mHandTransform;

	[SerializeField]
	Transform mCatchStartHandPosition;

	Vector3 mCatchStartBoxPosition;

	[SerializeField]
	Transform mCatchEndHandPosition;

	[SerializeField]
	Transform mCatchEndBoxPosition;

	[SerializeField]
	Transform mReleaseEndHandPosition;

	[SerializeField]
	Transform mReleaseEndBoxPosition;

	[SerializeField]
	float mCatchStartTime = 0.0f;

	[SerializeField]
	float mCatchEndTime = 1.0f;

	[SerializeField]
	float mReleaseStartTime = 0.0f;

	[SerializeField]
	float mReleaseEndTime = 1.0f;

	Animator mAnimator;

	bool mIsHold = false;
	public bool mIsHover = false;	//浮いているかどうか

	enum CState {

		cStandBy,
		cWalk,
		cJumpStart,
		cJumpMid,
		cJumpFall,
		cJumpLand,
		cHoldStandBy,
		cHoldWalk,
		cHoldJumpStart,
		cHoldJumpMid,
		cHoldJumpFall,
		cHoldJumpLand,
		cCatch,
		cCatchFailed,
		cRelease,

	}

	[SerializeField]
	CState mState;

	[SerializeField]
	CState mBeforeState;

	bool mIsInit = true;

	bool mCompleteCatchFailed = false;
	bool mCompleteCatch = false;
	bool mCompleteRelease = false;

	[SerializeField]
	public float mStateTime;

	float mCatchFailedStateTime;

	float mSpeed = 0.0f;
	Vector3 mBeforePosition;
	
	void ChangeState(CState aNextState) {
		mBeforeState = mState;
		mState = aNextState;
		mIsInit = true;
		mStateTime = 0.0f;
	}

	// Use this for initialization
	void Start () {
		mAnimator = mAnimationModel.GetComponent<Animator>();
		mStateTime = 0.0f;
	}
	

	#region UpdateState


	// Update is called once per frame
	void Update () {
		mStateTime += Time.deltaTime;

		UpdateState();

	}


	void UpdateState() {

		switch(mState) {
			case CState.cStandBy:
				UpdateStandBy();
				break;
			case CState.cWalk:
				UpdateWalk();
				break;
			case CState.cJumpStart:
				UpdateJumpStart();
				break;
			case CState.cJumpMid:
				UpdateJumpMid();
				break;
			case CState.cJumpFall:
				UpdateJumpFall();
				break;
			case CState.cJumpLand:
				UpdateJumpLand();
				break;
			case CState.cCatch:
				UpdateCatch();
				break;
			case CState.cCatchFailed:
				UpdateCatchFailed();
				break;

			case CState.cHoldStandBy:
				UpdateHoldStandBy();
				break;
			case CState.cHoldWalk:
				UpdateHoldWalk();
				break;
			case CState.cHoldJumpStart:
				UpdateHoldJumpStart();
				break;
			case CState.cHoldJumpMid:
				UpdateHoldJumpMid();
				break;
			case CState.cHoldJumpFall:
				UpdateHoldJumpFall();
				break;
			case CState.cHoldJumpLand:
				UpdateHoldJumpLand();
				break;
			case CState.cRelease:
				UpdateRelease();
				break;
		}
	}
	
	void InitStandBy() {
		mAnimator.CrossFadeInFixedTime("StandBy", 0.2f);
	}

	void UpdateStandBy() {
		if(mIsInit) {
			InitStandBy();
			mIsInit = false;
		}
	}


	void InitWalk() {
		mAnimator.CrossFadeInFixedTime("Walk", 0.2f);
	}

	void UpdateWalk() {
		if (mIsInit) {
			InitWalk();
			mIsInit = false;
		}

		mAnimator.SetFloat("Speed", mSpeed);
	}


	void InitJumpStart() {
		mAnimator.CrossFadeInFixedTime("JumpStart", 0.2f);
	}

	void UpdateJumpStart() {
		if (mIsInit) {
			InitJumpStart();
			mIsInit = false;
		}

		if (IsAnimationEnd("JumpStart")) {
			ChangeState(CState.cJumpMid);
		}
	}


	void InitJumpMid() {
		mAnimator.CrossFadeInFixedTime("JumpMid", 0.0f);
		mBeforePosition = transform.position;
	}

	void UpdateJumpMid() {
		if (mIsInit) {
			InitJumpMid();
			mIsInit = false;
		}

		//落下しているなら
		if(IsFall()) {
			ChangeState(CState.cJumpFall);
		}

		mBeforePosition = transform.position;
	}

	bool IsFall() {
		if(mIsHover == false) {
			if (mBeforePosition.y > transform.position.y) {
				return true;
			}
		}
		else {
			if (mBeforePosition.y < transform.position.y) {
				return true;
			}
		}
		return false;
	}

	void InitJumpFall() {
		mAnimator.CrossFadeInFixedTime("JumpFall", 0.2f);
	}

	void UpdateJumpFall() {
		if (mIsInit) {
			InitJumpFall();
			mIsInit = false;
		}
	}


	void InitJumpLand() {
		mAnimator.CrossFadeInFixedTime("JumpLand", 0.2f);
	}

	void UpdateJumpLand() {
		if (mIsInit) {
			InitJumpLand();
			mIsInit = false;
		}

		if (IsAnimationEnd("JumpLand")) {
			ChangeState(CState.cStandBy);
		}
	}

	void InitCatch() {
		mAnimator.CrossFadeInFixedTime("Catch", 0.2f);
		mAnimator.SetFloat("CatchSpeed", 1.0f);
	}

	void UpdateCatch() {
		if (mIsInit) {
			InitCatch();
			mIsInit = false;
		}

		if (IsAnimationEnd("Catch")) {
			mCompleteCatch = true;
		}
	}

	void InitCatchFailed() {
		mAnimator.SetFloat("CatchSpeed", -1.0f);
	}

	bool EndCatchFailed() {
		if (!mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Catch")) return false;
		Debug.Log("CatchFailedTime:" + mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
		return mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.0f;
	}

	void UpdateCatchFailed() {
		if (mIsInit) {
			InitCatchFailed();
			mIsInit = false;
		}

		mCatchFailedStateTime -= Time.deltaTime * 2.0f;
		mStateTime = mCatchFailedStateTime;

		if (EndCatchFailed()) {
			mCompleteCatchFailed = true;
		}
	}



	void InitHoldStandBy() {
		mAnimator.CrossFadeInFixedTime("HoldStandBy", 0.2f);
	}

	void UpdateHoldStandBy() {
		if (mIsInit) {
			InitHoldStandBy();
			mIsInit = false;
		}
	}


	void InitHoldWalk() {
		mAnimator.CrossFadeInFixedTime("HoldWalk", 0.2f);
	}

	void UpdateHoldWalk() {
		if (mIsInit) {
			InitHoldWalk();
			mIsInit = false;
		}

		mAnimator.SetFloat("Speed", mSpeed);
	}


	void InitHoldJumpStart() {
		mAnimator.CrossFadeInFixedTime("HoldJumpStart", 0.2f);
	}

	void UpdateHoldJumpStart() {
		if (mIsInit) {
			InitHoldJumpStart();
			mIsInit = false;
		}

		if (IsAnimationEnd("HoldJumpStart")) {
			ChangeState(CState.cHoldJumpMid);
		}
	}


	void InitHoldJumpMid() {
		mAnimator.CrossFadeInFixedTime("HoldJumpMid", 0.0f);
		mBeforePosition = transform.position;
	}

	void UpdateHoldJumpMid() {
		if (mIsInit) {
			InitHoldJumpMid();
			mIsInit = false;
		}

		//落下しているなら
		if (IsFall()) {
			ChangeState(CState.cHoldJumpFall);
		}

		mBeforePosition = transform.position;
	}


	void InitHoldJumpFall() {
		mAnimator.CrossFadeInFixedTime("HoldJumpFall", 0.2f);
	}

	void UpdateHoldJumpFall() {
		if (mIsInit) {
			InitHoldJumpFall();
			mIsInit = false;
		}
	}


	void InitHoldJumpLand() {
		mAnimator.CrossFadeInFixedTime("HoldJumpLand", 0.2f);
	}

	void UpdateHoldJumpLand() {
		if (mIsInit) {
			InitHoldJumpLand();
			mIsInit = false;
		}

		if (IsAnimationEnd("HoldJumpLand")) {
			ChangeState(CState.cHoldStandBy);
		}
	}

	void InitRelease() {
		mAnimator.CrossFadeInFixedTime("Release", 0.2f);
	}

	void UpdateRelease() {
		if (mIsInit) {
			InitRelease();
			mIsInit = false;
		}

		if (IsAnimationEnd("Release")) {
			mCompleteRelease = true;
		}
	}

	GameObject mBox;

	public Vector3 GetBoxPosition() {
		Vector3 lRes = mBox.transform.position;

		//持ち上げている途中なら
		if (IsCatching() || IsCatchFailed()) {
			if(mCatchStartTime <= mStateTime && mStateTime < mCatchEndTime) {
				Vector3 lStartDifference = mCatchStartBoxPosition - mCatchStartHandPosition.position;
				Vector3 lEndDifference = mCatchEndBoxPosition.position - mCatchEndHandPosition.position;
				float lRate = (mStateTime - mCatchStartTime) / (mCatchEndTime - mCatchStartTime);
				lRes = mHandTransform.position + Vector3.Lerp(lStartDifference, lEndDifference, lRate);
			}
			else {
				lRes = mBox.transform.position;
			}
		}
		if (IsReleasing()) {
			if (mReleaseStartTime <= mStateTime && mStateTime < mReleaseEndTime) {
				Vector3 lStartDifference = mCatchEndBoxPosition.position - mCatchEndHandPosition.position;
				Vector3 lEndDifference = mReleaseEndBoxPosition.position - mReleaseEndHandPosition.position;
				float lRate = (mStateTime - mReleaseStartTime) / (mReleaseEndTime - mReleaseStartTime);
				lRes = mHandTransform.position + Vector3.Lerp(lStartDifference, lEndDifference, lRate);
			}
			else {
				lRes = mBox.transform.position;
			}
		}

		return ToZeroZ(lRes);
	}
	Vector3 ToZeroZ(Vector3 aVec) {
		return new Vector3(aVec.x, aVec.y, 0.0f);
	}

	public bool CompleteCatch() {
		return mCompleteCatch;
	}
	public bool CompleteRelease() {
		return mCompleteRelease;
	}

	public bool IsCatching() {
		return mState == CState.cCatch;
	}
	public bool IsReleasing() {
		return mState == CState.cRelease;
	}

	public bool CompleteCatchFailed() {
		return mCompleteCatchFailed;
	}

	public bool IsCatchFailed() {
		return mState == CState.cCatchFailed;
	}


	bool IsAnimationEnd(string aAnimationName) {
		if (!mAnimator.GetCurrentAnimatorStateInfo(0).IsName(aAnimationName)) return false;
		return mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
	}

	public float GetStateTime() {
		return mStateTime;
	}



	#endregion

	public void SetSpeed(float aSpeed) {
		mSpeed = aSpeed;
	}

	public void StartStandBy() {
		if (mState != CState.cWalk) return;	//歩き状態からしか外からは呼び出せない
		ChangeState(CState.cStandBy);
	}
	public void StartWalk() {
		if (mState != CState.cStandBy) return; //立ち止まり状態からしか外からは呼び出せない
		ChangeState(CState.cWalk);
	}

	public void StartJump() {
		ChangeState(CState.cJumpStart);
	}

	bool StartLandCheck() {
		if (mState == CState.cJumpLand) return false;
		if (mState == CState.cJumpMid) return true;
		if (mState == CState.cJumpFall) return true;
		return false;
	}

	public void StartLand() {
		if (!StartLandCheck()) return;
		ChangeState(CState.cJumpLand);
	}

	public void StartCatch(GameObject aBox) {
		if (mIsHold == true) return;
		mBox = aBox;
		mCatchStartBoxPosition = aBox.transform.position;
		ChangeState(CState.cCatch);
		mCompleteCatch = false;
	}

	public void FailedCatch() {
		mCatchFailedStateTime = mStateTime;
		ChangeState(CState.cCatchFailed);
		mCompleteCatchFailed = false;
	}


	public void StartHoldStandBy() {
		if (mState != CState.cHoldWalk) return; //歩き状態からしか外からは呼び出せない
		ChangeState(CState.cHoldStandBy);
	}
	public void StartHoldWalk() {
		if (mState != CState.cHoldStandBy) return; //立ち止まり状態からしか外からは呼び出せない
		ChangeState(CState.cHoldWalk);
	}

	public void StartHoldJump() {
		ChangeState(CState.cHoldJumpStart);
	}

	bool StartHoldLandCheck() {
		if (mState == CState.cHoldJumpLand) return false;
		if (mState == CState.cHoldJumpMid) return true;
		if (mState == CState.cHoldJumpFall) return true;
		return false;
	}

	public void StartHoldLand() {
		if (!StartHoldLandCheck()) return;
		ChangeState(CState.cHoldJumpLand);
	}



	public void StartRelease() {
		if (mIsHold == false) return;
		ChangeState(CState.cRelease);
		mCompleteRelease = false;
	}

	public void ExitCatchFailed() {
		mIsHold = false;
		ChangeState(CState.cStandBy);
	}
	public void ExitCatch() {
		mIsHold = true;
		ChangeState(CState.cHoldStandBy);
	}
	public void ExitRelease() {
		mIsHold = false;
		ChangeState(CState.cStandBy);
	}
}



/*


	cNone
	cStandBy
	cWalk
	cJumpStart
	cJumpMid
	cJumpFall
	cJumpLand
	cHoldStandBy
	cHoldWalk
	cHoldJumpStart
	cHoldJumpMid
	cHoldJumpFall
	cHoldJumpLand
	cCatch
	cRelease

	cWaterStandBy
	cWaterWalk
	cWaterJumpStart
	cWaterJumpMid
	cWaterJumpFall
	cWaterJumpLand
	cWaterHoldStandBy
	cWaterHoldWalk
	cWaterHoldJumpStart
	cWaterHoldJumpMid
	cWaterHoldJumpFall
	cWaterHoldJumpLand
	cWaterCatch
	cWaterRelease

 
*/
