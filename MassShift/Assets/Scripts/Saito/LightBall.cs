using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LightBall : MonoBehaviour {

	Vector3 mFrom;
	Vector3 mTo;

	public Vector3 From {
		get { return mFrom; }
	}
	public Vector3 To {
		get { return mTo; }
	}

	public List<GameObject> mIgnoreList = new List<GameObject>();


	//Toまで到達したか
	bool mIsReached = false;
	public bool IsReached { 
		get { return mIsReached; }
	}

	//オブジェクトに当たったか
	bool mIsHit = false;
	public bool IsHit {
		get { return mIsHit; }
	}

	[EditOnPrefab, Tooltip("光の弾が進む速度")]
	public float mMoveSpeed = 20.0f;

	float mFromDistance;    //Fromから進んだ距離

	Vector3 mBeforePosition;


	[SerializeField, EditOnPrefab]
	GameObject mCollider;

	[SerializeField, EditOnPrefab]
	GameObject mModel;


	public void UpdatePoint() {
		
		mFromDistance += Time.deltaTime * mMoveSpeed;
		ReachCheck();

		mFromDistance = Mathf.Clamp(mFromDistance, 0.0f, (mFrom - mTo).magnitude);
		UpdatePosition();
		HitCheck();
	}
	public void SetPoint(Vector3 aFrom, Vector3 aTo) {
		mFrom = aFrom;
		mTo = aTo;
	}
	public void InitPoint(Vector3 aFrom, Vector3 aTo) {
		mFromDistance = 0.0f;
		mIsHit = false;
		transform.position = aFrom;
		SetPoint(aFrom, aTo);
	}
	
	void ReachCheck() {

		if ((mTo - mFrom).magnitude <= mFromDistance) {
			mIsReached = true;
		}
		else {
			mIsReached = false;
		}
	}

	void UpdatePosition() {

		Vector3 lDir = mTo - mFrom;
		transform.rotation = Quaternion.FromToRotation(Vector3.right, lDir);

		//距離を進む
		mBeforePosition = transform.position;
		transform.position = mFrom + lDir.normalized * mFromDistance;
	}

	//射線が通っているか
	public bool ThroughShotLine(Vector3 aFrom, Vector3 aTo, List<GameObject> aIgnoreList = null) {

		Vector3 lDir = (aTo - aFrom);
		if(lDir.magnitude == 0.0f) {
			lDir = Vector3.up;
		}

		LayerMask l = LayerMask.GetMask(new string[] { "Box", "Stage" });
		var rcs = Physics.SphereCastAll(aFrom, mCollider.transform.lossyScale.x / 2.0f, lDir, (aTo - aFrom).magnitude, l);
		foreach(var rc in rcs) {
			if (aIgnoreList == null) return false;
			if (aIgnoreList.Contains(rc.collider.gameObject) == false) {
				return false;
			}
		}
		return true;
	}
	public bool ThroughShotLine(Vector3 aFrom, Vector3 aTo, GameObject aIgnore) {
		return ThroughShotLine(aFrom, aTo, new GameObject[] { aIgnore }.ToList());
	}

	void HitCheck() {
		if(ThroughShotLine(mBeforePosition, transform.position, mIgnoreList)) {
			mIsHit = false;
		}
		else {
			mIsHit = true;
		}
	}

	public void PlayEffect() {
		foreach(var p in mModel.GetComponentsInChildren<ParticleSystem>()) {
			p.Play();
		}
		foreach (var p in mModel.GetComponentsInChildren<MeshRenderer>()) {
			p.enabled = true;
		}
	}
	public void StopEffect() {
		foreach (var p in mModel.GetComponentsInChildren<ParticleSystem>()) {
			p.Stop();
		}
		foreach (var p in mModel.GetComponentsInChildren<MeshRenderer>()) {
			p.enabled = false;
		}
	}
}
