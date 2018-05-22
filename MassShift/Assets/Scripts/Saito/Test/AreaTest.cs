using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Debug.Log("AreaIndex:" + Area.GetAreaIndex());
		Debug.Log("StageIndex:" + Area.GetStageIndex());

		Debug.Log("AreaNum:" + Area.GetAreaNum());
		Debug.Log("Area0-StageNum:" + Area.GetStageNum(0));
		Debug.Log("Area1-StageNum:" + Area.GetStageNum(1));
		Debug.Log("Area2-StageNum:" + Area.GetStageNum(2));
		Debug.Log("Area3-StageNum:" + Area.GetStageNum(3));

		Debug.Log("Stage1-1のシーン名:" + Area.GetStageSceneName(1, 1));
		Debug.Log("Stage0-3のシーン名:" + Area.GetStageSceneName(0, 3));
		Debug.Log("Stage3-0のシーン名:" + Area.GetStageSceneName(3, 0));

		Debug.Log("Stage0-1の次のシーン:" + Area.GetNextStageSceneName(0, 1));
		Debug.Log("Stage0-3の次のシーン:" + Area.GetNextStageSceneName(0, 3));
		Debug.Log("Stage1-1の次のシーン:" + Area.GetNextStageSceneName(1, 1));
		Debug.Log("Stage1-5の次のシーン:" + Area.GetNextStageSceneName(1, 5));
		Debug.Log("Stage2-5の次のシーン:" + Area.GetNextStageSceneName(2, 5));
		Debug.Log("Stage3-5の次のシーン:" + Area.GetNextStageSceneName(3, 5));

		Debug.Log("Stage0-1の次のシーンがあるか:" + Area.ExistNextStage(0, 1));
		Debug.Log("Stage2-5の次のシーンがあるか:" + Area.ExistNextStage(2, 5));
		Debug.Log("Stage1-3の次のシーンがあるか:" + Area.ExistNextStage(1, 3));
		Debug.Log("Stage3-5の次のシーンがあるか:" + Area.ExistNextStage(3, 5));

		Debug.Log("Stage0-1の次のシーンが同じエリアにあるか:" + Area.ExistNextStageSameArea(0, 1));
		Debug.Log("Stage0-3の次のシーンが同じエリアにあるか:" + Area.ExistNextStageSameArea(0, 3));
		Debug.Log("Stage1-1の次のシーンが同じエリアにあるか:" + Area.ExistNextStageSameArea(1, 1));
		Debug.Log("Stage1-5の次のシーンが同じエリアにあるか:" + Area.ExistNextStageSameArea(1, 5));
		Debug.Log("Stage2-3の次のシーンが同じエリアにあるか:" + Area.ExistNextStageSameArea(2, 3));
		Debug.Log("Stage3-5の次のシーンが同じエリアにあるか:" + Area.ExistNextStageSameArea(3, 5));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
