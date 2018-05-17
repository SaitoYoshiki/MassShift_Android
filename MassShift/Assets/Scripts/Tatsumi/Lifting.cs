using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour {
	enum LiftState {
		invalid,
		standby,
		liftUp,
		lifting,
		liftDown,
	}

	[SerializeField] Transform liftPoint = null;
	[SerializeField] GameObject liftObj = null;
	[SerializeField] LiftState st;

	// Use this for initialization
	void Start () {
		if (liftPoint == null) {
			Debug.LogError("LiftPointが設定されていません。");
			enabled = false;
		}
	}

	// Update is called once per frame
	void Update() {
		Lifting();
	}

	public void LiftUp(GameObject _obj) {
		if(st == LiftState.standby) {
			
		}
	}
	void Lifting() {
		switch (st) {
		case LiftState.liftUp:

			break;
		case LiftState.liftDown:

			break;
		case LiftState.lifting:

			break;
		default:
			break;
		}
	}
	public void LiftDown() {

	}
}
