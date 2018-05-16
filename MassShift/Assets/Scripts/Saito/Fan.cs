using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Fan : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		UpdateRotate();
		UpdateWindHitList();
		ApplyWindMove();
	}

	//モデルの回転処理
	void UpdateRotate() {
		mFanModel.transform.localRotation *= Quaternion.Euler(0.0f, 0.0f, Time.deltaTime * 360.0f / 1.0f);
	}

	//風に当たっているオブジェクトのリストを取得
	void UpdateWindHitList() {
		mWindHitList = GetWindHitList();
	}

	//風に当たっているオブジェクトを動かす
	void ApplyWindMove() {
		//TODO
	}

	List<GameObject> GetWindHitList() {

		var lBase = new List<HitData>();
		foreach(var c in mHitColliderList) {
			List<HitData> tHit = GetHitListEachCollider(c);
			MergeHitDataList(lBase, tHit);
		}

		var lHit = lBase.OrderBy(x => x.mHitDistance);

		var lRes = new List<GameObject>();

		foreach(var h in lHit) {
			//ステージに達するまで風は適用される
			if (h.mGameObject.layer == LayerMask.NameToLayer("Stage")) {
				break;
			}
			if(h.mHitTimes < 2) {
				continue;
			}
			lRes.Add(h.mGameObject);
		}

		return lRes;
	}

	List<HitData> GetHitListEachCollider(GameObject aCollider) {
		LayerMask l = LayerMask.GetMask(new string[] { "Player", "Box", "Stage" });
		var rc = Physics.BoxCastAll(aCollider.transform.position, aCollider.transform.lossyScale / 2.0f, GetDirectionVector(mDirection), aCollider.transform.rotation, 100.0f, l);
		return rc.Select(x => new HitData() { mGameObject = x.collider.gameObject, mHitTimes = 1, mHitDistance = x.distance }).ToList();
	}

	void MergeHitDataList(List<HitData> aBase, List<HitData> aAdd) {
		foreach(var a in aAdd) {
			bool lAlreadyExist = false;
			foreach (var b in aBase) {
				//追加するオブジェクトが、既に別のコライダーでヒット済みなら、ヒット数を増やす
				if(a.mGameObject == b.mGameObject) {
					lAlreadyExist = true;
					b.mHitTimes += 1;
					b.mHitDistance = Mathf.Min(a.mHitDistance, b.mHitDistance);
					break;
				}
			}
			//別のコライダーで当たっていなかったら、新規追加
			if(lAlreadyExist == false) {
				aBase.Add(a);
			}
		}
	}

	class HitData {
		public GameObject mGameObject;
		public int mHitTimes;
		public float mHitDistance;
	}

	enum CDirection {
		cNone,
		cLeft,
		cRight
	}

	Vector3 GetDirectionVector(CDirection aDirection) {
		switch (aDirection) {
			case CDirection.cLeft:
				return Vector3.left;
			case CDirection.cRight:
				return Vector3.right;
		}
		Debug.LogError("DirectionがNoneです", this);
		return Vector3.zero;
	}


#if UNITY_EDITOR

	//風の向きに応じてモデルを再配置
	void ReplaceModel() {

		//初期値から変わっていないのでエラー
		if (mDirection == CDirection.cNone) {
			Debug.LogError("DirectionがNoneです", this);
			return;
		}

		mFanModel.transform.rotation = Quaternion.Euler(0.0f, GetDirectionVector(mDirection).x * -30.0f, 0.0f);

		foreach(var c in mHitColliderList) {
			Vector3 lNewPos = c.gameObject.transform.localPosition;
			lNewPos.x = Mathf.Abs(lNewPos.x) * GetDirectionVector(mDirection).x;
			c.gameObject.transform.localPosition = lNewPos;
		}
	}

	[ContextMenu("Replace")]
	void Replace() {
		if (this == null) return;
		if (EditorUtility.IsPrefab(gameObject)) return;
		ReplaceModel();
	}

	private void OnValidate() {
		UnityEditor.EditorApplication.delayCall += Replace;
	}

#endif

	[SerializeField,Disable]
	List<GameObject> mWindHitList;	//風にヒットしたオブジェクト


	[SerializeField, Tooltip("風が吹く方向")]
	CDirection mDirection;

	[SerializeField, EditOnPrefab, Tooltip("回転するファンのモデル")]
	GameObject mFanModel;

	[SerializeField, EditOnPrefab, Tooltip("風の当たり判定のコライダー")]
	List<GameObject> mHitColliderList;
}
