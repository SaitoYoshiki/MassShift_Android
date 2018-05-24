using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPlayerPrefs : MonoBehaviour {
    // オプションデータキー
    private static string masterVolumeSettings = "masterVolumeSet";
    private static string bgmVolumeSettings = "bgmVolumeSet";
    private static string seVolumeSettings = "seVolumeSet";
    private static string windowSettings = "windowSet";

    // オプション設定値
    float   masterVolumeData;
    float   bgmVolumeData;
    float   seVolumeData;
    int     windowSizeData;

    // 解像度1920x1080
    static int widthLarge = 1920;
    static int heightLarge = 1080;

    // 解像度1600x900
    static int widthMedium = 1600;
    static int heightMedium = 900;

    // 解像度1280x720
    static int widthSmall = 1280;
    static int heightSmall = 720;

    // フルスクリーンなし
    static bool fullScreenFlg = false;

    // 60fps
    static int refreshRate = 60;

    void Start() {
        // 設定を読み込めなければ初期設定を行う
        if (!LoadOptionSetting()) {
            masterVolumeData = 1.0f;
            bgmVolumeData = 1.0f;
            seVolumeData = 1.0f;
            windowSizeData = 0;

            ResolutionSetting();
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Keypad0)) {
            windowSizeData = 0;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            windowSizeData = 1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)) {
            windowSizeData = 2;
        }

        // オプションでボタンが押された時に適応されるように
        ResolutionSetting();
        // オプションから抜けた時に設定が保存されるように
        SaveOptionSetting();
    }

    void ResolutionSetting() {
        int windowWidth = 0;
        int windowHeight = 0;

        switch (windowSizeData) {
            // 解像度1920x1080
            case 0:
                windowWidth = widthLarge;
                windowHeight = heightLarge;
                break;

            // 解像度1600x900
            case 1:
                windowWidth = widthMedium;
                windowHeight = heightMedium;
                break;

            // 解像度1280x720
            case 2:
                windowWidth = widthSmall;
                windowHeight = heightSmall;
                break;
            default:
                break;
        }
        Screen.SetResolution(windowWidth, windowHeight, fullScreenFlg, refreshRate);

        // fastest = 0
        QualitySettings.SetQualityLevel(0);
    }

    // オプション画面を閉じる時に呼び出す
	void SaveOptionSetting () {
        // 各種ボリュームとウィンドウサイズの登録
        PlayerPrefs.SetFloat(masterVolumeSettings, masterVolumeData);
        PlayerPrefs.SetFloat(bgmVolumeSettings, bgmVolumeData);
        PlayerPrefs.SetFloat(seVolumeSettings, seVolumeData);
        PlayerPrefs.SetInt(windowSettings, windowSizeData);

        // データ保存
        PlayerPrefs.Save();
	}

    // 起動時に呼び出す
    bool LoadOptionSetting() {
        // 設定データが保存されていたらオプション設定に適応
        // マスター
        if (PlayerPrefs.HasKey(masterVolumeSettings)) {
            masterVolumeData = PlayerPrefs.GetFloat(masterVolumeSettings, -1.0f);
        }
        else {
            return false;
        }

        // BGM
        if (PlayerPrefs.HasKey(bgmVolumeSettings)) {
            bgmVolumeData = PlayerPrefs.GetFloat(bgmVolumeSettings, -1.0f);
        }
        else {
            return false;
        }

        // SE
        if (PlayerPrefs.HasKey(seVolumeSettings)) {
            seVolumeData = PlayerPrefs.GetFloat(seVolumeSettings, -1.0f);
        }
        else {
            return false;
        }

        // ウィンドウサイズ
        if (PlayerPrefs.HasKey(windowSettings)) {
            windowSizeData = PlayerPrefs.GetInt(windowSettings, -1);
        }
        else {
            return false;
        }

        return true;
    }
}