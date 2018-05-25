using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	[SerializeField, EditOnPrefab]
	GameObject mSounds;

	private void Start() {
		//ゲーム起動中、複数のインスタンスが作られてはならない
		var sms = FindObjectsOfType<SoundManager>();
		if(sms.Length >= 2) {
			Debug.LogError("<color=#ff0000>SoundManagerが複数作成されています</color>", this);
			Destroy(this);
		}
	}

	public GameObject Play(GameObject aSoundPrefab, float aDelay) {
		if (aSoundPrefab == null) return null;
		var lSoundInstance = Instantiate(aSoundPrefab, mSounds.transform);
		lSoundInstance.GetComponent<AudioSource>().PlayDelayed(aDelay);
		return lSoundInstance;
	}
	public GameObject Play(GameObject aSoundPrefab) {
		return Play(aSoundPrefab, 0.0f);
	}

	public void Stop(GameObject aSoundInstance) {
		if (aSoundInstance == null) return;
		aSoundInstance.GetComponent<AudioSource>().Stop();
		Destroy(aSoundInstance);
	}

	public void Pause(GameObject aSoundInstance, bool aPause) {
		if (aSoundInstance == null) return;
		if (aPause) {
			aSoundInstance.GetComponent<AudioSource>().Pause();
		}
		else {
			aSoundInstance.GetComponent<AudioSource>().UnPause();
		}
	}

	public GameObject Volume(GameObject aSoundPrefab, float aVolume) {
		if (aSoundPrefab == null) return null;
		var lSoundInstance = Instantiate(aSoundPrefab, mSounds.transform);
		lSoundInstance.GetComponent<AudioSource>().volume = aVolume;
		return lSoundInstance;
	}


	//シングルトン的
	static SoundManager sInstance = null;
	public static SoundManager Instance {
		get {
			if (sInstance == null) {
				sInstance = FindObjectOfType<SoundManager>();
				if (sInstance == null) {
					Debug.LogError("<color=#ff0000>SoundManagerが存在しません</color>");
				}
			}
			return sInstance;
		}
	}


	public static GameObject SPlay(GameObject aSoundPrefab, float aDelay) {
		if (Instance == null) return null;
		return Instance.Play(aSoundPrefab, aDelay);
	}
	public static GameObject SPlay(GameObject aSoundPrefab) {
		return SPlay(aSoundPrefab, 0.0f);
	}

	public static void SStop(GameObject aSoundInstance) {
		if (Instance == null) return;
		Instance.Stop(aSoundInstance);
	}

	public static void SPause(GameObject aSoundInstance, bool aPause) {
		if (Instance == null) return;
		Instance.Pause(aSoundInstance, aPause);
	}
}
