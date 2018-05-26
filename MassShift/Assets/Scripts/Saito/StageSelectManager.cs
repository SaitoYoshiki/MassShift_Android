using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectManager : MonoBehaviour {

	[SerializeField]
	List<Goal> mGoal;

	[SerializeField]
	List<TextMesh> mText;

	Player mPlayer;

	[SerializeField, EditOnPrefab]
	GameObject mStageSelectBGMPrefab;

	[SerializeField]
	Color mStagePlateOnColor;

	[SerializeField]
	Color mStagePlateOffColor;

	[SerializeField]
	StageTransition mTransition;

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

		//プレートの色を変える
		SetEnterColor(-1);

		LimitPlayDoorSE();

		//ステージ開始時の演出
		mTransition.OpenDoorParent();

		//演出が終了するまで待機
		while (true) {
			if (mTransition.GetOpenEnd()) break;
			yield return null;
		}


		//BGMを流し始める
		SoundManager.SPlay(mStageSelectBGMPrefab);


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
		mTransition.CloseDoorParent();

		//演出が終了するまで待機
		while (true) {
			if (mTransition.GetCloseEnd()) break;
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
			t.color = mStagePlateOffColor;
		}
		if (aIndex == -1) return;
		mText[aIndex].color = mStagePlateOnColor;
	}

	//ドアが開く音を1つに制限する
	void LimitPlayDoorSE() {
		for(int i = 1; i < mGoal.Count; i++) {
			mGoal[i].mPlayOpenSE = false;
		}
	}
}
