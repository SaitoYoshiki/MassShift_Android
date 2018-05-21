using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassShiftLine : MonoBehaviour {

	[SerializeField, Tooltip("矢印のモデルを配置する親")]
	GameObject mModelParent;

	[SerializeField, Tooltip("矢印のモデル"), PrefabOnly, EditOnPrefab]
	GameObject mModelPrefab;


	[SerializeField, Tooltip("モデルを配置する間隔"), EditOnPrefab, Space(16)]
	float mInterval;

	[SerializeField, Tooltip("点線が動く速度"), EditOnPrefab]
	float mOffsetSpeed;

	[SerializeField, Tooltip("矢印を出し始めたりする位置"), EditOnPrefab]
	float mStartOffset;


	Vector3 mFrom;	//線の開始位置
	Vector3 mTo;	//線の終了位置

	float mOffset = 0.0f;  //矢印を配置するオフセット



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	

	//表示の線の色を変える
	//
	public void ChangeColor(Color aColor) {
		for (int i = mModelParent.transform.childCount - 1; i >= 0; i--) {
			foreach (var r in mModelParent.transform.GetChild(i).GetComponentsInChildren<Renderer>()) {
				r.material.SetColor("_EmissionColor", aColor);
			}
		}
	}

	//表示の線の矢印を移動させる
	//
	public void UpdatePosition() {

		//オフセットを増やすことで、矢印を移動させる
		mOffset += Time.deltaTime * mOffsetSpeed;
		mOffset = mOffset % (mInterval);

		//モデルの再配置
		ReplaceModel();
	}

	//表示の線の開始位置と終了位置を設定
	//
	public void SetLinePosition(Vector3 aFrom, Vector3 aTo) {
		mFrom = aFrom;
		mTo = aTo;
		ReplaceModel();
	}

	//矢印のモデルを、現在のオフセット値などに応じて再配置
	//
	void ReplaceModel() {

		//方向の取得
		Vector3 lDir = mTo - mFrom;

		//必要なモデルの数の計算
		int lModelNum = 0;
		for (int i = 0; ; i++) {
			float lNowDist = i * mInterval + mOffset + mStartOffset;
			if (lNowDist >= lDir.magnitude - mStartOffset) {
				lModelNum = i;
				break;
			}
		}

		
		//もし必要な数が、現在の数よりも少ないなら
		if(lModelNum < mModelParent.transform.childCount) {
			//要らない分を消す
			for (int i = mModelParent.transform.childCount - 1; i >= lModelNum; i--) {
				Destroy(mModelParent.transform.GetChild(i).gameObject);
			}
		}
		//現在の数よりも多いなら
		else {
			//必要な分を増やす
			for (int i = mModelParent.transform.childCount; i < lModelNum; i++) {
				Instantiate(mModelPrefab, mModelParent.transform);
			}
		}


		//矢印を移動する
		for (int i = 0; ; i++) {
			float lNowDist = i * mInterval + mOffset + mStartOffset;
			if (lNowDist >= lDir.magnitude - mStartOffset) {
				break;
			}

			GameObject g = mModelParent.transform.GetChild(i).gameObject;

			g.transform.position = mFrom + lDir.normalized * lNowDist;
			g.transform.rotation = Quaternion.FromToRotation(Vector3.right, lDir);
		}
	}

}
