﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pause : MonoBehaviour {
    [SerializeField]
    GameObject pauseCanvas;
    [SerializeField]
    GameObject optionCanvas;

    [SerializeField]
    Blur blur;

    // ゲームメイン側から受け取る、ポーズ可能かどうか
    bool canPause;

    bool pauseFlg = false;
    bool optionFlg = false;
    float intencity;
    float intencityMax = 10.0f;
    float prevTime;

    // ぼかし処理終了までの時間
    public float blurTime;

    // ポーズイベント
    public UnityEvent pauseEvent = new UnityEvent();

    void Update() {
        var deltaTime = Time.unscaledDeltaTime;
        //var deltaTime = Time.realtimeSinceStartup - prevTime;

        // Escキーでポーズ / ポーズ解除
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!optionFlg) {
                pauseFlg = !pauseFlg;
                PauseFunc();
            }
            else {
                // オプション画面が出ているときにEscキーを押すとポーズメニューに戻る
                OnOptionButtonDown();
            }
        }

        if (pauseFlg) {
            intencity += intencityMax * (deltaTime / blurTime);
        }
        else {
            intencity -= intencityMax * (deltaTime / blurTime);
        }

        // 0～1の範囲の値を返す
        intencity = Mathf.Clamp01(intencity);

        // intensityをintに変換
        blur.Resolution = (int)(intencity * 10);

        prevTime = Time.realtimeSinceStartup;
    }

    public void PauseFunc() {
        // ポーズ
        if (Time.timeScale != 0.0f) {
            Time.timeScale = 0.0f;
            pauseCanvas.SetActive(true);
        }
        // ポーズ解除
        else {
            Time.timeScale = 1.0f;
            pauseCanvas.SetActive(false);
        }

        // 登録された関数を実行
        pauseEvent.Invoke();
    }

    public void OnOptionButtonDown() {
        optionFlg = !optionFlg;

        if (optionFlg) {
            // ポーズ画面を閉じてオプション画面を開く
            pauseCanvas.SetActive(false);
            optionCanvas.SetActive(true);
        }
        else {
            // オプション画面を閉じる
            optionCanvas.SetActive(false);
            pauseCanvas.SetActive(true);
        }
    }
}
