using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

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


	public static int GetArea() {
		string lSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

		Regex r = new Regex("Stage(d)-(d)");

		int lRes = 0;
		if(int.TryParse(r.Match(lSceneName).Groups[1].Value, out lRes)) {
			return lRes;
		}
		return 0;
	}
}
