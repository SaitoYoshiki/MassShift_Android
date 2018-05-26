using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MassShiftFailed))]
public class MassShiftFailedAnimationMoveFloor : MonoBehaviour {

	[SerializeField]
	Animator mAnimator;

	[SerializeField]
	GameObject mFloorModel;

	[SerializeField]
	string mAnimationName;

	// Use this for initialization
	void Awake () {
		GetComponent<MassShiftFailed>().OnMassShiftFailed += PlayAnimation;
	}

	private void Update() {
		mFloorModel.transform.localRotation = Quaternion.Slerp(Quaternion.identity, mAnimator.transform.rotation, 1.0f / GetComponentInParent<MoveFloor>().Width);
	}

	void PlayAnimation() {
		mAnimator.PlayInFixedTime(mAnimationName, 0, 0.0f);
	}
}
