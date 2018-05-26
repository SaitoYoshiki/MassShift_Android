using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChangeScene : MonoBehaviour {
    private enum CHANGE_SCENE_MODE{
        NEXT,
        RETRY,
        STAGESELECT,
        TITLE,
        SELECTSCENE
    }

    Scene nowScene;

    public SceneObject titleScene;
    public SceneObject stageSelectScene;

    private bool changeSceneFlg;
    private CHANGE_SCENE_MODE changeSceneMode;
    private bool endGameFlg;
    private bool pauseFlg;

    void Start() {
        // 現在のシーンを取得
        nowScene = SceneManager.GetActiveScene();

        // ステセレ以外では
        /*if (nowScene.name != "Title" || nowScene.name != "StageSelect") {
            // 現在のシーンのステージインフォを取得
            nowStageInfo = FindObjectOfType<StageInfo>();
        }*/

        changeSceneFlg = false;
        endGameFlg = false;
        pauseFlg = false;

        if (titleScene == null) {
            Debug.LogError("タイトルシーンが指定されていません");
        }
        Debug.Log(titleScene);

        if (stageSelectScene == null) {
            Debug.LogError("ステージセレクトシーンが指定されていません");
        }
    }

    void Update() {
        // 
        if (changeSceneFlg) {
            switch (changeSceneMode) {
                // 次のステージへ
                case CHANGE_SCENE_MODE.NEXT: 
                    {
                        // 後でArea.cs対応に書き直し
                        int area, stage;
                        area = Area.GetAreaNumber();
                        stage = Area.GetStageNumber();

                        stage++;

                        if (stage == (int)StageInfo.STAGE.STAGE_MAX) {
                            area++;
                            stage = 1;
                            if (area == (int)StageInfo.AREA.AREA_MAX) {
                                endGameFlg = true;
                            }
                        }

                        string loadSceneName = "Stage" + area.ToString() + "-" + stage.ToString();

                        if (!endGameFlg) {
                            // 次のステージを読み込む
                            SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Single);
                        }
                        else {
                            // 最終ステージクリア後の処理
                        }
                    }
                    break;

                // リトライ
                case CHANGE_SCENE_MODE.RETRY:
                    // 現在のシーンを再読込
                    SceneManager.LoadSceneAsync(nowScene.name, LoadSceneMode.Single);
                    break;

                // ステージセレクト
                case CHANGE_SCENE_MODE.STAGESELECT:
                    // ステセレシーンを読み込み
                    SceneManager.LoadSceneAsync(stageSelectScene, LoadSceneMode.Single);
                    break;

                // タイトル
                case CHANGE_SCENE_MODE.TITLE:
                    // タイトルシーンを読み込み
                    SceneManager.LoadSceneAsync(titleScene, LoadSceneMode.Single);
                    break;

                case CHANGE_SCENE_MODE.SELECTSCENE:
                    // 選択されたステージを読み込み
                    {
                        // このへんステセレのシーン選択仕様決定後に書き直し
                        int area, stage;
                        area = Area.GetAreaNumber();
                        stage = Area.GetAreaNumber();
                        string loadSceneName = "Stage" + area.ToString() + "-" + stage.ToString();

                        SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Single);
                    }
                    break;

                default:
                    break;
            }
        }
    }

    public void OnNextButtonDown() {
        // ポーズを解除してシーン変更フラグを立てる
        pauseFlg = false;
        changeSceneMode = CHANGE_SCENE_MODE.NEXT;
        changeSceneFlg = true;
    }

    public void OnRetryButtonDown() {
        // ポーズを解除してシーン変更フラグを立てる
        pauseFlg = false;
        changeSceneMode = CHANGE_SCENE_MODE.RETRY;
        changeSceneFlg = true;
    }

    public void OnStageSelectButtonDown() {
        // ポーズを解除してシーン変更フラグを立てる
        pauseFlg = false;
        changeSceneMode = CHANGE_SCENE_MODE.STAGESELECT;
        changeSceneFlg = true;
    }

    public void OnTitleButtonDown() {
        // ポーズを解除してシーン変更フラグを立てる
        pauseFlg = false;
        changeSceneMode = CHANGE_SCENE_MODE.TITLE;
        changeSceneFlg = true;
    }

    public void OnStageSelected() {
        changeSceneMode = CHANGE_SCENE_MODE.SELECTSCENE;
        changeSceneFlg = true;
    }

    // 必要なもの
    /* 5/4
     *変数
     * 現在のシーン┐
     * 次のシーン　├シーンのリストを作ることで一つにまとめられる？
     * 前のシーン　┘
     * ポーズ中かどうか
     * オプション画面かどうか
     * 各エリアの最終ステージかどうか
     * 
     *関数
     * シーンロード/アンロード
     * スタートへ戻る
     * ポーズキャンバスのON/OFF
     * リザルトキャンバスのON/OFF
     * 
     *シーン
     * スタート
     * チュートリアル
     * ステージセレクト
     * 各ステージ
     * 
     *最終ステージ判別方法
     * シーン名
     * 最終ステージ内にのみ判別用オブジェクトを置いておく
     * 
     * 5/7
     *--シーン-----------------------------------------------------------------------
     *タイトルシーン
     * タイトルUI(チュートリアル・ステージセレクト)
     * カメラ制御スクリプト
     * ブルーム等の演出
     * 
     *チュートリアルシーン
     * 操作説明UI(デザイナーが全て描く)
     * ポーズ機能(ステージセレクト、タイトルへ戻る、オプション)
     * 
     *ステージセレクトシーン
     * ポーズ機能(タイトルへ戻る、オプション)
     *
     *ステージ
     * ステージ開始演出(デザイナー作成)
     * ポーズ機能(リトライ、ステージセレクト、タイトルへ戻る、オプション)
     * リザルト機能(次のステージ、もう一度遊ぶ、ステージセレクト)
     * StageInfoから現在のエリアとステージを取得して次のステージへ飛べるかどうか判断
     *-------------------------------------------------------------------------------
     *
     *--機能-------------------------------------------------------------------------
     * ポーズ機能(Escキーで開く・閉じる)
     * 　チュートリアル・ステージセレクト・ステージで使用可
     * 　[オプション]ボタンからオプション画面が開く、音量と解像度を調節可能
     * 
     * リザルト機能
     * 　ステージクリア時に自動的に開く
     * 　各エリアの最終ステージ以外の場合、[次のステージへ]ボタンが表示される
     *-------------------------------------------------------------------------------
     *
     * ステージのオブジェクト配置はプログラマー側で行うこと
     */
}