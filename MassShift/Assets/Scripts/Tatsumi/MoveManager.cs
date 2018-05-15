using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour {
	// 定数
	const float ColMargin = 0.0001f;

	// 優先度
	public enum MoveType {
		min = -3,

		gravity = -2,	// 重力
		prevMove = -1,	// 前回移動の保持
		other = 0,		// その他

		max = 1,
		all = int.MaxValue,
	}

	class MoveInfo {
		public Vector3 vec;
		public MoveType type;
	}

	[SerializeField] Vector3 prevMove;				// 前回の移動量
	public Vector3 PrevMove {
		get {
			return prevMove;
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
				GravityForce = WeightMng.NowWeightForce;
			}
			return gravityForce;
		}
	}

	[SerializeField] float airResistance = 0.025f;	// 空気抵抗
	[SerializeField] float maxSpd = 10.0f;			// 最高速度
	[SerializeField] Collider useCol = null;		// 当たり判定を行うコライダー

	[SerializeField] float gravityCustomTime = 0.0f;	// 通常の重力加速度を一時停止する時間
	public float GravityCustomTime {
		get {
			return gravityCustomTime;
		}
		set {
			gravityCustomTime = value;

			// 通常の重力加速度を一時停止しているかのフラグを更新
			GravityCustomFlg = (Time.time <= gravityCustomTime);
		}
	}

	[SerializeField] bool gravityCustomFlg;	// 通常の重力加速度を一時停止しているか
	bool GravityCustomFlg {
		get {
			GravityCustomFlg = (Time.time <= gravityCustomTime);
			return gravityCustomFlg;
		}
		set {
			gravityCustomFlg = value;
		}
	}

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
		if (useCol == null) {
			useCol = GetComponent<BoxCollider>();
		}
		if (useCol == null) {
			useCol = GetComponent<SphereCollider>();
		}
		if (useCol == null) {
			useCol = GetComponent<CapsuleCollider>();
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		// 前回の加速度
		AddMove(prevMove, MoveType.prevMove);

		// 重力加速度
		if (useGravity && !Land.IsLanding) {
			AddMove(new Vector3(0.0f, gravityForce * Time.deltaTime, 0.0f), MoveType.gravity);
		}

		// 無視指定優先度の移動を削除
//		for()


//		// 無視指定優先度より低い優先度の移動を削除
//		for (int idx = 0; idx < moveList.Count; idx++) {
//			if (moveList[idx].priority < stopVirticalMoveType) {
//				moveList[idx].vec = new Vector3(moveList[idx].vec.x, 0.0f, moveList[idx].vec.z);
//			}
//			if (moveList[idx].priority < stopHorizontalMoveType) {
//				moveList[idx].vec = new Vector3(0.0f, moveList[idx].vec.y, moveList[idx].vec.z);
//			}
//		}
//		stopVirticalMoveType = MovePriority.min;
//		stopHorizontalMoveType = MovePriority.min;

		// 今回の移動量を求める
		Vector3 move = TotalMove;

		// 空気抵抗
		move -= (move.normalized * move.magnitude * airResistance);

		// 最高速度制限
		move = move.normalized * Mathf.Min(move.magnitude, maxSpd);

		// 移動
		Vector3 resMove;    // 実際に移動出来た移動量
		Move(move * Time.deltaTime, useCol, LayerMask.GetMask(new string[] { "Stage", "Player", "Box" }), out resMove);

		// 今回の移動量を保持
		//prevMove = resMove;
		prevMove = move;

		// 計算済みの力を削除
		moveList.Clear();
	}

	public void AddMove(Vector3 _move, MoveType _type = 0) {
		MoveInfo moveInfo = new MoveInfo();
		moveInfo.vec = _move;
		moveInfo.type = _type;
		moveList.Add(moveInfo);
	}

	static public bool Move(GameObject _obj, Vector3 _move, Collider _col, int _mask, out Vector3 _resMove, out Collider _hitCol) {
		_resMove = Vector3.zero;
		_hitCol = null;

		// オブジェクトが存在するか
		if(_obj == null) {
			Debug.LogError("_objが設定されていません。");
			return false;
		}

		// 衝突リストを取得
		List<RaycastHit> hitInfoList = Support.GetColliderHitInfoList(_col, _move, _mask);

		// 衝突しなかったら
		if (hitInfoList.Count == 0) {
			// 今回の移動量を保持
			if (_obj.GetComponent<MoveManager>() != null) {
				_obj.GetComponent<MoveManager>().prevMove = _move;
			}

			// そのままの移動量を移動
			_resMove = _move;
			_obj.transform.position += _resMove;

			return false;
		}
		// 衝突したら
		else {
			// 最も近い衝突コライダーとの距離
			float dis = float.MaxValue;
			foreach (var hitInfo in hitInfoList) {
				if (dis > hitInfo.distance) {
					dis = hitInfo.distance;
					_hitCol = hitInfo.collider;
				}
			}

			// 衝突直前まで移動する移動量を移動
			_resMove = (_move.normalized * (dis - ColMargin));
			_obj.transform.position += _resMove;
			Debug.Log("位置調整 " + _obj.transform.position);

			// 接地
			if (_obj.GetComponent<Landing>() != null) {
				_obj.GetComponent<Landing>().CollisionLanding(_move);
			}

			// 移動量を削除
			if (_obj.GetComponent<MoveManager>() != null) {
				Debug.Log("MoveManager:接地");
				_obj.GetComponent<MoveManager>().StopMoveVirtical();
				_obj.GetComponent<MoveManager>().StopMoveHorizontal();
			}

			return true;
		}
	}
	static public bool Move(GameObject _obj, Vector3 _move, Collider _col, int _mask) {
		Vector3 dummyResMove;
		Collider dummyHitCol;
		return Move(_obj, _move, _col, _mask, out dummyResMove, out dummyHitCol);
	}
	static public bool Move(GameObject _obj, Vector3 _move, Collider _col, int _mask, out Vector3 _resMove) {
		Collider dummyHitCol;
		return Move(_obj, _move, _col, _mask, out _resMove, out dummyHitCol);
	}
	static public bool Move(GameObject _obj, Vector3 _move, Collider _col, int _mask, out Collider _hitCol) {
		Vector3 dummyResMove;
		return Move(_obj, _move, _col, _mask, out dummyResMove, out _hitCol);
	}

	public bool Move(Vector3 _move, Collider _col, int _mask, out Vector3 _resMove, out Collider _hitCol) {
		return MoveManager.Move(gameObject, _move, _col, _mask, out _resMove, out _hitCol);
	}
	public bool Move(Vector3 _move, Collider _col, int _mask) {
		Vector3 dummyResMove;
		Collider dummyHitCol;
		return Move(_move, _col, _mask, out dummyResMove, out dummyHitCol);
	}
	public bool Move(Vector3 _move, Collider _col, int _mask, out Vector3 _resMove) {
		Collider dummyHitCol;
		return Move(_move, _col, _mask, out _resMove, out dummyHitCol);
	}
	public bool Move(Vector3 _move, Collider _col, int _mask, out Collider _hitCol) {
		Vector3 dummyResMove;
		return Move(_move, _col, _mask, out dummyResMove, out _hitCol);
	}

	// 移動量ではなく移動先位置を基準に移動する
	static bool MoveTo(GameObject _obj, Vector3 _pos, Collider _col, int _mask, out Vector3 _resMove, out Collider _hitCol) {
		return Move(_obj, _pos - _col.transform.position, _col, _mask, out _resMove, out _hitCol);
	}
	static bool MoveTo(GameObject _obj, Vector3 _pos, Collider _col, int _mask) {
		Vector3 dummyResMove;
		Collider dummyHitCol;
		return MoveTo(_obj, _pos, _col, _mask, out dummyResMove, out dummyHitCol);
	}
	static bool MoveTo(GameObject _obj, Vector3 _pos, Collider _col, int _mask, out Vector3 _resMove) {
		Collider dummyHitCol;
		return MoveTo(_obj, _pos, _col, _mask, out _resMove, out dummyHitCol);
	}
	static bool MoveTo(GameObject _obj, Vector3 _pos, Collider _col, int _mask, out Collider _hitCol) {
		Vector3 dummyResMove;
		return MoveTo(_obj, _pos, _col, _mask, out dummyResMove, out _hitCol);
	}

	bool MoveTo(Vector3 _pos, Collider _col, int _mask, out Vector3 _resMove, out Collider _hitCol) {
		return MoveManager.MoveTo(gameObject, _pos, _col, _mask, out _resMove, out _hitCol);
	}
	bool MoveTo(Vector3 _pos, Collider _col, int _mask) {
		Vector3 dummyResMove;
		Collider dummyHitCol;
		return MoveTo(_pos, _col, _mask, out dummyResMove, out dummyHitCol);
	}
	bool MoveTo(Vector3 _pos, Collider _col, int _mask, out Vector3 _resMove) {
		Collider dummyHitCol;
		return MoveTo(_pos, _col, _mask, out _resMove, out dummyHitCol);
	}
	bool MoveTo(Vector3 _pos, Collider _col, int _mask, out Collider _hitCol) {
		Vector3 dummyResMove;
		return MoveTo(_pos, _col, _mask, out dummyResMove, out _hitCol);
	}

	public void StopUpperMoveVirtical(MoveType _stopTypePriority = MoveType.other) {
		for (MoveType type = _stopTypePriority; type <= (MoveType.max - 1); type++) {
			if (stopVirticalMoveType.Contains(type)) {
				stopVirticalMoveType.Add(type);
			}
		}
	}
	public void StopDownerMoveVirtical(MoveType _stopTypePriority = MoveType.other) {
		for (MoveType type = (MoveType.min + 1); type <= _stopTypePriority; type++) {
			if (stopVirticalMoveType.Contains(type)) {
				stopVirticalMoveType.Add(type);
			}
		}
	}
	public void StopUpperMoveHorizontal(MoveType _stopTypePriority = MoveType.other) {
		for (MoveType type = _stopTypePriority; type <= (MoveType.max - 1); type++) {
			if (stopHorizontalMoveType.Contains(type)) {
				stopHorizontalMoveType.Add(type);
			}
		}
	}
	public void StopDownerMoveHorizontal(MoveType _stopTypePriority = MoveType.other) {
		for (MoveType type = (MoveType.min + 1); type <= _stopTypePriority; type++) {
			if (stopHorizontalMoveType.Contains(type)) {
				stopHorizontalMoveType.Add(type);
			}
		}
	}
	public void StopMoveHorizontal(MoveType _stopType = MoveType.all) {
		Debug.Log("StopMoveHorizontal\n" + "name:" + name + " position:" + transform.position);
		stopHorizontalMoveType.Add(_stopType);
	}

	public void StopMoveVirtical(MoveType _stopType = MoveType.all) {
		Debug.LogWarning("StopMoveVirtical\n" + "name:" + name + " position:" + transform.position);
		stopVirticalMoveType.Add(_stopType);
	}

	public float AddGravityCustomTime(float _time) {
		return (GravityCustomTime + _time);
	}

}
