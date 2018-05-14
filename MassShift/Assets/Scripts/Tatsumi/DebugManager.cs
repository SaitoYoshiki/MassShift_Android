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
	[SerializeField]
	DebugState stt = DebugState.off;
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
	[SerializeField] KeyCode debugModeKey = KeyCode.Tab; // デバッグモード有効化/無効化キー
	[SerializeField] Vector3 textPos = Vector3.zero;
	[SerializeField] float switchTimeRange = 0.2f;          // 二度押し操作の有効時間

	Dictionary<DebugCmd, KeyCode> debugKey = new Dictionary<DebugCmd, KeyCode>();   // 各操作のキーコード
	TextMesh textMesh = null;
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
			textMesh.text = GetDebugModeText();

			// 初期状態がoffならテキストを無効化
			if (Stt == DebugState.off) {
				textMesh.gameObject.SetActive(false);
			}

			// デバッグ用テキストをデバッグ用オブジェクトに設定
			AddDebugObject(textMesh.gameObject, null);
		}

		SetActiveDebugObject();
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

		// デバッグテキストの位置をカメラの特定位置に移動
		textMesh.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));

		// モード変更
		if (Input.GetKey(debugKey[DebugCmd.debugModeSwitch])) {
			for (int cnt = 0; cnt < (int)DebugCmd.num; cnt++) {
				if (Input.GetKeyDown(debugKey[(DebugCmd)cnt])) {
					Mode = (DebugMode)cnt;

					// 各モード用のテキストを設定
					textMesh.text = GetDebugModeText();
				}
			}
		}

		DebugTargetting();
	}

	string GetDebugModeText() {
		string str = "";
		str += Mode.ToString();

		return str;
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
		if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Mouse2)) {
			RaycastHit hitInfo;
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, LayerMask.GetMask(new string[] { "Player", "Stage", "Box" }));
			if (hitInfo.collider == null) {
				DebugTarget = null;
			} else {
				DebugTarget = hitInfo.collider.transform;
			}
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
				Instantiate(debugDefaultCopyObject).transform.position = debugCursor.position;
			} else {
				Instantiate(debugCopyObject).transform.position = debugCursor.position;
			}
		}

		// デバッグターゲットが選択されていなければ以降の処理を行わない
		if (DebugTarget == null) return;

		// デバッグターゲットの重さ変更
		if (Input.GetKeyDown(KeyCode.Q)) {
			WeightManager weightMng = DebugTarget.GetComponent<WeightManager>();
			if (weightMng != null) {
				weightMng.SubWeightLevel(false);
			}
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			WeightManager weightMng = DebugTarget.GetComponent<WeightManager>();
			if (weightMng != null) {
				weightMng.AddWeightLevel(false);
			}
		}

		// デバッグターゲットの移動
		if (Input.GetKeyDown(KeyCode.F)) {
			DebugTarget.position = debugCursor.position;
		}

		// デバッグターゲットをコピー
		if (Input.GetKeyDown(KeyCode.C)) {
			debugCopyObject = DebugTarget.gameObject;
		}

		// デバッグターゲットの削除
		if (Input.GetKeyDown(KeyCode.R)) {
			Destroy(DebugTarget.gameObject);
		}
	}
}
