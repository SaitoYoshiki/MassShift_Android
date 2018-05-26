using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Landing)), RequireComponent(typeof(WeightManager))]
public class LandImpact : MonoBehaviour {

	public delegate void OnLandEvent(WeightManager.Weight aWeight, bool aIsWater);
	public event OnLandEvent OnLand;

	[SerializeField, Tooltip("この距離以上を落下すると、落下演出が起きる"), EditOnPrefab]
	float mImpactDistance = 1.0f;

	Vector3 mBeforePosition;    //前回の位置を保存

	Vector3 mHighestPosition;	//落下時の、最高地点を保存

	bool mBeforeLanding = false;    //前のフレームで設置していたか
	bool mBeforeInWater = false;	//前のフレームで水の中にいたか

	//コンポーネントのキャッシュ
	Landing mLanding;
	WeightManager mWeightManager;
	WaterState mWaterState;
	MoveManager mMoveManager;


	// Use this for initialization
	void Start () {
		mLanding = GetComponent<Landing>();
		mWeightManager = GetComponent<WeightManager>();
		mWaterState = GetComponent<WaterState>();
		mMoveManager = GetComponent<MoveManager>();

		mHighestPosition = transform.position;  //最高地点を更新
		mBeforePosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		if(Time.deltaTime == 0.0f) {
			return;
		}


		//接地判定
		//
		bool lLanding = mLanding.IsLanding;

		//このフレームに接地し始めていて
		if (mBeforeLanding == false && lLanding == true) {

			//一定距離以上落ちていたら
			if (Mathf.Abs(mHighestPosition.y - transform.position.y) >= mImpactDistance) {
				OnLand(mWeightManager.WeightLv, false);    //インパクトのイベントを呼び出す
				mHighestPosition = transform.position;	//最高地点を更新
			}
		}
		mBeforeLanding = lLanding;


		//着水判定
		//
		bool lInWater = mWaterState.IsInWater;

		//このフレームに水の中にいて
		if (mBeforeInWater == false && lInWater == true) {

			//一定距離以上落ちていたら
			if (Mathf.Abs(mHighestPosition.y - transform.position.y) >= mImpactDistance) {
				OnLand(mWeightManager.WeightLv, true);    //インパクトのイベントを呼び出す
				mHighestPosition = transform.position;  //最高地点を更新
			}
		}
		mBeforeInWater = lInWater;


		//最高地点の更新
		//

		//下向きに落ちるなら
		if(mMoveManager.GravityForce < 0.0f) {
			//上向きに進んでいたら
			if (transform.position.y > mBeforePosition.y) {
				mHighestPosition = transform.position;  //最高地点を更新
			}
		}
		//上向きに落ちるなら
		else {
			//下向きに進んでいたら
			if (transform.position.y < mBeforePosition.y) {
				mHighestPosition = transform.position;  //最高地点を更新
			}
		}

		
		mBeforePosition = transform.position;
	}
}
