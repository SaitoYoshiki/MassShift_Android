using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Landing)), RequireComponent(typeof(WeightManager))]
public class LandImpact : MonoBehaviour {

	public delegate void OnLandEvent(WeightManager.Weight aWeight);

	public event OnLandEvent OnLand;

	[SerializeField, Tooltip("この距離以上を落下すると、落下演出が起きる"), EditOnPrefab]
	float mImpactDistance = 1.0f;


	Vector3 mBeforePosition;    //前回の位置を保存

	[SerializeField, Disable]
	Vector3 mHighestPosition;	//落下時の、最高地点を保存

	bool mBeforeLanding = false;    //前のフレームで設置していたか

	//コンポーネントのキャッシュ
	Landing mLanding;
	WeightManager mWeightManager;


	// Use this for initialization
	void Start () {
		mLanding = GetComponent<Landing>();
		mWeightManager = GetComponent<WeightManager>();

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
			if (mHighestPosition.y - transform.position.y >= mImpactDistance) {
				OnLand(mWeightManager.WeightLv);    //インパクトのイベントを呼び出す
				mHighestPosition = transform.position;	//最高地点を更新
			}
		}
		mBeforeLanding = lLanding;


		//最高地点の更新
		//

		//上向きに進んでいたら
		if (transform.position.y > mBeforePosition.y) {
			mHighestPosition = transform.position;	//最高地点を更新
		}
		mBeforePosition = transform.position;
		
	}
}
