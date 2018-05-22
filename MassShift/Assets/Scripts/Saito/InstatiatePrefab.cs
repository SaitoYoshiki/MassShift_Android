using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstatiatePrefab : MonoBehaviour {

	[SerializeField, PrefabOnly]
	List<GameObject> mPrefabList;

	private void Awake() {
		foreach (var p in mPrefabList) {
			if (p == null) {
				return;	//要素にnullが来るまでインスタンス化し続ける
			}
			Instantiate(p, transform);
		}
	}
}
