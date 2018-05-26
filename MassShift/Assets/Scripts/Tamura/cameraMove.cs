using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMove : MonoBehaviour {

    private Vector3 cameraStartPoint = new Vector3(-5.0f, -6.0f, -15.0f);
    private Vector3 cameraZoomPoint = new Vector3(-7.0f, -6.5f, -5.0f);
    private Vector3 cameraEndPoint = new Vector3(0.0f, 1.0f, -50.0f);

    public float zoomInTime;
    public float zoomOutTime;

    [SerializeField]
    GameObject title;
    [SerializeField]
    GameObject text;
    [SerializeField]
    GameObject tutorial;
    [SerializeField]
    GameObject stageselect;

    float startZoomTime;
    float nowZoomTime;

    bool firstZoom = false;

    bool zoomInFlg = false;
    bool oldZoomInFlg;
    bool zoomOutFlg = false;
    bool oldZoomOutFlg;

    GameObject a;

    // ズームアウト終わった判定が必要

	void Start () {
        this.transform.position = cameraStartPoint;
	}
	
	void Update () {
        if (zoomInFlg) {
            oldZoomInFlg = zoomInFlg;
            Zoom(zoomInTime, ref zoomInFlg, cameraStartPoint, cameraZoomPoint);
        }
        else {
            if (oldZoomInFlg != zoomInFlg) {
                oldZoomInFlg = zoomInFlg;
                tutorial.SetActive(true);
                stageselect.SetActive(true);
            }
        }

        if (zoomOutFlg) {
            oldZoomOutFlg = zoomOutFlg;
            GameObject.Find("StageChangeCanvas").GetComponent<StageTransition>().CloseDoorParent();
            Zoom(zoomOutTime, ref zoomOutFlg, cameraZoomPoint, cameraEndPoint);
        }
        else {
            if (oldZoomOutFlg != zoomOutFlg) {
                oldZoomOutFlg = zoomOutFlg;
            }
        }

        if (GameObject.Find("StageChangeCanvas").GetComponent<StageTransition>().GetCloseEnd()) {
            GameObject.Find("UIObject").GetComponent<ChangeScene>().OnStageSelectButtonDown();
        }

        // ズームされていない初期状態なら
        if(!firstZoom){
            if (Input.anyKeyDown) {
                text.SetActive(false);
                firstZoom = true;
                zoomInFlg = true;
                startZoomTime = Time.realtimeSinceStartup;
            }
        }
	}

    // タイトルでボタンが押されたらズームアウト
    public void OnButtonDown() {
        Debug.Log("ズームアウト開始");
        zoomOutFlg = true;
        startZoomTime = Time.realtimeSinceStartup;
        title.SetActive(false);
        tutorial.SetActive(false);
        stageselect.SetActive(false);
    }

    // ズームイン/アウト
    void Zoom(float _zoomTime, ref bool _zoomFlg, Vector3 _startPos, Vector3 _endPos) {
        float zoomPer = 1.0f;
        nowZoomTime = Time.realtimeSinceStartup - startZoomTime;

        if (nowZoomTime < _zoomTime) {
            zoomPer = nowZoomTime / zoomInTime;
        }
        else {
            Debug.Log("ズーム終了");
            _zoomFlg = false;
        }
        this.transform.position = Vector3.Lerp(_startPos, _endPos, zoomPer);
    }

    public void OnTutorialSelected() {

    }

    public void OnStageSelectSelected() {
        cameraEndPoint = new Vector3(0.0f, 1.0f, -50.0f);
    }

    // チュートリアル1の部屋と、ステージセレクト前の部屋を同じサイズにして、カメラ引きの位置は同じにする
}
