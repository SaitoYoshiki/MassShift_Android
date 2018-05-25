using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MassShiftFailed))]
public class MassShiftFailedAnimation : MonoBehaviour {

	[SerializeField]
	Animator mAnimator;

	[SerializeField]
	string mAnimationName;

	// Use this for initialization
	void Awake () {
		GetComponent<MassShiftFailed>().OnMassShiftFailed += PlayAnimation;
	}
	
	void PlayAnimation() {
		mAnimator.PlayInFixedTime(mAnimationName, 0, 0.0f);
	}
}
