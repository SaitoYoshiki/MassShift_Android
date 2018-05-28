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

    StageTransition st;
    ChangeScene cs;

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
        st = GameObject.Find("StageChangeCanvas").GetComponent<StageTransition>();
        cs = GameObject.Find("UIObject").GetComponent<ChangeScene>();
	}
	
	void Update () {
        /*if (zoomInFlg) {
            oldZoomInFlg = zoomInFlg;
            Zoom(zoomInTime, ref zoomInFlg, cameraStartPoint, cameraZoomPoint);
        }
        else {
            if (oldZoomInFlg != zoomInFlg) {
                oldZoomInFlg = zoomInFlg;
                tutorial.SetActive(true);
                stageselect.SetActive(true);
            }
        }*/

        // ズームイン中でなくて
        if (!zoomInFlg) {
            // 前フレームでもズームインしていなければ何もしない
            if (oldZoomInFlg == zoomInFlg) {
                return;
            }
            // 前フレームでズームインが終わったなら
            else {
                // モード選択のボタンをActiveにする
                oldZoomInFlg = zoomInFlg;
                tutorial.SetActive(true);
                stageselect.SetActive(true);
            }
        }
        // ズームイン中なら
        else {
            oldZoomInFlg = zoomInFlg;
            Zoom(zoomInTime, ref zoomInFlg, cameraStartPoint, cameraZoomPoint);
        }

        /*if (zoomOutFlg) {
            oldZoomOutFlg = zoomOutFlg;
            st.CloseDoorParent();
            Zoom(zoomOutTime, ref zoomOutFlg, cameraZoomPoint, cameraEndPoint);
        }
        else {
            if (oldZoomOutFlg != zoomOutFlg) {
                oldZoomOutFlg = zoomOutFlg;
            }
        }*/

        // ズームアウト中でなくて
        if (!zoomOutFlg) {
            // 前フレームでもズームアウトしていなければ何もしない
            if (oldZoomOutFlg == zoomOutFlg) {
                return;
            }
            // 前フレームでズームアウトが終わったなら
            else {
                oldZoomOutFlg = zoomOutFlg;

                // ドア閉めの演出が終わったら
                if (st.GetCloseEnd()) {
                    // ステージセレクトへ飛ぶ(仮)
                    cs.OnStageSelectButtonDown();
                }
            }
        }
        // ズームアウト中なら
        else {
            oldZoomOutFlg = zoomOutFlg;
            st.CloseDoorParent();
            Zoom(zoomOutTime, ref zoomOutFlg, cameraZoomPoint, cameraEndPoint);
        }

        // ズームインし終わっていたら何もしない
        if(firstZoom){
            return;
        }
        // ズームされていない初期状態なら
        else {
            if (Input.anyKeyDown) {
                // 「InputAnyKey」の表示を消す
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
