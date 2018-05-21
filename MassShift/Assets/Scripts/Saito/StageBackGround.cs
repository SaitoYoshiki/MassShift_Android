using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBackGround : MonoBehaviour {

	[SerializeField, Tooltip("横方向の敷き詰め数")]
	int mWidth;

	[SerializeField, Tooltip("縦方向の敷き詰め数")]
	int mHeight;

	[SerializeField, EditOnPrefab, Tooltip("モデルを入れる場所"), Space(16)]
	GameObject mModel;

	[SerializeField, EditOnPrefab, Tooltip("敷き詰めるモデル")]
	GameObject mModelPrefab;

	[SerializeField, EditOnPrefab, Tooltip("横方向の敷き詰める間隔")]
	float mXInterval;

	[SerializeField, EditOnPrefab, Tooltip("縦方向の敷き詰める間隔")]
	float mYInterval;

	[SerializeField, EditOnPrefab, Tooltip("モデルを拡大する割合")]
	float mModelScale = 1.0f;

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}


#if UNITY_EDITOR

	//背景のサイズ変更
	void ResizeBackGround() {

		//現在のモデルの削除
		for (int i = mModel.transform.childCount - 1; i >= 0; i--) {
			if (EditorUtility.IsInPrefab(mModel.transform.GetChild(i).gameObject, EditorUtility.GetPrefab(gameObject))) continue;
			EditorUtility.DestroyGameObject(mModel.transform.GetChild(i).gameObject);
		}

		//モデルの配置
		for (int i = 0; i < mHeight; i++) {
			for (int j = 0; j < mWidth; j++) {
				float lXIndex = j - (mWidth - 1) / 2.0f;
				float lYIndex = i - (mHeight - 1) / 2.0f;
				GameObject lGameObject = EditorUtility.InstantiatePrefab(mModelPrefab, mModel);
				lGameObject.transform.localPosition = new Vector3(lXIndex * mXInterval * mModelScale, lYIndex * mYInterval * mModelScale, 0.0f);
				lGameObject.transform.localScale = new Vector3(mModelScale, mModelScale, 1.0f);
			}
		}

	}

	[ContextMenu("Resize")]
	void Resize() {
		if (this == null) return;
		if (EditorUtility.IsPrefab(gameObject)) return;
		ResizeBackGround();
	}

	private void OnValidate()
	{
		//UnityEditor.EditorApplication.delayCall += Resize;
	}

#endif
}
