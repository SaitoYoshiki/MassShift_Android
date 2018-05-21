using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShareWeightBox : MonoBehaviour {

	[SerializeField, Disable]
	List<ShareWeightBox> mShareWeightBoxList;

	// Use this for initialization
	void Start () {
		mShareWeightBoxList = FindObjectsOfType<ShareWeightBox>().ToList();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	//そのボックスが、このボックスと重さを共有しているか
	public bool IsShare(ShareWeightBox aShareWeightBox) {
		return mShareWeightBoxList.Contains(aShareWeightBox);
	}

	//このボックスと重さを共有している、すべての共有ボックスを取得（このボックスを含む）
	public List<ShareWeightBox> GetShareAllList() {
		return new List<ShareWeightBox>(mShareWeightBoxList);
	}

	//このボックスと重さを共有している、すべての共有ボックスを取得（このボックスを含まない）
	public List<ShareWeightBox> GetShareAllListExceptOwn() {
		var res = GetShareAllList();
		res.Remove(this);
		return res;
	}
}
