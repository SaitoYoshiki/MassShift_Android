using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileWeight : MonoBehaviour {
	// 四辺に存在する当たり判定
	[SerializeField] GameObject[] fourSideCol = new GameObject[4];

	// Use this for initialization
	//	void Start () {}

	// Update is called once per frame
	//	void Update () {}

	public List<GameObject> GetPileBoxList(Vector3 _vec) {
		List<GameObject> ret = new List<GameObject>();
		AddPileBoxList(ret, _vec);
		return ret;
	}

	void AddPileBoxList(List<GameObject> _boxList, Vector3 _vec) {
		List<GameObject> forward = new List<GameObject>();  // 対象コライダー
		List<GameObject> back = new List<GameObject>();     // 除外コライダー

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
		List<GameObject> hitObjList = new List<GameObject>();
		while (hitColList.Count > 0) {
			hitObjList.Add(hitColList[0].gameObject);
			hitColList.RemoveAt(0);
		}

		// 重複を排除
		RemoveDuplicateGameObject(hitObjList);

		// 自身を排除
		hitObjList.Remove(gameObject);

		// 指定方向の反対側の四辺コライダーに接触している対象オブジェクトのコライダーをリスト化	
		List<Collider> outColList = new List<Collider>();
		for (int idx = 0; idx < back.Count; idx++) {
			outColList.AddRange(Physics.OverlapBox(back[idx].transform.position, back[idx].transform.localScale * 0.5f, back[idx].transform.rotation, mask));
		}

		// 除外オブジェクトのコライダーのリストをオブジェクトのリストに変換
		List<GameObject> outObjList = new List<GameObject>();
		while (outColList.Count > 0) {
			outObjList.Add(outColList[0].gameObject);
			outColList.RemoveAt(0);
		}

		// 重複を排除
		RemoveDuplicateGameObject(outObjList);

		// 自身を排除
		outObjList.Remove(gameObject);

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
		RemoveDuplicateGameObject(_boxList);

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
	void DotFourSideCollider(Vector3 _vec, List<GameObject> _forward, List<GameObject> _back) {
		// 四辺コライダーが設定されていない場合
		if (fourSideCol.Length == 0) {
			Debug.LogError("四辺コライダー配列の要素が存在していません。");
			return;
		}

		// 正規化
		_vec = _vec.normalized;

		// リストの初期化
		_forward.Clear();
		_back.Clear();

		// 全ての四辺コライダーについて指定の方向に存在するか判定して振り分ける
		for (int idx = 0; idx < fourSideCol.Length; idx++) {
			Vector3 vec = fourSideCol[idx].transform.position - transform.position;
			if (Vector3.Dot(vec, _vec) > 0.0f) {
				_forward.Add(fourSideCol[idx]);
			} else {
				_back.Add(fourSideCol[idx]);
			}
		}
	}

	int RemoveDuplicateGameObject(List<GameObject> _list) {
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
