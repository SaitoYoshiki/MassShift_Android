using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorUtility {

	public static bool IsPrefab(Object aObject)
	{
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
}
