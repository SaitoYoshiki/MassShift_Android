using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {
	[SerializeField] WeightManager fromMng = null;
	[SerializeField] WeightManager toMng = null;
	[SerializeField] GameObject fromCursorPrefab = null;
	[SerializeField] GameObject toCursorPrefab = null;
	[SerializeField] float holdTime = 1.0f;	// ホールドに必要な時間(秒)
	GameObject fromCursor = null;
	GameObject toCursor = null;
	[SerializeField] float holdingTime = 0.0f; // 押下したオブジェクトに継続してオーバーした時間(秒)
	bool failedFlg = false;
	int layerMask;
	
	// Use this for initialization
	void Start () {
		// レイヤーマスクの作成
		layerMask = LayerMask.GetMask(new string[] { "Box" });
	}

	// Update is called once per frame
	void Update () {
		if (toMng == null) {
			DragAim();
		}
		else {
			Shot();
		}
	}

	void DragAim() {
		if (fromMng = null) {
			// 移し元選択	
			if (Input.GetAxis("Shot0") > 0.0f) {
				RaycastHit hitInfo;
				Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, layerMask);

				// マウスカーソルが対象となるオブジェクトを指していれば
				if (hitInfo.collider != null) {
					// 対象オブジェクトのWeightManagerを保持
					fromMng = hitInfo.collider.GetComponent<WeightManager>();
					// 確認
					if (fromMng == null) {
						Debug.LogError("WeightManagerが付いていないオブジェクトが移し元対象として選択されました。");
						failedFlg = true;
						return;
					}
				}
				else {
					// 選択失敗
					failedFlg = true;
				}
			}
			else {
				// 選択失敗解除
				failedFlg = false;
			}
		}
		else {
			// マウスカーソルが指しているオブジェクトを取得
			RaycastHit hitInfo;
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, layerMask);

			// ホールド選択がまだ行われていないなら
			if (holdingTime < holdTime) {
				// ホールド選択
				if (hitInfo.collider.GetComponent<WeightManager>() == fromMng) {
					holdingTime += Time.deltaTime;
					if (holdingTime >= holdTime) {
						holdingTime = holdTime;
					}
				}
				// ホールド完了前にマウスカーソルが移し元オブジェクトから外れたら
				else {
					holdingTime = 0.0f;
				}
			}

			// 移し先選択
			if (Input.GetAxis("Shot0") <= 0.0f) {
				// マウスカーソルが対象となるオブジェクトを指していれば
				if (hitInfo.collider != null) {
					// 対象オブジェクトのWeightManagerを保持
					toMng = hitInfo.collider.GetComponent<WeightManager>();
					// 確認
					if (toMng == null) {
						Debug.LogError("WeightManagerが付いていないオブジェクトが移し先対象として選択されました。");
						return;
					}
				}
			}
		}
	}

	void Shot() {
		// ショット


		// 対象オブジェクトをリセット
		fromMng = null;
		toMng = null;
	}
}
