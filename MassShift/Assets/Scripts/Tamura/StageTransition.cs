using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTransition : MonoBehaviour {
    public enum DOOR {
        UP = 0,
        DOWN,
        RIGHT,
        LEFT
    }

    bool isTransitioning;
    bool isDoorAnim;

    // ドアの開閉時間(秒)
    public float doorAnimTime;

    // ステージ名表示時間(秒)
    public float stageInfoTime;

    [SerializeField]
    List<GameObject> door = new List<GameObject>();

    [SerializeField]
    GameObject stageNameText;

    [SerializeField, Range(0.0f, 1.0f)]
    float animPer;
    float beforeScenematicPer;

    [SerializeField, Range(0.0f, 1.0f)]
    float animStopPer;
    float beforeScenematicStopPer;

    float startTime;
    float nowTime;
    float secondTime;

    public DOOR doorType;
    Vector3 startPos;
    Vector3 secondPos;

    int area;
    int stage;

    void Start() {
        area = Area.GetAreaNumber();
        stage = Area.GetStageNumber();
        startPos = this.transform.localPosition;

        isTransitioning = false;
        isDoorAnim = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            if (!isTransitioning) {
                isTransitioning = true;
                startTime = Time.realtimeSinceStartup;
            }
        }

        if (isTransitioning) {
            StartCoroutine(CloseDoor());
        }

        Debug.Log(startPos);
    }

    // ドア開く演出
    IEnumerator OpenDoor() {
        nowTime = Time.realtimeSinceStartup - startTime;

        float timePer = nowTime / doorAnimTime;
        Debug.Log("per" + (nowTime / doorAnimTime));

        if (timePer <= animPer) {
            this.transform.localPosition = new Vector3(startPos.x - startPos.x * timePer, startPos.y - startPos.y * timePer, 0.0f);
        }
        else if (timePer > animPer && timePer <= (animPer + animStopPer)) {
            secondPos = this.transform.localPosition;
            secondTime = nowTime;
        }
        else if (timePer > (animPer + animStopPer) && timePer <= 1.0f) {
            switch (doorType) {
                case DOOR.UP:
                    if (this.transform.localPosition.y > 0.0f) {
                        this.transform.localPosition = new Vector3(0.0f, secondPos.y - secondPos.y * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isTransitioning = false;
                    }
                    break;

                case DOOR.DOWN:
                    if (this.transform.localPosition.y < 0.0f) {
                        this.transform.localPosition = new Vector3(0.0f, secondPos.y - secondPos.y * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isTransitioning = false;
                    }
                    break;

                case DOOR.RIGHT:
                    if (this.transform.localPosition.x > 0.0f) {
                        this.transform.localPosition = new Vector3(secondPos.x - secondPos.x * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f, 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isTransitioning = false;
                    }
                    break;

                case DOOR.LEFT:
                    if (this.transform.localPosition.x < 0.0f) {
                        this.transform.localPosition = new Vector3(secondPos.x - secondPos.x * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f, 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isTransitioning = false;
                    }
                    break;
            }
        }
        else {
            this.transform.localPosition = Vector3.zero;
            isTransitioning = false;
        }

        yield return null;
    }

    // ドア閉まる演出
    IEnumerator CloseDoor() {
        nowTime = Time.realtimeSinceStartup - startTime;

        float timePer = nowTime / doorAnimTime;
        Debug.Log("per" + (nowTime / doorAnimTime));

        if (timePer <= animPer) {
            this.transform.localPosition = new Vector3(startPos.x - startPos.x * timePer, startPos.y - startPos.y * timePer, 0.0f);
        }
        else if (timePer > animPer && timePer <= (animPer + animStopPer)) {
            secondPos = this.transform.localPosition;
            secondTime = nowTime;
        }
        else if (timePer > (animPer + animStopPer) && timePer <= 1.0f) {
            this.transform.localPosition = new Vector3(secondPos.x - secondPos.x * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f, 0.0f);
        }
        else {
            this.transform.localPosition = Vector3.zero;
            isTransitioning = false;
        }

        yield return null;
    }

    // 数値に変更があった際呼ばれる
    void OnValidate() {

    }
}
