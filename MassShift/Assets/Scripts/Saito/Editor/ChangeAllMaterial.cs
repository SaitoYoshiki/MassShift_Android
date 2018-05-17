using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChangeAllMaterial : EditorWindow
{

	static ChangeAllMaterial window;

	[MenuItem("Window/ChangeAllMaterial")]
	static void Open()
	{
		if (window == null)
		{
			window = CreateInstance<ChangeAllMaterial>();
		}
		window.ShowUtility();
	}

	Material mMat;
	Material mTargetMat;

	private void OnGUI()
	{

		mMat = EditorGUILayout.ObjectField("Material", mMat, typeof(Material), false) as Material;
		mTargetMat = EditorGUILayout.ObjectField("Target", mTargetMat, typeof(Material), false) as Material;

		if (GUILayout.Button("Change"))
		{
			foreach (var s in Selection.gameObjects)
			{
				EditorUtility.ChangeMaterial(s, mMat, mTargetMat);
			}
		}
	}
}
