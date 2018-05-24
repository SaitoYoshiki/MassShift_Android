using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SceneObject{
	[SerializeField]
	private string m_SceneName;

    // sceneObjectをstringに型変換(inplicit operator [型名]でキャストせず型変換)
	public static implicit operator string(SceneObject sceneObject){
		return sceneObject.m_SceneName;
	}

    // stringをsceneObjectに型変換
	public static implicit operator SceneObject(string sceneName){
		return new SceneObject() { m_SceneName = sceneName };
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneObject))]
public class SceneObjectEditor : PropertyDrawer{
	protected SceneAsset GetSceneObject(string sceneObjectName){
        // インスペクター上でScene名が指定されていない場合
        if (string.IsNullOrEmpty(sceneObjectName)) {
            return null;
        }

        // 配列にBuildSettingにあるSceneを格納し、その中にインスペクターに登録したSceneがあるならば
		for (int i = 0; i < EditorBuildSettings.scenes.Length; i++){
			EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
			if (scene.path.IndexOf(sceneObjectName) != -1){
				return AssetDatabase.LoadAssetAtPath(scene.path, typeof(SceneAsset)) as SceneAsset;
			}
		}

        // BuildSetting上にインスペクターに登録したScene名と一致するものがなければ
		Debug.Log("Scene [" + sceneObjectName + "] cannot be used. Add this scene to the 'Scenes in the Build' in the build settings.");
		return null;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		var sceneObj = GetSceneObject(property.FindPropertyRelative("m_SceneName").stringValue);
		var newScene = EditorGUI.ObjectField(position, label, sceneObj, typeof(SceneAsset), false);
		if (newScene == null){
			var prop = property.FindPropertyRelative("m_SceneName");
			prop.stringValue = "";
		}
		else{
			if (newScene.name != property.FindPropertyRelative("m_SceneName").stringValue){
				var scnObj = GetSceneObject(newScene.name);
				if (scnObj == null){
					Debug.LogWarning("The scene " + newScene.name + " cannot be used. To use this scene add it to the build settings for the project.");
				}
				else{
					var prop = property.FindPropertyRelative("m_SceneName");
					prop.stringValue = newScene.name;
				}
			}
		}
	}
}
#endif