using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	// Use this for initialization
	void Awake() {

		mLampList = new List<GameObject>();
		for (int i = 0; i < mButtonList.Count; i++) {
			mLampList.Add(mLampModel.transform.GetChild(i).gameObject);
		}

		TurnLamp();

		//エリアによって、マテリアルの変更
		switch (Area.GetAreaNumber()) {
			case 0:
				Utility.ChangeMaterial(mModel, mArea1Material, mBackGroundMaterial);
				break;
			case 1:
				Utility.ChangeMaterial(mModel, mArea1Material, mBackGroundMaterial);
				break;
			case 2:
				Utility.ChangeMaterial(mModel, mArea2Material, mBackGroundMaterial);
				break;
			case 3:
				Utility.ChangeMaterial(mModel, mArea3Material, mBackGroundMaterial);
				break;
			default:
				//Utility.ChangeMaterial(mModel, null, mBackGroundMaterial);
				break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateOpenRate();
		ModelAnimation();

		UpdateLamp();
		UpdateInPlayer();
	}

	//開いている割合を更新
	void UpdateOpenRate() {

		if (IsAllButtonOn) {
			mOpenRate += 1.0f / mOpenTakeTime * Time.deltaTime;
		}
		else {
			mOpenRate -= 1.0f / mCloseTakeTime * Time.deltaTime;
		}
		mOpenRate = Mathf.Clamp01(mOpenRate);
	}

	//ゴールのアニメーションの再生
	void ModelAnimation() {

		//このフレームで開いたなら
		if(mBeforeAllButtonOn == false) {
			if(IsAllButtonOn == true) {
				SetAnimation(true);
			}
		}

		//このフレームで閉まったなら
		if (mBeforeAllButtonOn == true) {
			if (IsAllButtonOn == false) {
				SetAnimation(false);
			}
		}

		mBeforeAllButtonOn = IsAllButtonOn;
	}

	//ゴールエリアに含まれているプレイヤーを更新
	void UpdateInPlayer() {
		mInPlayerList = GetInPlayer();
	}

	//ランプを点灯させる
	void TurnLamp() {

		for (int i = 0; i < ButtonOnCount(); i++) {
			Utility.ChangeMaterialColor(mLampList[i], mLampMaterial, "_EmissionColor", mLampOnEmission * mLampOnEmissionPower);
		}

		for (int i = ButtonOnCount(); i < mButtonList.Count; i++) {
			Utility.ChangeMaterialColor(mLampList[i], mLampMaterial, "_EmissionColor", mLampOffEmission * mLampOffEmissionPower);
		}
	}

	void UpdateLamp()
	{
		int lButtonOnCount = ButtonOnCount();

		//ボタンの点灯数に変化があったら
		if (mBeforeButtonOnCount != lButtonOnCount) {
			TurnLamp();
		}

		mBeforeButtonOnCount = lButtonOnCount;
	}


	//扉が開いたり、閉まったりするアニメーションを再生する
	void SetAnimation(bool lOpen) {

		Animator a = mModel.GetComponentInChildren<Animator>();
		float lAnimLength = a.GetCurrentAnimatorClipInfo(0)[0].clip.length;

		a.Play("Open", 0, mOpenRate);

		if (lOpen) {
			a.SetFloat("Speed", 1.0f / mOpenTakeTime * lAnimLength);
		}
		else {
			a.SetFloat("Speed", -1.0f / mCloseTakeTime * lAnimLength);
		}
	}

	//ボタンが全てオンかどうか
	public bool IsAllButtonOn {
		get {
			if (mTotalButtonOn_Debug) return true;
			if (ButtonOnCount() == ButtonCount()) {
				return true;
			}
			return false;
		}
	}


	List<Player> GetInPlayer() {
		var pl = new List<Player>();
		foreach(var p in FindObjectsOfType<Player>())
		{
			if(IsCollisionComplete(mGoalTrigger.GetComponent<BoxCollider>(), p.GetComponent<Collider>())) {
				pl.Add(p);
			}
		}
		return pl;
	}

	public bool IsInPlayer(Player p) {
		return mInPlayerList.Contains(p);
	}

	//オンになっているボタンの数を取得する
	int ButtonOnCount() {

		int lTotal = 0;

		foreach (var g in mButtonList) {
			if (g == null) {
				Debug.LogWarning("Goal's Button is null");
				return -1;
			}
			if (g.IsButtonOn == true) {
				lTotal += 1;
			}
		}

		return lTotal;
	}

	//オンにする必要のあるボタンの数を取得する
	int ButtonCount() {
		return mButtonList.Count;
	}

	//ゴールが完全に開いているか
	bool IsOpen {
		get { return mOpenRate >= 1.0f; }
	}


	//コライダーが完全にエリアに入っているかどうか
	static bool IsCollisionComplete(BoxCollider aArea, Collider aObject) {

		Vector3 dir;
		float dis;

		bool res = Physics.ComputePenetration(aArea, aArea.bounds.center, aArea.transform.rotation, aObject, aObject.transform.position, aObject.transform.rotation, out dir, out dis);
		if (res == false) return false;

		res = Physics.ComputePenetration(aArea, GetPosition(aArea, Vector3.up), aArea.transform.rotation, aObject, aObject.transform.position, aObject.transform.rotation, out dir, out dis);
		if (res == true) return false;

		res = Physics.ComputePenetration(aArea, GetPosition(aArea, Vector3.down), aArea.transform.rotation, aObject, aObject.transform.position, aObject.transform.rotation, out dir, out dis);
		if (res == true) return false;

		res = Physics.ComputePenetration(aArea, GetPosition(aArea, Vector3.right), aArea.transform.rotation, aObject, aObject.transform.position, aObject.transform.rotation, out dir, out dis);
		if (res == true) return false;

		res = Physics.ComputePenetration(aArea, GetPosition(aArea, Vector3.left), aArea.transform.rotation, aObject, aObject.transform.position, aObject.transform.rotation, out dir, out dis);
		if (res == true) return false;

		//ここから先はZ方向のチェックなので、とりあえずは必要ない
		res = Physics.ComputePenetration(aArea, GetPosition(aArea, Vector3.forward), aArea.transform.rotation, aObject, aObject.transform.position, aObject.transform.rotation, out dir, out dis);
		//if (res == true) return false;

		res = Physics.ComputePenetration(aArea, GetPosition(aArea, Vector3.back), aArea.transform.rotation, aObject, aObject.transform.position, aObject.transform.rotation, out dir, out dis);
		//if (res == true) return false;

		return true;
	}

	static Vector3 GetPosition(BoxCollider aCollider, Vector3 aOffset) {
		Vector3 aPositionOffset = aCollider.transform.rotation * new Vector3(aCollider.bounds.size.x * aOffset.x, aCollider.bounds.size.y * aOffset.y, aCollider.bounds.size.z * aOffset.z);
		return aCollider.transform.position + aPositionOffset;
	}

#if UNITY_EDITOR

	[ContextMenu("Resize")]
	public void Resize() {

		if (this == null) return;
		if (EditorUtility.IsPrefab(gameObject)) return;
		if (UnityEditor.EditorApplication.isPlaying) return;

		//現在のモデルの削除
		for (int i = mLampModel.transform.childCount - 1; i >= 0; i--) {
			if (EditorUtility.IsInPrefab(mLampModel.transform.GetChild(i).gameObject, EditorUtility.GetPrefab(gameObject))) continue;
			EditorUtility.DestroyGameObject(mLampModel.transform.GetChild(i).gameObject);
		}

		//モデルの配置

		//ランプ
		for (int i = 0; i < mButtonList.Count; i++) {
			GameObject lLamp = EditorUtility.InstantiatePrefab(mLampPrefab, mLampModel);
			lLamp.transform.localPosition = mLampBasePosition + Vector3.down * mLampInterval * i;
		}

		//土台
		Vector3 lBase = mLampBasePosition;

		//上端
		GameObject lTop = EditorUtility.InstantiatePrefab(mLampTopPrefab, mLampModel);
		lTop.transform.localPosition = lBase;

		//真ん中
		for (int i = 0; i < mButtonList.Count - 1; i++) {
			lBase += Vector3.down * mLampInterval;
			GameObject lMid = EditorUtility.InstantiatePrefab(mLampMidPrefab, mLampModel);
			lMid.transform.localPosition = lBase - Vector3.down * mLampInterval * 0.5f;
		}

		//下端
		GameObject lBottom = EditorUtility.InstantiatePrefab(mLampBottomPrefab, mLampModel);
		lBottom.transform.localPosition = lBase;
	}


	private void OnValidate() {
		//UnityEditor.EditorApplication.delayCall += Resize;
	}

#endif

	[SerializeField]
	List<Button> mButtonList;

	[SerializeField]
	bool mTotalButtonOn_Debug;

	bool mBeforeAllButtonOn = false;
	int mBeforeButtonOnCount = 0;

	float mOpenRate = 0.0f;	//扉が開いている割合

	[SerializeField, Tooltip("扉が開くのに何秒かかるか")]
	float mOpenTakeTime = 1.0f;

	[SerializeField, Tooltip("扉が閉まるのに何秒かかるか")]
	float mCloseTakeTime = 1.0f;

	[SerializeField, Disable]
	List<Player> mInPlayerList = new List<Player>();

	[SerializeField, EditOnPrefab, Tooltip("背景の変更対象のマテリアル")]
	Material mBackGroundMaterial;

	[SerializeField, EditOnPrefab, Tooltip("エリア1の背景のマテリアル")]
	Material mArea1Material;

	[SerializeField, EditOnPrefab, Tooltip("エリア2の背景のマテリアル")]
	Material mArea2Material;

	[SerializeField, EditOnPrefab, Tooltip("エリア3の背景のマテリアル")]
	Material mArea3Material;

	[SerializeField, PrefabOnly,EditOnPrefab, Tooltip("ランプの上端のモデル")]
	GameObject mLampTopPrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("ランプの真ん中のモデル")]
	GameObject mLampMidPrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("ランプの下端のモデル")]
	GameObject mLampBottomPrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("ランプのモデル")]
	GameObject mLampPrefab;

	[SerializeField, EditOnPrefab, Tooltip("ランプを配置する間隔")]
	float mLampInterval = 1.0f;

	List<GameObject> mLampList;	//ランプのインスタンス。０から順に、上から


	[SerializeField, EditOnPrefab, Tooltip("ゴールのモデル")]
	GameObject mModel;

	[SerializeField, EditOnPrefab, Tooltip("プレイヤーが完全に入っているとゴール")]
	GameObject mGoalTrigger;

	[SerializeField, EditOnPrefab, Tooltip("ランプを配置する基準となる")]
	Vector3 mLampBasePosition;

	[SerializeField, EditOnPrefab, Tooltip("ランプ全てのモデルの親")]
	GameObject mLampModel;

	[SerializeField, EditOnPrefab, Tooltip("色を変更するライトのマテリアル")]
	Material mLampMaterial;

	[SerializeField, EditOnPrefab, Tooltip("オンの時のライトのエミッション")]
	Color mLampOnEmission;

	[SerializeField, EditOnPrefab, Tooltip("オンの時のライトのエミッションの強さ")]
	float mLampOnEmissionPower = 1.0f;

	[SerializeField, EditOnPrefab, Tooltip("オフの時のライトのエミッション")]
	Color mLampOffEmission;

	[SerializeField, EditOnPrefab, Tooltip("オフの時のライトのエミッションの強さ")]
	float mLampOffEmissionPower = 1.0f;
}
