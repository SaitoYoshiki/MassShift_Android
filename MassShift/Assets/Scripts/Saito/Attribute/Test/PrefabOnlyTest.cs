using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabOnlyTest : MonoBehaviour {

	[SerializeField, PrefabOnly]
	GameObject mGameObject;

	[SerializeField, PrefabOnly]
	Texture mTexture;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
