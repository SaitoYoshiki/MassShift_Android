using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour {
	enum DebugState {
		disable,    // 有効化不可
		off,        // 無効
		on,         // 有効
	}

	public enum DebugMode {
		drawInfo,
		objectCreate,
		objectDelete,
		objectMove,

		noSelect,
	}

	enum DebugCmd {
		debugModeSwitch,

		drawInfo,
		objectCreate,
		objectDelete,
		objectMove,

		num,
	}

	// デバッグ用オブジェクトリスト化用クラス
	[System.Serializable]
	class DebugObject {
		[SerializeField] GameObject obj;
		public GameObject Obj {
			get {
				return obj;
			}
		}
		[SerializeField] DebugMode mode;
		public DebugMode Mode {
			get {
				return mode;
			}
		}
		public void Set(GameObject _obj, DebugMode _mode) {
			obj = _obj;
			mode = _mode;
		}
	}

	[Multiline(3), SerializeField]
	string manual = "実行中にDebugModeKeyを二度押しすると\nデバッグモードを有効化/無効化できます。\n(SttがDisable以外である必要があります。)";
	[SerializeField] DebugState stt = DebugState.off;
	DebugState Stt {
		get {
			return stt;
		}
		set {
			stt = value;
			SetActiveDebugObject();
		}
	}
	[SerializeField]
	DebugMode mode = DebugMode.drawInfo;
	DebugMode Mode {
		get {
			return mode;
		}
		set {
			mode = value;
			SetActiveDebugObject();
		}
	}
	[SerializeField] KeyCode debugModeKey = KeyCode.Tab;	// デバッグモード有効化/無効化キー
	[SerializeField] float switchTimeRange = 0.2f;			// 二度押し操作の有効時間
	[SerializeField] float timeScl = 1.0f;
	float prevTimeScl = 1.0f;
	[SerializeField] float timeSclSpd = 0.1f;
	[SerializeField] float timeSclRepeatTime = 0.5f;
	[SerializeField] float timeSclNowRepeatTime = 0.0f;
	
	// デバッグテキスト
	[SerializeField] float textDis = 30.0f;
	[SerializeField] float textCharSize = 0.3f;
	[SerializeField] Color textColor = Color.green;
	[SerializeField, Multiline(10)] string manText = "DebugManual";

	Dictionary<DebugCmd, KeyCode> debugKey = new Dictionary<DebugCmd, KeyCode>();   // 各操作のキーコード
	TextMesh textMesh = null;
	TextMesh textManMesh = null;
	float switchLimitTime = 0.0f;

	[SerializeField] List<DebugObject> debugObjList = new List<DebugObject>();   // デバッグ用オブジェクトリスト

	[SerializeField] Transform debugTargetCursorViewer = null;  // デバッグ用ターゲットオブジェクト
	[SerializeField] Transform debugTarget = null;
	Transform DebugTarget {
		get {
			if ((debugTarget == null) && (!Input.GetKey(KeyCode.LeftShift))) {
				RaycastHit hitInfo;
				Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(debugCursor.position)), out hitInfo, LayerMask.GetMask(new string[] { "Player", "Stage", "Box" }));
				if (hitInfo.collider != null) {
					return hitInfo.collider.transform;
				}
			}
			return debugTarget;
		}
		set {
			debugTarget = value;
		}
	}

	[SerializeField] Transform debugCursor = null;
	[SerializeField] GameObject debugCopyObject = null;
	[SerializeField] GameObject debugDefaultCopyObject = null;

	// デバッグモードの機能で増減したリソースの数
	Dictionary<string, int> debugResources = new Dictionary<string, int>();

	// Use this for initialization
	void Awake() {
		if (Stt == DebugState.disable) {
			enabled = false;
			return;
		}

		//	キーコード登録
		debugKey.Add(DebugCmd.debugModeSwitch, debugModeKey);
		debugKey.Add(DebugCmd.drawInfo, KeyCode.I);
		debugKey.Add(DebugCmd.objectCreate, KeyCode.C);
		debugKey.Add(DebugCmd.objectDelete, KeyCode.D);
		debugKey.Add(DebugCmd.objectMove, KeyCode.M);

		// タグの付いたオブジェクトをデバッグオブジェクトとして自動登録
		AddDebugObjcetTagObject();

		// 既存のテキストオブジェクトが存在しなければ
		if (textMesh == null) {
			// テキストオブジェクトを生成
			Transform textTransform = new GameObject("DebugText", typeof(MeshRenderer), typeof(TextMesh)).transform;
			textTransform.parent = transform;
			textMesh = textTransform.GetComponent<TextMesh>();

			// 初期モード用のテキストを設定
			UpdateDebugText();

			// 初期状態がoffならテキストを無効化
			if (Stt == DebugState.off) {
				textMesh.gameObject.SetActive(false);
			}

			// デバッグ用テキストをデバッグ用オブジェクトに設定
			AddDebugObject(textMesh.gameObject, null);
		}
		// 既存のマニュアルテキストオブジェクトが存在しなければ
		if (textManMesh == null) {
			// マニュアルテキストオブジェクトを生成
			Transform textManTransform = new GameObject("DebugManualText", typeof(MeshRenderer), typeof(TextMesh)).transform;
			textManTransform.parent = transform;
			textManMesh = textManTransform.GetComponent<TextMesh>();

			// マニュアルテキストを設定
			textManMesh.text = manText;
			textManMesh.anchor = TextAnchor.UpperRight;
			textManMesh.alignment = TextAlignment.Right;

			// 初期状態がoffならテキストを無効化
			if (Stt == DebugState.off) {
				textManMesh.gameObject.SetActive(false);
			}

			// デバッグ用マニュアルテキストをデバッグ用オブジェクトに設定
			AddDebugObject(textManMesh.gameObject, null);
		}


		// デバッグオブジェクトを開始時のデバッグモードに合わせて有効化/無効化
		SetActiveDebugObject();

		// デバッグリソースに重さの項目を追加
		AddDebugResource("Weight", 0);

		// ゲームスピードの初期設定
		prevTimeScl = timeScl;
		Time.timeScale = timeScl;
	}

	// Update is called once per frame
	void Update() {
		// デバッグモード有効化/無効化
		if (Input.GetKeyDown(debugKey[DebugCmd.debugModeSwitch])) {
			// デバッグモード有効化/無効化の有効時間内なら
			if (switchLimitTime > Time.time) {
				if (Stt == DebugState.off) {
					Stt = DebugState.on;
				} else if (Stt == DebugState.on) {
					Stt = DebugState.off;
				}
			}
			// デバッグモード有効化/無効化の有効時間外なら
			else {
				// 次の有効時間を設定
				switchLimitTime = (Time.time + switchTimeRange);
			}
		}

		// デバッグモードが有効でなければ以降の処理を行わない
		if (Stt != DebugState.on) {
			return;
		}

		// デバッグテキストの初期設定
		textMesh.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, textDis));
		textMesh.characterSize = textCharSize;
		textMesh.color = textColor;

		// デバッグマニュアルテキストの初期設定
		textManMesh.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, textDis));
		textManMesh.characterSize = textCharSize;
		textManMesh.color = textColor;

		// モード変更
		if (Input.GetKey(debugKey[DebugCmd.debugModeSwitch])) {
			for (int cnt = 0; cnt < (int)DebugCmd.num; cnt++) {
				if (Input.GetKeyDown(debugKey[(DebugCmd)cnt])) {
					Mode = (DebugMode)cnt;
					// テキストを更新
					UpdateDebugText();
				}
			}
		}

		// ゲームスピード変更
		if(timeScl != prevTimeScl) {
			Time.timeScale = timeScl;
			prevTimeScl = timeScl;

			// デバッグテキストを更新
			UpdateDebugText();
		}
		// 外部から変更があった場合
		if (timeScl != Time.timeScale) {
			// 外部の変更に合わせる
			timeScl = Time.timeScale;
			prevTimeScl = timeScl;

			// デバッグテキストを更新
			UpdateDebugText();
		}

		// カーソルを使うデバッグ操作
		DebugTargetting();
	}

	void UpdateDebugText() {
		textMesh.text = "DebugMode\n";

		// タイムスケール
		textMesh.text += "TimeScale:" + Time.timeScale + "\n";

		// 選択中オブジェクト
		textMesh.text += "TargetObject:";
		if(DebugTarget!= null) {
			textMesh.text += DebugTarget.name;

		} else {
			textMesh.text += DebugTarget;
		}
		textMesh.text += "\n";

		// コピー中オブジェクト
		textMesh.text += "CopyObject:";
		if (debugCopyObject != null) {
			textMesh.text += debugCopyObject.name;

		} else {
			textMesh.text += debugCopyObject;
			if (debugDefaultCopyObject != null) {
				textMesh.text += "(" + debugDefaultCopyObject.name + ")";
			}
		}
		textMesh.text += "\n";

		// デバッグリソース
		foreach (var debugRes in debugResources) {
			textMesh.text += debugRes.Key + ":" + (debugRes.Value > 0 ? "+" : (debugRes.Value == 0 ? " " : "")) + debugRes.Value + "\n";
		}
	}

	public void AddDebugObject(GameObject _obj, DebugMode? _mode) {
		DebugObject debugObj = new DebugObject();
		if (_mode != null) {
			debugObj.Set(_obj, _mode.Value);
		} else {
			debugObj.Set(_obj, DebugMode.noSelect);
		}
		debugObjList.Add(debugObj);
	}

	void SetActiveDebugObject() {
		foreach (var debugObj in debugObjList) {
			debugObj.Obj.SetActive((Stt == DebugState.on) && (debugObj.Mode == DebugMode.noSelect) || (debugObj.Mode == Mode));
		}
	}

	void AddDebugObjcetTagObject() {
		// タグが付いたオブジェクトを全て探す
		GameObject[] tagObjs = GameObject.FindGameObjectsWithTag("DebugObject");
		foreach (var tagObj in tagObjs) {
			bool continueFlg = false;
			// 見つけたオブジェクトが既にリストに入っていれば処理しない
			foreach (var debugObj in debugObjList) {
				if (debugObj.Obj == tagObj) {
					continueFlg = true;
					break;
				}
			}
			if (continueFlg) continue;

			// リストにタグの付いたオブジェクトを追加
			AddDebugObject(tagObj, null);
		}
	}

	void DebugTargetting() {
		// デバッグターゲットの選択
		if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Mouse1)) {
			RaycastHit hitInfo;
			Physics.Raycast(Camera.main.transform.position, (debugCursor.position - Camera.main.transform.position), out hitInfo, LayerMask.GetMask(new string[] { "Player", "Stage", "Box" }));
//			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, LayerMask.GetMask(new string[] { "Player", "Stage", "Box" }));
			if (hitInfo.collider == null) {
				DebugTarget = null;
			} else {
				DebugTarget = hitInfo.collider.transform;
			}
			// デバッグテキストの更新
			UpdateDebugText();
		}
		
		// デバッグターゲットカーソルの追従
		if (debugTargetCursorViewer != null) {
			if (DebugTarget == null) {
				if (debugCursor != null) {
					debugTargetCursorViewer.position = debugCursor.position;
				}
			} else {
				debugTargetCursorViewer.position = DebugTarget.position;
			}
		}

		// デバッグターゲット位置にコピーオブジェクト生成
		if (Input.GetKeyDown(KeyCode.V)) {
			if (debugCopyObject == null) {
				GameObject newObj = Instantiate(debugDefaultCopyObject);
				newObj.transform.position = debugCursor.position;
				// デバッグリソースに変更を追加
				AddDebugResource(newObj.name, 1);
				if (newObj.GetComponent<WeightManager>()) {
					AddDebugResource("Weight", (int)newObj.GetComponent<WeightManager>().WeightLv);
				}
			} else {
				GameObject newObj = Instantiate(debugCopyObject);
				newObj.transform.position = debugCursor.position;
				// デバッグリソースに変更を追加
				AddDebugResource(newObj.name, 1);
				if (newObj.GetComponent<WeightManager>()) {
					AddDebugResource("Weight", (int)newObj.GetComponent<WeightManager>().WeightLv);
				}
			}
		}

		// デバッグターゲットが選択されていなければ以降の処理を行わない
		if (DebugTarget == null) return;

		// デバッグターゲットの重さ変更
		if (Input.GetKeyDown(KeyCode.Q)) {
			WeightManager weightMng = DebugTarget.GetComponent<WeightManager>();
			if (weightMng != null) {
				// 重さ変更
				if (weightMng.SubWeightLevel(false)) {
					// 成功したらデバッグリソースに変更を追加
					AddDebugResource("Weight", -1);
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			WeightManager weightMng = DebugTarget.GetComponent<WeightManager>();
			if (weightMng != null) {
				// 重さ変更
				if (weightMng.AddWeightLevel(false)) {
					// 成功したらデバッグリソースに変更を追加
					AddDebugResource("Weight", 1);
				}
			}
		}

		// デバッグターゲットの移動
		if (Input.GetKey(KeyCode.F)) {
			DebugTarget.position = debugCursor.position;
		}

		// デバッグターゲットをコピー
		if (Input.GetKeyDown(KeyCode.C)) {
			debugCopyObject = DebugTarget.gameObject;
			// デバッグテキストの更新
			UpdateDebugText();
		}

		// デバッグターゲットの削除
		if (Input.GetKeyDown(KeyCode.R)) {
			// デバッグリソースに変更を追加
			AddDebugResource(DebugTarget.name, -1);
			if (DebugTarget.GetComponent<WeightManager>()) {
				AddDebugResource("Weight", -(int)DebugTarget.GetComponent<WeightManager>().WeightLv);
			}
			// 削除
			Destroy(DebugTarget.gameObject);
		}

		// タイムスケール変更
		bool timeSclRepeat = false; // 継続入力チェック
		if (Input.GetKey(KeyCode.T)) {
			timeSclRepeat = true;
			if ((Input.GetKeyDown(KeyCode.T)) || (timeSclNowRepeatTime > timeSclRepeatTime)) {
				timeScl -= timeSclSpd;
				if (timeScl < 0.0f) {
					timeScl = 0.0f;
				}
			}
		}
		if (Input.GetKey(KeyCode.Y)) {
			timeSclRepeat = true;
			if ((Input.GetKeyDown(KeyCode.Y)) || (timeSclNowRepeatTime > timeSclRepeatTime)) {
				timeScl += timeSclSpd;
				if (timeScl > 100.0f) {   // Time.timeScaleの最大値
					timeScl = 100.0f;
				}
			}
		}
		if (timeSclRepeat) {
			timeSclNowRepeatTime += Time.fixedDeltaTime;
		} else {
			timeSclNowRepeatTime = 0.0f;
		}
	}

	int AddDebugResource(string _name, int _num) {
		// 同じnameの項目が既にあれば
		if (debugResources.ContainsKey(_name)) {
			debugResources[_name] += _num;
		}
		// 同じnameの項目がなければ
		else {
			// 項目を追加
			debugResources.Add(_name, _num);
		}

		// デバッグテキストを更新
		UpdateDebugText();

		return debugResources[_name];
	}
}
