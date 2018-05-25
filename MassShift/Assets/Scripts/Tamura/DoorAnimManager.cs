using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimManager : MonoBehaviour {
    public List<GameObject> doorList = new List<GameObject>();
    public GameObject StageName;

    int openDoorCount;
    int closeDoorCount;

    bool isDoorOpenEnd;
    bool isDoorCloseEnd;

	void Start () {
        openDoorCount = 0;
        closeDoorCount = 0;
	}

    void Update() {
        Debug.Log(doorList.Count);
        if (openDoorCount >= doorList.Count) {
            // ドア開くアニメーションが全て終了したらステージ名フェード開始
            StageName.SetActive(true);
            if (!StageName.GetComponent<MonoColorFade>().IsFadeEnd()) {
                // ステージ名フェードアウトが終了した
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
