using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PrefabOnly))]
public class PrefabOnlyDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		string lOldLabelText = label.text;

		string lNewLabelText = label.text + " (Prefab Only)";
		label.text = lNewLabelText;

		EditorGUI.PropertyField(position, property, label);

		if (property.objectReferenceValue != null) {

			var lPrefabType = PrefabUtility.GetPrefabType(property.objectReferenceValue);
			switch (lPrefabType) {
				case PrefabType.Prefab:
				case PrefabType.ModelPrefab:
				case PrefabType.None:
					break;
				default:
					// Prefab以外がアタッチされた場合アタッチを外す
					Debug.LogError(lOldLabelText + "にPrefab以外が選択されました", property.serializedObject.targetObject);
					property.objectReferenceValue = null;
					break;
			}
		}
	}
}