using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTest : MonoBehaviour {

	[SerializeField, Disable]
	GameObject mGameObject;

	[SerializeField, Disable]
	float mFloat;

	[SerializeField, Disable]
	List<int> mIntList;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
