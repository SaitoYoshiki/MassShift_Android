using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageChangeScenematic : MonoBehaviour {
    public enum DOOR {
        UP = 0,
        DOWN,
        RIGHT,
        LEFT
    }

    bool isOpening = false;
    bool isClosing = false;
    public float doorAnimTime;

    [SerializeField, Range(0.0f, 1.0f)]
    float scenematicPercent;
    float beforeScenematicPer;

    [SerializeField, Range(0.0f, 1.0f)]
    float scenematicStopPercent;
    float beforeScenematicStopPer;

    float startTime;
    float nowTime;
    float secondTime;

    public DOOR doorType;
    Vector3 startPos;
    Vector3 secondPos;

    int area;
    int stage;

	void Start () {
        area = Area.GetAreaNumber();
        stage = Area.GetStageNumber();
        startPos = this.transform.localPosition;

        isOpening = true;
        startTime = Time.realtimeSinceStartup;
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            if (!isOpening) {
                isOpening = true;
                startTime = Time.realtimeSinceStartup;
            }
        }

        if (isOpening) {
            StartCoroutine(OpenDoor());
        }

        // シーン切り替え時に呼ばれるよう後で変更
        if (Input.GetKeyDown(KeyCode.D)) {
            if (!isClosing) {
                isClosing = true;
                startTime = Time.realtimeSinceStartup;
            }
        }

        if (isClosing) {
            StartCoroutine(CloseDoor());
        }

        Debug.Log(startPos);
    }

    // ドア開く演出
    IEnumerator OpenDoor() {
        nowTime = Time.realtimeSinceStartup - startTime;

        float timePer = nowTime / doorAnimTime;
        Debug.Log("per" + (nowTime / doorAnimTime));

        // ここらへんlerpで書き直したほうが良さげ
        if (timePer <= scenematicPercent) {
            this.transform.localPosition = new Vector3(startPos.x * timePer, startPos.y * timePer, 0.0f);
        }
        else if (timePer > scenematicPercent && timePer <= (scenematicPercent + scenematicStopPercent)) {
            secondPos = this.transform.localPosition;
            secondTime = nowTime;
        }
        else if (timePer > (scenematicPercent + scenematicStopPercent) && timePer <= 1.0f) {
            switch (doorType) {
                case DOOR.UP:
                    if (this.transform.localPosition.y > 0.0f) {
                        timePer = (nowTime - secondTime) / (doorAnimTime - secondTime);
                        this.transform.localPosition = new Vector3(secondPos.x + (startPos.x - secondPos.x) * timePer, secondPos.y + (startPos.y - secondPos.y) * timePer, 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isOpening = false;
                    }
                    break;

                case DOOR.DOWN:
                    if (this.transform.localPosition.y < 0.0f) {
                        timePer = (nowTime - secondTime) / (doorAnimTime - secondTime);
                        this.transform.localPosition = new Vector3(secondPos.x + (startPos.x - secondPos.x) * timePer, secondPos.y + (startPos.y - secondPos.y) * timePer, 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isOpening = false;
                    }
                    break;

                case DOOR.RIGHT:
                    if (this.transform.localPosition.x > 0.0f) {
                        timePer = (nowTime - secondTime) / (doorAnimTime - secondTime);
                        this.transform.localPosition = new Vector3(secondPos.x + (startPos.x - secondPos.x) * timePer, secondPos.y + (startPos.y - secondPos.y) * timePer, 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isOpening = false;
                    }
                    break;

                case DOOR.LEFT:
                    if (this.transform.localPosition.x < 0.0f) {
                        timePer = (nowTime - secondTime) / (doorAnimTime - secondTime);
                        this.transform.localPosition = new Vector3(secondPos.x + (startPos.x - secondPos.x) * timePer, secondPos.y + (startPos.y - secondPos.y) * timePer, 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isOpening = false;
                    }
                    break;
            }
        }
        else {
            isOpening = false;
        }

        yield return null;
    }

    // ドア閉まる演出
    IEnumerator CloseDoor() {
        nowTime = Time.realtimeSinceStartup - startTime;

        float timePer = nowTime / doorAnimTime;
        Debug.Log("per" + (nowTime / doorAnimTime));

        if (timePer <= scenematicPercent) {
            this.transform.localPosition = new Vector3(startPos.x - startPos.x * timePer, startPos.y - startPos.y * timePer, 0.0f);
        }
        else if (timePer > scenematicPercent && timePer <= (scenematicPercent + scenematicStopPercent)) {
            secondPos = this.transform.localPosition;
            secondTime = nowTime;
        }
        else if (timePer > (scenematicPercent + scenematicStopPercent) && timePer <= 1.0f) {
            switch (doorType) {
                case DOOR.UP:
                    if (this.transform.localPosition.y > 0.0f) {
                        this.transform.localPosition = new Vector3(0.0f, secondPos.y - secondPos.y * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isClosing = false;
                    }
                break;

                case DOOR.DOWN:
                if (this.transform.localPosition.y < 0.0f) {
                    this.transform.localPosition = new Vector3(0.0f, secondPos.y - secondPos.y * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f);
                }
                else {
                    this.transform.localPosition = Vector3.zero;
                    isClosing = false;
                }
                break;

                case DOOR.RIGHT:
                if (this.transform.localPosition.x > 0.0f) {
                    this.transform.localPosition = new Vector3(secondPos.x - secondPos.x * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f, 0.0f);
                }
                else {
                    this.transform.localPosition = Vector3.zero;
                    isClosing = false;
                }
                break;

                case DOOR.LEFT:
                if (this.transform.localPosition.x < 0.0f) {
                    this.transform.localPosition = new Vector3(secondPos.x - secondPos.x * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f, 0.0f);
                }
                else {
                    this.transform.localPosition = Vector3.zero;
                    isClosing = false;
                }
                break;
            }
        }
        else {
            this.transform.localPosition = Vector3.zero;
            isClosing = false;
        }

        yield return null;
    }

    // 数値に変更があった際呼ばれる
    void OnValidate() {

    }
}
