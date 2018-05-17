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
	public static bool IsInPrefab(GameObject aObject, GameObject aPrefab) {
		var g = PrefabUtility.FindPrefabRoot(aObject);
		if (g == null) return false;
		return aPrefab == g;
	}
	public static GameObject GetPrefab(GameObject aObject) {
		var g = PrefabUtility.FindPrefabRoot(aObject);
		return g as GameObject;
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

	public static void ChangeMaterial(GameObject aGameObject, Material aMaterial, Material aTarget) {

		Renderer[] renderers = aGameObject.GetComponentsInChildren<Renderer>();
		foreach (var r in renderers)
		{

			Material[] materials = r.sharedMaterials;
			bool lIsChange = false;
			for (int i = 0; i < materials.Length; i++) {
				if (materials[i] == aTarget || aTarget == null) {
					lIsChange = true;
					materials[i] = aMaterial;
				}
			}
			if (lIsChange) {
				r.sharedMaterials = materials;
			}
		}
	}



#endif
}
