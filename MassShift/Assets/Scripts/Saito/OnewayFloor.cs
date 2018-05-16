using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnewayFloor : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		UpdateInObject();
	}


	void UpdateInObject() {
		mInObjectList = GetInObject();
	}

	List<GameObject> GetInObject() {

		var l = new List<GameObject>();

		LayerMask lMask = LayerMask.GetMask(new string[] { "Player", "Box" });
		var rc = Physics.OverlapBox(mFloorCollider.transform.position, mFloorCollider.transform.lossyScale / 2.0f, mFloorCollider.transform.rotation, lMask);

		foreach (var r in rc) {
			l.Add(r.gameObject);
		}

		return l;
	}

	[SerializeField, Disable]
	List<GameObject> mInObjectList = new List<GameObject>();	//めり込んでいるオブジェクト


	enum CDirection {
		cNone,	//無効値
		cUp,	//上方向にすり抜けられる
		cDown	//下方向にすり抜けられる
	}
	[SerializeField, Tooltip("すり抜けられる方向")]
	CDirection mDirection;

	//その方向にすり抜けられるか
	public bool IsThrough(Vector3 aVec, GameObject aGameObject) {
		//そのオブジェクトがすり抜け床にめり込んでいたら、自由に動ける
		if (mInObjectList.Contains(aGameObject)) {
			return true;
		}
		return IsThrough(mDirection, aVec);
	}

	public bool IsThrough(Vector3 aVec) {
		return IsThrough(mDirection, aVec);
	}

	static bool IsThrough(CDirection aDirection, Vector3 aVec) {
		return Vector3.Dot(GetDirectionVector(aDirection), aVec) > 0.0f;
	}

	static Vector3 GetDirectionVector(CDirection aDirection) {
		switch(aDirection) {
			case CDirection.cUp:
				return Vector3.up;
			case CDirection.cDown:
				return Vector3.down;
		}
		return Vector3.zero;
	}


#if UNITY_EDITOR

	//床のサイズ変更
	void ResizeFloor() {

		//初期値から変わっていないのでエラー
		if(mDirection == CDirection.cNone) {
			Debug.LogError("DirectionがNoneです", this);
			return;
		}

		//現在のモデルの削除
		for (int i = mFloorModel.transform.childCount - 1; i >= 0; i--) {
			EditorUtility.DestroyGameObject(mFloorModel.transform.GetChild(i).gameObject);
		}

		//モデルの配置

		//左端
		GameObject lLeft = EditorUtility.InstantiatePrefab(mFloorLeftPrefab, mFloorModel);
		lLeft.transform.localPosition = Vector3.left * ((mWidth - 3) / 2 + 0.5f);

		//真ん中
		for (int i = 0; i < mWidth - 2; i++) {
			int lIndexFromCenter = i - (mWidth - 3) / 2;
			GameObject lMiddle = EditorUtility.InstantiatePrefab(mFloorMiddlePrefab, mFloorModel);
			lMiddle.transform.localPosition = Vector3.right * lIndexFromCenter;
		}

		//右端
		GameObject lRight = EditorUtility.InstantiatePrefab(mFloorRightPrefab, mFloorModel);
		lRight.transform.localPosition = Vector3.right * ((mWidth - 3) / 2 + 0.5f);


		//コライダーの大きさ変更
		mFloorCollider.transform.localScale = new Vector3(mWidth, 1.0f, 1.0f);
	}

	[ContextMenu("Resize")]
	void Resize() {
		if (this == null) return;
		if (EditorUtility.IsPrefab(gameObject)) return;
		ResizeFloor();
	}

	private void OnValidate() {
		UnityEditor.EditorApplication.delayCall += Resize;
	}

#endif

	[SerializeField, Tooltip("床の幅")]
	int mWidth;


	[SerializeField, EditOnPrefab,  Tooltip("床のコライダー"), Space(16)]
	GameObject mFloorCollider;

	[SerializeField, EditOnPrefab, Tooltip("床の全てのモデルの親")]
	GameObject mFloorModel;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("床の左端のモデル")]
	GameObject mFloorLeftPrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("床の真ん中モデル")]
	GameObject mFloorMiddlePrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("床の右端のモデル")]
	GameObject mFloorRightPrefab;
}
