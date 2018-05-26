using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	MassShift mMassShift;
	Player mPlayer;
	Goal mGoal;

	[SerializeField, EditOnPrefab]
	List<GameObject> mAreaBGM;

	[SerializeField]
	StageTransition mTransition;

	[SerializeField]
	Result mResult;

	[SerializeField]
	Pause mPause;


	// Use this for initialization
	void Start() {
		mMassShift = FindObjectOfType<MassShift>();
		mPlayer = FindObjectOfType<Player>();
		mGoal = FindObjectOfType<Goal>();

		Time.timeScale = 1.0f;
		mPause.pauseEvent.Invoke();

		//ゲーム進行のコルーチンを開始
		StartCoroutine(GameMain());
	}

	// Update is called once per frame
	void Update() {

	}

	IEnumerator GameMain() {

		float lTakeTime;

		//ステージ開始時の演出
		//

		//プレイヤーを操作不可に
		OnCantOperation();

		mTransition.OpenDoorParent();

		//演出が終了するまで待機
		while (true) {
			if (mTransition.GetOpenEnd()) break;
			yield return null;
		}

		//BGMを再生する
		SoundManager.SPlay(mAreaBGM[Area.GetAreaNumber()]);


		//ゲームメインの開始
		//

		//プレイヤーが操作可能になる
		OnCanOperation();

		//ゲームメインのループ
		while (true) {

			//ポーズ中なら
			if(mPause.pauseFlg) {
				//mMassShift.CanShift = false;
				Cursor.visible = true;
			}
			else {
				//mMassShift.CanShift = true;
				Cursor.visible = false;
			}

			//ゴール判定
			//
			if (CanGoal()) {
				break;
			}

			yield return null;	//ゲームメインを続ける
		}


		//ゴール時の、プレイヤーがドアから出ていく演出
		//

		//Playerを操作不可にする
		OnCantOperation();

		Cursor.visible = true;
		mResult.canGoal = true;
	}

	bool CanGoal() {
		//全てのボタンがオンでないなら
		if (!mGoal.IsAllButtonOn) {
			return false;
		}

		//プレイヤーがゴール枠に完全に入っていないなら
		if (!mGoal.IsInPlayer(mPlayer)) {
			return false;
		}

		//重さを移した後1秒以内なら
		if(mMassShift) {
			//return false;
		}

		return true;	//ゴール可能
	}

	void OnCantOperation() {
		mMassShift.CanShift = false;    //重さを移せない
		mPlayer.CanWalk = false;
		mPlayer.CanJump = false;
		mPlayer.CanRotation = false;
		mPause.canPause = false;
	}
	void OnCanOperation() {
		mMassShift.CanShift = true;    //重さを移せる
		mPlayer.CanWalk = true;
		mPlayer.CanJump = true;
		mPlayer.CanRotation = true;
		mPause.canPause = true;
	}
}
