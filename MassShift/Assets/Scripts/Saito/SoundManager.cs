using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	[SerializeField, EditOnPrefab]
	GameObject mSounds;

	[System.Serializable]
	class FadeData {
		public GameObject mSoundInstance;	//対象となるオブジェクト
		public float mStartVolume;	//開始時のボリューム
		public float mEndVolume;	//終了時のボリューム
		public float mFadeTime;	//フェードにかける時間
		public float mNowTime;	//経過時間
		public bool mEndDestroy;	//終わってから破棄するか
	}

	[SerializeField, Disable]
	List<FadeData> mFadeList = new List<FadeData>();


	private void Start() {
		//ゲーム起動中、複数のインスタンスが作られてはならない
		var sms = FindObjectsOfType<SoundManager>();
		if(sms.Length >= 2) {
			Debug.LogError("<color=#ff0000>SoundManagerが複数作成されています</color>", this);
			Destroy(this);
		}
	}

	private void Update() {
		
		//フェードの更新をする
		for(int i = mFadeList.Count - 1; i >= 0; i--) {
			FadeData f = mFadeList[i];

			//何らかの理由で破棄されていたら
			if (f.mSoundInstance == null) {
				mFadeList.RemoveAt(i);
				continue;
			}

			//経過時間の更新
			f.mNowTime += Time.deltaTime;
			f.mNowTime = Mathf.Clamp(f.mNowTime, 0.0f, f.mFadeTime);

			//音量の更新
			Volume(f.mSoundInstance, Mathf.Lerp(f.mStartVolume, f.mEndVolume, f.mNowTime / f.mFadeTime));

			//終了かの判定
			if(f.mNowTime == f.mFadeTime) {
				mFadeList.RemoveAt(i);
				//終了時に破棄する設定なら
				if(f.mEndDestroy == true) {
					Destroy(f.mSoundInstance);
				}
				continue;
			}
		}
	}



	public GameObject Play(GameObject aSoundPrefab, float aDelay) {
		if (aSoundPrefab == null) return null;
		var lSoundInstance = Instantiate(aSoundPrefab, mSounds.transform);
		lSoundInstance.GetComponent<AudioSource>().PlayDelayed(aDelay);
		lSoundInstance.GetComponent<SoundSelfDestroy>().mIsPause = false;    //削除されないようにする
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

	public void Pause(GameObject aSoundInstance) {
		if (aSoundInstance == null) return;
		aSoundInstance.GetComponent<AudioSource>().Pause();
		aSoundInstance.GetComponent<SoundSelfDestroy>().mIsPause = true;	//削除されないようにする
	}
	public void UnPause(GameObject aSoundInstance) {
		if (aSoundInstance == null) return;
		aSoundInstance.GetComponent<AudioSource>().UnPause();
		aSoundInstance.GetComponent<SoundSelfDestroy>().mIsPause = false;    //削除されないようにする
	}

	public void Volume(GameObject aSoundInstance, float aVolume) {
		if (aSoundInstance == null) return;
		aSoundInstance.GetComponent<AudioSource>().volume = aVolume;
	}

	public float Volume(GameObject aSoundInstance) {
		if (aSoundInstance == null) return 0.0f;
		return aSoundInstance.GetComponent<AudioSource>().volume;
	}


	public void Fade(GameObject aSoundInstance, float aStartVolume, float aEndVolume, float aFadeTime, bool aEndDestroy) {
		FadeData f = new FadeData();
		f.mSoundInstance = aSoundInstance;
		f.mStartVolume = aStartVolume;
		f.mEndVolume = aEndVolume;
		f.mFadeTime = aFadeTime;
		f.mEndDestroy = aEndDestroy;
		f.mNowTime = 0.0f;

		mFadeList.Add(f);

		Volume(aSoundInstance, aStartVolume);
	}
	public void Fade(GameObject aSoundInstance, float aStartVolume, float aEndVolume, float aFadeTime) {
		Fade(aSoundInstance, aStartVolume, aEndVolume, aFadeTime, false);
	}

	public void Fade(GameObject aSoundInstance, float aEndVolume, float aFadeTime, bool aEndDestroy) {
		Fade(aSoundInstance, Volume(aSoundInstance), aEndVolume, aFadeTime, aEndDestroy);
	}
	public void Fade(GameObject aSoundInstance, float aEndVolume, float aFadeTime) {
		Fade(aSoundInstance, Volume(aSoundInstance), aEndVolume, aFadeTime, false);
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

	public static void SPause(GameObject aSoundInstance) {
		if (Instance == null) return;
		Instance.Pause(aSoundInstance);
	}
	public static void SUnPause(GameObject aSoundInstance) {
		if (Instance == null) return;
		Instance.UnPause(aSoundInstance);
	}

	public static void SVolume(GameObject aSoundInstance, float aVolume) {
		if (Instance == null) return;
		Instance.Volume(aSoundInstance, aVolume);
	}
	

	public static float SVolume(GameObject aSoundInstance) {
		if (aSoundInstance == null) return 0.0f;
		return aSoundInstance.GetComponent<AudioSource>().volume;
	}


	public static void SFade(GameObject aSoundInstance, float aStartVolume, float aEndVolume, float aFadeTime, bool aEndDestroy) {
		if (Instance == null) return;
		Instance.Fade(aSoundInstance, aStartVolume, aEndVolume, aFadeTime, aEndDestroy);
	}
	public static void SFade(GameObject aSoundInstance, float aStartVolume, float aEndVolume, float aFadeTime) {
		if (Instance == null) return;
		Instance.Fade(aSoundInstance, aStartVolume, aEndVolume, aFadeTime);
	}

	public static void SFade(GameObject aSoundInstance, float aEndVolume, float aFadeTime, bool aEndDestroy) {
		if (Instance == null) return;
		Instance.Fade(aSoundInstance, aEndVolume, aFadeTime, aEndDestroy);
	}
	public static void SFade(GameObject aSoundInstance, float aEndVolume, float aFadeTime) {
		if (Instance == null) return;
		Instance.Fade(aSoundInstance, aEndVolume, aFadeTime);
	}





}
