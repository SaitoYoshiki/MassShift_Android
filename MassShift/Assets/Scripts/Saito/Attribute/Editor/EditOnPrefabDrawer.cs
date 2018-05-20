using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(EditOnPrefab))]
public class EditOnPrefabDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position,
					  SerializedProperty property, GUIContent label)
	{
		//ラベル名を変更
		label.text += " (EditOnPrefab)";

		//もしプレハブなら、編集可能
		if (EditorUtility.IsPrefab(property.serializedObject.targetObject))
		{
			EditorGUI.PropertyField(position, property, label, true);
		}
		//プレハブでなく、インスタンスなら
		else
		{
			//値の上書きを無効にして、プレハブと同じ値を使う
			property.prefabOverride = false;

			using (new EditorGUI.DisabledGroupScope(true))
			{
				//編集不可能にして表示する
				EditorGUI.PropertyField(position, property, label, true);
			}
		}
	}
}