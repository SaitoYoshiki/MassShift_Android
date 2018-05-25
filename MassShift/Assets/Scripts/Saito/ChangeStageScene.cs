using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStageScene : MonoBehaviour {

	[SerializeField, SceneName]
	List<string> mSceneNameList;

	// Use this for initialization
	void Start() {
		
	}

	// Update is called once per frame
	void Update() {

	}


	private void OnGUI() {

		using (new GUILayout.HorizontalScope()) {

			foreach(var s in mSceneNameList) {
				if (GUILayout.Button(s, GUILayout.MinWidth(200.0f), GUILayout.MinHeight(100.0f))) {
					UnityEngine.SceneManagement.SceneManager.LoadScene(s);
				}
			}
		}
	}
}
