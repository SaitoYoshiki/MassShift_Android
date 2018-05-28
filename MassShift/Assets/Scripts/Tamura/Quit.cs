using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour {
    // exe終了
    public void OnQuitButtonDown() {
        Application.Quit();
    }

    // ウィンドウ閉じる
    public void OnCancelButtonDonw() {
        this.gameObject.SetActive(false);
    }
}
