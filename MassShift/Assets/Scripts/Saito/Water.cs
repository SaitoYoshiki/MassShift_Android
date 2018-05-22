using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

	// Use this for initialization
	void Start () {

		//シェーダーの波の大きさの設定
		mModel.GetComponentInChildren<Renderer>().material.SetFloat("_Amp", 1.0f / mHeight * mAmp);
		mModel.GetComponentInChildren<Renderer>().material.SetFloat("_Cycle", 1.0f / mWidth * mCycle);
		mModel.GetComponentInChildren<Renderer>().material.SetFloat("_Hz", 1.0f * mSpeed);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[SerializeField, Tooltip("横の大きさ")]
	int mWidth;

	[SerializeField, Tooltip("縦の大きさ")]
	int mHeight;

	[SerializeField, EditOnPrefab, Tooltip("波の間隔")]
	float mCycle = 1.0f;

	[SerializeField, EditOnPrefab, Tooltip("波の大きさ")]
	float mAmp = 0.01f;

	[SerializeField, EditOnPrefab, Tooltip("波の速さ")]
	float mSpeed = 60.0f;

	[SerializeField]
	GameObject mModel;

	[SerializeField]
	GameObject mWaterArea;

#if UNITY_EDITOR

	[ContextMenu("Resize")]
	void Resize() {

		mModel.transform.localScale = new Vector3(mWidth, mHeight, 1.0f);
		mModel.transform.localPosition = new Vector3(mWidth / 2.0f, mHeight / 2.0f, 0.0f);

		mWaterArea.transform.localScale = new Vector3(mWidth, mHeight - 0.5f, 1.0f);
		mWaterArea.transform.localPosition = new Vector3(mWidth / 2.0f, mHeight / 2.0f - 0.25f, 0.0f);
	}

	private void OnValidate() {
		if (this == null) return;
		if (EditorUtility.IsPrefab(gameObject)) return;
		Resize();
	}

#endif
}
