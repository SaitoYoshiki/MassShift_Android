using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomPropertyDrawer(typeof(SceneName))]
public class SceneNameDrawer : PropertyDrawer
{
	int mPopupIndex = 0;
	bool mNeedInit = true;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var lSceneNames = new List<string>();

		//ビルド設定に含まれている、シーン一覧を取得
		foreach(var tScene in EditorBuildSettings.scenes) {
			Regex tRegex = new Regex(@".*/(.*)\..*");
			var tMatch = tRegex.Match(tScene.path);
			if(tMatch == null) {
				Debug.Log("Sceneのパスがおかしいです", property.serializedObject.targetObject);
			}
			lSceneNames.Add(tMatch.Groups[1].Value);
		}

		//シーンが1つもなかったら
		if(lSceneNames.Count == 0) {
			EditorGUI.LabelField(position, "Sceneが存在しません");
			return;
		}

		//もしアトリビュートが作り直されたら（ゲームオブジェクトが選択されなくなると、アトリビュートは破棄される）
		if(mNeedInit) {
			//以前選択したシーン名から、現在のインデックスを取得
			mPopupIndex = GetSceneIndex(property.stringValue, lSceneNames.ToArray());
			mNeedInit = false;
		}

		mPopupIndex = EditorGUI.Popup(position, label.text, mPopupIndex, lSceneNames.ToArray());
		property.stringValue = lSceneNames[mPopupIndex];
	}

	int GetSceneIndex(string aSceneName, string[] aScenes) {
		int lIndex = 0;
		foreach (var tScene in aScenes) {
			if(aSceneName == tScene) {
				return lIndex;
			}
			lIndex++;
		}
		return 0;
	}
}