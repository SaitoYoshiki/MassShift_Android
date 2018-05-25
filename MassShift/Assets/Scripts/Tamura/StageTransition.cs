using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTransition : MonoBehaviour {
    [SerializeField]
    List<GameObject> door = new List<GameObject>();

    int area;
    int stage;

    void Start() {
        area = Area.GetAreaNumber();
        stage = Area.GetStageNumber();

        //ActivateDoor();
    }

    // シーン開始時にエリアに対応したドアキャンバスをActivateする
    public void ActivateDoor() {
        if (area != 0) {
            door[area - 1].SetActive(true);
        }
    }

    // エリアに対応したドアを開ける
    public void OpenDoorParent() {
        door[area - 1].GetComponent<DoorAnimManager>().StartDoorOpen();
    }

    // エリアに対応したドアを閉じる
    public void CloseDoorParent() {
        door[area - 1].GetComponent<DoorAnimManager>().StartDoorClose();
    }

    // ドア開き演出が終了したかどうか取得
    public bool GetOpenEnd() {
        return door[area - 1].GetComponent<DoorAnimManager>().isOpenEnd();
    }

    // ドア閉め演出が終了したかどうか取得
    public bool GetCloseEnd() {
        return door[area - 1].GetComponent<DoorAnimManager>().isCloseEnd();
    }
}
