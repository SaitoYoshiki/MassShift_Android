using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour {

	[SerializeField]
	PlayerAnimation mPlayerAnimation;

	[SerializeField]
	GameObject mLandCollider;

	[SerializeField]
	GameObject mHoldBox;

	bool mHolding = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		mPlayerAnimation.GetComponent<Rigidbody>().AddForce(Vector3.down * 2.0f, ForceMode.Acceleration);   //重力加速度

		if (Input.GetKey(KeyCode.D)) {
			if(mHolding == false) {
				mPlayerAnimation.StartWalk();
			}
			else {
				mPlayerAnimation.StartHoldWalk();
			}
			mPlayerAnimation.SetSpeed(0.5f);
		}
		else if (Input.GetKey(KeyCode.F)) {
			if (mHolding == false) {
				mPlayerAnimation.StartWalk();
			}
			else {
				mPlayerAnimation.StartHoldWalk();
			}
			mPlayerAnimation.SetSpeed(1.0f);
		}
		else if (Input.GetKey(KeyCode.S)) {
			if (mHolding == false) {
				mPlayerAnimation.StartStandBy();
			}
			else {
				mPlayerAnimation.StartHoldStandBy();
			}
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			if (mHolding == false) {
				mPlayerAnimation.StartJump();
			}
			else {
				mPlayerAnimation.StartHoldJump();
			}
			mPlayerAnimation.GetComponent<Rigidbody>().AddForce(Vector3.up * 4.0f, ForceMode.VelocityChange);
		}

		if (IsLand()) {
			if (mHolding == false) {
				mPlayerAnimation.StartLand();
			}
			else {
				mPlayerAnimation.StartHoldLand();
			}
		}

		if(mHolding == false) {
			if (Input.GetKeyDown(KeyCode.LeftShift)) {
				mPlayerAnimation.StartCatch(mHoldBox);
			}
			if(mPlayerAnimation.IsCatching()) {
				if(mPlayerAnimation.CompleteCatch()) {
					mHolding = true;
					mPlayerAnimation.ExitCatch();
				}
			}
		}
		else {
			if (Input.GetKeyDown(KeyCode.LeftShift)) {
				mPlayerAnimation.StartRelease();
			}
			if (mPlayerAnimation.IsReleasing()) {
				if (mPlayerAnimation.CompleteRelease()) {
					mHolding = false;
					mPlayerAnimation.ExitRelease();
				}
			}
		}
		
		if(mHolding || mPlayerAnimation.IsReleasing() || mPlayerAnimation.IsCatching()) {
			mHoldBox.transform.position = mPlayerAnimation.GetBoxPosition();
		}
	}

	bool IsLand() {
		LayerMask lLayerMask = LayerMask.GetMask(new string[] { "Stage" });
		return Physics.OverlapBox(mLandCollider.transform.position, mLandCollider.transform.lossyScale / 2.0f, mLandCollider.transform.rotation, lLayerMask).Length > 0;
	}
}
