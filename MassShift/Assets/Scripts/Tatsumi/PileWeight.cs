using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileWeight : MonoBehaviour {
	// 四辺に存在する当たり判定
	FourSideCollider fourSideCol = null;
	FourSideCollider FourSideCol {
		get {
			if (fourSideCol == null) {
				fourSideCol = GetComponent<FourSideCollider>();
				if (fourSideCol == null) {
					Debug.LogError("FourSideColliderが見つかりませんでした。");
				}
			}
			return fourSideCol;
		}
	}

	// Use this for initialization
	//	void Start () {}

	// Update is called once per frame
	//	void Update () {}

	public List<Transform> GetPileBoxList(Vector3 _vec) {
		List<Transform> ret = new List<Transform>();
		AddPileBoxList(ret, _vec);
		return ret;
	}

	void AddPileBoxList(List<Transform> _boxList, Vector3 _vec) {
		List<Transform> forward = new List<Transform>();  // 対象コライダー
		List<Transform> back = new List<Transform>();     // 除外コライダー

		// 判定用マスク
		int mask = LayerMask.GetMask(new string[] { "Player", "Box" });

		//Debug.Log("AddChainBoxList");
		// 四辺コライダーを指定方向側と反対方向側に振り分け
		DotFourSideCollider(_vec, forward, back);

		// 指定方向側の四辺コライダーに接触している対象オブジェクトのコライダーをリスト化
		List<Collider> hitColList = new List<Collider>();
		for (int idx = 0; idx < forward.Count; idx++) {
			hitColList.AddRange(Physics.OverlapBox(forward[idx].transform.position, forward[idx].transform.localScale * 0.5f, forward[idx].transform.rotation, mask));
		}

		// 対象オブジェクトのコライダーのリストをオブジェクトのリストに変換
		List<Transform> hitObjList = new List<Transform>();
		while (hitColList.Count > 0) {
			hitObjList.Add(hitColList[0].transform);
			hitColList.RemoveAt(0);
		}

		// 重複を排除
		RemoveDuplicateObject(hitObjList);

		// 自身を排除
		hitObjList.Remove(transform);

		//パイルを持っていないオブジェクトは排除
		for(int i = hitObjList.Count - 1; i >= 0; i--) {
			if(hitObjList[i].GetComponent<PileWeight>() == null) {
				hitObjList.RemoveAt(i);
			}
		}


		// 指定方向の反対側の四辺コライダーに接触している対象オブジェクトのコライダーをリスト化	
		List<Collider> outColList = new List<Collider>();
		for (int idx = 0; idx < back.Count; idx++) {
			outColList.AddRange(Physics.OverlapBox(back[idx].transform.position, back[idx].transform.localScale * 0.5f, back[idx].transform.rotation, mask));
		}

		// 除外オブジェクトのコライダーのリストをオブジェクトのリストに変換
		List<Transform> outObjList = new List<Transform>();
		while (outColList.Count > 0) {
			outObjList.Add(outColList[0].transform);
			outColList.RemoveAt(0);
		}

		// 重複を排除
		RemoveDuplicateObject(outObjList);

		// 自身を排除
		outObjList.Remove(transform);

		// 指定方向から遠いコライダ－に接触している対象オブジェクトをリストから排除
		for (int outObjIdx = 0; outObjIdx < outObjList.Count; outObjIdx++) {
			hitObjList.Remove(outObjList[outObjIdx]);
		}

		// 既存リストに存在する排除対象オブジェクトをリストから除外
		for (int boxListIdx = 0; boxListIdx < _boxList.Count; boxListIdx++) {
			hitObjList.Remove(_boxList[boxListIdx]);
		}

		// リスト内の対象オブジェクトを既存リストと統合
		_boxList.AddRange(hitObjList);

		// リストの重複を排除
		RemoveDuplicateObject(_boxList);

		// 新たな対象オブジェクトそれぞれで再帰呼び出し
		for (int hitObjIdx = 0; hitObjIdx < hitObjList.Count; hitObjIdx++) {
			PileWeight otherBox = hitObjList[hitObjIdx].GetComponent<PileWeight>();
			if (otherBox != null) {
				// 再帰呼び出し
				otherBox.AddPileBoxList(_boxList, _vec);
			}
		}
	}

	// 四辺コライダーが指定方向に存在するか判定して振り分ける
	void DotFourSideCollider(Vector3 _vec, List<Transform> _forward, List<Transform> _back) {
		// 正規化
		_vec = _vec.normalized;

		// リストの初期化
		_forward.Clear();
		_back.Clear();

		// 全ての四辺コライダーについて指定の方向に存在するか判定して振り分ける
		foreach (var sideCol in FourSideCol.colList) {
			Vector3 vec = sideCol.transform.position - transform.position;
			if (Vector3.Dot(vec, _vec) > 0.75f) {
				_forward.Add(sideCol);
			} else {
				_back.Add(sideCol);
			}
		}
	}

	int RemoveDuplicateObject(List<Transform> _list) {
		int cnt = 0;
		// 対象リストから重複を排除
		for (int targetIdx = 0; targetIdx < _list.Count; targetIdx++) {
			// 以降の同様の要素を探す
			while (_list.LastIndexOf(_list[targetIdx]) > targetIdx) {
				// 重複している要素を排除
				_list.RemoveAt(_list.LastIndexOf(_list[targetIdx]));
				cnt++;
			}
		}
		return cnt;
	}
}
