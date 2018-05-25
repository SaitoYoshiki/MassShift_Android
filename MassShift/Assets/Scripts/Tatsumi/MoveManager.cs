using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveManager : MonoBehaviour {
	// 定数
	const float ColMargin = 0.01f;

	public enum MoveType {
		min = -3,

		gravity = -2,   // 重力
		prevMove = -1,  // 前回移動の保持
		other = 0,      // その他

		max,
	}

	[System.Serializable] class MoveInfo {
		public Vector3 vec;
		public MoveType type;
	}

	[SerializeField] Vector3 prevMove;   // 前回の移動量
	public Vector3 PrevMove {
		get {
			return prevMove;
		}
		private set {
			prevMove = value;
		}
	}
	[SerializeField] bool useGravity = true;		// 重力加速度適用フラグ
	[SerializeField] bool useAirResistance = true;	// 空気抵抗適用フラグ
	[SerializeField] float gravityForce = -9.8f;	// 重力加速度
	public float GravityForce {
		set {
			gravityForce = value;
		}
		get {
			if (!GravityCustomFlg) {
				GravityForce = WeightMng.WeightForce;
			}
			return gravityForce;
		}
	}

	[SerializeField] float airResistance = 0.025f;				// 空気抵抗
	[SerializeField] List<float> customWeightLvMaxSpd = null;	// 標準時以外の重さ毎の最高速度、nullならdefaultWeightLvMaxSpdが代わりに適用される
	public List<float> CustomWeightLvMaxSpd {
		get {
			return customWeightLvMaxSpd;
		}
		set {
			if (customWeightLvMaxSpd != null) {
				customWeightLvMaxSpd.Clear();
			}
			customWeightLvMaxSpd = value;
		}
	} 
	[SerializeField] List<float> defaultWeightLvMaxSpd = new List<float>();	// 標準の重さ毎の最高速度
	[SerializeField] float? oneTimeMaxSpd = null;							// 一度の更新に限り最高速度を制限する制限速度
	public float? OneTimeMaxSpd {
		set {
//			Debug.Log("oneTimeMaxSpd:" + oneTimeMaxSpd);
			oneTimeMaxSpd = value;
		}
	}

	// 現在適用されている最大速度を返す
	public float CurMaxSpd {
		get {
			if (oneTimeMaxSpd != null) {
				return (float)oneTimeMaxSpd;
			}
			if (WeightMng && (CustomWeightLvMaxSpd != null) && (CustomWeightLvMaxSpd.Count > 0)) {
				if (CustomWeightLvMaxSpd.Count <= (int)WeightMng.WeightLv) {
					Debug.LogError("設定されたCustomWeightLvMaxSpdに" + WeightMng.WeightLv + "[" + (int)WeightMng.WeightLv + "]に対応する値が存在しません。");
					return float.MaxValue;
				}
				return CustomWeightLvMaxSpd[(int)WeightMng.WeightLv];
			}
			if (WeightMng && (defaultWeightLvMaxSpd != null) && (defaultWeightLvMaxSpd.Count > 0)) {
				if (defaultWeightLvMaxSpd.Count <= (int)WeightMng.WeightLv) {
					Debug.LogError("設定されたDefaultWeightLvMaxSpdに" + WeightMng.WeightLv + "[" + (int)WeightMng.WeightLv + "]に対応する値が存在しません。");
					return float.MaxValue;
				}
				return defaultWeightLvMaxSpd[(int)WeightMng.WeightLv];
			}
			return float.MaxValue;
		}
	}
	[SerializeField] Collider useCol = null;			// 当たり判定を行うコライダー
	public Collider UseCol {
		get {
			return useCol;
		}
		set {
			useCol = value;
		}
	}

	[SerializeField] float gravityCustomTime = 0.0f;	// 通常の重力加速度を一時停止する時間
	public float GravityCustomTime {
		get {
			return gravityCustomTime;
		}
		set {
			// 値の更新がなければ
			if (gravityCustomTime == value) return;

			// 値の更新
			gravityCustomTime = value;

			// 通常の重力加速度を一時停止しているかのフラグを更新
			GravityCustomFlg = (Time.time <= gravityCustomTime);
		}
	}

	[SerializeField] bool gravityCustomFlg;  // 通常の重力加速度を一時停止しているか
	bool GravityCustomFlg {
		get {
			GravityCustomFlg = (Time.time <= gravityCustomTime);
			return gravityCustomFlg;
		}
		set {
			// 値の更新がなければ
			if (gravityCustomFlg == value) return;

			// 値の更新
			gravityCustomFlg = value;

			if (!gravityCustomFlg) {
				// 通常の重力加速度の一時停止時間を終了
				GravityCustomTime = 0.0f;
			}
		}
	}

	[SerializeField] bool extrusionIgnore = false;  // 押し出し無効フラグ
	[SerializeField] bool extrusionForcible = false;	// 押し出し優先フラグ

	//	[SerializeField] WeightManager.Weight prevWeight = WeightManager.Weight.light;
	[SerializeField] float prevWeightForce;

	List<MoveInfo> moveList = new List<MoveInfo>();  // 処理待ち状態の力
	Landing land = null;
	Landing Land {
		get {
			if (land == null) {
				land = GetComponent<Landing>();
				if (land == null) {
					Debug.LogError("Landingが見つかりませんでした。");
				}
			}
			return land;
		}
	}

	PileWeight pile;
	PileWeight Pile {
		get {
			if (pile == null) {
				pile = GetComponent<PileWeight>();
			}
			return pile;
		}
	}

	WeightManager weightMng = null;
	WeightManager WeightMng {
		get {
			if (weightMng == null) {
				weightMng = GetComponent<WeightManager>();
			}
			return weightMng;
		}
	}

	public Vector3 TotalMove {
		get {
			Vector3 totalMove = Vector3.zero;
			foreach (var move in moveList) {
				totalMove += move.vec;
			}

			return totalMove;
		}
	}

	// 今回の更新処理で移動を無視(削除)する移動種類のリスト
	List<MoveType> stopHorizontalMoveType = new List<MoveType>();
	List<MoveType> stopVirticalMoveType = new List<MoveType>();

	// Use this for initialization
	void Start() {
		if (UseCol == null) {
			UseCol = GetComponent<BoxCollider>();
		}
		if (UseCol == null) {
			UseCol = GetComponent<SphereCollider>();
		}
		if (UseCol == null) {
			UseCol = GetComponent<CapsuleCollider>();
		}
	}

	// Update is called once per frame
	void FixedUpdate() {
		// 重力加速度
		if (useGravity/* && !Land.IsLanding*/) {
			AddMove(new Vector3(0.0f, GravityForce * Time.fixedDeltaTime, 0.0f), MoveType.gravity);

//			float maxGravityForce = GravityForce;
//			// 積み重なっている重さオブジェクトから最も重いオブジェクトを取得
//			if (Pile) {
//				WeightManager maxWeightMng = WeightMng; // 最も重い重さ
//				List<Transform> pileObjs = Pile.GetPileBoxList(new Vector3(0.0f, GravityForce, 0.0f));
////				Debug.LogError("pile:" + name + " " + pileObjs.Count);
//
//				foreach (var pileObj in pileObjs) {
//					WeightManager pileWeight = pileObj.GetComponent<WeightManager>();
//					if (!pileWeight) continue;
//					// 積み重なっている重さオブジェクトの重さと比較
//					if (pileWeight.WeightLv > maxWeightMng.WeightLv) {
//						maxWeightMng = pileWeight;
//						maxGravityForce = pileWeight.WeightForce;
////						Debug.LogError("pileWeight>" + pileWeight.name);
//						prevWeight = pileWeight.WeightLv;
//						prevWeightForce = pileWeight.WeightForce;
//					}
//				}
//			}
//			
//			AddMove(new Vector3(0.0f, maxGravityForce * Time.deltaTime, 0.0f), MoveType.gravity);
		}

		// 前回の加速度
		AddMove(prevMove, MoveType.prevMove);

		// 無効指定種類の移動を削除
		for (int idx = 0; idx < moveList.Count; idx++) {
			if (stopHorizontalMoveType.Contains(moveList[idx].type)) {
				moveList[idx].vec = new Vector3(0.0f, moveList[idx].vec.y, moveList[idx].vec.z);
			}
			if (stopVirticalMoveType.Contains(moveList[idx].type)) {
				moveList[idx].vec = new Vector3(moveList[idx].vec.x, 0.0f, moveList[idx].vec.z);
			}
		}
		stopVirticalMoveType.Clear();
		stopHorizontalMoveType.Clear();

		// 今回の移動量を求める
		Vector3 move = TotalMove;

		// 空気抵抗
		if (useAirResistance) {
			move -= (move.normalized * move.magnitude * airResistance);
		}

		// 最高速度制限
		move = move.normalized * Mathf.Min(move.magnitude, CurMaxSpd);
		OneTimeMaxSpd = null;

		if (!extrusionIgnore && Pile) {
			// 上に積まれている自身より重い重さオブジェクトの方が速ければそれに合わせる
			foreach (var pileObj in Pile.GetPileBoxList(Vector3.up)) {
				MoveManager pileObjMoveMng = pileObj.GetComponent<MoveManager>();
				WeightManager pileObjWeight = pileObj.GetComponent<WeightManager>();
				if (pileObjMoveMng && pileObjWeight && (pileObjWeight.WeightLv > WeightMng.WeightLv) &&
					(pileObjMoveMng.PrevMove.y < 0.0f) && (pileObjMoveMng.PrevMove.y < move.y)) {
					move = new Vector3(move.x, pileObjMoveMng.PrevMove.y, move.z);
				}
			}
		}

		// 移動
		Vector3 resMove;    // 実際に移動出来た移動量
		Move(move * Time.fixedDeltaTime, (BoxCollider)useCol, LayerMask.GetMask(new string[] { "Stage", "Player", "Box", "Fence" }), out resMove);

		// 今回の移動量を保持
		//prevMove = resMove;
		PrevMove = move;

		// 計算済みの力を削除
		moveList.Clear();

		//test
//		if (!Input.GetKey(KeyCode.Tab)) {
//			//UnityEditor.EditorApplication.isPaused = true;
//		}
	}

	public void AddMove(Vector3 _move, MoveType _type = 0) {
		MoveInfo moveInfo = new MoveInfo();
		moveInfo.vec = _move;
		moveInfo.type = _type;
		moveList.Add(moveInfo);
	}

	// 可能な限り移動、指定分全て移動できたらtrueを返す
	static public bool Move(Vector3 _move, BoxCollider _moveCol, int _mask, out Vector3 _resMove, out Collider _hitCol,
		bool _dontExtrusionFlg = false, bool _extrusionForcible = false, List<Collider> _ignoreColList = null) {
		bool ret = true;    // 自身が指定位置まで移動出来たらtrue
		Vector3 moveVec = new Vector3((_move.x == 0.0f ? 0.0f : Mathf.Sign(_move.x)), (_move.y == 0.0f ? 0.0f : Mathf.Sign(_move.y)), 0.0f);
		MoveManager moveMng = _moveCol.GetComponent<MoveManager>();
		Vector3 befPos = _moveCol.transform.position;

		// 回帰呼び出し時に自身を衝突対象としない
		if (_ignoreColList == null) {
			_ignoreColList = new List<Collider>();
		}
		if (!_ignoreColList.Contains(_moveCol)) {
			_ignoreColList.Add(_moveCol);
		}

//		string testStr = _moveCol.name;
//		foreach (var ignoreCol in _ignoreColList) {
//			testStr += "\n" + ignoreCol.name;
//		}
//		Debug.LogWarning(testStr);

		///Debug.LogError(moveVec);

		// y軸判定
		if (moveVec.y != 0) {
			// y軸の衝突を全て取得
			//			RaycastHit[] hitInfos = Physics.BoxCastAll(_moveCol.bounds.center, _moveCol.size * 0.5f, new Vector3(0.0f, _move.y, 0.0f));
			RaycastHit[] hitInfos = Support.GetColliderHitInfoList(_moveCol, new Vector3(0.0f, _move.y, 0.0f), _mask, _ignoreColList).ToArray();

			// y軸判定衝突判定
			if (hitInfos.Length > 0) {
				// 近い順にソート
				hitInfos = hitInfos.OrderBy(x => x.distance).ToArray();

				///Debug.LogError(_moveCol.name + " y軸衝突");
				///foreach (var hitInfo in hitInfos) {
				///	Debug.LogError(hitInfo.collider.name);
				///}
				//UnityEditor.EditorApplication.isPaused = true;

				// y軸の全ての衝突を取得
				RaycastHit nearHitinfo = new RaycastHit();
//				float dis = float.MinValue;
				foreach (var hitInfo in hitInfos) {
//					float cmpDis = (Mathf.Abs(_moveCol.bounds.center.y - hitInfo.collider.bounds.center.y) - (_moveCol.bounds.size.y + hitInfo.collider.bounds.size.y) * 0.5f) * -1;
					float dis = (Mathf.Abs(_moveCol.bounds.center.y - hitInfo.collider.bounds.center.y) - (_moveCol.bounds.size.y + hitInfo.collider.bounds.size.y) * 0.5f);
//					if (cmpDis > dis) {
//					dis = cmpDis;
						nearHitinfo = hitInfo;
//					}

					/**/
					nearHitinfo = hitInfo;
					dis -= ColMargin;
					dis = Mathf.Clamp(dis, 0, dis);

					// 押し出し判定
					WeightManager moveWeightMng = _moveCol.GetComponent<WeightManager>();
					WeightManager hitWeightMng = nearHitinfo.collider.GetComponent<WeightManager>();
					MoveManager hitMoveMng = nearHitinfo.collider.GetComponent<MoveManager>();
					bool canExtrusion = // 自身が衝突相手を押し出せるか
						(moveWeightMng) && (hitWeightMng) && (hitMoveMng) &&		// 判定に必要なコンポーネントが揃っている
						(!_dontExtrusionFlg) && (!hitMoveMng.extrusionIgnore) &&	// 今回の移動が押し出し不可でなく、相手が押し出し不可設定ではない
						((moveWeightMng.WeightLv > hitWeightMng.WeightLv) ||		// 自身の重さレベルが相手の重さレベルより重い、又は
						(moveMng.extrusionForcible || _extrusionForcible));         // 自身が押し出し優先設定であるか、今回の移動が押し出し優先設定であれば
					bool stopFlg = false;   // 移動量を削除するフラグ
	
					// 押し出せない場合
					if (!canExtrusion) {
						// 直前まで移動
						///Debug.LogError("yMove bef pos:" + _moveCol.transform.position.x + ", " + _moveCol.transform.position.y);
						//					UnityEditor.EditorApplication.isPaused = true;
						_moveCol.transform.position += new Vector3(0.0f, moveVec.y * dis, 0.0f);
						///Debug.LogError("yMove aft pos:" + _moveCol.transform.position.x + ", " + _moveCol.transform.position.y);
						//					UnityEditor.EditorApplication.isPaused = true;

						// 指定位置まで移動できない
						ret = false;
					}
					// 押し出せる場合
					else {
						if (hitMoveMng && moveMng) {
							// 押し出し相手の上下移動量を削除
							hitMoveMng.StopMoveVirtical(MoveType.prevMove);
							hitMoveMng.StopMoveVirtical(MoveType.gravity);

							// 押し出し相手に自身の移動量をコピー
							hitMoveMng.AddMove(new Vector3(0.0f, moveMng.PrevMove.y * 1.1f, 0.0f));
						}

						// 押し出しを行い、押し出し切れた場合
						if (Move(new Vector3(0.0f, (_move.y - dis), 0.0f), (BoxCollider)nearHitinfo.collider, _mask,
							false, (moveMng.extrusionForcible || _extrusionForcible), _ignoreColList)) {	// 押し出し優先情報を使用
							// 自身は指定通り移動
							Move(new Vector3(0.0f, _move.y, 0.0f), _moveCol, _mask, true, false, _ignoreColList);  // 押し出し不可移動
						}
						// 押し出しきれない場合
						else {
							// 自身も直前まで移動
							Move(new Vector3(0.0f, (_move.y - dis), 0.0f), _moveCol, _mask, true, false, _ignoreColList);  // 押し出し不可移動

							// 指定位置まで移動できない
							ret = false;

							// 移動量を削除
							stopFlg = true;
						}
					}

					// 着地判定
					Landing land = _moveCol.GetComponent<Landing>();
					if (land != null) {
						// 着地先がステージ又はステージに接地中のオブジェクトなら
						Landing hitLand = nearHitinfo.collider.GetComponent<Landing>(); // nullならステージ
						if ((hitLand == null) || (hitLand.IsLanding) || (hitLand.IsExtrusionLanding)) {
							land.IsLanding = land.GetIsLanding(Vector3.up * moveVec.y);
							land.IsExtrusionLanding = land.GetIsLanding(Vector3.up * -moveVec.y);
						}
					}

					// 移動量を削除
					if (stopFlg && moveMng) {
						moveMng.StopMoveVirtical(MoveType.prevMove);
						moveMng.StopMoveVirtical(MoveType.gravity);
						moveMng.GravityCustomFlg = false;
					}
					/**/
				}
///				dis += ColMargin;
///				///Debug.LogError("dis:" + dis);
///
///				// 押し出し判定
///				WeightManager moveWeightMng = _moveCol.GetComponent<WeightManager>();
///				WeightManager hitWeightMng = nearHitinfo.collider.GetComponent<WeightManager>();
///				bool canExtrusion = // 自身が衝突相手を押し出せるか
///					(!_dontExtrusionFlg) && (moveWeightMng) && (hitWeightMng) &&    // 必要なコンポーネントが揃っている
///					((moveWeightMng.WeightLv > hitWeightMng.WeightLv));             // 自身の重さが相手の重さより重い
///
///				// 押し出せない場合
///				if (!canExtrusion) {
///					///Debug.LogError("押し出せない");
///					// 直前まで移動
///					///Debug.LogError("yMove bef pos:" + _moveCol.transform.position.x + ", " + _moveCol.transform.position.y);
/////					UnityEditor.EditorApplication.isPaused = true;
///					_moveCol.transform.position += new Vector3(0.0f, -moveVec.y * dis, 0.0f);
///					///Debug.LogError("yMove aft pos:" + _moveCol.transform.position.x + ", " + _moveCol.transform.position.y);
/////					UnityEditor.EditorApplication.isPaused = true;
///
///					// 指定位置まで移動できない
///					ret = false;
///				}
///				// 押し出せる場合
///				else {
///					///Debug.LogError("押し出せる");
///					// 押し出しを行い、押し出し切れた場合
///					if (Move(new Vector3(0.0f, (_move.y - dis), 0.0f), (BoxCollider)nearHitinfo.collider, _mask)) {
///						// 自身は指定通り移動
///						_moveCol.transform.position += _move;
///					}
///					// 押し出しきれない場合
///					else {
///						///Debug.LogError("押し出しきれない");
///						// 自身も直前まで移動
///						Move(new Vector3(0.0f, (_move.y - dis), 0.0f), _moveCol, _mask, true);	// 押し出し不可移動
///
///						// 指定位置まで移動できない
///						ret = false;
///					}
///				}
///				//UnityEditor.EditorApplication.isPaused = true;
///
///				// 着地判定
///				Landing land = _moveCol.GetComponent<Landing>();
///				if (land != null) {
///					// 着地先がステージ又はステージに接地中のオブジェクトなら
///					Landing hitLand = nearHitinfo.collider.GetComponent<Landing>();	// nullならステージ
///					if ((hitLand == null) || (hitLand.IsLanding) || (hitLand.IsExtrusionLanding)) {
///						land.IsLanding = land.GetIsLanding(Vector3.up * moveVec.y);
///						land.IsLanding = land.GetIsLanding(-Vector3.up * moveVec.y);
///					}
///				}
///
///				// 移動量を削除
///				MoveManager moveMng = _moveCol.GetComponent<MoveManager>();
///				if (moveMng) {
///					moveMng.StopMoveVirtical(MoveType.prevMove);
///					moveMng.StopMoveVirtical(MoveType.gravity);
///				}
				}
			// 衝突が無ければ
			else {
				// 指定通り移動
				_moveCol.transform.position += new Vector3(0.0f, _move.y, 0.0f);
				///Debug.LogError("指定移動:" + _moveCol.transform.position.x + ", " + _moveCol.transform.position.y + "\n _move:" + _move);
			}
		}

		// x軸判定
		if (moveVec.x != 0.0f) {
			// x軸の衝突を全て取得
			//			RaycastHit[] hitInfos = Physics.BoxCastAll(_moveCol.bounds.center, _moveCol.size * 0.5f, new Vector3(_move.x, 0.0f, 0.0f));
			RaycastHit[] hitInfos = Support.GetColliderHitInfoList(_moveCol, new Vector3(_move.x, 0.0f, 0.0f), _mask, _ignoreColList).ToArray();

			// x軸衝突判定
			if (hitInfos.Length > 0) {
				///Debug.LogError(_moveCol.name + " x軸衝突");
				//foreach (var hitInfo in hitInfos) {
					///Debug.LogError(hitInfo.collider.name);
				//}
				//UnityEditor.EditorApplication.isPaused = true;

				// x軸で最もめり込んでいる衝突を取得
//				RaycastHit nearHitinfo = new RaycastHit();
				float dis = float.MinValue;
				foreach (var hitInfo in hitInfos) {
					float cmpDis = (Mathf.Abs(_moveCol.bounds.center.x - hitInfo.collider.bounds.center.x) - (_moveCol.bounds.size.x + hitInfo.collider.bounds.size.x) * 0.5f) * -1;
					if (cmpDis > dis) {
						dis = cmpDis;
//						nearHitinfo = hitInfo;
					}
				}
				dis += ColMargin;
				///Debug.LogError("dis:" + dis);

				// x軸は押し出しを行わない

				// 直前まで移動
				_moveCol.transform.position += new Vector3(-moveVec.x * dis, 0.0f, 0.0f);
				
				// 指定位置まで移動できない
				ret = false;
				//UnityEditor.EditorApplication.isPaused = true;

				// 移動量を削除
				if (moveMng) {
					moveMng.StopMoveHorizontal(MoveType.prevMove);
				}
			}
			// 衝突が無ければ
			else {
				// 指定通り移動
				///Debug.LogError("xMove bef pos:" + _moveCol.transform.position.x + ", " + _moveCol.transform.position.y);
				_moveCol.transform.position += new Vector3(_move.x, 0.0f, 0.0f);
				///Debug.LogError("xMove aft pos:" + _moveCol.transform.position.x + ", " + _moveCol.transform.position.y);
			}
		}

		_resMove = (_moveCol.transform.position - befPos);
		_hitCol = null;
		return ret;









		//		_resMove = Vector3.zero;
		//		_hitCol = null;
		//		GameObject obj = _moveCol.gameObject;
		//
		//		// 衝突リストを取得
		//		List<RaycastHit> hitInfoList = Support.GetColliderHitInfoList(_moveCol, _move, _mask);
		//
		//		// 衝突しなかったら
		//		if (hitInfoList.Count == 0) {
		//			// 今回の移動量を保持
		//			if (obj.GetComponent<MoveManager>() != null) {
		//				obj.GetComponent<MoveManager>().prevMove = _move;
		//			}
		//
		//			// そのままの移動量を移動
		//			_resMove = _move;
		//			obj.transform.position += _resMove;
		//
		//			return true;
		//		}
		//		// 衝突したら
		//		else {
		//			// 移動を停止
		//			if (obj.GetComponent<MoveManager>() != null) {
		//				Debug.Log("MoveManager:接地");
		//				obj.GetComponent<MoveManager>().StopMoveVirtical(MoveType.gravity);
		//				obj.GetComponent<MoveManager>().StopMoveVirtical(MoveType.prevMove);
		//				obj.GetComponent<MoveManager>().StopMoveHorizontal(MoveType.prevMove);
		//			}
		//
		//			// 最も近い衝突を取得
		//			Vector3 moveVec = _move.normalized;
		//			// 最初の比較対象
		//			RaycastHit nearHit = hitInfoList[0];
		//			float nearDis = nearHit.distance;
		//			// 比較開始
		//			for (int idx = 1; idx < hitInfoList.Count; idx++) {
		//				float cmpDis = hitInfoList[idx].distance;
		//				if (nearDis > cmpDis) {
		//					nearDis = cmpDis;
		//					nearHit = hitInfoList[idx];
		//				}
		//			}
		//
		//			Collider extrusionCol;	// 押し出しを行うコライダー
		//			Collider otherCol;      // 押し出されないコライダー
		//
		//			// 相手を押し出せる場合
		//			if () {
		//				extrusionCol = nearHit.collider;
		//				otherCol = _moveCol;
		//
		//				// 押し出そうとする
		//
		//
		//				// 押し出しきれれば
		//				if () {
		//					// 自身を移動
		//
		//
		//					// 指定位置まで移動できている
		//					return true;
		//				} else {
		//					// 押し出し切れなければ
		//					
		//
		//					// 押し出し不可で自身も出来る限り詰める移動を行う
		//
		//
		//					// 指定位置までは移動できていない
		//					return false;
		//				}
		//			}
		//			// 自身が押し出される場合
		//			else {
		//				extrusionCol = _moveCol;
		//				otherCol = nearHit.collider;
		//
		//				// 自身を可能な限り移動
		//
		//
		//				// 指定位置までは移動できていない
		//				return false;
		//			}
		//
		//			// 重複部分を取得
		//			Vector3 min = new Vector3(
		//				Mathf.Max((extrusionCol.bounds.center.x - extrusionCol.bounds.size.x * 0.5f), (otherCol.bounds.center.x - otherCol.bounds.size.x * 0.5f)),
		//				Mathf.Max((extrusionCol.bounds.center.y - extrusionCol.bounds.size.y * 0.5f), (otherCol.bounds.center.y - otherCol.bounds.size.y * 0.5f)), 0.5f);
		//			Vector3 max = new Vector3(
		//				Mathf.Min((extrusionCol.bounds.center.x + extrusionCol.bounds.size.x * 0.5f), (otherCol.bounds.center.x + otherCol.bounds.size.x * 0.5f)),
		//				Mathf.Min((extrusionCol.bounds.center.y + extrusionCol.bounds.size.y * 0.5f), (otherCol.bounds.center.y + otherCol.bounds.size.y * 0.5f)), -0.5f);
		//
		//			// 重複部分のBoundsを作成
		//			Bounds colBounds = new Bounds((min + max) * 0.5f, (max - min));
		//			Debug.Log("_moveCol.bounds.center:" + extrusionCol.bounds.center + " size:" + extrusionCol.bounds.size +
		//				"\ncolBounds center:" + colBounds.center + " size:" + colBounds.size);
		//
		//			// 押し出しを行うベクトルを求める
		//			float extrusionDis = (Vector3.Magnitude(extrusionCol.bounds.center - colBounds.ClosestPoint(-moveVec.normalized * MaxDis)) * 2 + Vector3.Magnitude(colBounds.center - _moveCol.bounds.center));
		//			Vector3 extrusionVec = -moveVec.normalized * extrusionDis;
		//
		//			// 押し出すオブジェクトを設定
		//			Transform extrusionObj;	// 押し出されるオブジェクト
		//			Transform otherObj;        // 動かない側のオブジェクト
		//			bool ret = true;
		//
		//
		//
		//			Debug.Log("extrusionDis:" + extrusionDis + " Vec:" + extrusionVec);
		//
		//			//test
		//			testStaticCube.transform.position = colBounds.center;
		//			testStaticCube.transform.localScale = colBounds.size;
		//
		//
		//
		//
		//
		//
		//
		//			/*
		//			// 直前まで移動するように調整
		//			extrusionCol.transform.position += extrusionVec + (-moveVec.normalized * ColMargin);
		//			extrusionCol.transform.position = (colBounds.center + extrusionVec);
		//			extrusionCol.transform.position = (colBounds.center) + ((BoxCollider)extrusionCol).center;
		//			*/
		//
		//			// 押し出し処理
		//			//			bool landFlg = false;
		//			//			foreach (var hitInfo in hitInfoList) {
		//			//				Debug.Log("hitInfo p:" + hitInfo.point + " dis:" + hitInfo.distance);
		//			//				Vector3 resMove;
		//			//				resMove = ExtrusionMove(_move, (BoxCollider)_moveCol, (BoxCollider)hitInfo.collider, _mask);
		//			//			}
		//			//			if (landFlg) {
		//			//				if (obj.GetComponent<Landing>() != null) {
		//			//					obj.GetComponent<Landing>().IsLanding = true;
		//			//					}
		//			//			}
		//
		//			// 全ての衝突コライダーから押し出し
		//			/*			Vector3 vec = Vector3.zero;
		//						float dotMax = 0.0f;
		//						foreach (var hitInfo in hitInfoList) {
		//							// 上下左右の内、最も対抗しているコライダーから押し出しを行う
		//							vec = Extrusion(_obj.GetComponent<BoxCollider>(), (BoxCollider)hitInfo.collider); // BoxCollider前提
		//
		//							// 接地方向から押し出されたら
		//			//				if(vec == FourSideCollider)
		//						}
		//
		//						// 押し出し後にまだ衝突しているコライダーがあれば
		//						hitInfoList = Support.GetColliderHitInfoList(_moveCol, _move, _mask);
		//						if (hitInfoList.Count > 0) {
		//							foreach (var hitInfo in hitInfoList) {
		//								// 上下左右の内、次に対抗しているコライダーから押し出しを行う
		//								Extrusion(_obj.GetComponent<BoxCollider>(), (BoxCollider)hitInfo.collider); // BoxCollider前提
		//
		//							}
		//						}
		//
		//						// 接地方向からの押し出しがあれば接地
		//						if() {
		//
		//						}*/
		//
		//			/*			// 最も近い衝突コライダーとの距離
		//						float dis = float.MaxValue;
		//						foreach (var hitInfo in hitInfoList) {
		//							if (dis > hitInfo.distance) {
		//								dis = hitInfo.distance;
		//								_hitCol = hitInfo.collider;
		//							}
		//						}
		//			
		//						// 衝突直前まで移動する移動量を移動
		//						_resMove = (_move.normalized * (dis - ColMargin));
		//						_obj.transform.position += _resMove;
		//			
		//						//test
		//						if (!Input.GetKey(KeyCode.Tab)) {
		//							//UnityEditor.EditorApplication.isPaused = true;
		//						}
		//			
		//						// 接地
		//						if (_obj.GetComponent<Landing>() != null) {
		//							_obj.GetComponent<Landing>().CollisionLanding(_move);
		//						}
		//						*/
		//		}
	}
	static public bool Move(Vector3 _move, BoxCollider _moveCol, int _mask,
		bool _dontExtrusionFlg = false, bool _extrusionForcible = false, List<Collider> ignoreColList = null) {
		Vector3 dummyResMove;
		Collider dummyHitCol;
		return Move(_move, _moveCol, _mask, out dummyResMove, out dummyHitCol,
			_dontExtrusionFlg, _extrusionForcible, ignoreColList);
	}
	static public bool Move(Vector3 _move, BoxCollider _moveCol, int _mask, out Vector3 _resMove,
		bool _dontExtrusionFlg = false, bool _extrusionForcible = false, List<Collider> ignoreColList = null) {
		Collider dummyHitCol;
		return Move( _move, _moveCol, _mask, out _resMove, out dummyHitCol,
			_dontExtrusionFlg, _extrusionForcible, ignoreColList);
	}
	static public bool Move(Vector3 _move, BoxCollider _moveCol, int _mask, out Collider _hitCol,
		bool _dontExtrusionFlg = false, bool _extrusionForcible = false, List<Collider> ignoreColList = null) {
		Vector3 dummyResMove;
		return Move( _move, _moveCol, _mask, out dummyResMove, out _hitCol,
			_dontExtrusionFlg, _extrusionForcible, ignoreColList);
	}

	static public bool Move(Vector3 _move, GameObject _moveObj, int _mask, out Vector3 _resMove, out Collider _hitCol,
		bool _dontExtrusionFlg = false, bool _extrusionForcible = false, List<Collider> ignoreColList = null) {
		if (!_moveObj) {
			if (!_moveObj) {
				Debug.LogError("_moveObjが見つかりませんでした。");
			}
		}
		return Move(_move, _moveObj.GetComponent<BoxCollider>(), _mask, out _resMove, out _hitCol,
			_dontExtrusionFlg, _extrusionForcible, ignoreColList);
	}
	static public bool Move(Vector3 _move, GameObject _moveObj, int _mask,
		bool _dontExtrusionFlg = false, bool _extrusionForcible = false, List<Collider> ignoreColList = null) {
		Vector3 dummyResMove;
		Collider dummyHitCol;
		return Move(_move, _moveObj, _mask, out dummyResMove, out dummyHitCol,
			_dontExtrusionFlg, _extrusionForcible, ignoreColList);
	}
	static public bool Move(Vector3 _move, GameObject _moveObj, int _mask, out Vector3 _resMove,
		bool _dontExtrusionFlg = false, bool _extrusionForcible = false, List<Collider> ignoreColList = null) {
		Collider dummyHitCol;
		return Move(_move, _moveObj, _mask, out _resMove, out dummyHitCol,
			_dontExtrusionFlg, _extrusionForcible, ignoreColList);
	}
	static public bool Move(Vector3 _move, GameObject _moveObj, int _mask, out Collider _hitCol,
		bool _dontExtrusionFlg = false, bool _extrusionForcible = false, List<Collider> ignoreColList = null) {
		Vector3 dummyResMove;
		return Move(_move, _moveObj, _mask, out dummyResMove, out _hitCol,
			_dontExtrusionFlg, _extrusionForcible, ignoreColList);
	}


	//	public bool Move(Vector3 _move, Collider _col, int _mask, out Vector3 _resMove, out Collider _hitCol) {
	//		return MoveManager.Move(_move, _col, _mask, out _resMove, out _hitCol);
	//	}
	//	public bool Move(Vector3 _move, Collider _col, int _mask) {
	//		Vector3 dummyResMove;
	//		Collider dummyHitCol;
	//		return Move(_move, _col, _mask, out dummyResMove, out dummyHitCol);
	//	}
	//	public bool Move(Vector3 _move, Collider _col, int _mask, out Vector3 _resMove) {
	//		Collider dummyHitCol;
	//		return Move(_move, _col, _mask, out _resMove, out dummyHitCol);
	//	}
	//	public bool Move(Vector3 _move, Collider _col, int _mask, out Collider _hitCol) {
	//		Vector3 dummyResMove;
	//		return Move(_move, _col, _mask, out dummyResMove, out _hitCol);
	//	}

	// 移動量ではなく移動先位置を基準に移動する
	static public bool MoveTo(Vector3 _pos, BoxCollider _moveCol, int _mask, out Vector3 _resMove, out Collider _hitCol, bool _dontExtrusionFlg = false) {
		return Move(_pos - _moveCol.transform.position, _moveCol, _mask, out _resMove, out _hitCol, _dontExtrusionFlg);
	}
	static public bool MoveTo(Vector3 _pos, BoxCollider _moveCol, int _mask, bool _dontExtrusionFlg = false) {
		Vector3 dummyResMove;
		Collider dummyHitCol;
		return MoveTo(_pos, _moveCol, _mask, out dummyResMove, out dummyHitCol, _dontExtrusionFlg);
	}
	static public bool MoveTo(Vector3 _pos, BoxCollider _moveCol, int _mask, out Vector3 _resMove, bool _dontExtrusionFlg = false) {
		Collider dummyHitCol;
		return MoveTo(_pos, _moveCol, _mask, out _resMove, out dummyHitCol, _dontExtrusionFlg);
	}
	static public bool MoveTo(Vector3 _pos, BoxCollider _moveCol, int _mask, out Collider _hitCol, bool _dontExtrusionFlg = false) {
		Vector3 dummyResMove;
		return MoveTo(_pos, _moveCol, _mask, out dummyResMove, out _hitCol, _dontExtrusionFlg);
	}

	static public bool MoveTo(Vector3 _pos, GameObject _moveObj, int _mask, out Vector3 _resMove, out Collider _hitCol, bool _dontExtrusionFlg = false) {
		return Move(_pos - _moveObj.transform.position, _moveObj, _mask, out _resMove, out _hitCol, _dontExtrusionFlg);
	}
	static public bool MoveTo(Vector3 _pos, GameObject _moveObj, int _mask, bool _dontExtrusionFlg = false) {
		Vector3 dummyResMove;
		Collider dummyHitCol;
		return MoveTo(_pos, _moveObj, _mask, out dummyResMove, out dummyHitCol, _dontExtrusionFlg);
	}
	static public bool MoveTo(Vector3 _pos, GameObject _moveObj, int _mask, out Vector3 _resMove, bool _dontExtrusionFlg = false) {
		Collider dummyHitCol;
		return MoveTo(_pos, _moveObj, _mask, out _resMove, out dummyHitCol, _dontExtrusionFlg);
	}
	static public bool MoveTo(Vector3 _pos, GameObject _moveObj, int _mask, out Collider _hitCol, bool _dontExtrusionFlg = false) {
		Vector3 dummyResMove;
		return MoveTo(_pos, _moveObj, _mask, out dummyResMove, out _hitCol, _dontExtrusionFlg);
	}

	//	bool MoveTo(Vector3 _pos, Collider _col, int _mask, out Vector3 _resMove, out Collider _hitCol) {
	//		return MoveManager.MoveTo(_pos, _col, _mask, out _resMove, out _hitCol);
	//	}
	//	bool MoveTo(Vector3 _pos, Collider _col, int _mask) {
	//		Vector3 dummyResMove;
	//		Collider dummyHitCol;
	//		return MoveTo(_pos, _col, _mask, out dummyResMove, out dummyHitCol);
	//	}
	//	bool MoveTo(Vector3 _pos, Collider _col, int _mask, out Vector3 _resMove) {
	//		Collider dummyHitCol;
	//		return MoveTo(_pos, _col, _mask, out _resMove, out dummyHitCol);
	//	}
	//	bool MoveTo(Vector3 _pos, Collider _col, int _mask, out Collider _hitCol) {
	//		Vector3 dummyResMove;
	//		return MoveTo(_pos, _col, _mask, out dummyResMove, out _hitCol);
	//	}

	public void StopOverMoveVirtical(MoveType _stopMinPriority = MoveType.other) {
		for (MoveType type = _stopMinPriority; type <= (MoveType.max - 1); type++) {
			StopMoveVirtical(type);
		}
	}
	public void StopUnderMoveVirtical(MoveType _stopMaxPriority = MoveType.other) {
		for (MoveType type = (MoveType.min + 1); type <= _stopMaxPriority; type++) {
			StopMoveVirtical(type);
		}
	}
	public void StopOverMoveHorizontal(MoveType _stopMinPriority = MoveType.other) {
		for (MoveType type = _stopMinPriority; type <= (MoveType.max - 1); type++) {
			StopMoveHorizontal(type);
		}
	}
	public void StopUnderMoveHorizontal(MoveType _stopMaxPriority = MoveType.other) {
		for (MoveType type = (MoveType.min + 1); type <= _stopMaxPriority; type++) {
			StopMoveHorizontal(type);
		}
	}
	public void StopMoveVirtical(MoveType _stopType) {
		if (!stopVirticalMoveType.Contains(_stopType)) {
			stopVirticalMoveType.Add(_stopType);
		}
	}
	public void StopMoveHorizontal(MoveType _stopType) {
		if (!stopHorizontalMoveType.Contains(_stopType)) {
			stopHorizontalMoveType.Add(_stopType);
		}
	}

	public void StopMoveVirticalAll() {
		StopOverMoveVirtical(MoveType.min + 1);
	}
	public void StopMoveHorizontalAll() {
		StopOverMoveHorizontal(MoveType.min + 1);
	}

	public float AddGravityCustomTime(float _time) {
		return (GravityCustomTime + _time);
	}

	// 衝突しているコライダーのどちらかを押し出す
//	static Vector3 ExtrusionMove(Vector3 _move, BoxCollider _moveCol, BoxCollider _hitCol, int _mask, bool _disExtFlg = false) {
//		//		Vector3 extrusionVec = Vector3.zero;
//		//		// 横
//		//		extrusionVec.x = (Mathf.Min(
//		//			(_moveCol.transform.position.x + _moveCol.center.x + _moveCol.size.x * 0.5f) - (_hitCol.transform.position.x + _hitCol.center.x - _hitCol.size.x * 0.5f),
//		//			(_hitCol.transform.position.x + _hitCol.center.x + _hitCol.size.x * 0.5f) - (_moveCol.transform.position.x + _moveCol.center.x - _moveCol.size.x * 0.5f)));
//		//
//		//		// 縦
//		//		extrusionVec.y = (Mathf.Min(
//		//			(_moveCol.transform.position.y + _moveCol.center.y + _moveCol.size.y * 0.5f) - (_hitCol.transform.position.y + _hitCol.center.y - _hitCol.size.y * 0.5f),
//		//			(_hitCol.transform.position.y + _hitCol.center.y + _hitCol.size.y * 0.5f) - (_moveCol.transform.position.y + _moveCol.center.y - _moveCol.size.y * 0.5f)));
//		//
//		//		extrusionVec += (_moveCol.size * 0.5f + _hitCol.size * 0.5f);
//		//
//		//		// 差が小さい方向に押し出し
//		//		if (extrusionVec.x < extrusionVec.y) {
//		//			extrusionVec.y = 0.0f;
//		//		}
//		//		else {
//		//			extrusionVec.x = 0.0f;
//		//		}
//		//		extrusionVec *= -1.0f;
//
//		Vector3 moveVec = new Vector3(Mathf.Sign(_move.x), Mathf.Sign(_move.y), 0.0f);
//		Vector3 befPos = (_moveCol.transform.position + _moveCol.center);
//		Vector3 aftPos = (befPos + _move);
//		Vector3 colPos = (_hitCol.transform.position + _hitCol.center);
//		Vector3 resMove = _move;
//		Vector3 moveColSize = (new Vector3((_moveCol.transform.lossyScale.x * _moveCol.size.x), (_moveCol.transform.lossyScale.y * _moveCol.size.y), (_moveCol.transform.lossyScale.z * _moveCol.size.z)));
//		Vector3 hitColSize = (new Vector3((_hitCol.transform.lossyScale.x * _hitCol.size.x), (_hitCol.transform.lossyScale.y * _hitCol.size.y), (_hitCol.transform.lossyScale.z * _hitCol.size.z)));
//
//		Debug.Log("size:" + moveColSize + " " + hitColSize);
//
//		// 各軸の移動により重なった長さを求める
//		Vector3 dis = new Vector3(
//			(((aftPos.x + moveColSize.x) * 0.5f * moveVec.x) - ((colPos.x - hitColSize.x) * 0.5f * moveVec.x)),
//			(((aftPos.y + moveColSize.y) * 0.5f * moveVec.y) - ((colPos.y - hitColSize.y) * 0.5f * moveVec.y)), 0.0f);
//
//		// 重なった長さが0未満の軸は衝突していない
//		if (dis.x < 0.0f) {
//			dis.x = 0.0f;
//		}
//		if (dis.y < 0.0f) {
//			dis.y = 0.0f;
//		}
//		dis.x *= moveVec.x;
//		dis.y *= moveVec.y;
//		dis.z *= moveVec.z;
//		Debug.Log("dis:" + dis);
//
//		WeightManager moveWeightMng = _moveCol.GetComponent<WeightManager>(), hitWeightMng = _hitCol.GetComponent<WeightManager>();
//		MoveManager hitMoveMng = _hitCol.GetComponent<MoveManager>();
//
//		// _hitCol側が押される場合
//		if (((moveWeightMng != null) && (hitWeightMng != null) && (hitMoveMng != null)) &&
//			!_disExtFlg &&										// 押し出し不可フラグがfalse
//			(moveWeightMng.WeightLv > hitWeightMng.WeightLv) &&	// _hitCol側の方が軽い
//			!hitMoveMng.extrusionIgnore) {                  // _hitColの他オブジェクトからの押し出しが無効
//
//			Debug.LogWarning("押し出したい");
//
//			// 押し出そうとする
//			Vector3 extResMove;
//			Move((_move - dis), _hitCol, _mask, out extResMove);
//
//			// 押し出し切れなかった場合
//			if ((_move - dis) != extResMove) {
//				Debug.LogWarning("押し出しきれない");
//
//				// それ以上は押せないので自身も移動できる限りの移動に留める
//				ExtrusionMove(_move, _moveCol, _hitCol, _mask, true);	// 押し出し不可でもう一度押し出され移動処理
//			}
//			// 押し出し切れた場合
//			else {
//				Debug.LogWarning("押し出しきれた");
//
//				// 自身はそのまま移動
//				resMove = (_move - dis);
//				_moveCol.transform.position += _move;
//			}
//		}
//		// _moveCol側が押される場合
//		else {
//			Debug.LogWarning("押し出される");
//
//			// 移動できる限りの移動を行う
////			resMove = (_move - dis);
//			_moveCol.transform.position += _move;
//
//			Debug.LogWarning("pos:" + _moveCol.transform.position);
//		}
//
//		Debug.Log("_move:" + _move + " resMove(return):" + resMove);
//
//		return resMove;
//	}
}
