using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour {
    [SerializeField]
    GameObject ResultCanvas;

    // ゴールしたかどうか、GameManager側から変更
    public bool canGoal;

	void Update () {
        // ゴールしていないなら何もしない
        if (!canGoal) {
            return;
        }
        // ゴールしたなら
        else {
            // リザルト画面が表示されているなら何もしない
            if (ResultCanvas.activeSelf) {
                return;
            }
            // リザルト画面が表示されていなければ
            else {
                // ポーズ機能を無効に
                GetComponent<Pause>().enabled = false;

                // リザルト画面を表示
                ResultCanvas.SetActive(true);

                // クリアしたのが最終ステージならば
                if (!Area.ExistNextStage(Area.GetAreaNumber(), Area.GetStageNumber())) {
                    // 「次のステージへ」ボタンを出さない
                    ResultCanvas.transform.Find("NextStage").gameObject.SetActive(false);
                }
            }
        }
	}
}
