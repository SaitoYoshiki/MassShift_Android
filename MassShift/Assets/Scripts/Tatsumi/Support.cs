using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Support {
	const float FloatMin = 0.001f;

	// 指定コライダーが接触するコライダーのリストを返す
	static public List<RaycastHit> GetColliderHitInfoList(Collider _col, Vector3 _move, int _mask, List<Collider> _ignoreColList = null) {
		List<RaycastHit> hitInfoList = new List<RaycastHit>();

		// コライダーが存在しなければ
		if (_col == null) {
			Debug.LogWarning("コライダーが見つかりませんでした。衝突が検知されません。" + Support.ObjectInfoToString(_col.gameObject));
			return hitInfoList;
		}

		// 移動量に0があると判定されないので小単位で増加
		_move = new Vector3(
			Mathf.Max(Mathf.Abs(_move.x), FloatMin) * Mathf.Sign(_move.x),
			Mathf.Max(Mathf.Abs(_move.y), FloatMin) * Mathf.Sign(_move.y),
			Mathf.Max(Mathf.Abs(_move.z), FloatMin) * Mathf.Sign(_move.z));

		// コライダーを特定し衝突を検知
		System.Type colType = _col.GetType();
		if (colType == typeof(BoxCollider)) {
			BoxCollider boxCol = (BoxCollider)_col;
			hitInfoList.AddRange(Physics.BoxCastAll(boxCol.bounds.center, boxCol.bounds.size * 0.5f, _move.normalized, boxCol.transform.rotation, _move.magnitude, _mask));
//			Debug.LogError("sap _move:" + _move);
		} else if (colType == typeof(SphereCollider)) {
			SphereCollider sphereCol = (SphereCollider)_col;
			hitInfoList.AddRange(Physics.SphereCastAll(sphereCol.bounds.center, sphereCol.radius, _move.normalized, _move.magnitude, _mask));
		} else if (colType == typeof(CapsuleCollider)) {
			CapsuleCollider capsuleCol = (CapsuleCollider)_col;
			hitInfoList.AddRange(Physics.CapsuleCastAll(
				(capsuleCol.bounds.center) + capsuleCol.transform.up * (capsuleCol.height - capsuleCol.radius) * 0.5f,
				(capsuleCol.bounds.center) - capsuleCol.transform.up * (capsuleCol.height - capsuleCol.radius) * 0.5f,
				capsuleCol.radius, _move.normalized, _move.magnitude, _mask));
		} else if (colType == typeof(MeshCollider)) {
			Debug.LogWarning("コライダーがMeshColliderでした。衝突が検知されません。" + Support.ObjectInfoToString(_col.gameObject));
		} else {
			Debug.LogWarning("コライダーがBoxCollider, SphereCollider, CapsuleCollider, MeshColliderのいずれでもありませんでした。衝突が検知されません。" + Support.ObjectInfoToString(_col.gameObject));
		}

		if (_ignoreColList == null) {
			_ignoreColList = new List<Collider>();
		}
		// 自身を判定無視対象に追加
		if (!_ignoreColList.Contains(_col)) {
			_ignoreColList.Add(_col);
		}

		// 判定無視対象をリストから除く
		foreach (var ignoreCol in _ignoreColList) {
			for (int idx = (hitInfoList.Count - 1); idx >= 0; idx--) {
				if (hitInfoList[idx].collider == ignoreCol) {
		//			hitInfoList.RemoveAt(idx);
				}
			}
		}

		// 自身との判定があれば除く
		for (int idx = (hitInfoList.Count - 1); idx >= 0; idx--) {
			if (hitInfoList[idx].collider.gameObject == _col.gameObject) {
				hitInfoList.RemoveAt(idx);
			}
		}

		return hitInfoList;
	}

	static public string ObjectInfoToString(GameObject _obj) {
		return ("\nname:" + _obj.name + " pos:" + _obj.transform.position);
	}

}
