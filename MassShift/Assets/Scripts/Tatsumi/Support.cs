using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Support {
	// 指定コライダーが接触するコライダーのリストを返す
	static public List<RaycastHit> GetColliderHitInfoList(Collider _col, Vector3 _move, int _mask) {
		List<RaycastHit> hitInfoList = new List<RaycastHit>();

		// コライダーが存在しなければ
		if (_col == null) {
			Debug.LogWarning("コライダーが見つかりませんでした。衝突が検知されません。" + Support.ObjectInfoToString(_col.gameObject));
			return hitInfoList;
		}

		// 移動量に0があると判定されないので最小単位で増加
		_move = new Vector3(Mathf.Max(_move.x, 0.1f), Mathf.Max(_move.y, 0.1f), Mathf.Max(_move.z, 0.1f));

		// コライダーを特定し衝突を検知
		System.Type colType = _col.GetType();
		if (colType == typeof(BoxCollider)) {
			BoxCollider boxCol = (BoxCollider)_col;
			hitInfoList.AddRange(Physics.BoxCastAll(boxCol.bounds.center, boxCol.bounds.size * 0.5f, _move.normalized, boxCol.transform.rotation, _move.magnitude, _mask));
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
