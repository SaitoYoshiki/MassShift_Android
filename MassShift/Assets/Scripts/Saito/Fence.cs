using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fence : MonoBehaviour
{

	enum CDirection {
		cNone,
		cUp,
		cRight,
	}

	[SerializeField, Tooltip("向き")]
	CDirection mDirection;

	[SerializeField, Tooltip("長さ")]
	int mLength;

	[SerializeField, EditOnPrefab, PrefabOnly, Tooltip("モデル")]
	GameObject mModelPrefab;

	[SerializeField, EditOnPrefab, Tooltip("モデルの親となるオブジェクト")]
	GameObject mModel;

	[SerializeField, EditOnPrefab, Tooltip("コライダー")]
	GameObject mCollider;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}


#if UNITY_EDITOR

	[ContextMenu("Resize")]
	void Resize()
	{
		if (this == null) return;
		if (EditorUtility.IsPrefab(gameObject)) return;

		if (!(mLength >= 2)) {
			Debug.LogError("Length is small", this);
			return;
		}

		mCollider.transform.localScale = new Vector3(mLength, 1.0f, 1.0f);
		mCollider.transform.localPosition = new Vector3(mLength / 2.0f, 0.0f, 0.0f);

		//現在のモデルの削除
		for (int i = mModel.transform.childCount - 1; i >= 0; i--) {
			if (EditorUtility.IsInPrefab(mModel.transform.GetChild(i).gameObject, EditorUtility.GetPrefab(gameObject))) continue;
			EditorUtility.DestroyGameObject(mModel.transform.GetChild(i).gameObject);
		}

		for (int i = 0; i < mLength; i++) {
			var mi = EditorUtility.InstantiatePrefab(mModelPrefab, mModel);
			mi.transform.localPosition = GetModelPosition(i);
		}

		switch (mDirection) {
			case CDirection.cUp:
				transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
				break;
			case CDirection.cRight:
				transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
				break;
		}
	}


	private void OnValidate()
	{
		//EditorApplication.delayCall += () => Resize();
	}

	static Vector3 GetModelPosition(int aIndex)
	{
		return new Vector3(aIndex, 0.0f, 0.0f);
	}

#endif
}
