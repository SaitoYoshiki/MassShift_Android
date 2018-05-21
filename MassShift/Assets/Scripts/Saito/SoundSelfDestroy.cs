using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSelfDestroy : MonoBehaviour {

	AudioSource mAudioSource;

	// Use this for initialization
	void Start() {
		mAudioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update() {
		if (mAudioSource.isPlaying == false) {
			Destroy(gameObject);
		}
	}
}