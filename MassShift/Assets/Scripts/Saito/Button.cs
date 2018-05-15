using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

	// Use this for initialization
	void Start () {
		mLedgeStartPosition = mLedge.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		UpdatePushRate();
		MoveLedge();
		
	}

	//押されている割合を更新する
	void UpdatePushRate() {

		if (IsPush) {
			mPushRate += 1.0f / mPushTakeTime * Time.deltaTime;
		}
		else {
			mPushRate -= 1.0f / mReleaseTakeTime * Time.deltaTime;
		}

		mPushRate = Mathf.Clamp01(mPushRate);
	}

	//ボタンを動かす
	void MoveLedge() {

		mLedge.transform.position = mLedgeStartPosition + (mLedgeMoveEnd.transform.position - mLedgeMoveStart.transform.position) * mPushRate;
	}


	private void OnValidate()
	{
		switch (mDirection)
		{
			case CDirection.cUp:
				transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
				break;
			case CDirection.cDown:
				transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
				break;
		}
	}


	[SerializeField, Tooltip("押されるのにかかる時間"), EditOnPrefab]
	float mPushTakeTime;

	[SerializeField, Tooltip("離されるのにかかる時間"), EditOnPrefab]
	float mReleaseTakeTime;

	enum CDirection {
		cUp,
		cDown,
	}
	[SerializeField, Tooltip("スイッチの方向")]
	CDirection mDirection;


	float mPushRate;    //現在押されている割合

	//ボタンの上にオブジェクトが乗って、押されているか
	public bool IsPush {
		get {
			if (mIsPush_Debug) return true;
			LayerMask lLayerMask = LayerMask.GetMask(new string[] { "Box", "Player"});
			Collider[] lHitColliders = Physics.OverlapBox(mWeightCheckCollider.transform.position, mWeightCheckCollider.transform.localScale / 2.0f, mWeightCheckCollider.transform.rotation, lLayerMask);
			return lHitColliders.Length > 0;
		}
	}

	[SerializeField]
	bool mIsPush_Debug;

	//ボタンが完全に押されているか
	public bool IsButtonOn {
		get { return mPushRate >= 1.0f; }
	}

	Vector3 mLedgeStartPosition;    //Ledgeの開始位置

	[SerializeField, Tooltip("Ledge"), EditOnPrefab]
	GameObject mLedge;

	[SerializeField, Tooltip("Ledgeの移動開始位置"), EditOnPrefab]
	GameObject mLedgeMoveStart;

	[SerializeField, Tooltip("Ledgeの移動終了位置"), EditOnPrefab]
	GameObject mLedgeMoveEnd;

	[SerializeField, Tooltip("押されているオブジェクトを見つけるときに使うコライダー"), EditOnPrefab]
	GameObject mWeightCheckCollider;
	
}
