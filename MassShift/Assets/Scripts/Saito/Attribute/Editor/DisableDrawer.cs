using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Disable))]
public class DisableDrawer : PropertyDrawer
{

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		using (new EditorGUI.DisabledGroupScope(true))
		{
			EditorGUI.PropertyField(position, property, label);
		}
	}

}