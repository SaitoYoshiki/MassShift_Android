using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	MassShift mMassShift;
	Player mPlayer;
	Goal mGoal;


	// Use this for initialization
	void Start() {
		mMassShift = FindObjectOfType<MassShift>();
		mPlayer = FindObjectOfType<Player>();
		mGoal = FindObjectOfType<Goal>();

		//ゲーム進行のコルーチンを開始
		StartCoroutine(GameMain());
	}

	// Update is called once per frame
	void Update()
	{

	}

	IEnumerator GameMain() {

		float lTakeTime;

		//ステージ開始時の演出
		//
		
		//TODO: プレイヤーを操作不可に
		mMassShift.CanShift = false;	//重さを移せない
		
		lTakeTime = 0.0f;

		//演出が終了するまで待機
		while (true) {
			lTakeTime += Time.deltaTime;
			if (lTakeTime >= 2.0f) {
				break;
			}
			yield return null;
		}


		//ゲームメインの開始
		//

		//TODO プレイヤーが操作可能になる

		//重さを移せるようになる
		mMassShift.CanShift = true;

		//ゲームメインのループ
		while (true) {
			
			//ゴール判定
			//
			if(CanGoal()) {
				break;
			}
			
			yield return null;	//ゲームメインを続ける
		}


		//ゴール時の、プレイヤーがドアから出ていく演出
		//

		//TODO Playerを操作不可にする

		//重さを移せなくする
		mMassShift.CanShift = false;


		lTakeTime = 0.0f;

		//ドアから出ていく演出が終了するまで待機
		while (true) {
			lTakeTime += Time.deltaTime;
			if (lTakeTime >= 2.0f) {
				break;
			}
			yield return null;
		}


		//リザルト画面を出す
		//
		lTakeTime = 0.0f;

		//リザルト画面で、シーン移動するので、これ以上先にはいかない
		while (true) {
			yield return null;
		}
	}

	bool CanGoal() {
		//全てのボタンがオンでないなら
		if (mGoal.IsAllButtonOn) {
			return false;
		}

		//プレイヤーがゴール枠に完全に入っていないなら
		if (!mGoal.IsInPlayer(mPlayer)) {
			return false;
		}

		//重さを移した後1秒以内なら
		if(mMassShift) {
			return false;
		}

		return true;	//ゴール可能
	}
}
