using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFloor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


#if UNITY_EDITOR

	//床のサイズ変更
	void ResizeFloor() {

		//現在のモデルの削除
		for (int i = mFloorModel.transform.childCount - 1; i >= 0; i--) {
			EditorUtility.DestroyGameObject(mFloorModel.transform.GetChild(i).gameObject);
		}

		//モデルの配置
		
		//左端
		GameObject lLeft = EditorUtility.InstantiatePrefab(mFloorLeftPrefab, mFloorModel);
		lLeft.transform.localPosition = Vector3.left * (float)(mWidth - 1) / 2;

		//真ん中
		for (int i = 1; i < mWidth - 1; i++) {
			float lIndexFromMiddle =  i - (float)(mWidth - 1) / 2;
			GameObject lMiddle = EditorUtility.InstantiatePrefab(mFloorMiddlePrefab, mFloorModel);
			lMiddle.transform.localPosition = lIndexFromMiddle * Vector3.right;
		}

		//右端
		GameObject lRight = EditorUtility.InstantiatePrefab(mFloorRightPrefab, mFloorModel);
		lRight.transform.localPosition = Vector3.right * (float)(mWidth - 1) / 2;


		//コライダーの大きさ変更
		mFloorCollider.transform.localScale = new Vector3(mWidth, 1.0f, 1.0f);
	}

	//ベルトのサイズ変更
	void ResizeBelt() {

	}

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


	[SerializeField, EditOnPrefab, Tooltip("床のコライダー"), Space(16)]
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
