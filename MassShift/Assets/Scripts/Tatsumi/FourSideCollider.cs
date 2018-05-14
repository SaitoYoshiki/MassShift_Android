using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourSideCollider : MonoBehaviour {
	[SerializeField] Transform topCol = null;
	public Transform TopCol {
		get {
			return topCol;
		}
	}
	[SerializeField] Transform bottomCol = null;
	public Transform BottomCol {
		get {
			return bottomCol;
		}
	}
	[SerializeField] Transform leftCol = null;
	public Transform LeftCol {
		get {
			return leftCol;
		}
	}
	[SerializeField] Transform rightCol = null;
	public Transform RightCol {
		get {
			return rightCol;
		}
	}

	public List<Transform> colList {
		get {
			List<Transform> ret = new List<Transform>();
			ret.Add(TopCol);
			ret.Add(BottomCol);
			ret.Add(LeftCol);
			ret.Add(RightCol);
			return ret;
		}
	}

	// Use this for initialization
	void Start() {
		if (TopCol == null) {
			Debug.LogError("TopColがnullです。");
		}
		if (BottomCol == null) {
			Debug.LogError("BottomColがnullです。");
		}
		if (LeftCol == null) {
			Debug.LogError("LeftColがnullです。");
		}
		if (RightCol == null) {
			Debug.LogError("RightColがnullです。");
		}
	}

	// Update is called once per frame
	//	void Update () {}
}
