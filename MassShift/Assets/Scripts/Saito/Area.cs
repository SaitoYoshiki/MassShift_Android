using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Area {

	List<int> mStageNum = new List<int>() {
		3,	//チュートリアル
		5,	//エリア１
		5,	//エリア２
		5	//エリア３
	};

	const string cStageSceneName = "Stage{0}-{1}";
	const string cTutorialSceneName = "Tutorial-{0}";

	static Area sInstance = null;
	
	//シングルトン
	//
	static Area GetInstance() {
		if(sInstance == null) {
			sInstance = new Area();
		}
		return sInstance;
	}


	//現在のシーン名を返す
	//
	static string GetSceneName() {
		return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
	}

	//ステージのシーン名を返す
	//
	public static string GetStageSceneName(int aAreaIndex, int aStageIndex) {
		if(aAreaIndex == 0) {
			return string.Format(cTutorialSceneName, aStageIndex);
		}
		if(1 <= aAreaIndex && aAreaIndex < GetAreaNum()) {
			return string.Format(cStageSceneName, aAreaIndex, aStageIndex);
		}
		return "";
	}

	//現在のエリア番号を返す（ステージ中なら、０・１・２・３．それ以外は-1）
	//
	public static int GetAreaIndex() {
		string lSceneName = GetSceneName();

		Regex r = new Regex(string.Format(cTutorialSceneName, @"(\d+?)"));

		if (r.IsMatch(lSceneName)) {
			return 0;
		}

		r = new Regex(string.Format(cStageSceneName, @"(\d+?)", @"(\d+?)"));

		if(r.IsMatch(lSceneName)) {
			var m = r.Match(lSceneName);
			int lRes = 0;
			int.TryParse(m.Groups[1].Value, out lRes);
			return lRes;
		}

		return -1;
	}

	//現在のステージ番号を返す（ステージ中なら、０・１・２・３…。それ以外は-1）
	//
	public static int GetStageIndex() {
		string lSceneName = GetSceneName();

		Regex r = new Regex(string.Format(cTutorialSceneName, @"(\d+?)"));

		if (r.IsMatch(lSceneName)) {
			var m = r.Match(lSceneName);
			int lRes = 0;
			int.TryParse(m.Groups[1].Value, out lRes);
			return lRes;
		}

		r = new Regex(string.Format(cStageSceneName, @"(\d+?)", @"(\d+?)"));

		if (r.IsMatch(lSceneName)) {
			var m = r.Match(lSceneName);
			int lRes = 0;
			int.TryParse(m.Groups[2].Value, out lRes);
			return lRes;
		}

		return -1;
	}


	//エリア数を返す
	//
	public static int GetAreaNum() {
		return GetInstance().mStageNum.Count;
	}

	//ステージ数を返す（エリア番号は、０・１・２・３）
	//
	public static int GetStageNum(int aAreaIndex) {
		if (GetAreaNum() <= aAreaIndex) return -1;
		if (aAreaIndex < 0) return -1;
		return GetInstance().mStageNum[aAreaIndex];
	}


	//次のステージのシーン名を返す
	//
	public static string GetNextStageSceneName(int aAreaIndex, int aStageIndex) {
		if (!ExistNextStage(aAreaIndex, aStageIndex)) return "";
		if(!ExistNextStageSameArea(aAreaIndex, aStageIndex)) {
			aAreaIndex += 1;
			aStageIndex = 0;
		}
		else {
			aStageIndex += 1;
		}
		return GetStageSceneName(aAreaIndex, aStageIndex);
	}


	//ゲーム中に、次のステージが存在しない
	//
	public static bool ExistNextStage(int aAreaIndex, int aStageIndex) {

		int lStageListIndex = GetStageListIndex(aStageIndex);

		//最終ステージだけ、次のステージが存在しない
		if(GetAreaNum() - 1 == aAreaIndex) {
			if (GetStageNum(aAreaIndex) - 1 == lStageListIndex) {
				return false;
			}
		}

		return true;
	}


	//同じエリアに、次のステージが存在しない
	//
	public static bool ExistNextStageSameArea(int aAreaIndex, int aStageIndex) {
		//エリア3の場合、aAreaIndex→3
		//GetAreaNum()→4となる
		if (GetAreaNum() - 1 < aAreaIndex) return false;
		if (aAreaIndex < 0) return false;

		int lStageListIndex = GetStageListIndex(aStageIndex);
		if (GetStageNum(aAreaIndex) - 1 <= lStageListIndex) return false;
		if (lStageListIndex < 0) return false;

		return true;
	}


	//ステージのインデックスは１・２・３で、配列を参照するためにずらす
	//
	static int GetStageListIndex(int aStageIndex) {
		return aStageIndex - 1;
	}
}
