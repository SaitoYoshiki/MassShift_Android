using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour {
    [SerializeField]
    GameObject ResultCanvas;

    public bool canGoal;

	void Update () {
        // ゴールしたなら
        if (canGoal) {
            ResultCanvas.SetActive(true);
            // ポーズ機能を無効に
            GetComponent<Pause>().enabled = false;
        }
	}
}
