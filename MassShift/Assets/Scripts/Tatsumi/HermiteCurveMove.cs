using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermiteCurveMove : MonoBehaviour {
	[SerializeField] Vector3 startPoint;
	[SerializeField] Vector3 startVec;
	[SerializeField] Vector3 endPoint;
	[SerializeField] Vector3 endVec;
	[SerializeField] float necessaryTime = 1.0f;	// 所要時間
	[SerializeField] float elapsedTime = 0.0f;		// 経過時間

	// Use this for initialization
//	void Start () {}
	
	// Update is called once per frame
	void Update () {
		// 時間経過
		elapsedTime += Time.deltaTime;

		// 位置の更新
		transform.position = GetHermiteCurvePoint();
	}

	void SetPoints(Vector3 _startPoint, Vector3 _endPoint, Vector3 _approachPoint) {
		// 開始点を設定
		startPoint = _startPoint;

		// 終了点を設定
		endPoint = _endPoint;

		// 開始点から目安点へ向かうベクトルを設定
		startVec = _approachPoint - _startPoint;

		// 終了点から目安点の反対側へ向かうベクトルを設定
		endVec = _endPoint - _approachPoint;
	}

	Vector3 GetHermiteCurvePoint() {
		Vector3 ret;
		float u1 = (elapsedTime / necessaryTime);
		float u2 = u1 * u1;
		float u3 = u1 * u1 * u1;
		float p0 =	2 * u3 - 3 * u2 + 1;
		float v0 =		u3 - 2 * u2 + u1;
		float p1 = -2 * u3 + 3 * u2;
		float v1 =		u3 -	 u2;
		ret = new Vector3(
			startPoint.x * p0 + startVec.x * v0 + endPoint.x * p1 + endVec.x * v1,
			startPoint.y * p0 + startVec.y * v0 + endPoint.y * p1 + endVec.y * v1,
			startPoint.z * p0 + startVec.z * v0 + endPoint.z * p1 + endVec.z * v1);
		return ret;
	}
}
