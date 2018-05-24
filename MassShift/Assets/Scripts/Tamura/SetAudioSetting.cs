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
}
