using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonoColorFade : MyFade {
    public GameObject fadeObject;

    public float fadeTime;
    public Color fadeColor;

    bool isFading;
    bool isFadeEnd;

    // 初期化
    void Start() {
        isFading = false;
        isFadeEnd = false;

        fadeColor.a = 0.0f;
        fadeObject.GetComponent<Text>().color = fadeColor;
        fadeObject.GetComponent<Text>().text = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        FadeStart();
    }

    void Update() {
        if (isFading) {
            // 単色フェードのコルーチンを開始
            StartCoroutine(MonoFade());
        }
    }

    // フェード開始
    public override void FadeStart() {
        // エラー防止
        if (isFading) {
            return;
        }

        Debug.Log("Fade Start",this);

        isFading = true;
    }

    // フェードイン/アウト
    IEnumerator MonoFade() {
        if (!isFadeEnd) {
            Debug.Log("Fade In");
            // フェードイン処理
            fadeColor.a += 1.0f * (Time.deltaTime / fadeTime);
            fadeObject.GetComponent<Text>().color = fadeColor;
            if (fadeColor.a >= 1.0f) {
                fadeColor.a = 1.0f;
                isFadeEnd = true;
            }

            yield return null;
        }
        else {
            Debug.Log("Fade Out");
            // フェードアウト処理
            fadeColor.a -= 1.0f * (Time.deltaTime / fadeTime);
            fadeObject.GetComponent<Text>().color = fadeColor;
            if (fadeColor.a <= 0.0f) {
                fadeColor.a = 0.0f;
                isFading = false;
                Debug.Log("Fade End");
            }

            yield return null;
        }
    }

    public bool IsFadeEnd() {
        return isFading;
    }
}