using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorUtility {

	public static bool IsPrefab(Object aObject) {

#if UNITY_EDITOR
		var prefabType = PrefabUtility.GetPrefabType(aObject);
		switch (prefabType)
		{
			case PrefabType.Prefab:
			case PrefabType.ModelPrefab:
				return true;
			default:
				return false;
		}
#endif
		return false;

	}
}
