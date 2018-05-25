using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectManager : MonoBehaviour {

	[SerializeField]
	List<Goal> mGoal;

	[SerializeField]
	List<TextMesh> mText;

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
		
		//ステージ開始時の演出
		//Fade.Start();

		//フェードが終わるのを待つ
		while(true) {
			//if(Fade.IsFinish()) break;
			break;
			yield return null;
		}

		int lSelectStageNum = -1;

		//ゲームメインのループ
		while (true) {

			SetEnterColor(-1);

			bool lIsEnter = Input.GetKeyDown(KeyCode.W);

			//ゴール判定
			//
			for(int i = 0; i < mGoal.Count; i++) {
				if(CanEnter(mGoal[i])) {

					SetEnterColor(i);

					//もし入る操作が行われているなら
					if(lIsEnter) {
						lSelectStageNum = i;
						break;
					}
				}
			}

			//ステージが選択されていたら
			if(lSelectStageNum != -1) {
				break;	//遷移へ
			}
			
			yield return null;	//ゲームメインを続ける
		}

		//ステージ終了時の演出
		//Fade.Start();

		//フェードが終わるのを待つ
		while (true) {
			//if(Fade.IsFinish()) break;
			break;
			yield return null;
		}


		//ステージ遷移
		UnityEngine.SceneManagement.SceneManager.LoadScene(Area.GetStageSceneName(1, lSelectStageNum + 1));

	}

	bool CanEnter(Goal lGoal) {
		if (lGoal == null) return false;
		if (!lGoal.IsInPlayer(mPlayer)) return false;
		return true;
	}

	void SetEnterColor(int aIndex) {
		foreach(var t in mText) {
			t.color = Color.gray;
		}
		if (aIndex == -1) return;
		mText[aIndex].color = Color.green;
	}
}
