using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorUtility {


#if UNITY_EDITOR

	public static bool IsPrefab(Object aObject) {

		var prefabType = PrefabUtility.GetPrefabType(aObject);
		switch (prefabType)
		{
			case PrefabType.Prefab:
			case PrefabType.ModelPrefab:
				return true;
			default:
				return false;
		}
	}

	public static GameObject InstantiatePrefab(GameObject aPrefab, GameObject aParent) {
		GameObject go = GameObject.Instantiate(aPrefab, aParent.transform);
		GameObject g = UnityEditor.PrefabUtility.ConnectGameObjectToPrefab(go, aPrefab);
		UnityEditor.Undo.RegisterCreatedObjectUndo(g, "InstantiatePrefab");
		return g;
	}
	public static void DestroyGameObject(GameObject aGameObject) {
		UnityEditor.Undo.DestroyObjectImmediate(aGameObject);
	}

#endif
}
