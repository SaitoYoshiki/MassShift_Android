using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFade : MonoBehaviour {
    // フェード開始
    public virtual void FadeStart(){
        Debug.Log("virtual");
    }

// フェード
// GameManagerにそのシーン上にある単色フェードprefabやトランジションフェードprefab、シーン遷移スクリプトを登録し、呼び出す
// 継承を用いて、Fadeクラスを継承したMonoColorFadeクラスなどを作ることでGetComponent<Fade>()で取得できるようにする
//　 →これにより、GameManagerからは登録されたフェードの種類に関わらず同じ処理でフェードを呼び出せる
}
