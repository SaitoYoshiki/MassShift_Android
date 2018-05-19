using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBlock : MonoBehaviour {

	[SerializeField, Tooltip("幅")]
	int mWidth;

	[SerializeField, Tooltip("高さ")]
	int mHeight;

	[SerializeField, Tooltip("モデルの敷き詰めで、上に線をつけるか")]
	bool mIsUp;

	[SerializeField, Tooltip("モデルの敷き詰めで、上に線をつけるか")]
	bool mIsDown;

	[SerializeField, Tooltip("モデルの敷き詰めで、上に線をつけるか")]
	bool mIsLeft;

	[SerializeField, Tooltip("モデルの敷き詰めで、上に線をつけるか")]
	bool mIsRight;


	[SerializeField, EditOnPrefab, Space(16)]
	GameObject mModel;

	[SerializeField, EditOnPrefab]
	GameObject mHitCollider;

	[SerializeField, EditOnPrefab]
	GameObject mModelUpPrefab;

	[SerializeField, EditOnPrefab]
	GameObject mModelDownPrefab;

	[SerializeField, EditOnPrefab]
	GameObject mModelLeftPrefab;

	[SerializeField, EditOnPrefab]
	GameObject mModelRightPrefab;

	[SerializeField, EditOnPrefab]
	GameObject mModelUpLeftPrefab;

	[SerializeField, EditOnPrefab]
	GameObject mModelUpRightPrefab;

	[SerializeField, EditOnPrefab]
	GameObject mModelDownLeftPrefab;

	[SerializeField, EditOnPrefab]
	GameObject mModelDownRightPrefab;

	[SerializeField, EditOnPrefab]
	GameObject mModelNonePrefab;


	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}


#if UNITY_EDITOR

	//床のサイズ変更
	void ResizeFloor()
	{

		//現在のモデルの削除
		for (int i = mModel.transform.childCount - 1; i >= 0; i--) {
			if (EditorUtility.IsInPrefab(mModel.transform.GetChild(i).gameObject, EditorUtility.GetPrefab(gameObject))) continue;
			EditorUtility.DestroyGameObject(mModel.transform.GetChild(i).gameObject);
		}

		//モデルの配置
		for (int i = 0; i < mHeight; i++) {

			for (int j = 0; j < mWidth; j++) {

				bool lUp = false;
				bool lDown = false;
				bool lLeft = false;
				bool lRight = false; ;
				if (mIsUp && i == mHeight - 1) {
					lUp = true;
				}
				if (mIsDown && i == 0) {
					lDown = true;
				}
				if (mIsLeft && j == 0) {
					lLeft = true;
				}
				if (mIsRight && j == mWidth - 1) {
					lRight = true;
				}

				GameObject lPrefab = null;
				if (lUp && lLeft) {
					lPrefab = mModelUpLeftPrefab;
				}
				else if (lUp && lRight) {
					lPrefab = mModelUpRightPrefab;
				}
				else if (lDown && lLeft) {
					lPrefab = mModelDownLeftPrefab;
				}
				else if (lDown && lRight) {
					lPrefab = mModelDownRightPrefab;
				}
				else if (lUp) {
					lPrefab = mModelUpPrefab;
				}
				else if (lDown) {
					lPrefab = mModelDownPrefab;
				}
				else if (lLeft) {
					lPrefab = mModelLeftPrefab;
				}
				else if (lRight) {
					lPrefab = mModelRightPrefab;
				}
				else {
					lPrefab = mModelNonePrefab;
				}

				GameObject lGameObject = EditorUtility.InstantiatePrefab(lPrefab, mModel);
				lGameObject.transform.localPosition = new Vector3(j, i, 0.0f);
			}
		}

		//コライダーの大きさ変更
		mHitCollider.transform.localScale = new Vector3(mWidth, mHeight, 1.0f);
		mHitCollider.transform.localPosition = new Vector3(mWidth / 2.0f, mHeight / 2.0f, 0.0f);
	}

	[ContextMenu("Resize")]
	void Resize()
	{
		if (this == null) return;
		if (EditorUtility.IsPrefab(gameObject)) return;
		ResizeFloor();

	}

	private void OnValidate()
	{
		//UnityEditor.EditorApplication.delayCall += Resize;
	}

#endif
}
