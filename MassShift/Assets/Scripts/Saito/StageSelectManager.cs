using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectManager : MonoBehaviour {

	[SerializeField]
	List<Goal> mGoal;

	Player mPlayer;

	// Use this for initialization
	void Start() {

		mPlayer = FindObjectOfType<Player>();

		//ゲーム進行のコルーチンを開始
		StartCoroutine(StageSelectMain());
	}

	// Update is called once per frame
	void Update() {

	}

	IEnumerator StageSelectMain() {

		float lTakeTime;

		int lSelectStageNum = -1;

		//ゲームメインのループ
		while (true) {

			bool lIsEnter = Input.GetKeyDown(KeyCode.W);

			//ゴール判定
			//
			for(int i = 0; i < mGoal.Count; i++) {
				if(lIsEnter && CanEnter(mGoal[i])) {
					lSelectStageNum = i;
					break;
				}
			}

			//ステージが選択されていたら
			if(lSelectStageNum != -1) {
				break;	//遷移へ
			}
			
			yield return null;	//ゲームメインを続ける
		}

		//扉が閉まる演出

		//扉が閉まる演出の終了待ち


		//ステージ遷移
		UnityEngine.SceneManagement.SceneManager.LoadScene(Area.GetStageSceneName(1, lSelectStageNum + 1));

		//リザルト画面を出す
		//
		lTakeTime = 0.0f;

		//リザルト画面で、シーン移動するので、これ以上先にはいかない
		while (true) {
			yield return null;
		}
	}

	bool CanEnter(Goal lGoal) {
		if (lGoal == null) return false;
		if (!lGoal.IsInPlayer(mPlayer)) return false;
		return true;
	}
}
