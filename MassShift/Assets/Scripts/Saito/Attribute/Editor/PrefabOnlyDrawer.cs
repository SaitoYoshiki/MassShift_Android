using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PrefabOnly))]
public class PrefabOnlyDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		label.text = label.text + " (Prefab Only)";

		EditorGUI.PropertyField(position, property, label);

		if (property.objectReferenceValue != null) {

			if(!EditorUtility.IsPrefab(property.objectReferenceValue)) {
				// Prefab以外がアタッチされた場合アタッチを外す
				Debug.LogError(label.text + "にPrefab以外が選択されました", property.serializedObject.targetObject);
				property.objectReferenceValue = null;
			}
		}
	}
}