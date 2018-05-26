using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class StageChangeScenematic : MonoBehaviour {
    public enum DOOR {
        UP = 0,
        DOWN,
        RIGHT,
        LEFT
    }

    public DoorAnimManager daManager;

    bool isOpening = false;
    bool isClosing = false;
    public float doorAnimTime;

    [SerializeField, Range(0.0f, 1.0f)]
    float scenematicPercent;

    [SerializeField, Range(0.0f, 1.0f)]
    float scenematicStopPercent;

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
                        daManager.openCountPlus();
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
                        daManager.openCountPlus();
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
                        daManager.openCountPlus();
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
                        daManager.openCountPlus();
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
                        daManager.closeCountPlus();
                    }
                break;

                case DOOR.DOWN:
                    if (this.transform.localPosition.y < 0.0f) {
                        this.transform.localPosition = new Vector3(0.0f, secondPos.y - secondPos.y * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isClosing = false;
                        daManager.closeCountPlus();
                    }
                break;

                case DOOR.RIGHT:
                    if (this.transform.localPosition.x > 0.0f) {
                        this.transform.localPosition = new Vector3(secondPos.x - secondPos.x * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f, 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isClosing = false;
                        daManager.closeCountPlus();
                    }
                break;

                case DOOR.LEFT:
                    if (this.transform.localPosition.x < 0.0f) {
                        this.transform.localPosition = new Vector3(secondPos.x - secondPos.x * ((nowTime - secondTime) / (doorAnimTime - secondTime)), 0.0f, 0.0f);
                    }
                    else {
                        this.transform.localPosition = Vector3.zero;
                        isClosing = false;
                        daManager.closeCountPlus();
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
*/

public class StageChangeScenematic : MonoBehaviour {
    public enum DOOR {
        UP = 0,
        DOWN,
        RIGHT,
        LEFT
    }

    public DoorAnimManager daManager;

    bool isOpening = false;
    bool isClosing = false;
    
    public float doorAnimTime;
    
    [SerializeField, Range(0.0f, 1.0f)]
    public float doorAnimPer;
    [SerializeField, Range(0.0f, 1.0f)]
    public float doorStopPer;

    float startTime;
    float stopTime;
    
    public Vector3 openPos;
    public Vector3 stopPos;
    public Vector3 closePos;

    int area;
    int stage;

    void Start() {
        area = Area.GetAreaNumber();
        stage = Area.GetStageNumber();
        openPos = this.transform.localPosition;

        // Active時に開く
        //StartOpening();
    }

    void Update() {
        if (isOpening) {
            //AnimDoor(closePos, stopPos, openPos, isOpening, true);
            OpenDoor();
        }

        if (isClosing) {
            //AnimDoor(openPos, stopPos, closePos, isClosing, false);
            CloseDoor();
        }
    }

    public void StartOpening() {
        isOpening = true;
        startTime = Time.realtimeSinceStartup;
    }

    public void StartClosing(){
        isClosing = true;
        startTime = Time.realtimeSinceStartup;
    }

    // ドア開く演出
    void OpenDoor() {
        float nowTime = Time.realtimeSinceStartup - startTime;

        float timePer = nowTime / doorAnimTime;

        // ドア開きアニメーション一段階目
        if (timePer <= doorAnimPer) {
            //this.transform.localPosition = Vector3.Lerp(closePos, stopPos, timePer / doorAnimPer);
            this.transform.localPosition = Vector3.Lerp(closePos, openPos, timePer / doorAnimPer);
        }
        // ドア開きアニメーション二段階目
        else if (timePer > doorAnimPer && timePer <= (doorAnimPer + doorStopPer)) {
            stopTime = nowTime;
        }
        // ドア開きアニメーション三段階目
        else if (timePer > (doorAnimPer + doorStopPer) && timePer <= 1.0f) {
            nowTime = nowTime - stopTime;
            timePer = nowTime / (doorAnimTime - stopTime);
            this.transform.localPosition = new Vector3(stopPos.x + (openPos.x - stopPos.x) * timePer, stopPos.y + (openPos.y - stopPos.y) * timePer, 0.0f);
        }
        // ドア開きアニメーション終了
        else {
            Debug.Log("DoorOpened");
            this.transform.localPosition = openPos;
            isOpening = false;
            daManager.OpenCountPlus();
        }
    }

    // ドア閉まる演出
    void CloseDoor() {
        float nowTime = Time.realtimeSinceStartup - startTime;

        float timePer = nowTime / doorAnimTime;

        // アニメーション一段階目
        if (timePer <= doorAnimPer) {
            //this.transform.localPosition = Vector3.Lerp(openPos, stopPos, timePer / doorAnimPer);
            this.transform.localPosition = Vector3.Lerp(openPos, closePos, timePer / doorAnimPer);
        }
        // アニメーション二段階目
        else if (timePer > doorAnimPer && timePer <= (doorAnimPer + doorStopPer)) {
            stopTime = nowTime;
        }
        // アニメーション三段階目
        else if (timePer > (doorAnimPer + doorStopPer) && timePer <= 1.0f) {
            timePer = (nowTime - stopTime) / (doorAnimTime - stopTime);
            this.transform.localPosition = new Vector3(stopPos.x + (closePos.x - stopPos.x) * timePer, stopPos.y + (closePos.y - stopPos.y) * timePer, 0.0f);
        }
        // アニメーション終了
        else {
            Debug.Log("DoorClosed");
            this.transform.localPosition = closePos;
            isClosing = false;
            daManager.CloseCountPlus();
        }
    }

    // ドア開閉演出
    /*void AnimDoor(Vector3 _startPos, Vector3 _stopPos, Vector3 _endPos, bool _flg, bool openFlg) {
        float nowTime = Time.realtimeSinceStartup - startTime;

        float timePer = nowTime / doorAnimTime;

        // アニメーション一段階目
        if (timePer <= doorAnimPer) {
            this.transform.localPosition = Vector3.Lerp(_startPos, _stopPos, timePer / doorAnimPer);

        }
        // アニメーション二段階目
        else if (timePer > doorAnimPer && timePer <= (doorAnimPer + doorStopPer)) {
            stopTime = nowTime;
        }
        // アニメーション三段階目
        else if (timePer > (doorAnimPer + doorStopPer) && timePer <= 1.0f) {
            timePer = (nowTime - stopTime) / (doorAnimTime - stopTime);
            this.transform.localPosition = new Vector3(_stopPos.x + (_endPos.x - _stopPos.x) * timePer, _stopPos.y + (_endPos.y - _endPos.y) * timePer, 0.0f);
        }
        // アニメーション終了
        else {
            this.transform.localPosition = _endPos;
            _flg = false;
            if (openFlg) {
                daManager.openCountPlus();
            }
            else {
                daManager.closeCountPlus();
            }
        }
    }*/
}
