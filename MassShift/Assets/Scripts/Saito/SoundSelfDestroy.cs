using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSelfDestroy : MonoBehaviour {

	AudioSource mAudioSource;

	[HideInInspector]
	public bool mIsPause = false;

	// Use this for initialization
	void Start() {
		mAudioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update() {
		if (mAudioSource.isPlaying == true) return;
		if (mIsPause == true) return;   //ポーズ中なら破棄しない
		Destroy(gameObject);
	}
}