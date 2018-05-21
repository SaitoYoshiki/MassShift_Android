using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility {

	public static void ChangeMaterialColor(GameObject aGameObject, Material aMaterial, string aPropertyName, Color aColor) {

		Renderer[] renderers = aGameObject.GetComponentsInChildren<Renderer>();
		foreach (var r in renderers) {

			Material[] materials = r.materials;
			bool lIsChange = false;
			foreach (var m in materials) {

				if (aMaterial == null ||  m.name == aMaterial.name + " (Instance)") {
					lIsChange = true;
					m.SetColor(aPropertyName, aColor);
				}
			}
			if (lIsChange) {
				r.materials = materials;
			}
		}
	}

	public static bool IsJoystickConnect() {

		foreach (var j in Input.GetJoystickNames()) {
			if (j != "") {
				return true;
			}
		}
		return false;
	}
}
