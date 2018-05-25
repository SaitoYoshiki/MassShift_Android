using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMove : MonoBehaviour {

    private Vector3 cameraStartPoint = new Vector3(4.0f, 2.5f, -5.0f);
    private Vector3 cameraZoomPoint = new Vector3(1.5f, 1.5f, -2.0f);
    private Vector3 cameraEndPoint = new Vector3(10.0f, 5.0f, -10.0f);

    public float zoomInTime;
    public float zoomOutTime;

    float startZoomTime;
    float nowZoomTime;

    bool zoomInFlg = false;
    bool zoomOutFlg = false;

    GameObject a;

	void Start () {
        this.transform.position = cameraStartPoint;
	}
	
	void Update () {
        if (zoomInFlg) {
            Zoom(zoomInTime, zoomInFlg, cameraStartPoint, cameraZoomPoint);
        }

        if (zoomOutFlg) {
            Zoom(zoomOutTime, zoomOutFlg, cameraZoomPoint, cameraEndPoint);
        }

        // PushAnyKeyなどのTextが表示されている間のみ有効化するようにしないといけない
        if (Input.anyKeyDown) {
            zoomInFlg = true;
            startZoomTime = Time.realtimeSinceStartup;
        }
	}

    // タイトルでボタンが押されたらズームアウト
    public void OnButtonDown() {
        zoomOutFlg = true;
        startZoomTime = Time.realtimeSinceStartup;
    }

    // ズームイン/アウト
    void Zoom(float _zoomTime, bool _zoomFlg, Vector3 _startPos, Vector3 _endPos) {
        float zoomPer = 1.0f;
        nowZoomTime = Time.realtimeSinceStartup - startZoomTime;

        if (nowZoomTime < _zoomTime) {
            zoomPer = nowZoomTime / zoomInTime;
        }
        else {
            _zoomFlg = false;
        }
        this.transform.position = Vector3.Lerp(_startPos, _endPos, zoomPer);
    }
}
