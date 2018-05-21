using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour {

	//このスクリプトがキー入力を受け取って、自身でスクリーンショットを撮れるか
	public bool mCanSelf = true;

	// Update is called once per frame
	void Update() {

		//Zキーが押されたら
		if (Input.GetKeyDown(KeyCode.Z)) {
			if(mCanSelf) {
				SaveScreenShot();   //スクリーンショットを撮る
			}
		}
	}

	//現在の日時をファイル名に入った、スクリーンショットを保存する
	//
	public void SaveScreenShot() {

		//スクリーンショットを保存するディレクトリが存在しなかったら、作成する
		if (!System.IO.Directory.Exists(GetScreenShotPath())) {
			System.IO.Directory.CreateDirectory(GetScreenShotPath());
		};

		//倍率の計算
		float max = Mathf.Max(Screen.width, Screen.height);
		int scale = Mathf.RoundToInt(1960 / max);

		//ファイル名の作成
		string lNowDate = string.Format("{0:00}{1:00}-{2:00}{3:00}-{4:00}{5:00}", System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second, System.DateTime.Now.Millisecond / 10);
		string lFileName = string.Format("{0}/MassShift_{1}.png", GetScreenShotPath(), lNowDate);

		//スクリーンショットの作成と保存
		//CaptureScreenShotAlpha(lFileName);	//Aも保存できる版。原因は分からないが、エディターだとうまく保存できない
		Application.CaptureScreenshot(lFileName, scale);
	}


	//スクリーンショットが保存されるディレクトリのパスを取得
	//
	string GetScreenShotPath() {

		//環境によって、保存されるパスが異なる
		string lSavePath = Application.dataPath + "/../";
		//string lSavePath = Application.pernantokaPath + "/";
#if UNITY_EDITOR
		lSavePath = "";
#endif

		string lSaveFolderPath = lSavePath + "ScreenShot";

		return lSaveFolderPath;
	}


	//Aチャンネル付きで保存する。エディタ上だとうまく動かない。解像度が一致しないから？
	//
	void CaptureScreenShotAlpha(string aFilePath) {

		var lHeight = Screen.height;
		var lWidth = Screen.width;
		var lTex = new Texture2D(lWidth, lHeight, TextureFormat.ARGB32, false);
		lTex.ReadPixels(new Rect(0, 0, lWidth, lHeight), 0, 0);
		lTex.Apply();

		var lBytes = lTex.EncodeToPNG();
		Destroy(lTex);

		System.IO.File.WriteAllBytes(aFilePath, lBytes);
	}
}
