using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditOnPrefabTest : MonoBehaviour {

	[SerializeField, EditOnPrefab]
	GameObject mGameObject;

	[SerializeField, PrefabOnly, EditOnPrefab]
	List<GameObject> mGameObjectPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
