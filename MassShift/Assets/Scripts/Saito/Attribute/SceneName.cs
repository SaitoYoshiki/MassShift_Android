using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class SceneName : PropertyAttribute
{
	bool mActiveOnly;
	
	public bool ActiveOnly {
		get { return mActiveOnly; }
	}


	public SceneName(bool aActiveOnly = true) {
		mActiveOnly = aActiveOnly;
	}
}