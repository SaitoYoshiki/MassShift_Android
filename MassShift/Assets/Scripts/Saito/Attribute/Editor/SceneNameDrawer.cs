using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomPropertyDrawer(typeof(SceneName))]
public class SceneNameDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		property.serializedObject.Update();

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

		string lBeforeName = property.stringValue;

		int lPopupIndex = GetSceneIndex(lBeforeName, lSceneNames.ToArray());
		if (lPopupIndex == -1) {
			lPopupIndex = 0;    //そのシーン名が存在しなかったら、0にする
		}
		
		lPopupIndex = EditorGUI.Popup(position, label.text, lPopupIndex, lSceneNames.ToArray());
		
		if(lBeforeName != lSceneNames[lPopupIndex]) {
			property.stringValue = lSceneNames[lPopupIndex];
			property.serializedObject.ApplyModifiedProperties();
		}
	}

	int GetSceneIndex(string aSceneName, string[] aScenes) {
		int lIndex = 0;
		foreach (var tScene in aScenes) {
			if(aSceneName == tScene) {
				return lIndex;
			}
			lIndex++;
		}
		return -1;
	}
}