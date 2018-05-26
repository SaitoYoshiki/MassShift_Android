using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetAudioSetting : MonoBehaviour {
    // オーディオミキサー
    public AudioMixer mixer;

    public void SetAudio() {
        // オーディオミキサー内のグループを指定して再生停止(pitchを0.0fに)
        mixer.SetFloat("gameSEPitch", Time.timeScale);
    }

    public void SetMasterSetting(float _sliderValue) {
        // オプション画面での音量設定をミキサーに反映
        mixer.SetFloat("masterVolume", Mathf.Lerp(-80, 0, _sliderValue));
    }

    public void SetBGMSetting(float _sliderValue) {
        // オプション画面での音量設定をミキサーに反映
        mixer.SetFloat("bgmVolume", Mathf.Lerp(-80, 0, _sliderValue));
    }

    public void SetSESetting(float _sliderValue) {
        // オプション画面での音量設定をミキサーに反映
        mixer.SetFloat("seVolume", Mathf.Lerp(-80, 0, _sliderValue));
    }
}
