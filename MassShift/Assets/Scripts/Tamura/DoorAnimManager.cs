using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimManager : MonoBehaviour {
    public List<GameObject> doorList = new List<GameObject>();
    public GameObject StageName;
    MonoColorFade monoFade;

    string sceneName;       // 現在のシーン名

    int openDoorCount;      // 開き終わったドアの数
    int closeDoorCount;     // 閉まり終わったドアの数

    bool isDoorOpenEnd;     // ドアの開き演出が終わったかどうか
    bool isDoorCloseEnd;    // ドアの閉まり演出が終わったかどうか

	void Start () {
        openDoorCount = 0;
        closeDoorCount = 0;

        monoFade = StageName.GetComponent<MonoColorFade>();
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
	}

    void Update() {
        // ドアが全て開き終わったら
        if (openDoorCount >= doorList.Count) {
            // ステージセレクトシーン以外では
            if (sceneName != "StageSelect") {
                // ステージ名フェードイン開始
                StageName.SetActive(true);
                if (!monoFade.IsFading()) {
                    Debug.Log("文字フェード終了");
                    // ステージ名フェードアウトが終了した
                    isDoorOpenEnd = true;
                    Debug.Log("開き演出終了" + isDoorOpenEnd);
                }
            }
            // ステージセレクトシーンでは
            else {
                // ステージ名を出さず開き演出終了
                isDoorOpenEnd = true;
            }
        }

        if (closeDoorCount >= doorList.Count) {
            // ドア閉まるアニメーションが全て終了した
            isDoorCloseEnd = true;
        }
    }

    // ドア開き開始(親から呼び出し)
    public void StartDoorOpen() {
        foreach (GameObject door in doorList) {
            door.GetComponent<StageChangeScenematic>().StartOpening();
        }
    }

    // ドア閉じ開始(親から呼び出し)
    public void StartDoorClose() {
        foreach (GameObject door in doorList) {
            door.GetComponent<StageChangeScenematic>().StartClosing();
        }
    }

    // 開き終わったか
    public bool isOpenEnd() {
        return isDoorOpenEnd;
    }

    // 閉じ終わったか
    public bool isCloseEnd() {
        return isDoorCloseEnd;
    }

    public void OpenCountPlus() {
        openDoorCount++;
    }

    public void CloseCountPlus() {
        closeDoorCount++;
    }
}
