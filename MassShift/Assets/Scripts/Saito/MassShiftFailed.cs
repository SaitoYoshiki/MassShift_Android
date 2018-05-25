using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassShiftFailed : MonoBehaviour {

	public delegate void OnMassShiftFailedEvent();

	public event OnMassShiftFailedEvent OnMassShiftFailed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void MassShiftFail() {
		OnMassShiftFailed();
	}
}
